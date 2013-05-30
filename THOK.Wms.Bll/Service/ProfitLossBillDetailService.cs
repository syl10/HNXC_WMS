using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;
using THOK.Wms.SignalR;
using THOK.Wms.SignalR.Common;

namespace THOK.Wms.Bll.Service
{
    public class ProfitLossBillDetailService : ServiceBase<ProfitLossBillDetail>, IProfitLossBillDetailService
    {
        [Dependency]
        public IProfitLossBillDetailRepository ProfitLossBillDetailRepository { get; set; }
        [Dependency]
        public IUnitRepository UnitRepository { get; set; }
        [Dependency]
        public ICellRepository CellRepository { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }
        [Dependency]
        public IStorageLocker Locker { get; set; }
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IProfitLossBillDetailService 成员

        public string resultStr = "";//错误信息字符串

        /// <summary>
        /// 查询损益细单信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="BillNo">损益单号</param>
        /// <returns></returns>
        public object GetDetails(int page, int rows, string BillNo)
        {
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<ProfitLossBillDetail> ProfitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
                var profitLossBillDetail = ProfitLossBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(i => new
                {
                    i.ID,
                    i.BillNo,
                    i.CellCode,
                    i.StorageCode,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.Unit.UnitName,
                    i.Price,
                    Quantity = i.Quantity / i.Unit.Count,
                    i.Description
                });
                int total = profitLossBillDetail.Count();
                profitLossBillDetail = profitLossBillDetail.Skip((page - 1) * rows).Take(rows);
                return new { total, rows = profitLossBillDetail.ToArray() };
            }
            return "";
        }

        /// <summary>
        /// 新增损益单明细
        /// </summary>
        /// <param name="profitLossBillDetail">损益单细表</param>
        /// <returns></returns>
        public bool Add(ProfitLossBillDetail profitLossBillDetail, out string strResult)
        {
            bool result = false;
            try
            {
                IQueryable<ProfitLossBillDetail> profitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == profitLossBillDetail.UnitCode);
                var cell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == profitLossBillDetail.CellCode);
                var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == profitLossBillDetail.ProductCode);
                var storage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == profitLossBillDetail.StorageCode);
                if (Locker.LockStorage(storage, product) != null)
                {
                    if (IsQuntityRight(profitLossBillDetail.Quantity * unit.Count, storage.InFrozenQuantity, storage.OutFrozenQuantity, cell.MaxQuantity * unit.Count, storage.Quantity))
                    {
                        var pbd = new ProfitLossBillDetail();
                        pbd.BillNo = profitLossBillDetail.BillNo;
                        pbd.CellCode = profitLossBillDetail.CellCode;
                        pbd.StorageCode = profitLossBillDetail.StorageCode;
                        pbd.ProductCode = profitLossBillDetail.ProductCode;
                        pbd.UnitCode = profitLossBillDetail.UnitCode;
                        pbd.Price = profitLossBillDetail.Price;
                        pbd.Quantity = profitLossBillDetail.Quantity * unit.Count;
                        pbd.Description = profitLossBillDetail.Description;
                        if (profitLossBillDetail.Quantity > 0)
                        {
                            storage.InFrozenQuantity += profitLossBillDetail.Quantity * unit.Count;
                        }
                        else
                        {
                            storage.OutFrozenQuantity += Math.Abs(profitLossBillDetail.Quantity * unit.Count);
                        }

                        ProfitLossBillDetailRepository.Add(pbd);
                        ProfitLossBillDetailRepository.SaveChanges();
                        storage.LockTag = string.Empty;
                        StorageRepository.SaveChanges();
                        result = true;
                    }
                }
                strResult = resultStr == "" ? "该库存的当前库存-出库冻结量小于0或者已经处于编辑状态！" : resultStr;
            }
            catch (Exception ex)
            {
                strResult = "新增失败，原因：" + ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 判断输入的损益数量是否是合理的数量
        /// </summary>
        /// <param name="inputQuantity">用户输入的数量可以是正的也可以是负的</param>
        /// <param name="inFrozenQuantity">当前库存的入库冻结量</param>
        /// <param name="outFrozenQuantity">当前库存的出库冻结量</param>
        /// <param name="maxQuantity">当前货位的最大存储量</param>
        /// <param name="currentQuantity">当前货位的库存数量</param>
        /// <returns></returns>
        public bool IsQuntityRight(decimal inputQuantity, decimal inFrozenQuantity, decimal outFrozenQuantity, decimal maxQuantity, decimal currentQuantity)
        {
            bool result = false;
            if (inputQuantity > 0)
            {
                if (inputQuantity <= (maxQuantity - inFrozenQuantity - currentQuantity))
                {
                    result = true;
                }
                else
                {
                    resultStr = "入库的数量必须小于或等于[货位最大量-（当前货位库存+入库冻结量）]";
                    return result;
                }
            }
            else if (inputQuantity < 0)
            {
                if (Math.Abs(inputQuantity) <= (currentQuantity - outFrozenQuantity))
                {
                    result = true;
                }
                else
                {
                    resultStr = "出库数量必须小于或等于[当前库存量-出库冻结量]";
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除损益细单
        /// </summary>
        /// <param name="ID">损益单细单ID</param>
        /// <returns></returns>
        public bool Delete(string ID, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                IQueryable<ProfitLossBillDetail> profitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
                int intID = Convert.ToInt32(ID);
                var pbd = profitLossBillDetailQuery.FirstOrDefault(i => i.ID == intID);
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == pbd.UnitCode);
                var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == pbd.ProductCode);
                var storage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == pbd.StorageCode);
                if (pbd != null)
                {
                    if (Locker.LockStorage(storage, product) != null)
                    {
                        if (pbd.Quantity > 0)
                        {
                            storage.InFrozenQuantity -= pbd.Quantity;
                        }
                        else
                        {
                            storage.OutFrozenQuantity -= Math.Abs(pbd.Quantity);
                        }
                        storage.LockTag = string.Empty;
                        StorageRepository.SaveChanges();
                        ProfitLossBillDetailRepository.Delete(pbd);
                        ProfitLossBillDetailRepository.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        strResult = "库存加锁失败！";
                        result = false;
                    }
                }
                else
                {
                    strResult = "删除失败，找不到该条单据！";
                    result = false;
                }
            }
            catch (Exception ex)
            {
                strResult = "删除失败，原因：" + ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 修改损益细单
        /// </summary>
        /// <param name="profitLossBillDetail">损益细单</param>
        /// <returns></returns>
        public bool Save(ProfitLossBillDetail profitLossBillDetail, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                IQueryable<ProfitLossBillDetail> profitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
                var pbd = profitLossBillDetailQuery.FirstOrDefault(i => i.ID == profitLossBillDetail.ID && i.BillNo == profitLossBillDetail.BillNo);
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == profitLossBillDetail.UnitCode);
                var cell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == profitLossBillDetail.CellCode);
                var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == pbd.ProductCode);
                var storage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == pbd.StorageCode);
                if (Locker.LockStorage(storage, product) != null)
                {
                    if (IsQuntityRight(profitLossBillDetail.Quantity * unit.Count, storage.InFrozenQuantity - Math.Abs(pbd.Quantity), storage.OutFrozenQuantity - Math.Abs(pbd.Quantity), cell.MaxQuantity * unit.Count, storage.Quantity))
                    {
                        pbd.CellCode = profitLossBillDetail.CellCode;
                        pbd.StorageCode = profitLossBillDetail.StorageCode;
                        pbd.ProductCode = profitLossBillDetail.ProductCode;
                        pbd.UnitCode = profitLossBillDetail.UnitCode;
                        pbd.Price = profitLossBillDetail.Price;
                        //原来的数量撤销
                        if (pbd.Quantity > 0)
                        {
                            storage.InFrozenQuantity -= pbd.Quantity;
                        }
                        else
                        {
                            storage.OutFrozenQuantity -= Math.Abs(pbd.Quantity);
                        }
                        //新的数量生效
                        if (profitLossBillDetail.Quantity > 0)
                        {
                            storage.InFrozenQuantity += profitLossBillDetail.Quantity * unit.Count;
                        }
                        else
                        {
                            storage.OutFrozenQuantity += Math.Abs(profitLossBillDetail.Quantity * unit.Count);
                        }
                        pbd.Quantity = profitLossBillDetail.Quantity * unit.Count;
                        pbd.Description = profitLossBillDetail.Description;
                        ProfitLossBillDetailRepository.SaveChanges();
                        result = true;
                    }
                }
                strResult = resultStr;
            }
            catch (Exception ex)
            {
                strResult = "修改失败，原因：" + ex.Message;
            }
            return result;
        }

        #endregion

        #region IProfitLossBillDetailService 成员
        /// <summary>获得损益细单信息</summary>
        public System.Data.DataTable GetProfitLoassBillDetail(int page, int rows, string BillNo)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<ProfitLossBillDetail> ProfitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
                var profitLossBillDetail = ProfitLossBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(i => new
                {
                    i.CellCode,
                    i.StorageCode,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.Unit.UnitName,
                    i.Price,
                    Quantity = i.Quantity / i.Unit.Count,
                    i.Description
                });
                dt.Columns.Add("储位编码", typeof(string));
                dt.Columns.Add("存储编码", typeof(string));
                dt.Columns.Add("产品代码", typeof(string));
                dt.Columns.Add("产品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("单价", typeof(string));
                dt.Columns.Add("数量", typeof(string));
                dt.Columns.Add("备注", typeof(string));
                foreach (var p in profitLossBillDetail)
                {
                    dt.Rows.Add
                        (
                            p.CellCode,
                            p.StorageCode,
                            p.ProductCode,
                            p.ProductName,
                            p.UnitCode,
                            p.UnitName,
                            p.Price,
                            p.Quantity,
                            p.Description
                        );
                }
            }
            return dt;
        } 
        #endregion
    }
}

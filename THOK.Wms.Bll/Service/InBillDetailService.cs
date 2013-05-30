using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class InBillDetailService : ServiceBase<InBillDetail>, IInBillDetailService
    {
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }
        [Dependency]
        public IUnitRepository UnitRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IInBillDetailService 成员

        public object GetDetails(int page, int rows, string BillNo)
        {
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                var inBillDetail = inBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(i => i);
                int total = inBillDetail.Count();
                inBillDetail = inBillDetail.Skip((page - 1) * rows).Take(rows);

                var temp = inBillDetail.ToArray().AsEnumerable().Select(i => new
                {
                    i.ID,
                    i.BillNo,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.Unit.UnitName,
                    BillQuantity = i.BillQuantity / i.Unit.Count,
                    RealQuantity = i.RealQuantity / i.Unit.Count,
                    AllotQuantity = i.AllotQuantity / i.Unit.Count,
                    i.Price,
                    i.Description
                });
                return new { total, rows = temp.ToArray() };
            }
            return "";
        }

        public bool Add(InBillDetail inBillDetail, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                var isExistProduct = inBillDetailQuery.FirstOrDefault(i => i.BillNo == inBillDetail.BillNo && i.ProductCode == inBillDetail.ProductCode);
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == inBillDetail.UnitCode);
                if (isExistProduct == null)
                {
                    var ibd = new InBillDetail();
                    ibd.BillNo = inBillDetail.BillNo;
                    ibd.ProductCode = inBillDetail.ProductCode;
                    ibd.UnitCode = inBillDetail.UnitCode;
                    ibd.Price = inBillDetail.Price;
                    ibd.BillQuantity = inBillDetail.BillQuantity * unit.Count;
                    ibd.AllotQuantity = 0;
                    ibd.RealQuantity = 0;
                    ibd.Description = inBillDetail.Description;

                    InBillDetailRepository.Add(ibd);
                    InBillDetailRepository.SaveChanges();
                    result = true;
                }
                else
                {
                    var ibd = inBillDetailQuery.FirstOrDefault(i => i.BillNo == inBillDetail.BillNo && i.ProductCode == inBillDetail.ProductCode);
                    ibd.UnitCode = inBillDetail.UnitCode;
                    ibd.BillQuantity = ibd.BillQuantity + inBillDetail.BillQuantity * unit.Count;
                    InBillDetailRepository.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                strResult = "新增失败，原因：" + ex.Message;
            }
            return result;
        }

        public bool Delete(string ID, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                int intID = Convert.ToInt32(ID);
                var ibd = inBillDetailQuery.FirstOrDefault(i => i.ID == intID);
                InBillDetailRepository.Delete(ibd);
                InBillDetailRepository.SaveChanges();
                result = true;
            }
            catch (Exception ex)
            {
                strResult = "删除失败，原因：" + ex.Message;
            }
            return result;
        }

        public bool Save(InBillDetail inBillDetail, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                var ibd = inBillDetailQuery.FirstOrDefault(i => i.BillNo == inBillDetail.BillNo && i.ProductCode == inBillDetail.ProductCode);
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == inBillDetail.UnitCode);
                if ((ibd != null && ibd.ID == inBillDetail.ID) || ibd == null)
                {
                    if (ibd == null)
                    {
                        ibd = inBillDetailQuery.FirstOrDefault(i => i.BillNo == inBillDetail.BillNo && i.ID == inBillDetail.ID);
                    }
                    ibd.BillNo = inBillDetail.BillNo;
                    ibd.ProductCode = inBillDetail.ProductCode;
                    ibd.UnitCode = inBillDetail.UnitCode;
                    ibd.Price = inBillDetail.Price;
                    ibd.BillQuantity = inBillDetail.BillQuantity * unit.Count;
                    ibd.Description = inBillDetail.Description;
                    InBillDetailRepository.SaveChanges();
                    result = true;
                }
                else if (ibd != null && ibd.ID != inBillDetail.ID)
                {
                    bool delDetail = this.Delete(inBillDetail.ID.ToString(), out strResult);
                    ibd.BillNo = inBillDetail.BillNo;
                    ibd.ProductCode = inBillDetail.ProductCode;
                    ibd.UnitCode = inBillDetail.UnitCode;
                    ibd.Price = inBillDetail.Price;
                    ibd.BillQuantity = ibd.BillQuantity + inBillDetail.BillQuantity * unit.Count;
                    ibd.Description = inBillDetail.Description;
                    InBillDetailRepository.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                strResult = "修改失败，原因：" + ex.Message;
            }
            return result;
        }

        #endregion

        #region IInBillDetailService 成员


        public object GetProductDetails(int page, int rows, string QueryString, string Value)
        {
            string ProductName = "";
            string ProductCode = "";
            if (QueryString == "ProductCode")
            {
                ProductCode = Value;
            }
            else
            {
                ProductName = Value;
            }
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.Where(c => c.ProductName.Contains(ProductName) && c.ProductCode.Contains(ProductCode) && c.IsActive == "1")
                .OrderBy(c => c.ProductCode)
                .Select(c => c);
            int total = product.Count();
            product = product.Skip((page - 1) * rows).Take(rows);

            var temp = product.ToArray().Select(c => new
            {
                c.AbcTypeCode,
                c.BarBarcode,
                c.BelongRegion,
                c.BrandCode,
                c.BuyPrice,
                c.CostPrice,
                c.CustomCode,
                c.Description,
                IsAbnormity = c.IsAbnormity == "1" ? "是" : "不是",
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                c.IsConfiscate,
                c.IsFamous,
                c.IsFilterTip,
                c.IsMainProduct,
                c.IsNew,
                c.IsProvinceMainProduct,
                c.OneProjectBarcode,
                c.PackageBarcode,
                c.PackTypeCode,
                c.PieceBarcode,
                c.PriceLevelCode,
                c.ProductCode,
                c.ProductName,
                c.ProductTypeCode,
                c.RetailPrice,
                c.ShortCode,
                c.StatisticType,
                c.SupplierCode,
                c.TradePrice,
                c.UniformCode,
                c.UnitCode,
                c.Unit.UnitName,
                c.UnitListCode,
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        #endregion

        #region IInBillDetailService 成员
        public System.Data.DataTable GetInBillDetail(int page, int rows, string BillNo)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<InBillDetail> inBillDetailQuery = InBillDetailRepository.GetQueryable();
                var inBillDetail = inBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(i => i);
                var temp = inBillDetail.ToArray().AsEnumerable().Select(i => new
                {
                    i.BillNo,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.UnitCode,
                    i.Unit.UnitName,
                    BillQuantity = i.BillQuantity / i.Unit.Count,
                    RealQuantity = i.RealQuantity / i.Unit.Count,
                    AllotQuantity = i.AllotQuantity / i.Unit.Count,
                    i.Price,
                    i.Description
                });
                dt.Columns.Add("商品编码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("订单数量", typeof(decimal));
                dt.Columns.Add("已分配数量", typeof(decimal));
                dt.Columns.Add("实际入库量", typeof(decimal));
                dt.Columns.Add("备注", typeof(string));
                foreach (var item in temp)
                {
                    dt.Rows.Add
                        (
                            item.ProductCode,
                            item.ProductName,
                            item.UnitCode,
                            item.UnitName,
                            item.BillQuantity,
                            item.AllotQuantity,
                            item.RealQuantity,
                            item.Description
                        );
                }
                if (temp.Count() > 0)
                {
                    dt.Rows.Add(
                        null, null, null, "总数：",
                        temp.Sum(m => m.BillQuantity), 
                        temp.Sum(m => m.AllotQuantity),
                        temp.Sum(m => m.RealQuantity),
                        null);
                }
            }
            return dt;
        }
        #endregion
    }
}

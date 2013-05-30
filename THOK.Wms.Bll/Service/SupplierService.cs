using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SupplierService : ServiceBase<Supplier>, ISupplierService
    {
        [Dependency]
        public ISupplierRepository SupplierRepository { get; set; }


        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISupplierService 成员

        public object GetDetails(int page, int rows, string SupplierCode, string SupplierName, string IsActive)
        {
            IQueryable<Supplier> supplierQuery = SupplierRepository.GetQueryable();
            var supplier = supplierQuery.Where(s => s.SupplierCode.Contains(SupplierCode)
                && s.SupplierName.Contains(SupplierName))
                .OrderBy(s => s.SupplierCode).Select(s => s);
            if (!IsActive.Equals(""))
            {
                supplier = supplier.Where(s => s.IsActive.Contains(IsActive));
            }
            int total = supplier.Count();
            supplier = supplier.Skip((page - 1) * rows).Take(rows);

            var temp = supplier.ToArray().Select(s => new
            {
                s.SupplierCode,
                s.UniformCode,
                s.CustomCode,
                s.SupplierName,
                s.ProvinceName,
                IsActive = s.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        public new bool Add(Supplier supplier)
        {
            var su = new Supplier();
            su.SupplierCode = supplier.SupplierCode;
            su.UniformCode = supplier.UniformCode;
            su.CustomCode = supplier.CustomCode;
            su.SupplierName = supplier.SupplierName;
            su.ProvinceName = supplier.ProvinceName;
            su.IsActive = supplier.IsActive;
            su.UpdateTime = DateTime.Now;

            SupplierRepository.Add(su);
            SupplierRepository.SaveChanges();
            return true;
        }

        public bool Delete(string SupplierCode)
        {
            var supplier = SupplierRepository.GetQueryable()
                .FirstOrDefault(s => s.SupplierCode == SupplierCode);
            if (SupplierCode != null)
            {
                SupplierRepository.Delete(supplier);
                SupplierRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(Supplier supplier)
        {
            var su = SupplierRepository.GetQueryable().FirstOrDefault(s => s.SupplierCode == supplier.SupplierCode);
            su.UniformCode = supplier.UniformCode;
            su.CustomCode = supplier.CustomCode;
            su.SupplierName = supplier.SupplierName;
            su.ProvinceName = supplier.ProvinceName;
            su.IsActive = supplier.IsActive;
            su.UpdateTime = DateTime.Now;

            SupplierRepository.SaveChanges();
            return true;
        }

        #endregion

        public System.Data.DataTable GetSupplier(int page, int rows, string SupplierCode, string SupplierName, string IsActive)
        {
            IQueryable<Supplier> supplierQuery = SupplierRepository.GetQueryable();
            var supplier = supplierQuery.Where(s => s.SupplierCode.Contains(SupplierCode)
                && s.SupplierName.Contains(SupplierName))
                .OrderBy(s => s.SupplierCode).Select(s => s);
            if (!IsActive.Equals(""))
            {
                supplier = supplier.Where(s => s.IsActive.Contains(IsActive));
            }
            var temp = supplier.ToArray().Select(s => new
            {
                s.SupplierCode,
                s.UniformCode,
                s.CustomCode,
                s.SupplierName,
                s.ProvinceName,
                IsActive = s.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("统一编码", typeof(string));
            dt.Columns.Add("自定义编码", typeof(string));
            dt.Columns.Add("厂商名称", typeof(string));
            dt.Columns.Add("省份名称", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add
                    (
                        item.UniformCode,
                        item.CustomCode,
                        item.SupplierName,
                        item.ProvinceName,
                        item.IsActive,
                        item.UpdateTime
                    );                
            }
            return dt;
        }
    }
}


 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;

namespace THOK.Wms.Bll.Service
{
    public class ATBillDetailService : ServiceBase<ATBillDetail>, IATBillDetailService
    {
        [Dependency]
        public IATBillDetailRepository iatBillDetail { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string WMS_BILL_MASTER_ID)
        {
            //AsEnumerable().
            IQueryable<ATBillDetail> atBillDetail = iatBillDetail.GetQueryable();
            var DB = atBillDetail.OrderBy(b => b.WMS_BILL_MASTER_ID).Select(b => b);
            if (!WMS_BILL_MASTER_ID.Equals(""))
                DB = DB.Where(b => b.WMS_BILL_MASTER_ID == WMS_BILL_MASTER_ID.Trim());
            DB=DB.Skip((page - 1) * rows).Take(rows);
            return DB.ToArray(); 
          
        }


        public bool Add(ATBillDetail ATBillDetail, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var atBD = new ATBillDetail();
            try
            {
                atBD.AMOUNT = ATBillDetail.AMOUNT;
                atBD.ITEM_ORDER = ATBillDetail.ITEM_ORDER;
                atBD.PRODUCT_CODE = ATBillDetail.PRODUCT_CODE;
                atBD.QUANTITY = ATBillDetail.QUANTITY;
                atBD.REAL_QUANTITY = ATBillDetail.REAL_QUANTITY;
                atBD.UNIT_PRICE = ATBillDetail.UNIT_PRICE;
                atBD.WMS_BILL_DETAIL_ID = ATBillDetail.WMS_BILL_DETAIL_ID;
                atBD.WMS_BILL_MASTER_ID = ATBillDetail.WMS_BILL_MASTER_ID;
                iatBillDetail.Add(atBD);
                iatBillDetail.SaveChanges();

                result = true;
            }
            catch (Exception ex)
            {
                strResult = "新增失败，原因：" + ex.Message;
            }
            return result;

        }

        public bool Delete(string ID, out string strResult)
        {
            bool Result = true;
            strResult = "";
            return Result;
        }

        public bool Save(ATBillDetail ATBillDetail, out string strResult)
        {
            bool Result = true;
            strResult = "";
            return Result;
        }




        public object GetInBillDetail(int page, int rows, string WMS_BILL_MASTER_ID)
        {
            IQueryable<ATBillDetail> atBillDetail = iatBillDetail.GetQueryable();
            var DB = atBillDetail.OrderBy(b => b.WMS_BILL_MASTER_ID).AsEnumerable().Select(b =>b).Skip((page - 1) * rows).Take(rows);
            if (!WMS_BILL_MASTER_ID.Equals(""))
                DB = DB.Where(b => b.WMS_BILL_MASTER_ID == WMS_BILL_MASTER_ID.Trim());
            return DB.ToArray(); 
        }




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
    }
}

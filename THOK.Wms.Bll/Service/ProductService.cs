using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.WMS.DownloadWms.Bll;
using System.Data;
namespace THOK.Wms.Bll.Service
{
    public class ProductService:ServiceBase<Product>,IProductService
    {
        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        DownProductBll Product = new DownProductBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IProductService 增，删，改，查等方法

        /// <summary>
        /// 判断卷烟调度时的取整类型
        /// </summary>
        /// <param name="type">类型代码</param>
        /// <returns>取整类型</returns>
        public string WhatRoundingType(string type)
        {
            string typeStr = "";
            switch (type)
            {
                case "0":
                    typeStr = "取整件";
                    break;
                case "1":
                    typeStr = "不取整";
                    break;
                case "2":
                    typeStr = "取整托盘";
                    break;
            }
            return typeStr;
        }

        public object GetDetails(int page, int rows, string ProductName, string ProductCode, string CustomCode, string BrandCode, string UniformCode, string AbcTypeCode, string ShortCode, string PriceLevelCode, string SupplierCode)
        {
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.Where(c => c.ProductName.Contains(ProductName)
                && c.ProductCode.Contains(ProductCode)
                && c.BrandCode.Contains(BrandCode)
                && c.UniformCode.Contains(UniformCode)
                && c.SupplierCode.Contains(SupplierCode))
                .OrderBy(c => c.ProductCode)
                .Select(c => c);
            if (!CustomCode.Equals(string.Empty))
            {
                product = product.Where(p => p.CustomCode == CustomCode);
            }
            if (!AbcTypeCode.Equals(string.Empty))
            {
                product = product.Where(p => p.AbcTypeCode == AbcTypeCode);
            }
            if (!UniformCode.Equals(string.Empty))
            {
                product = product.Where(p => p.UniformCode == UniformCode);
            }
            if (!PriceLevelCode.Equals(string.Empty))
            {
                product = product.Where(p => p.PriceLevelCode == PriceLevelCode);
            }
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
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                IsRounding = WhatRoundingType(c.IsRounding)
            });
            return new { total, rows = temp.ToArray() };
        }
        public bool Add(Product product)
        {
            var prod = new Product();
            prod.AbcTypeCode = product.AbcTypeCode;
            prod.BarBarcode = product.BarBarcode;
            prod.BelongRegion = product.BelongRegion;
            prod.BrandCode = product.BrandCode;

            prod.BuyPrice = product.BuyPrice;
            prod.CostPrice = product.CostPrice;
            prod.CustomCode = product.CustomCode;
            prod.Description = product.Description;
            prod.IsAbnormity = product.IsAbnormity;
            prod.IsActive = product.IsActive;
            prod.IsConfiscate = product.IsConfiscate;
            prod.IsFamous = product.IsFamous;
            prod.IsFilterTip = product.IsFilterTip;
            prod.IsMainProduct = product.IsMainProduct;
            prod.IsNew = product.IsNew;
            prod.IsProvinceMainProduct = product.IsProvinceMainProduct;
            prod.OneProjectBarcode = product.OneProjectBarcode;
            prod.PackageBarcode = product.PackageBarcode;
            prod.PackTypeCode = product.PackTypeCode;
            prod.PieceBarcode = product.PieceBarcode;
            prod.PriceLevelCode = product.PriceLevelCode;
            prod.ProductCode = product.ProductCode;
            prod.ProductName = product.ProductName;
            prod.ProductTypeCode = product.ProductTypeCode;
            prod.RetailPrice = product.RetailPrice;
            prod.ShortCode = product.ShortCode;
            prod.StatisticType = product.StatisticType;
            prod.SupplierCode = product.SupplierCode;
            prod.TradePrice = product.TradePrice;
            prod.UniformCode = product.UniformCode;
            prod.UnitCode = product.UnitCode;
            prod.UnitListCode = product.UnitListCode;
            prod.UpdateTime = DateTime.Now;
            prod.IsRounding = product.IsRounding;

            ProductRepository.Add(prod);
            ProductRepository.SaveChanges();
            //产品信息上报
            //DataSet ds = this.Insert(prod);
            //Product.InsertProduct(ds);
            return true;
        }
        public bool Delete(string ProductCode)
        {
            var product = ProductRepository.GetQueryable()
                .FirstOrDefault(b => b.ProductCode == ProductCode);
            if (ProductCode != null)
            {
                ProductRepository.Delete(product);
                ProductRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
        public bool Save(Product product)
        {
            var prod = ProductRepository.GetQueryable().FirstOrDefault(b => b.ProductCode == product.ProductCode);
            prod.AbcTypeCode = product.AbcTypeCode;
            prod.BarBarcode = product.BarBarcode;
            prod.BelongRegion = product.BelongRegion;
            prod.BrandCode = product.BrandCode;
            prod.BuyPrice = product.BuyPrice;
            prod.CostPrice = product.CostPrice;
            prod.CustomCode = product.CustomCode;
            prod.Description = product.Description;
            prod.IsAbnormity = product.IsAbnormity;
            prod.IsActive = product.IsActive;
            prod.IsConfiscate = product.IsConfiscate;
            prod.IsFamous = product.IsFamous;
            prod.IsFilterTip = product.IsFilterTip;
            prod.IsMainProduct = product.IsMainProduct;
            prod.IsNew = product.IsNew;
            prod.IsProvinceMainProduct = product.IsProvinceMainProduct;
            prod.OneProjectBarcode = product.OneProjectBarcode;
            prod.PackageBarcode = product.PackageBarcode;
            prod.PackTypeCode = product.PackTypeCode;
            prod.PieceBarcode = product.PieceBarcode;
            prod.PriceLevelCode = product.PriceLevelCode;
            prod.ProductCode = product.ProductCode;
            prod.ProductName = product.ProductName;
            prod.ProductTypeCode = product.ProductTypeCode;
            prod.RetailPrice = product.RetailPrice;
            prod.ShortCode = product.ShortCode;
            prod.StatisticType = product.StatisticType;
            prod.SupplierCode = product.SupplierCode;
            prod.TradePrice = product.TradePrice;
            prod.UniformCode = product.UniformCode;
            prod.UnitCode = product.UnitCode;
            prod.UnitListCode = product.UnitListCode;
            prod.UpdateTime = DateTime.Now;
            prod.IsRounding = product.IsRounding;
            ProductRepository.SaveChanges();
            //产品信息上报
            //DataSet ds = this.Insert(prod);
            //Product.InsertProduct(ds);
            return true;
        }
        #endregion

        #region 插入数据到虚拟表
        public DataSet Insert(Product product)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["WMS_PRODUCT"].NewRow();
            inbrddr["product_code"] = product.ProductCode;
            inbrddr["product_name"] = product.ProductName;
            inbrddr["uniform_code"] = product.UniformCode;
            inbrddr["custom_code"] = product.CustomCode ?? "";
            inbrddr["short_code"] = product.ShortCode ?? "";
            inbrddr["unit_list_code"] = product.UnitListCode;
            inbrddr["unit_code"] = product.UnitCode;
            inbrddr["supplier_code"] = product.SupplierCode;
            inbrddr["brand_code"] = product.BrandCode;
            inbrddr["abc_type_code"] = product.AbcTypeCode ?? "";
            inbrddr["product_type_code"] = product.ProductTypeCode ?? "";
            inbrddr["pack_type_code"] = product.PackTypeCode ?? "";
            inbrddr["price_level_code"] = product.PriceLevelCode ?? "";
            inbrddr["statistic_type"] = product.StatisticType ?? "";
            inbrddr["piece_barcode"] = product.PieceBarcode ?? "";
            inbrddr["bar_barcode"] = product.BarBarcode ?? "";
            inbrddr["package_barcode"] = product.PackageBarcode ?? "";
            inbrddr["one_project_barcode"] = product.OneProjectBarcode ?? "";
            inbrddr["buy_price"] = string.IsNullOrEmpty(product.BuyPrice.ToString()) ? 0 : product.BuyPrice;
            inbrddr["trade_price"] = string.IsNullOrEmpty(product.TradePrice.ToString()) ? 0 : product.TradePrice;
            inbrddr["retail_price"] = string.IsNullOrEmpty(product.RetailPrice.ToString()) ? 0 : product.RetailPrice;
            inbrddr["cost_price"] = string.IsNullOrEmpty(product.CostPrice.ToString()) ? 0 : product.CostPrice;
            inbrddr["is_filter_tip"] = product.IsFilterTip;
            inbrddr["is_new"] = product.IsNew;
            inbrddr["is_famous"] = product.IsFamous;
            inbrddr["is_main_product"] = product.IsMainProduct;
            inbrddr["is_province_main_product"] = product.IsProvinceMainProduct;
            inbrddr["belong_region"] = product.BelongRegion;
            inbrddr["is_confiscate"] = product.IsConfiscate;
            inbrddr["is_abnormity"] = product.IsAbnormity;
            inbrddr["description"] = product.Description;
            inbrddr["is_active"] = product.IsActive;
            inbrddr["update_time"] = DateTime.Now;
            ds.Tables["WMS_PRODUCT"].Rows.Add(inbrddr);
            return ds;
        }
        #endregion

        #region 创建一个空的产品表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("WMS_PRODUCT");
            inbrtable.Columns.Add("product_code");
            inbrtable.Columns.Add("product_name");
            inbrtable.Columns.Add("uniform_code");
            inbrtable.Columns.Add("custom_code");
            inbrtable.Columns.Add("short_code");
            inbrtable.Columns.Add("unit_list_code");
            inbrtable.Columns.Add("unit_code");
            inbrtable.Columns.Add("supplier_code");
            inbrtable.Columns.Add("brand_code");
            inbrtable.Columns.Add("abc_type_code");
            inbrtable.Columns.Add("product_type_code");
            inbrtable.Columns.Add("pack_type_code");
            inbrtable.Columns.Add("price_level_code");
            inbrtable.Columns.Add("statistic_type");
            inbrtable.Columns.Add("piece_barcode");
            inbrtable.Columns.Add("bar_barcode");
            inbrtable.Columns.Add("package_barcode");
            inbrtable.Columns.Add("one_project_barcode");
            inbrtable.Columns.Add("buy_price");
            inbrtable.Columns.Add("trade_price");
            inbrtable.Columns.Add("retail_price");
            inbrtable.Columns.Add("cost_price");
            inbrtable.Columns.Add("is_filter_tip");
            inbrtable.Columns.Add("is_new");
            inbrtable.Columns.Add("is_famous");
            inbrtable.Columns.Add("is_main_product");
            inbrtable.Columns.Add("is_province_main_product");
            inbrtable.Columns.Add("belong_region");
            inbrtable.Columns.Add("is_confiscate");
            inbrtable.Columns.Add("is_abnormity");
            inbrtable.Columns.Add("description");
            inbrtable.Columns.Add("is_active");
            inbrtable.Columns.Add("update_time");

            return ds;
        }
        #endregion

        #region IProductService 成员

        /// <summary>
        /// 查询卷烟信息 zxl 2012年7月24日 16:26:24
        /// </summary>
        /// <returns></returns>
        public object FindProduct(int page, int rows, string QueryString, string value)
        {
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.OrderBy(p => p.ProductCode).Where(p => p.ProductCode == p.ProductCode);

            if (QueryString == "ProductCode")
            {
                var storages = StorageRepository.GetQueryable().OrderBy(s => s.ProductCode).Where(s => s.ProductCode == value).GroupBy(s=>s.ProductCode).Select(s => s.Key);
                product = product.Where(p => storages.Any(s => s == p.ProductCode));
            }
            else if (QueryString == "ProductName")
            {
                var storages = StorageRepository.GetQueryable().OrderBy(s => s.ProductCode).Where(s => s.Product.ProductName.Contains(value)).GroupBy(s => s.ProductCode).Select(s => s.Key);
                product = product.Where(p => storages.Any(s => s == p.ProductCode));
            }
            if (QueryString == string.Empty || QueryString == null || value == string.Empty || value == null)
            {
                var storages = StorageRepository.GetQueryable().OrderBy(s => s.ProductCode).Where(s => s.Product != null).GroupBy(s => s.ProductCode).Select(s => s.Key);
                product = ProductQuery.Where(p => storages.Any(s => s == p.ProductCode));
            }
            var temp = product.OrderBy(p => p.ProductCode).Select(c=>c);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);

            var tmp = temp.ToArray().Select(c => new
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
            return new { total, rows = tmp.ToArray() };
        }

        /// <summary>
        /// 产品盘点显示卷烟信息，入库新增显示卷烟数据
        /// </summary>
        /// <returns></returns>
        public object checkFindProduct(string QueryString, string value)
        {
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            IQueryable<Storage> StorageQuery = StorageRepository.GetQueryable();
            string ProductName = "";
            string ProductCode = "";
            if (QueryString == "ProductCode")
            {
                ProductCode = value;
            }
            else
            {
                ProductName = value;
            }
            var storage = StorageQuery.Join(ProductQuery,
                                           s => s.ProductCode,
                                           p => p.ProductCode,
                                           (s, p) => new { p.ProductCode, p.ProductName, s.Quantity, s.Product, p.Unit, p.BuyPrice }
                                           ).GroupBy(s => new { s.ProductCode, s.ProductName, s.Unit, s.BuyPrice })
                                           .Select(s => new
                                           {
                                               ProductCode = s.Key.ProductCode,
                                               ProductName = s.Key.ProductName,
                                               UnitCode = s.Key.Unit.UnitCode,
                                               UnitName = s.Key.Unit.UnitName,
                                               BuyPrice = s.Key.BuyPrice,
                                               Quantity = s.Sum(st => (st.Quantity / st.Product.Unit.Count))
                                           }).Where(p=>p.ProductCode.Contains(ProductCode)&&p.ProductName.Contains(ProductName));
            // var product = ProductQuery.OrderBy(p => p.ProductCode).Where(p => p.Storages.Any(s => s.ProductCode == p.ProductCode));
            return storage.ToArray();
        }

        /// <summary>浏览加载卷烟信息</summary>
        public object LoadProduct(int page, int rows)
        {
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.OrderBy(p => p.ProductCode).Select(p => new { p.ProductCode, p.ProductName });
            int total = product.Count();
            product = product.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = product.ToArray() };
        }
        

        /// <summary>获取卷烟信息</summary>
        public object GetProductBy(int page, int rows, string QueryString, string Value)
        {
            string productCode = "", productName = "";
            
            if (QueryString == "ProductCode")
            {
                productCode = Value;
            }
            else
            {
                productName = Value;
            }
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.Where(c => c.ProductCode.Contains(productCode) && c.ProductName.Contains(productName))
                .OrderBy(c => c.ProductCode)
                .Select(c => new { 
                    c.ProductCode,
                    c.ProductName
                });
            int total = product.Count();
            product = product.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = product.ToArray() };
        }

        #endregion


        public object FindProduct()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="ProductName"></param>
        /// <param name="ProductCode"></param>
        /// <param name="CustomCode"></param>
        /// <param name="BrandCode"></param>
        /// <param name="UniformCode"></param>
        /// <param name="AbcTypeCode"></param>
        /// <param name="ShortCode"></param>
        /// <param name="PriceLevelCode"></param>
        /// <param name="SupplierCode"></param>
        /// <returns></returns>
        public System.Data.DataTable GetProduct(int page, int rows, string ProductName, string ProductCode, string CustomCode, string BrandCode, string UniformCode, string AbcTypeCode, string ShortCode, string PriceLevelCode, string SupplierCode)
        {
            IQueryable<Product> ProductQuery = ProductRepository.GetQueryable();
            var product = ProductQuery.Where(c => c.ProductName.Contains(ProductName)
                && c.ProductCode.Contains(ProductCode)
                && c.BrandCode.Contains(BrandCode)
                && c.UniformCode.Contains(UniformCode)
                && c.SupplierCode.Contains(SupplierCode))
                .OrderBy(c => c.ProductCode)
                .Select(c => c);
            if (!CustomCode.Equals(string.Empty))
            {
                product = product.Where(p => p.CustomCode == CustomCode);
            }
            if (!AbcTypeCode.Equals(string.Empty))
            {
                product = product.Where(p => p.AbcTypeCode == AbcTypeCode);
            }
            if (!UniformCode.Equals(string.Empty))
            {
                product = product.Where(p => p.UniformCode == UniformCode);
            }
            if (!PriceLevelCode.Equals(string.Empty))
            {
                product = product.Where(p => p.PriceLevelCode == PriceLevelCode);
            }

            var temp = product.ToArray().Select(c => new
            {
                c.BrandCode,
                IsAbnormity = c.IsAbnormity == "1" ? "是" : "不是",
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                c.ProductCode,
                c.ProductName,
                c.ProductTypeCode,
                c.RetailPrice,
                c.SupplierCode,
                c.UniformCode,
                c.UnitCode,
                c.Unit.UnitName,
                c.UnitListCode,
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("商品编码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("统一编码", typeof(string));
            dt.Columns.Add("计量单位系列", typeof(string));
            dt.Columns.Add("缺省计量单位", typeof(string));
            dt.Columns.Add("厂商", typeof(string));
            dt.Columns.Add("商品品牌", typeof(string));
            dt.Columns.Add("异形烟", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add
                    (
                        item.ProductCode,
                        item.ProductName,
                        item.UniformCode,
                        item.UnitListCode,
                        item.UnitCode,
                        item.SupplierCode,
                        item.BrandCode,
                        item.IsAbnormity,
                        item.IsActive,
                        item.UpdateTime
                    );
            }
            return dt;
        }
    }
}

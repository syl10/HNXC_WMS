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
    public class CMDProductService:ServiceBase<CMD_PRODUCT>,ICMDProductService
    {
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }

        //[Dependency]
        //public IStorageRepository StorageRepository { get; set; }

      //  DownProductBll Product = new DownProductBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IProductService 增，删，改，查等方法

        public object GetDetails(int page, int rows, string ProductName, string ORIGINAL, string YEARS, string GRADE, string STYLE, string WEIGHT, string MEMO, string CATEGORY_CODE)
        {
           IQueryable<CMD_PRODUCT> ProductQuery = ProductRepository.GetQueryable();

           var products = ProductQuery.OrderBy(i => i.PRODUCT_CODE).Select(i => new {
               i.PRODUCT_CODE, 
               i.PRODUCT_NAME, 
               i.YEARS,
               i.WEIGHT,
               i.STYLE_NO ,
               i.CMD_PRODUCT_STYLE .STYLE_NAME,
               i.ORIGINAL_CODE,
               ORIGINAL=i.CMD_PRODUCT_ORIGINAL .ORIGINAL_NAME , 
               i.GRADE_CODE,GRADE=i.CMD_PRODUCT_GRADE .GRADE_NAME , 
               i.MEMO, 
               i.CATEGORY_CODE, 
               CATEGORYNAME = i.CMD_PRODUCT_CATEGORY.CATEGORY_NAME
           });

           if (!string.IsNullOrEmpty(ProductName))
           {
               products = products.Where(i => i.PRODUCT_NAME.Contains(ProductName));
           }
           if (!string.IsNullOrEmpty(ORIGINAL))
           {
               products = products.Where(i => i.ORIGINAL_CODE .Contains(ORIGINAL));
           }
           if (!string.IsNullOrEmpty(YEARS))
           {
               products = products.Where(i => i.YEARS.Contains(YEARS));
           }
           if (!string.IsNullOrEmpty(GRADE))
           {
               products = products.Where(i => i.GRADE_CODE.Contains(GRADE));
           }
           if (!string.IsNullOrEmpty(STYLE))
           {
               products = products.Where(i => i.STYLE_NAME.Contains(STYLE));
           }
           if (!string.IsNullOrEmpty(WEIGHT))
           {
               products = products.Where(i => i.WEIGHT.ToString().Contains(WEIGHT));
           }
           if (!string.IsNullOrEmpty(MEMO))
           {
               products = products.Where(i => i.MEMO.Contains(MEMO));
           }
           if (!string.IsNullOrEmpty(CATEGORY_CODE))
           {
               products = products.Where(i => i.CATEGORY_CODE == CATEGORY_CODE);
           }
           products = products.Where(i => i.PRODUCT_CODE != "0000");
            int total = products.Count();
            products = products.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = products.ToArray() };
        }
       
        public bool Add(CMD_PRODUCT product)
        {
            product.PRODUCT_CODE = ProductRepository.GetNewID("CMD_PRODUCT", "PRODUCT_CODE");
            ProductRepository.Add(product);
            ProductRepository.SaveChanges();
            //产品信息上报
            //DataSet ds = this.Insert(prod);
            //Product.InsertProduct(ds);
            return true;
        }
        public bool Delete(string ProductCode)
        {
            var product = ProductRepository.GetQueryable()
                .FirstOrDefault(b => b.PRODUCT_CODE == ProductCode);
            if (ProductCode != null)
            {
                ProductRepository.Delete(product);
                ProductRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
        public bool Save(CMD_PRODUCT product)
        {
            var prod = ProductRepository.GetQueryable().FirstOrDefault(b => b.PRODUCT_CODE == product.PRODUCT_CODE);
            prod.PRODUCT_NAME = product.PRODUCT_NAME;
            prod.GRADE_CODE  = product.GRADE_CODE ;
            prod.ORIGINAL_CODE = product.ORIGINAL_CODE;
            prod.STYLE_NO = product.STYLE_NO;
            prod.WEIGHT = product.WEIGHT;
            prod.YEARS = product.YEARS;
            prod.CATEGORY_CODE = product.CATEGORY_CODE;
            prod.MEMO = product.MEMO;
            ProductRepository.SaveChanges();
            return true;
        }
        #endregion


        public object Selectprod(int page, int rows, string QueryString, string value)
        {
            IQueryable<CMD_PRODUCT> ProductQuery = ProductRepository.GetQueryable();
            var products = ProductQuery.OrderBy(i => i.PRODUCT_CODE).Select(i => new
            {
                i.PRODUCT_CODE,
                i.PRODUCT_NAME,
                i.YEARS,
                i.WEIGHT,
                i.STYLE_NO,
                i.CMD_PRODUCT_STYLE.STYLE_NAME,
                i.ORIGINAL_CODE,
                ORIGINAL = i.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                i.GRADE_CODE,
                GRADE = i.CMD_PRODUCT_GRADE.GRADE_NAME,
                i.MEMO,
                i.CATEGORY_CODE,
                CATEGORYNAME = i.CMD_PRODUCT_CATEGORY.CATEGORY_NAME
            });
            if (!string.IsNullOrEmpty(QueryString))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (QueryString == "ProductCode")
                    {
                        products = products.Where(i => i.PRODUCT_CODE == value);
                    }
                    if (QueryString == "ProductName")
                    {
                        products = products.Where(i => i.PRODUCT_NAME.Contains(value));
                    }
                }
            }
            products = products.Where(i => i.PRODUCT_CODE != "0000");
            int total = products.Count();
            products = products.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = products.ToArray() };
        }
    }
}

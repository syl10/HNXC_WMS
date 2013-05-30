using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Models;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using Microsoft.Practices.Unity;

namespace THOK.Wms.Bll.Service
{
    public class ATCmdProductService:ServiceBase<ATCmdProduct>,IATCmdProductService
    {
        [Dependency]
        public IATCmdProductRepository cmdProductRep { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        #region IATCmdProductRepository 成员

        public object GetDatails(int page, int rows, string CMD_PRODUCT_ID, string PRODUCT_CODE, string PRODUCT_NAME, string BAR_CODE, string PALLET_QUANTITY, string QUANTITY)
        // public object GetDatails(int page, int rows)
        {
            
            IQueryable<ATCmdProduct> cmdProduct = cmdProductRep.GetQueryable();
            var product = cmdProduct.OrderBy(b => b.BAR_CODE).AsEnumerable().Select(b => new { b.CMD_PRODUCT_ID, b.BAR_CODE, b.PRODUCT_NAME, b.PRODUCT_CODE, b.PALLET_QUANTITY, b.QUANTITY }).Skip((page - 1) * rows).Take(rows);
            if (!CMD_PRODUCT_ID.Equals(""))
                product = product.Where(b => b.CMD_PRODUCT_ID == CMD_PRODUCT_ID.Trim());
            if (!PRODUCT_CODE.Equals(""))
                product = product.Where(b => b.PRODUCT_CODE == PRODUCT_CODE.Trim());
            if (!PRODUCT_NAME.Equals(""))
                product = product.Where(b => b.PRODUCT_NAME == PRODUCT_NAME.Trim());
            if (!BAR_CODE.Equals(""))
                product = product.Where(b => b.BAR_CODE == BAR_CODE.Trim());
            if (!CMD_PRODUCT_ID.Equals(""))
                product = product.Where(b => b.CMD_PRODUCT_ID == CMD_PRODUCT_ID.Trim());
            if (!PALLET_QUANTITY.Equals(""))
                product = product.Where(b => b.PALLET_QUANTITY == int.Parse(PALLET_QUANTITY.Trim()));
            if (!QUANTITY.Equals(""))
                product = product.Where(b => b.QUANTITY == int.Parse(QUANTITY.Trim()));
           
            return product.ToArray();
        }
        public bool Add(string CMD_PRODUCT_ID,string PRODUCT_CODE,string PRODUCT_NAME,string BAR_CODE,string PALLET_QUANTITY,string QUANTITY)
        {
            var cmdPro = new ATCmdProduct();
            //cmdPro.CMD_PRODUCT_ID =cmdProduct.CMD_PRODUCT_ID;
            //cmdPro.BAR_CODE = cmdProduct.BAR_CODE;
            //cmdPro.PALLET_QUANTITY = cmdProduct.PALLET_QUANTITY;
            //cmdPro.PRODUCT_CODE = cmdProduct.PRODUCT_CODE;
            //cmdPro.PRODUCT_NAME = cmdProduct.PRODUCT_NAME;
            //cmdPro.QUANTITY = cmdProduct.QUANTITY;


            cmdPro.CMD_PRODUCT_ID = CMD_PRODUCT_ID;
            cmdPro.BAR_CODE = BAR_CODE;
            cmdPro.PALLET_QUANTITY =System.Convert.ToDecimal(PALLET_QUANTITY.Trim());
            cmdPro.PRODUCT_CODE = PRODUCT_CODE;
            cmdPro.PRODUCT_NAME = PRODUCT_NAME;
            cmdPro.QUANTITY = System.Convert.ToDecimal(QUANTITY.Trim());
            cmdProductRep.Add(cmdPro);
            cmdProductRep.SaveChanges();
            return true;
            }
        public bool Del(string CMD_PRODUCT_ID)
        {
            var product = cmdProductRep.GetQueryable().First(t => t.CMD_PRODUCT_ID == CMD_PRODUCT_ID);
            if (product != null)
            {
                cmdProductRep.Delete(product);
                cmdProductRep.SaveChanges();
                return true;
            }
            else
                return false;
        }

        public bool Save(ATCmdProduct cmdProduct) {
            var cmdPro = cmdProductRep.GetQueryable().First(t => t.CMD_PRODUCT_ID == cmdProduct.CMD_PRODUCT_ID);
            cmdPro.CMD_PRODUCT_ID = cmdProduct.CMD_PRODUCT_ID;
            cmdPro.BAR_CODE = cmdProduct.BAR_CODE;
            cmdPro.PALLET_QUANTITY = cmdProduct.PALLET_QUANTITY;
            cmdPro.PRODUCT_CODE = cmdProduct.PRODUCT_CODE;
            cmdPro.PRODUCT_NAME = cmdProduct.PRODUCT_NAME;
            cmdPro.QUANTITY = cmdProduct.QUANTITY;
       
            cmdProductRep.SaveChanges();
            return true;
        }

       // public object Search(string parameter);
        //public object GetCmdProuct();

        #endregion
    }
}

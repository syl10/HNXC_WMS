using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
namespace THOK.Wms.Bll.Interfaces
{
   public interface IATCmdProductService:IService<ATCmdProduct>
    {
      object GetDatails(int page, int rows,string CMD_PRODUCT_ID,string PRODUCT_CODE,string PRODUCT_NAME,string BAR_CODE,string PALLET_QUANTITY,string QUANTITY);
      //object GetDatails(int page, int rows);
      // bool Add(ATCmdProduct cmdProduct);
      bool Add(string CMD_PRODUCT_ID,string PRODUCT_CODE,string PRODUCT_NAME,string BAR_CODE,string PALLET_QUANTITY,string QUANTITY);
       bool Del(string CMD_PRODUCT_ID);
       bool Save(ATCmdProduct cmdProduct);
       //object FindArea(string parameter);
       //object GetCmdProuct();
    }
}

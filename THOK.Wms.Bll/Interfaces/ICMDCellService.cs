using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDCellService : IService<CMD_CELL>
    {
        object GetDetails(int page, int rows, string cellCode);

        object  GetDetail(int page, int rows, string type, string id);
        /// <summary>
        /// 点击获取单个信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        object GetSingleDetail(string type, string id);

        bool Add(CMD_CELL cell, out string errorInfo);

        bool Delete(string cellCode);

        bool Save(CMD_CELL cell);

        object GetWareCheck(string shelfCode);

        object GetSearch(string wareCode);

        object FindCell(string parameter);

        object GetCell(string shelfCode);

        object GetCellCode(string shelfCode);

        //object GetMoveCellDetails(string shelfCode, string inOrOut, string productCode);

        bool SaveCell(string wareCodes, string areaCodes, string shelfCodes, string cellCodes, string defaultProductCode, string editType);


        object GetCellInfo();

        object GetCellInfo(string productCode);

        object GetSortCell(string areaType);

        object GetCellCheck(string productCode);

        bool DeleteCell(string productCodes);

        bool SetTree2(string strId, string proCode);

        object GetCellBy(int page, int rows, string QueryString, string Value);

        System.Data.DataTable GetProductCell(string queryString, string value);

        System.Data.DataTable GetCell(int page, int rows, string type, string id);

        System.Data.DataTable GetCellByE(int page, int rows, string QueryString, string Value);
        //安排 获取货位
        object GetCellByshell(string shelfcode);
        //根据货位代码  获取对应的货物信息
        object Getproductbycellcode(string cellcode);
        //获取对应的带有错误标记的货位号
        object GeterrorCell(string BillNo, string productcode);
        //清空有异常货位上的产品信息
        bool ClearerrorCell(string BillNo,string cellcode, string inBillNo, string productcode,string tasker,ref string errorinfo);
        //补录数据
        bool Completedata(WMS_TASKRECORD taskrecord, string tasker,DateTime indate, ref string errorinfo);
        //获取某损益单下的作业明细
        object GetTaskrecordDetail(int page, int rows, string billno, string productcode);
      //  bool uploadCell();
    }
}

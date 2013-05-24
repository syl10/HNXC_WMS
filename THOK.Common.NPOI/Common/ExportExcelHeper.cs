using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data;
using NPOI.HSSF.UserModel;

namespace THOK.NPOI.Common
{
    public class ExportExcelHeper
    {
        #region 浏览器下载
        /// <summary>浏览器下载</summary>
        public static void BrowserLoad(string headText1)
        {
            string filename = headText1 + DateTime.Now.ToString("yyMMdd-HHmm-ss");
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.BufferOutput = false;
            response.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            response.AddHeader("Content-Disposition", "attachment;filename=" + Uri.EscapeDataString(filename) + ".xls");
            response.ContentType = "application/ms-excel";
        }
        #endregion

        #region SHEET分页
        public static DataTable SetPage(DataTable dt, int currentPageIndex, int pageSize)
        {
            if (currentPageIndex == 0)
            {
                return dt;
            }
            DataTable newdt = dt.Clone();
            int rowbegin = (currentPageIndex - 1) * pageSize;   //当前页的第一条数据在dt中的地位
            int rowend = currentPageIndex * pageSize;           //当前页的最后一条数据在dt中的地位

            if (rowbegin >= dt.Rows.Count)
            {
                return newdt;
            }
            if (rowend > dt.Rows.Count)
            {
                rowend = dt.Rows.Count;
            }
            DataView dv = dt.DefaultView;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                newdt.ImportRow(dv[i].Row);
            }
            return newdt;
        }
        #endregion

        #region 转换格式
        public static void ChangeFormat(DataColumn column, string drValue, HSSFCell newCell, HSSFCellStyle contentDateStyle)
        {
            switch (column.DataType.ToString())
            {
                case "System.String":   //字符串类型
                    string result = drValue;
                    newCell.SetCellValue(result);
                    break;
                case "System.DateTime": //日期类型
                    DateTime dateV;
                    DateTime.TryParse(drValue, out dateV);
                    newCell.SetCellValue(dateV);
                    newCell.CellStyle = contentDateStyle; //格式化显示
                    break;
                case "System.Boolean":  //布尔型
                    bool boolV = false;
                    bool.TryParse(drValue, out boolV);
                    newCell.SetCellValue(boolV);
                    break;
                case "System.Int16":    //整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV = 0;
                    int.TryParse(drValue, out intV);
                    newCell.SetCellValue(intV);
                    break;
                case "System.Decimal":  //浮点型
                case "System.Double":
                    double doubV = 0;
                    double.TryParse(drValue, out doubV);
                    newCell.SetCellValue(doubV);
                    break;
                case "System.DBNull":   //空值处理
                    newCell.SetCellValue("");
                    break;
                default:
                    newCell.SetCellValue("");
                    break;
            }
        }
        #endregion
    }
}

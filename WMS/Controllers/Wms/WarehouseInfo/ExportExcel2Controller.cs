using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;

using System.IO;
using System.Text;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.POIFS;
using NPOI.SS.UserModel;
using NPOI.Util;
using NPOI.SS;
using NPOI.DDF;
using NPOI.SS.Util;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data;
using NPOI.SS.Formula.Eval;
using System.Web.UI;
using THOK.Security;

namespace Wms.Controllers.Wms.WarehouseInfo
{
    public class ExportExcel2Controller : Controller
    {
        [Dependency]
        public ICellService CellService { get; set; }

        //
        // GET: /ExportExcel2/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public FileStreamResult CreateExcelToClient()
        {
            string queryString = Request.QueryString["queryString"];
            string value = Request.QueryString["value"];
            if (queryString == null) queryString = "ProductCode";
            if (value == null) value = "";
            System.Data.DataTable dt = CellService.GetProductCell(queryString, value);

            string excelname = "卷烟信息表";
            string filename = excelname + DateTime.Now.ToString("yyMMdd-HHmm-ss");
            Response.Clear();
            Response.BufferOutput = false;
            //Response.ContentEncoding = System.Text.Encoding.UTF8;
            //HttpUtility.UrlDecode(filename+".xls",System.Text.Encoding.UTF8); //乱码转换
            Response.ContentEncoding = Encoding.GetEncoding("gb2312");
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Uri.EscapeDataString(filename) + ".xls");
            Response.ContentType = "application/ms-excel";
            string[] str = {
                               "20",        //[0]大标题字体大小
                               "700",       //[1]大标题字体粗宽
                               "10",        //[2]列标题字体大小
                               "700",       //[3]列标题字体粗宽
                               "300",       //[4]excel中有数据表格的大小
                               "微软雅黑",  //[5]大标题字体
                               "Arial",     //[6]小标题字体
                           };
            return new FileStreamResult(ExportDT(dt, excelname, str), "application/ms-excel");
        }

        #region 导出到单表Excel公用方法
        /// <summary>DataTable导出到Excel的MemoryStream</summary>
        static MemoryStream ExportDT(DataTable dtSource, string strHeaderText, string[] str)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = workbook.CreateSheet() as HSSFSheet;

            HSSFCellStyle dateStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //取得列宽
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表，填充表头，填充列头，样式
                if (rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet() as HSSFSheet;
                    }
                    #region 表头及样式
                    {
                        HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                        headerRow.HeightInPoints = 25;
                        headerRow.CreateCell(0).SetCellValue(strHeaderText);

                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontName = str[5];                             //[5]
                        font.FontHeightInPoints = Convert.ToInt16(str[0]);  //[0]
                        font.Boldweight = Convert.ToInt16(str[1]);          //[1]
                        headStyle.SetFont(font);
                        headerRow.GetCell(0).CellStyle = headStyle;
                        sheet.AddMergedRegion(new Region(0, 0, 0, dtSource.Columns.Count - 1));
                        //headerRow.Dispose();
                    }
                    #endregion
                    #region 列头及样式
                    {
                        HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;
                        HSSFCellStyle headStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                        headStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
                        HSSFFont font = workbook.CreateFont() as HSSFFont;
                        font.FontName = str[6];                             //[6]
                        font.FontHeightInPoints = Convert.ToInt16(str[2]);  //[2]
                        font.Boldweight = Convert.ToInt16(str[3]);          //[3]
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * Convert.ToInt32(str[4]));  //[4]
                        }
                        //headerRow.Dispose();
                    }
                    #endregion
                    rowIndex = 2;
                }
                #endregion
                #region 填充内容
                HSSFRow dataRow = sheet.CreateRow(rowIndex) as HSSFRow;
                foreach (DataColumn column in dtSource.Columns)
                {
                    HSSFCell newCell = dataRow.CreateCell(column.Ordinal) as HSSFCell;
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String": //字符串类型
                            string result = drValue;
                            newCell.SetCellValue(result);
                            break;
                        case "System.DateTime": //日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle; //格式化显示
                            break;
                        case "System.Boolean": //布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16": //整型
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal": //浮点型
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull": //空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            MemoryStream ms = new MemoryStream();

            workbook.Write(ms);

            ms.Flush();
            ms.Position = 0;
            //sheet;
            //workbook.Dispose();
            return ms;

        }
        public static bool isNumeric(String message, out double result)
        {
            Regex rex = new Regex(@"^[-]?\d+[.]?\d*$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
        }
        #endregion
    }
}

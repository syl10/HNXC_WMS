using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;

namespace THOK.NPOI.Service
{
    public class ExportExcelTemplate
    {
        #region 使用模版导出EXCEL
        /// <summary>
        /// 使用模版导出EXCEL
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="excelTemplatePath"></param>
        /// <param name="sheetName1"></param>
        /// <param name="sheetName2"></param>
        /// <returns></returns>
        public static MemoryStream ExportFromTemplate(DataTable dt1, DataTable dt2
            , string excelTemplatePath
            , string sheetName1, string sheetName2)
        {
             THOK.NPOI.Common.ExportExcelHeper.BrowserLoad(sheetName1);

            FileStream file = new FileStream(excelTemplatePath, FileMode.Open, FileAccess.Read);//读入excel模板
            HSSFWorkbook hssfworkbook = new HSSFWorkbook(file);
            HSSFSheet sheet1 = (HSSFSheet)hssfworkbook.GetSheet(sheetName1);
            HSSFSheet sheet2 = (HSSFSheet)hssfworkbook.GetSheet(sheetName2);
            sheet1.PrintSetup.FitHeight = 0;
            sheet2.PrintSetup.FitHeight = 0;

            string exportDate = "导出时间：" + System.DateTime.Now.ToString("yyyy-MM-dd");
            sheet1.GetRow(0).GetCell(0).SetCellValue(sheetName1);
            sheet1.GetRow(1).GetCell(0).SetCellValue(exportDate);
            sheet2.GetRow(0).GetCell(0).SetCellValue(sheetName2);
            sheet2.GetRow(1).GetCell(0).SetCellValue(exportDate);

            #region 取得列宽 表一
            int[] arrColWidth = new int[dt1.Columns.Count];
            foreach (DataColumn item in dt1.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;//936是指GB2312编码
            }
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dt1.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            #endregion

            #region 取得列宽 表二
            int[] arrColWidth2 = new int[0];
            if (dt2 != null && sheetName2 != null)
            {
                arrColWidth2 = new int[dt2.Columns.Count];
                foreach (DataColumn item in dt2.Columns)
                {
                    arrColWidth2[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        int intTemp = Encoding.GetEncoding(936).GetBytes(dt2.Rows[i][j].ToString()).Length;
                        if (intTemp > arrColWidth2[j])
                        {
                            arrColWidth2[j] = intTemp;
                        }
                    }
                }
            }
            #endregion

            #region dt1
            int rowIndex1 = 2, colIndex1 = 0;
            foreach (DataRow row in dt1.Rows)
            {
                rowIndex1++;
                colIndex1 = 0;
                HSSFRow xlsRow = sheet1.CreateRow(rowIndex1) as HSSFRow;
                HSSFCellStyle contentStyle = hssfworkbook.CreateCellStyle() as HSSFCellStyle;

                foreach (DataColumn column in dt1.Columns)
                {
                    HSSFCell newCell = xlsRow.CreateCell(column.Ordinal) as HSSFCell;
                    contentStyle.BorderBottom = BorderStyle.THIN;
                    contentStyle.BorderLeft = BorderStyle.THIN;
                    contentStyle.BorderRight = BorderStyle.THIN;
                    contentStyle.BorderTop = BorderStyle.THIN;
                    xlsRow.GetCell(column.Ordinal).CellStyle = contentStyle;

                    FillContent(row, column, xlsRow, newCell);
                    sheet1.SetColumnWidth(column.Ordinal, Convert.ToInt32((arrColWidth[column.Ordinal] + 0.5) * 256));

                    colIndex1++;
                }
            }
            #endregion

            #region dt2
            int rowIndex2 = 2, colIndex2 = 0;
            foreach (DataRow row in dt2.Rows)
            {
                rowIndex2++;
                colIndex2 = 0;
                HSSFRow xlsRow = sheet2.CreateRow(rowIndex2) as HSSFRow;
                HSSFCellStyle contentStyle1 = hssfworkbook.CreateCellStyle() as HSSFCellStyle;
                HSSFCellStyle contentStyle2 = hssfworkbook.CreateCellStyle() as HSSFCellStyle;
                HSSFFont font = hssfworkbook.CreateFont() as HSSFFont;

                foreach (DataColumn column in dt2.Columns)
                {
                    HSSFCell newCell = xlsRow.CreateCell(column.Ordinal) as HSSFCell;
                    if (column.Ordinal == 5 || column.Ordinal == 7 || column.Ordinal == 10)
                    {
                        contentStyle2.BorderBottom = BorderStyle.THIN;
                        contentStyle2.BorderLeft = BorderStyle.THIN;
                        contentStyle2.BorderRight = BorderStyle.THIN;
                        contentStyle2.BorderTop = BorderStyle.THIN;
                        font.Color = HSSFColor.RED.index;
                        contentStyle2.SetFont(font);
                        xlsRow.GetCell(column.Ordinal).CellStyle = contentStyle2;
                    }
                    else
                    {
                        contentStyle1.BorderBottom = BorderStyle.THIN;
                        contentStyle1.BorderLeft = BorderStyle.THIN;
                        contentStyle1.BorderRight = BorderStyle.THIN;
                        contentStyle1.BorderTop = BorderStyle.THIN;
                        xlsRow.GetCell(column.Ordinal).CellStyle = contentStyle1;
                    }
                    FillContent(row, column, xlsRow, newCell);
                    sheet2.SetColumnWidth(column.Ordinal, Convert.ToInt32((arrColWidth2[column.Ordinal] + 0.5) * 256));

                    colIndex2++;
                }
            }
            #endregion

            MemoryStream ms = new MemoryStream();
            hssfworkbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            file.Close();
            return ms;
        }
        static void FillContent(DataRow row, DataColumn column, HSSFRow xlsRow, HSSFCell newCell)
        {
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


        //// GET: /DailyBalance/ExportFromTemplate/
        //public FileStreamResult ExportFromTemplate()
        //{
        //    int page = 0, rows = 0;
        //    string warehouseCode = Request.QueryString["warehouseCode"];
        //    string settleDate = Request.QueryString["settleDate"];
        //    string unitType = Request.QueryString["unitType"];

        //    System.Data.DataTable dt1 = DailyBalanceService.GetInfoDetail(page, rows, warehouseCode, settleDate, unitType);
        //    System.Data.DataTable dt2 = DailyBalanceService.GetInfoChecking(page, rows, warehouseCode, settleDate, unitType);
        //    string excelTemplate = Server.MapPath("~/ExcelTemplate/DailyBalance.xls");
        //    string title1 = "仓库库存日结明细";
        //    string title2 = "仓库库存日结核对";

        //    System.IO.MemoryStream ms = THOK.Common.ExportExcel.ExportFromTemplate(dt1, dt2, excelTemplate, title1, title2);
        //    return new FileStreamResult(ms, "application/ms-excel");
        //}
    }
}

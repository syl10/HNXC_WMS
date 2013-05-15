using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#region
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using System.Web;
using System.Web.Mvc;
#endregion

namespace THOK.Common
{
    public class ExportExcel
    {
        static HSSFWorkbook workbook;

        #region 导出EXCEL单表双表
        /// <summary>导出EXCEL单表双表</summary>
        /// <param name="dt1">DataTable1</param>
        /// <param name="dt2">DataTable2 如果没有给null值</param>
        /// <param name="headText1">第一张表标题</param>
        /// <param name="headText2">第二张表标题 如果没有给null值</param>
        /// <param name="headFont">标题字体</param>
        /// <param name="headSize">标题大小</param>
        /// <param name="headColor">标题颜色</param>
        /// <param name="headBorder">标题是否显示边框</param>
        /// <param name="colHeadFont">列头、内容字体</param>
        /// <param name="colHeadSize">列头、内容大小</param>
        /// <param name="colHeadColor">列头颜色</param>
        /// <param name="colHeadBorder">是否显示边框(除了标题外)</param>
        /// <param name="contentColor">内容字体颜色</param>
        /// <param name="HeaderFooter">页眉页脚:[0]左上角[1]上中间[2]右上角[3]左下角[4]下中间[5]右下角</param>
        /// <param name="contentChangeColColorFrom">内容标记名称（使得在这个标记内单独控制列的颜色）</param>
        /// <param name="contentChageColColor">内容标记颜色（控制标记内容某列的颜色）</param>
        /// <returns></returns>
        public static MemoryStream ExportDT(DataTable dt1, DataTable dt2
                , string headText1, string headText2
                , string headFont, short headSize, short headColor, bool headBorder
                , string colHeadFont, short colHeadSize, short colHeadColor, bool colHeadBorder
                , short contentColor
                , string[] HeaderFooter
                , string contentChangeColorFrom, short contentChangeColor)
        {
            #region 变量
            string exportDate = "导出时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            double columnWidth = colHeadSize - 9;
            short printHeight = 0;
            short printWidth = 10;
            int sheetCount = 65500; //一个sheet中最多存65536行数据
            int page = 0;
            #endregion

            #region 浏览器下载
            BrowserLoad(headText1);
            #endregion

            #region 创建工作表
            workbook = new HSSFWorkbook();
            HSSFSheet sheet = null;
            #endregion

            #region 创建样式
            HSSFCellStyle contentDateStyle = workbook.CreateCellStyle() as HSSFCellStyle;

            HSSFCellStyle styleHead = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFFont fontHead = workbook.CreateFont() as HSSFFont;

            HSSFCellStyle styleDate = workbook.CreateCellStyle() as HSSFCellStyle;

            HSSFCellStyle styleColHead = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFFont fontColHead = workbook.CreateFont() as HSSFFont;

            HSSFCellStyle contentStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFFont fontContent = workbook.CreateFont() as HSSFFont;

            HSSFCellStyle contentStyleDailyBalance = workbook.CreateCellStyle() as HSSFCellStyle;
            HSSFFont fontDailyBalance = workbook.CreateFont() as HSSFFont; 
            #endregion            

            #region 全局样式 方法
            HSSFCellStyle headStyle = GetTitleStyle(headFont, headSize, headColor, styleHead, fontHead);
            HSSFCellStyle dateStyle = GetExportDate(styleDate);
            HSSFCellStyle colHeadStyle = GetColumnStyle(colHeadFont, colHeadSize, colHeadColor, colHeadBorder,styleColHead,fontColHead);
            #endregion

            #region 取得列宽
            int[] arrColWidth1 = new int[0];
            int[] arrColWidth2 = new int[0];
            if (dt1 != null && headText1 != null)
            {
                arrColWidth1 = new int[dt1.Columns.Count];
                GetColumnWidth(dt1, arrColWidth1);
            }            
            if (dt2 != null && headText2 != null)
            {
                arrColWidth2 = new int[dt2.Columns.Count];
                GetColumnWidth(dt2, arrColWidth2);
            }
            #endregion

            #region 创建 表一
            if (dt1 != null && headText1 != null)
            {
                int dt1count = dt1.Rows.Count;
                if (dt1count % sheetCount == 0)
                {
                    page = dt1count / sheetCount;
                }
                else
                {
                    page = dt1count / sheetCount + 1;
                }
                for (int a = 0; a < page; a++)
                {
                    string strA = a.ToString();
                    if (a == 0)
                    {
                        strA = strA.Substring(0, a.ToString().Length - 1);
                    }
                    string headText1strA = headText1 + strA;
                    sheet = workbook.CreateSheet(headText1strA) as HSSFSheet;
                    sheet.PrintSetup.FitHeight = printHeight;
                    sheet.PrintSetup.FitWidth = printWidth;

                    int rowIndex1 = 0;
                    DataTable newdt1 = SetPage(dt1, a + 1, sheetCount);

                    #region 填充数据
                    foreach (DataRow row in newdt1.Rows)
                    {
                        if (rowIndex1 == 0)
                        {
                            if (rowIndex1 != 0)
                            {
                                sheet = workbook.CreateSheet() as HSSFSheet;
                                sheet.PrintSetup.FitHeight = printHeight;
                                sheet.PrintSetup.FitWidth = printWidth;
                            }
                            #region 填充表头、样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                                headerRow.HeightInPoints = Convert.ToInt16(headSize * 1.4);
                                headerRow.CreateCell(0).SetCellValue(headText1);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                CellRangeAddress region = new CellRangeAddress(0, 0, 0, newdt1.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (headBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);//给合并的画线
                                }
                            } 
                            #endregion
                            #region 导出时间、样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;
                                headerRow.CreateCell(0).SetCellValue(exportDate);
                                headerRow.GetCell(0).CellStyle = dateStyle;
                                CellRangeAddress region = new CellRangeAddress(1, 1, 0, newdt1.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (colHeadBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);
                                }
                            } 
                            #endregion
                            #region 填充列头、样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(2) as HSSFRow;
                                headerRow.HeightInPoints = Convert.ToInt16(colHeadSize * 1.4);
                                foreach (DataColumn column in newdt1.Columns)
                                {
                                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                    headerRow.GetCell(column.Ordinal).CellStyle = colHeadStyle;
                                    sheet.SetColumnWidth(column.Ordinal, Convert.ToInt32((arrColWidth1[column.Ordinal] + columnWidth) * 256));//设置列宽
                                }
                            }
                            rowIndex1 = 3; 
                            #endregion
                        }
                        #region 填充内容
                        HSSFRow dataRow = sheet.CreateRow(rowIndex1) as HSSFRow;
                        foreach (DataColumn column in newdt1.Columns)
                        {
                            FillContent(dataRow, column, row, contentStyle, fontContent
                                , contentDateStyle
                                , contentStyleDailyBalance, fontDailyBalance                                
                                , colHeadFont, colHeadSize, colHeadColor, contentColor, colHeadBorder
                                , contentChangeColorFrom, contentChangeColor
                                , sheet, headText1strA);
                        }
                        rowIndex1++; 
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            #region 创建 表二
            if (dt2 != null && headText2 != null)
            {
                int dt2count = dt2.Rows.Count;
                if (dt2count % sheetCount == 0)
                {
                    page = dt2count / sheetCount;
                }
                else
                {
                    page = dt2count / sheetCount + 1;
                }
                for (int a = 0; a < page; a++)
                {
                    int rowIndex2 = 0;
                    DataTable newdt2 = SetPage(dt2, a + 1, sheetCount);
                    string strA = a.ToString();
                    if (a == 0)
                    {
                        strA = strA.Substring(0, a.ToString().Length - 1);
                    }
                    string headText2strA = headText2 + strA;

                    #region 填充数据
                    foreach (DataRow row in newdt2.Rows)
                    {
                        if (rowIndex2 == 0)
                        {
                            HSSFRow headerRow;
                            if (rowIndex2 != 1)
                            {
                                sheet = workbook.CreateSheet(headText2strA) as HSSFSheet;
                                sheet.PrintSetup.FitHeight = printHeight;
                                sheet.PrintSetup.FitWidth = printWidth;
                            }
                            #region 填充表头、样式
                            {
                                headerRow = sheet.CreateRow(0) as HSSFRow;
                                headerRow.HeightInPoints = Convert.ToInt16(headSize * 1.4);
                                headerRow.CreateCell(0).SetCellValue(headText2);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                CellRangeAddress region = new CellRangeAddress(0, 0, 0, newdt2.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (headBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);
                                }
                            } 
                            #endregion
                            #region 导出时间、样式
                            {
                                headerRow = sheet.CreateRow(1) as HSSFRow;
                                headerRow.CreateCell(0).SetCellValue(exportDate);
                                headerRow.GetCell(0).CellStyle = dateStyle;
                                CellRangeAddress region = new CellRangeAddress(1, 1, 0, newdt2.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (colHeadBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);
                                }
                            } 
                            #endregion
                            #region 填充列头、样式
                            {
                                headerRow = sheet.CreateRow(2) as HSSFRow;
                                foreach (DataColumn column in newdt2.Columns)
                                {
                                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                    headerRow.GetCell(column.Ordinal).CellStyle = colHeadStyle;
                                    sheet.SetColumnWidth(column.Ordinal, Convert.ToInt32((arrColWidth2[column.Ordinal] + columnWidth) * 256));
                                }
                            }
                            rowIndex2 = 3; 
                            #endregion
                        }
                        #region 填充内容
                        HSSFRow dataRow = sheet.CreateRow(rowIndex2) as HSSFRow;
                        foreach (DataColumn column in newdt2.Columns)
                        {
                            FillContent(dataRow, column, row, contentStyle, fontContent
                                , contentDateStyle
                                , contentStyleDailyBalance, fontDailyBalance
                                , colHeadFont, colHeadSize, colHeadColor, contentColor, colHeadBorder
                                , contentChangeColorFrom, contentChangeColor
                                , sheet, headText2strA);
                        }
                        rowIndex2++; 
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            #region 页眉 页脚
            sheet.Header.Left = HeaderFooter[0].ToString();
            sheet.Header.Center = HeaderFooter[1].ToString();
            sheet.Header.Right = HeaderFooter[2].ToString();
            sheet.Footer.Left = HeaderFooter[3].ToString();
            sheet.Footer.Center = HeaderFooter[4].ToString();
            sheet.Footer.Right = HeaderFooter[5].ToString();
            #endregion

            #region 返回内存流
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
            #endregion
        }
        #endregion

        #region sheet分页
        static DataTable SetPage(DataTable dt, int currentPageIndex, int pageSize)
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

        #region 填充内容
        /// <summary>填充内容</summary>
        static void FillContent(HSSFRow hssfRow
            , DataColumn column, DataRow row
            , HSSFCellStyle contentStyle, HSSFFont contentFont
            , HSSFCellStyle contentDateStyle
            , HSSFCellStyle contentStyleDailyBalance, HSSFFont fontDailyBalance            
            , string colHeadFont, short colHeadSize, short colHeadColor
            , short contentColor, bool contentBorder
            , string contentChangeColorFrom, short contentChangeColor
            , HSSFSheet sheet, string headTextStrA)
        {
            HSSFCell newCell = hssfRow.CreateCell(column.Ordinal) as HSSFCell;

            #region 当数据访问转换DataTime时生效
            HSSFDataFormat format = workbook.CreateDataFormat() as HSSFDataFormat;
            contentDateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");
            #endregion

            string columnF = row[column].ToString();
            decimal i;
            bool b = decimal.TryParse(columnF, out i);

            #region 判断如果是仓库库存日结核对
            if (contentChangeColorFrom == "DailyBalance" && sheet == workbook.GetSheet(headTextStrA)
                    && ((column.Ordinal == 5 && b == false)
                    || (column.Ordinal == 6 && b == false)
                    || (column.Ordinal == 7 && b == false)
                    || (column.Ordinal == 8 && b == false)
                    || (column.Ordinal == 9 && b == false)
                    || (column.Ordinal == 10 && b == false)))
            {
                fontDailyBalance.FontName = colHeadFont;
                fontDailyBalance.FontHeightInPoints = colHeadSize;
                fontDailyBalance.Color = contentChangeColor;
                contentStyleDailyBalance.SetFont(fontDailyBalance);
                if (contentBorder == true)
                {
                    contentStyleDailyBalance.BorderBottom = BorderStyle.THIN;
                    contentStyleDailyBalance.BorderLeft = BorderStyle.THIN;
                    contentStyleDailyBalance.BorderRight = BorderStyle.THIN;
                    contentStyleDailyBalance.BorderTop = BorderStyle.THIN;
                }

                hssfRow.GetCell(column.Ordinal).CellStyle = contentStyleDailyBalance;
            } 
            #endregion
            else
            {
                contentFont.FontName = colHeadFont;
                contentFont.FontHeightInPoints = colHeadSize;
                contentFont.Color = contentColor;
                contentStyle.SetFont(contentFont);
                //画边框
                if (contentBorder == true)
                {
                    contentStyle.BorderBottom = BorderStyle.THIN;
                    contentStyle.BorderLeft = BorderStyle.THIN;
                    contentStyle.BorderRight = BorderStyle.THIN;
                    contentStyle.BorderTop = BorderStyle.THIN;
                }
                hssfRow.GetCell(column.Ordinal).CellStyle = contentStyle;
            }
            string drValue = row[column].ToString();
            ChangeFormat(column,drValue,newCell,contentDateStyle);
        }
        #endregion

        #region 转换格式
        static void ChangeFormat(DataColumn column, string drValue, HSSFCell newCell, HSSFCellStyle contentDateStyle)
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

        #region 取得列宽
        static void GetColumnWidth(DataTable dt,int[] arrColWidth)
        {
            foreach (DataColumn item in dt.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;//936是指GB2312编码
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dt.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
        }
        #endregion

        #region 标题样式
        /// <summary>标题样式</summary>
        static HSSFCellStyle GetTitleStyle(string headFont, short headSize, short headColor
            ,HSSFCellStyle cellStyle,HSSFFont font)
        {
            cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            font = workbook.CreateFont() as HSSFFont;
            font.FontName = headFont;
            font.FontHeightInPoints = headSize;
            font.Color = headColor;
            font.Boldweight = 700;
            cellStyle.SetFont(font);
            return cellStyle;
        }
        #endregion

        #region 列头样式
        /// <summary>列头样式</summary>
        static HSSFCellStyle GetColumnStyle(string colHeadFont, short colHeadSize, short colHeadColor, bool colHeadBorder
            ,HSSFCellStyle cellStyle,HSSFFont font)
        {
            cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.Alignment = HorizontalAlignment.CENTER; //居中
            if (colHeadBorder == true)
            {
                //边框
                cellStyle.BorderBottom = BorderStyle.THIN;
                cellStyle.BorderLeft = BorderStyle.THIN;
                cellStyle.BorderRight = BorderStyle.THIN;
                cellStyle.BorderTop = BorderStyle.THIN;
            }
            //font
            font = workbook.CreateFont() as HSSFFont;
            font.FontName = colHeadFont;
            font.FontHeightInPoints = colHeadSize;
            font.Color = colHeadColor;
            font.Boldweight = 700;

            cellStyle.SetFont(font);
            return cellStyle;
        }
        #endregion

        #region 导出时间样式
        /// <summary>导出时间样式</summary>
        static HSSFCellStyle GetExportDate(HSSFCellStyle cellStyle)
        {
            cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            return cellStyle;
        }
        #endregion

        #region 浏览器下载
        /// <summary>浏览器下载</summary>
        static void BrowserLoad(string headText1)
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
            BrowserLoad(sheetName1);

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
    }
}

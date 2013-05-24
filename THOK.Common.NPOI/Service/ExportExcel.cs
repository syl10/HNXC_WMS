using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using System.Web;
using System.Web.Mvc;

namespace THOK.NPOI.Service
{
    public class ExportExcel
    {
        static HSSFWorkbook workbook;

        #region 导出EXCEL单表双表
        /// <summary>导出EXCEL单表双表</summary>
        public static MemoryStream ExportDT(THOK.NPOI.Models.ExportParam ep)
        {
            #region 变量
            string exportDate = "导出时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            double columnWidth = ep.ColHeadSize - 9;
            short printHeight = 0;
            short printWidth = 10;
            int sheetCount = 65500; //一个Sheet中最多存65536行数据
            int page = 0;
            #endregion

            #region 浏览器下载
            THOK.NPOI.Common.ExportExcelHeper.BrowserLoad(ep.HeadTitle1);
            #endregion

            #region 创建工作表
            workbook = new HSSFWorkbook();
            HSSFSheet sheet = null;
            #endregion

            #region 创建样式
            HSSFCellStyle contentDateStyle = workbook.CreateCellStyle() as HSSFCellStyle;           //内容时间 单元格
            HSSFCellStyle styleHead = workbook.CreateCellStyle() as HSSFCellStyle;                  //大标题   单元格
            HSSFFont fontHead = workbook.CreateFont() as HSSFFont;                                  //大标题   字体
            HSSFCellStyle styleDate = workbook.CreateCellStyle() as HSSFCellStyle;                  //导出时间 单元格
            HSSFCellStyle styleColHead = workbook.CreateCellStyle() as HSSFCellStyle;               //列头     单元格
            HSSFFont fontColHead = workbook.CreateFont() as HSSFFont;                               //列头     字体
            HSSFCellStyle contentStyle = workbook.CreateCellStyle() as HSSFCellStyle;               //内容     单元格
            HSSFFont fontContent = workbook.CreateFont() as HSSFFont;                               //内容     字体
            contentStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");                      //内容     设置所有列整型格式，也可以通过数据访问层来改变内容格式
            HSSFCellStyle contentStyleDailyBalance = workbook.CreateCellStyle() as HSSFCellStyle;   //特殊模块 单元格
            HSSFFont fontDailyBalance = workbook.CreateFont() as HSSFFont;                          //特殊模块 字体
            #endregion

            #region 全局样式
            HSSFCellStyle headStyle = GetTitleStyle(ep.BigHeadFont, ep.BigHeadSize, ep.BigHeadColor, styleHead, fontHead);
            HSSFCellStyle dateStyle = GetExportDate(styleDate);
            HSSFCellStyle colHeadStyle = GetColumnStyle(ep.ColHeadFont, ep.ColHeadSize, ep.ColHeadColor, ep.ColHeadBorder, styleColHead, fontColHead);
            #endregion

            #region 取得列宽
            int[] arrColWidth1 = new int[0];
            int[] arrColWidth2 = new int[0];
            if (ep.DT1 != null && ep.HeadTitle1 != null)
            {
                arrColWidth1 = new int[ep.DT1.Columns.Count];
                GetColumnWidth(ep.DT1, arrColWidth1);
            }
            if (ep.DT2 != null && ep.HeadTitle2 != null)
            {
                arrColWidth2 = new int[ep.DT2.Columns.Count];
                GetColumnWidth(ep.DT2, arrColWidth2);
            }
            #endregion

            #region 创建EXCEL 表一
            if (ep.DT1 != null && ep.HeadTitle1 != null)
            {
                int dt1count = ep.DT1.Rows.Count;

                #region 判断多少页
                if (dt1count % sheetCount == 0)
                {
                    page = dt1count / sheetCount;
                }
                else
                {
                    page = dt1count / sheetCount + 1;
                }
                #endregion

                for (int a = 0; a < page; a++)
                {
                    #region SHEET分页标题
                    string sheetNum = a.ToString();
                    if (a == 0)
                    {
                        sheetNum = sheetNum.Substring(0, a.ToString().Length - 1);
                    }
                    string headTitle1_sheetNum = ep.HeadTitle1 + sheetNum;
                    sheet = workbook.CreateSheet(headTitle1_sheetNum) as HSSFSheet;
                    #endregion

                    #region SHEET打印设置
                    sheet.PrintSetup.FitHeight = printHeight;
                    sheet.PrintSetup.FitWidth = printWidth;
                    #endregion

                    int rowIndex1 = 0;

                    #region 内容分页
                    DataTable newdt1 = THOK.NPOI.Common.ExportExcelHeper.SetPage(ep.DT1, a + 1, sheetCount);
                    #endregion

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
                            #region 填充“表头”和它的样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(0) as HSSFRow;
                                headerRow.HeightInPoints = Convert.ToInt16(ep.BigHeadSize * 1.4);
                                headerRow.CreateCell(0).SetCellValue(ep.HeadTitle1);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                CellRangeAddress region = new CellRangeAddress(0, 0, 0, newdt1.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (ep.BigHeadBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);//给合并的画线
                                }
                            }
                            #endregion
                            #region 填充“导出时间”和它的样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(1) as HSSFRow;
                                headerRow.CreateCell(0).SetCellValue(exportDate);
                                headerRow.GetCell(0).CellStyle = dateStyle;
                                CellRangeAddress region = new CellRangeAddress(1, 1, 0, newdt1.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (ep.ColHeadBorder == true)
                                {
                                    sheet.SetEnclosedBorderOfRegion(region, BorderStyle.THIN, HSSFColor.BLACK.index);
                                }
                            }
                            #endregion
                            #region 填充“列头”和它的样式
                            {
                                HSSFRow headerRow = sheet.CreateRow(2) as HSSFRow;
                                headerRow.HeightInPoints = Convert.ToInt16(ep.ColHeadSize * 1.4);
                                foreach (DataColumn column in newdt1.Columns)
                                {
                                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                                    headerRow.GetCell(column.Ordinal).CellStyle = colHeadStyle;
                                    sheet.SetColumnWidth(column.Ordinal, Convert.ToInt32((arrColWidth1[column.Ordinal] + columnWidth) * 256));//设置列宽
                                }
                            }
                            #endregion
                            rowIndex1 = 3;
                        }
                        #region 填充内容
                        HSSFRow dataRow = sheet.CreateRow(rowIndex1) as HSSFRow;
                        foreach (DataColumn column in newdt1.Columns)
                        {
                            FillContent(dataRow, column, row, contentStyle, fontContent
                                , contentDateStyle
                                , contentStyleDailyBalance, fontDailyBalance
                                , ep
                                , sheet, headTitle1_sheetNum);
                        }
                        rowIndex1++;
                        #endregion
                    }
                    #endregion
                }
            }
            #endregion

            #region 创建EXCEL 表二
            if (ep.DT2 != null && ep.HeadTitle2 != null)
            {
                int dt2count = ep.DT2.Rows.Count;
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
                    DataTable newdt2 = THOK.NPOI.Common.ExportExcelHeper.SetPage(ep.DT2, a + 1, sheetCount);
                    string strA = a.ToString();
                    if (a == 0)
                    {
                        strA = strA.Substring(0, a.ToString().Length - 1);
                    }
                    string headText2strA = ep.HeadTitle2 + strA;

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
                                headerRow.HeightInPoints = Convert.ToInt16(ep.BigHeadSize * 1.4);
                                headerRow.CreateCell(0).SetCellValue(ep.HeadTitle2);
                                headerRow.GetCell(0).CellStyle = headStyle;
                                CellRangeAddress region = new CellRangeAddress(0, 0, 0, newdt2.Columns.Count - 1);
                                sheet.AddMergedRegion(region);
                                if (ep.BigHeadBorder == true)
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
                                if (ep.ColHeadBorder == true)
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
                                , ep
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
            sheet.Header.Left = ep.HeaderFooter.ToString();
            sheet.Header.Center = ep.HeaderFooter[1].ToString();
            sheet.Header.Right = ep.HeaderFooter[2].ToString();
            sheet.Footer.Left = ep.HeaderFooter[3].ToString();
            sheet.Footer.Center = ep.HeaderFooter[4].ToString();
            sheet.Footer.Right = ep.HeaderFooter[5].ToString();
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

        #region 样式 大标题
        static HSSFCellStyle GetTitleStyle(string headFont, short headSize, short headColor
            , HSSFCellStyle cellStyle, HSSFFont font)
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

        #region 样式 导出时间
        static HSSFCellStyle GetExportDate(HSSFCellStyle cellStyle)
        {
            cellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
            cellStyle.Alignment = HorizontalAlignment.CENTER;
            return cellStyle;
        }
        #endregion

        #region 样式 列头
        static HSSFCellStyle GetColumnStyle(string colHeadFont, short colHeadSize, short colHeadColor, bool colHeadBorder
            , HSSFCellStyle cellStyle, HSSFFont font)
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
        
        #region 取得列宽
        static void GetColumnWidth(DataTable dt, int[] arrColWidth)
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
                        #region NPOI最大限度是255 否则报错
                        if (intTemp > 250)
                        {
                            arrColWidth[j] = 50;
                        } 
                        #endregion
                        else
                        {
                            arrColWidth[j] = intTemp;
                        }
                    }
                }
            }
        }
        #endregion

        #region 填充内容
        /// <summary>填充内容</summary>
        static void FillContent(HSSFRow hssfRow
            , DataColumn column, DataRow row
            , HSSFCellStyle contentStyle, HSSFFont contentFont
            , HSSFCellStyle contentDateStyle
            , HSSFCellStyle contentStyleDailyBalance, HSSFFont fontDailyBalance
            , THOK.NPOI.Models.ExportParam ep
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
            if (ep.ContentModule == "DailyBalance" && sheet == workbook.GetSheet(headTextStrA)
                    && ((column.Ordinal == 5 && b == false)
                    || (column.Ordinal == 6 && b == false)
                    || (column.Ordinal == 7 && b == false)
                    || (column.Ordinal == 8 && b == false)
                    || (column.Ordinal == 9 && b == false)
                    || (column.Ordinal == 10 && b == false)))
            {
                fontDailyBalance.FontName = ep.ColHeadFont;
                fontDailyBalance.FontHeightInPoints = ep.ColHeadSize;
                fontDailyBalance.Color = ep.ContentModuleColor;
                contentStyleDailyBalance.SetFont(fontDailyBalance);
                if (ep.ColHeadBorder == true)
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
                contentFont.FontName = ep.ColHeadFont;
                contentFont.FontHeightInPoints = ep.ColHeadSize;
                contentFont.Color = ep.ContentColor;
                contentStyle.SetFont(contentFont);
                //画边框
                if (ep.ColHeadBorder == true)
                {
                    contentStyle.BorderBottom = BorderStyle.THIN;
                    contentStyle.BorderLeft = BorderStyle.THIN;
                    contentStyle.BorderRight = BorderStyle.THIN;
                    contentStyle.BorderTop = BorderStyle.THIN;
                }
                hssfRow.GetCell(column.Ordinal).CellStyle = contentStyle;
            }
            string drValue = row[column].ToString();
            THOK.NPOI.Common.ExportExcelHeper.ChangeFormat(column, drValue, newCell, contentDateStyle);
        }
        #endregion

    }
}
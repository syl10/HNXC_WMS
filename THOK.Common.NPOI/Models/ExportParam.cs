using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.NPOI.Models
{
    public class ExportParam
    {
        string bigHeadFont = "微软雅黑";
        short bigHeadSize = 20;
        bool bigHeadBorder = true;
        string colHeadFont = "Arial";
        short colHeadSize = 10;
        bool colHeadBorder = true;
        string[] headerFooder = {   
                                     "……"  //眉左
                                    ,"……"  //眉中
                                    ,"……"  //眉右
                                    ,"&D"    //脚左 日期
                                    ,"……"  //脚中
                                    ,"&P"    //脚右 页码
                                };

        /// <summary>第一张Excel表Sheet1的标题</summary>
        public string HeadTitle1 { get; set; }
        /// <summary>第二张Excel表Sheet2的标题</summary>
        public string HeadTitle2 { get; set; }

        /// <summary>第一个DataTable</summary>
        public System.Data.DataTable DT1 { get; set; }
        /// <summary>第二个DataTable，如果没有就给NULL</summary>
        public System.Data.DataTable DT2 { get; set; }

        /// <summary>大标题字体</summary>
        public string BigHeadFont { get { return bigHeadFont; } set { bigHeadFont = value; } }
        /// <summary>大标题字体大小</summary>
        public short BigHeadSize { get { return bigHeadSize; } set { bigHeadSize = value; } }
        /// <summary>大标题字体颜色</summary>
        public short BigHeadColor { get; set; }
        /// <summary>大标题是否有边框</summary>
        public bool BigHeadBorder { get { return bigHeadBorder; } set { bigHeadBorder = value; } }

        /// <summary>列标题和内容字体</summary>
        public string ColHeadFont { get { return colHeadFont; } set { colHeadFont = value; } }
        /// <summary>列标题和内容字体大小</summary>
        public short ColHeadSize { get { return colHeadSize; } set { colHeadSize = value; } }
        /// <summary>列标题和内容字体颜色</summary>
        public short ColHeadColor { get; set; }
        /// <summary>列标题和内容是否有边框</summary>
        public bool ColHeadBorder { get { return colHeadBorder; } set { colHeadBorder = value; } }

        /// <summary>内容字体颜色</summary>
        public short ContentColor { get; set; }

        /// <summary>特殊的模块内容</summary>
        public string ContentModule { get; set; }
        /// <summary>特殊的模块内容的颜色</summary>
        public short ContentModuleColor { get; set; }

        /// <summary>页眉页脚:[0]左上角[1]上中间[2]右上角[3]左下角[4]下中间[5]右下角</summary>
        public string[] HeaderFooter { get { return headerFooder; } set { headerFooder = value; } }

    }
}
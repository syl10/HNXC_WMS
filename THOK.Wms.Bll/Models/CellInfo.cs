using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.Bll.Models
{
    /// <summary>
    /// 货位上的产品信息
    /// </summary>
   public  class CellInfo
    {
       /// <summary>
       /// 条码
       /// </summary>
       public string Barcode { get; set; }
       /// <summary>
       /// 牌号
       /// </summary>
       public string CIGARETTE { get; set; }
       /// <summary>
       /// 配方
       /// </summary>
       public string FORMULA { get; set; }
       /// <summary>
       /// 入库批次
       /// </summary>
       public string BILLNO { get; set; }
       /// <summary>
       /// 产地
       /// </summary>
       public string ORIGINAL { get; set; }
       /// <summary>
       /// 等级
       /// </summary>
       public string GRADE { get; set; }
       /// <summary>
       /// 年份
       /// </summary>
       public string YEARS { get; set; }
       /// <summary>
       /// 形态
       /// </summary>
       public string STYLENO { get; set; }
       /// <summary>
       /// 实际重量
       /// </summary>
       public string REALWEIGHT { get; set; }
       /// <summary>
       /// 入库日期
       /// </summary>
       public string INDATE { get; set; }
    }
}

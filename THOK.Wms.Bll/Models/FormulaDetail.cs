using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.Bll.Models
{
    /// <summary>
    /// 暂存,载入配方的明细
    /// </summary>
   public   class FormulaDetail
    {
       /// <summary>
       /// 序号
       /// </summary>
       public decimal ITEM_NO { set; get; }
       /// <summary>
       /// 产品代号
       /// </summary>
       public string PRODUCT_CODE { set; get; }
       /// <summary>
       /// 产品名称
       /// </summary>
       public string PRODUCT_NAME { set; get; }
       /// <summary>
       ///产品每包重量,或者混装中的不足一包的重量
       /// </summary>
       public decimal REAL_WEIGHT { set; get; }
       /// <summary>
       /// 指产品表中的重量
       /// </summary>
       public decimal WEIGHT { set; get; }
       /// <summary>
       /// 包数
       /// </summary>
       public int  PACKAGE_COUNT { set; get; }
       /// <summary>
       /// 总重量
       /// </summary>
       public decimal TOTAL_WEIGHT { set; get; }
       /// <summary>
       /// 是否混装
       /// </summary>
       public string IS_MIX { get; set; }
       /// <summary>
       /// 混装产品代码
       /// </summary>
       public string FPRODUCT_CODE { get; set; }
    }
}

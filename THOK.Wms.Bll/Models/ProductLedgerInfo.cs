using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.Bll.Models
{
    /// <summary>
    /// 产品总账 信息
    /// </summary>
    public  class ProductLedgerInfo
    {
        /// <summary>
        /// 开始年月
        /// </summary>
        public string BEGINMONTH { get; set; }
        /// <summary>
        /// 结束年月
        /// </summary>
        public string ENDMONTH { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string WAREHOUSE_CODE { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WAREHOUSE_NAME { get; set; }
        /// <summary>
        /// 产品代码
        /// </summary>
        public string PRODUCT_CODE { get; set; }
        /// <summary>
        ///产品名称
        /// </summary>
        public string PRODUCT_NAME { get; set; }
        /// <summary>
        /// 单据日期
        /// </summary>
        public string  BILLDATE { get; set; }
        /// <summary>
        /// 单据号
        /// </summary>
        public string BILL_NO { get; set; }
        /// <summary>
        /// 入库数量
        /// </summary>
        public decimal? IN_QUANTITY { get; set; }
        /// <summary>
        /// 损益数量
        /// </summary>
        public decimal? INCOME_QUANTITY { get; set; }
        /// <summary>
        /// 抽检补料入数量
        /// </summary>
        public decimal? INSPECTIN_QUANTITY { get; set; }
        /// <summary>
        /// 抽检出数量
        /// </summary>
        public decimal? INSPECTOUT_QUANTITY { get; set; }
        /// <summary>
        /// 出库数量
        /// </summary>
        public decimal? OUT_QUANTITY { get; set; }
        /// <summary>
        /// 差异数量
        /// </summary>
        public decimal? DIFF_QUANTITY { get; set; }
        /// <summary>
        /// 期末数量
        /// </summary>
        public decimal? ENDQUANTITY { get; set; }
        /// <summary>
        /// 期初数量
        /// </summary>
        public decimal? BEGIN_QUANTITY { get; set; }
        /// <summary>
        /// 补料出数量
        /// </summary>
        public decimal? FEEDING_QUANTITY { get; set; }
    }
}

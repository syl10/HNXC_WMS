using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public  interface IWMSProductStateService : IService<WMS_PRODUCT_STATE>
    {
        //获取某条单据下的作业任务
        object Details(int page, int rows, string billno);
        //单据拆分为作业,(入库)
        bool Task(string billno, string btypecode,string tasker,out string error);
        //出库作业功能.
        bool Task(string billno, string cigarettecode, string formulacode, string batchweight, string tasker, out string error);
        //托盘入库作业
        bool Task(string billno, string tasker, out string error);
    }
}

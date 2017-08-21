using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public interface IWMSScheduleMasterService : IService<WMS_SCHEDULE_MASTER >
    {
        //获取生产计划单
       object GetDetails(int page, int rows, string SCHEDULE_NO, string SCHEDULE_DATE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string BILLNOFROM, string BILLNOTO);
       //根据计划单号获取详细的信息
        object GetSubDetails(int page, int rows, string SCHEDULE_NO);
        //获取计划单号
        object GetSchedulno(string userName, DateTime dt, string SCHEDULE_NO);
        //添加计划生产单
        bool  Add(WMS_SCHEDULE_MASTER mast, object detail);
       //修改计划生产单
        bool Edit(WMS_SCHEDULE_MASTER mast, object detail);
       //删除
        bool Delete(string scheduleno);
        //计划生产单审核
        bool Audit(string checker, string scheduleno);
        //反审
        bool Antitrial(string scheduleno);
       //获取制丝线
        object GetProductLine(int page, int rows);
       //生成出库单
        bool CreateOutBill(string Scheduleno,string userid);
       //计划单打印
        bool SchedulePrint(string SCHEDULENO, string BILLDATEFROM, string BILLDATETO, string STATE);

    }
}

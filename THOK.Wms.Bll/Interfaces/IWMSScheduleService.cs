using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IWMSScheduleService : IService<WMS_SCHEDULE>
    {
        //获取生产计划单
        object GetDetails(int page, int rows, string SCHEDULE_NO, string SCHEDULE_DATE, string CIGARETTE, string FORMULA_CODE, string QUANTITY, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE);
        //获取计划单号
        object GetSchedulno(string userName, DateTime dt, string SCHEDULE_NO);
        //新增计划生产单
        bool Add(WMS_SCHEDULE  schedule);
        //修改计划生产单
        bool Save(WMS_SCHEDULE schedule);
        //删除计划生产单
        bool Delete(string  scheduleno);
        //计划生产单审核
        bool Audit(string checker,string scheduleno);
        //反审
        bool Antitrial(string scheduleno);
    }
}

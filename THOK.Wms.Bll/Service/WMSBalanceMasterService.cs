using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Wms.Bll.Service
{
    class WMSBalanceMasterService : ServiceBase<WMS_BALANCE_MASTER>, IWMSBalanceMasterService 
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSBalanceMasterRepository BalanceMasterRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public IWMSProductStateRepository ProductStateRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }

        public object GetDetails(int page, int rows, string BALANCENO, string BALANCEDATE, string STATE, string OPERATER, string CHECKER, string CHECKDATE)
        {
            IQueryable<WMS_BALANCE_MASTER> query = BalanceMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            var balance = from a in query
                          join b in statequery on a.STATE equals b.STATE
                          join c in userquery on a.OPERATER equals c.USER_ID 
                          where b.TABLE_NAME == "WMS_BALANCE_MASTER" && b.FIELD_NAME == "STATE"
                          select new { 
                              a.BALANCE_NO ,
                              a.BALANCE_DATE ,
                              a.CHECK_DATE ,
                              a.CHECKER ,
                              a.OPERATER ,
                              OPERATERNAME=c.USER_NAME,
                              a.STATE ,
                              b.STATE_DESC //状态描述
                          };
            if (!string.IsNullOrEmpty(BALANCENO)) {
                balance = balance.Where(i => i.BALANCE_NO == BALANCENO);
            }
            if (!string.IsNullOrEmpty(BALANCEDATE)) {
                DateTime operatedt = DateTime.Parse(BALANCEDATE);
                DateTime operatedt2 = operatedt.AddDays(1);
                balance = balance.Where(i => i.BALANCE_DATE .Value.CompareTo(operatedt) >= 0);
                balance = balance.Where(i => i.BALANCE_DATE.Value.CompareTo(operatedt2) < 0);
            }
            if (!string.IsNullOrEmpty(STATE)) {
                balance = balance.Where(i => i.STATE == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER)) {
                balance = balance.Where(i => i.OPERATERNAME.Contains(OPERATER));
            }
            if (!string.IsNullOrEmpty(CHECKER)) {
                balance = balance.Where(i => i.CHECKER.Contains(CHECKER));
            }
            if (!string.IsNullOrEmpty(CHECKDATE))
            {
                DateTime operatedt = DateTime.Parse(BALANCEDATE);
                DateTime operatedt2 = operatedt.AddDays(1);
                balance = balance.Where(i => i.CHECK_DATE .Value.CompareTo(operatedt) >= 0);
                balance = balance.Where(i => i.CHECK_DATE .Value.CompareTo(operatedt2) < 0);
            }
            var temp = balance.OrderBy(i => i.BALANCE_NO).ToArray().Select(i => new
            { 
                i.BALANCE_NO ,
                BALANCE_DATE=i.BALANCE_DATE == null ? "" : ((DateTime)i.BALANCE_DATE ).ToString("yyyy-MM-dd HH:mm:ss") ,
                CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE ).ToString("yyyy-MM-dd HH:mm:ss"),
                i.CHECKER ,
                i.OPERATER ,
                i.OPERATERNAME ,
                i.STATE ,
                i.STATE_DESC
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //审核
        public bool Audit(string checker, string BalancNo)
        {
            var billquery = BalanceMasterRepository.GetQueryable().FirstOrDefault(i => i.BALANCE_NO == BalancNo);
            if (billquery != null)
            {
                billquery.CHECK_DATE = DateTime.Now;
                billquery.CHECKER = checker;
                billquery.STATE = "1";
                int result = BalanceMasterRepository.SaveChanges();
                if (result == -1) return false;
            }
            else
                return false;
            return true;
        }
        //反审
        public bool Antitrial(string BalancNo)
        {
            var billquery = BalanceMasterRepository.GetQueryable().FirstOrDefault(i => i.BALANCE_NO  ==BalancNo );
            if (billquery != null)
            {
                billquery.CHECK_DATE = null;
                billquery.CHECKER = "";
                billquery.STATE = "0";
                BalanceMasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        //月结
        public bool Balance(string Balanceno, DateTime dt, string operater, out string error)
        {
            string sqlstr = "begin BALANCE('" + Balanceno + "','" + dt.ToString () + "','" + operater + "');end;";
            //string sqlstr = "update WMS_BILL_MASTER set state='2' where bill_no='"+billno+"' ";
            int result = ProductStateRepository.Exeprocedure(sqlstr, out error);
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
        }

        //获取已月结的年月
        public object GetBalanceNo()
        {
            IQueryable<WMS_BALANCE_MASTER> query = BalanceMasterRepository.GetQueryable();
            var temp =( from a in query
                       select new { 
                            a.BALANCE_NO
                       }).Distinct ();
            temp = temp.OrderByDescending(i => i.BALANCE_NO);
            int total = temp.Count();
            //temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
    }
}

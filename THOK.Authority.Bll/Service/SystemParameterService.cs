using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class SystemParameterService : ServiceBase<AUTH_SYSTEM_PARAMETER>, ISystemParameterService
    {
        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetSystemParameter(int page, int rows, AUTH_SYSTEM_PARAMETER systemParameter)
        {
            var query = SystemParameterRepository.GetQueryable()
                            .Where(a => a.PARAMETER_NAME.Contains(systemParameter.PARAMETER_NAME)
                                || a.PARAMETER_VALUE.Contains(systemParameter.PARAMETER_VALUE)
                                || a.REMARK.Contains(systemParameter.REMARK)
                                || a.USER_NAME.Contains(systemParameter.USER_NAME)
                                || a.SYSTEM_ID == systemParameter.SYSTEM_ID);
            if (!systemParameter.PARAMETER_NAME.Equals(string.Empty))
            {
                query.Where(a => a.PARAMETER_NAME == systemParameter.PARAMETER_NAME);
            }
            if (!systemParameter.PARAMETER_VALUE.Equals(string.Empty))
            {
                query.Where(a => a.PARAMETER_VALUE == systemParameter.PARAMETER_VALUE);
            }
            if (!systemParameter.REMARK.Equals(string.Empty))
            {
                query.Where(a => a.REMARK == systemParameter.REMARK);
            }
            if (!systemParameter.USER_NAME.Equals(string.Empty))
            {
                query.Where(a => a.USER_NAME == systemParameter.USER_NAME);
            }
            if (systemParameter.SYSTEM_ID != null)
            {
                query.Where(a => a.SYSTEM_ID == systemParameter.SYSTEM_ID);
            }
            query = query.OrderBy(a => a.ID);
            int total = query.Count();
            query = query.Skip((page - 1) * rows).Take(rows);
            var info = query.ToArray().Select(a => new
            {
                Id = a.ID,
                a.PARAMETER_NAME,
                a.PARAMETER_VALUE,
                a.REMARK,
                a.USER_NAME,
                a.SYSTEM_ID,
                SystemName = a.SYSTEM_ID == null ? "" : a.AUTH_SYSTEM.SYSTEM_NAME
            });
            return new { total, rows = info.ToArray() };
        }

        public bool SetSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter, string userName, out string error)
        {
            error = string.Empty;
            bool result = false;

            var query = SystemParameterRepository.GetQueryable().FirstOrDefault(a => a.ID == systemParameter.ID);
            if (query != null)
            {
                try
                {
                    query.PARAMETER_NAME = systemParameter.PARAMETER_NAME;
                    query.PARAMETER_VALUE = systemParameter.PARAMETER_VALUE;
                    query.REMARK = systemParameter.REMARK;
                    query.USER_NAME = userName;
                    query.SYSTEM_ID = systemParameter.SYSTEM_ID;

                    SystemParameterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    error = "原因：" + ex.Message;
                }
            }
            else
            {
                error = "原因：未找到当前需要修改的数据！";
            }
            return result;
        }

        public bool AddSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter, string userName, out string error)
        {
            error = string.Empty;
            bool result = false;
            var query = SystemParameterRepository.GetQueryable().FirstOrDefault(a => a.ID == systemParameter.ID);
            if (query == null)
            {
                AUTH_SYSTEM_PARAMETER sp = new AUTH_SYSTEM_PARAMETER();
                if (sp != null)
                {
                    try
                    {
                        sp.PARAMETER_NAME = systemParameter.PARAMETER_NAME;
                        sp.PARAMETER_VALUE = systemParameter.PARAMETER_VALUE;
                        sp.REMARK = systemParameter.REMARK;
                        sp.USER_NAME = userName;
                        sp.SYSTEM_ID = systemParameter.SYSTEM_ID;

                        SystemParameterRepository.Add(sp);
                        SystemParameterRepository.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        error = "原因：" + ex.Message;
                    }
                }
                else
                {
                    error = "原因：找不到当前登陆用户！请重新登陆！";
                }
            }
            else
            {
                error = "原因：该编号已存在！";
            }
            return result;
        }

        public bool DelSystemParameter(int id, out string error)
        {
            error = string.Empty;
            bool result = false;

            var query = SystemParameterRepository.GetQueryable().FirstOrDefault(a => a.ID == id);
            if (query != null)
            {
                try
                {
                    SystemParameterRepository.Delete(query);
                    SystemParameterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    error = "原因：" + ex.Message;
                }
            }
            else
            {
                error = "原因：未找到当前需要删除的数据！";
            }

            return result;
        }

        public bool SetSystemParameter()
        {
            var query = SystemParameterRepository.GetQueryable().FirstOrDefault(a => a.PARAMETER_NAME == "DownInterFace").PARAMETER_VALUE;
            if (query == "2")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

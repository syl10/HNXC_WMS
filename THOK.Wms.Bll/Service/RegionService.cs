using System;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using System.Linq;

namespace THOK.Wms.Bll.Service
{
    public class RegionService : ServiceBase<Region>, IRegionService
    {
        [Dependency]
        public IRegionRepository RegionRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows,string RegionName, string State)
        {
            IQueryable<Region> regionQuery = RegionRepository.GetQueryable();
            var region = regionQuery.Where(r => r.RegionName.Contains(RegionName) && r.State.Contains(State))
                .OrderBy(r => r.ID).AsEnumerable()
                .Select(r => new
                {
                    r.ID,
                    r.RegionName,
                    r.Description,
                    State = r.State == "01" ? "可用" : "不可用",
                });
            
            int total = region.Count();
            region = region.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = region.ToArray() };
        }

        public bool Add(Region region, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var reg = new Region();
            if (reg != null)
            {
                try
                {
                    reg.RegionName = region.RegionName;
                    reg.Description = region.Description;
                    reg.State = region.State;

                    RegionRepository.Add(reg);
                    RegionRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "原因：找不到当前登陆用户！请重新登陆！";
            }
            return result;
        }

        public bool Save(Region region, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var reg = RegionRepository.GetQueryable().FirstOrDefault(r => r.ID == region.ID);

            if (reg != null)
            {
                try
                {
                    reg.RegionName = region.RegionName;
                    reg.Description =region.Description;
                    reg.State = region.State;

                    RegionRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "原因：未找到当前需要修改的数据！";
            }
            return result;
        }

        public bool Delete(int regionId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var region = RegionRepository.GetQueryable().FirstOrDefault(r => r.ID == regionId);
            if (region != null)
            {
                try
                {
                    RegionRepository.Delete(region);
                    RegionRepository.SaveChanges();
                    result = true;
                }
                catch (Exception)
                {
                    strResult = "原因：已在使用";
                }
            }
            else
            {
                strResult = "原因：未找到当前需要删除的数据！";
            }
            return result;
        }

        public object GetRegion(int page, int rows, string queryString, string value)
        {

            string id = "", regionName = "";

            if (queryString == "id")
            {
                id = value;

            //string regionName = "";
            //int id=-1;
            //if (queryString == "ID" && value!="")
            //{
            //    try { id = Convert.ToInt32(value); }
            //    catch { id =0; }

            }
            else
            {
                regionName = value;
            }

            IQueryable<Region> employeeQuery = RegionRepository.GetQueryable();
            var region = employeeQuery.Where(r =>r.RegionName.Contains(regionName) && r.State == "01")
                .OrderBy(r => r.ID).AsEnumerable()
                .Select(r => new
                {
                    r.ID,
                    r.Description,
                    r.RegionName,
                    State = r.State == "01" ? "可用" : "不可用",

            //IQueryable<Region> regionQuery = RegionRepository.GetQueryable();
            //var region = regionQuery.Where(r=> r.State == "01")
            //    .OrderBy(r => r.ID).AsEnumerable().
            //    Select(r => new
            //    {
            //        r.ID,
            //        r.RegionName,
            //        State = r.State == "01" ? "可用" : "不可用"
            //    });
            //if (id >=0)
            //{
            //    region = region.Where(r => r.ID == id);
            //}
            //else 
            //{
            //    region = region.Where(r => r.RegionName.Contains(regionName));
            //}
            //region = region.AsEnumerable().
            //    Select(r => new
            //    {
            //        r.ID,
            //        r.RegionName,
            //        r.State

            });
            int total = region.Count();
            region = region.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = region.ToArray() };
           
        }

        public System.Data.DataTable GetRegion(int page, int rows, string regionName, string state, string t)
        {
            IQueryable<Region> regionQuery = RegionRepository.GetQueryable();
            var srm = regionQuery.Where(r => r.RegionName.Contains(regionName))
                .OrderBy(r => r.ID).AsEnumerable()
                .Select(r => new
                {
                    r.ID,
                    r.RegionName,
                    r.Description,
                    State = r.State == "01" ? "可用" : "不可用"
                });
            if (!state.Equals(""))
            {
                srm = regionQuery.Where(r =>r.RegionName.Contains(regionName) && r.State.Contains(state))
                    .OrderBy(r => r.ID).AsEnumerable()
                    .Select(r => new
                    {
                        r.ID,
                        r.RegionName,
                        r.Description,
                        State = r.State == "01" ? "可用" : "不可用"
                    });
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("区域编码", typeof(string));
            dt.Columns.Add("区域名称", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            foreach (var item in srm)
            {
                dt.Rows.Add
                    (
                        item.ID,
                        item.RegionName,
                        item.Description,
                        item.State
                    );
            }
            return dt;
        }
    }
}

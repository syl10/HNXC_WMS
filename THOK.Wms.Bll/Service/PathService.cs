using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class PathService : ServiceBase<Path>, IPathService
    {
        [Dependency]
        public IPathRepository PathRepository { get; set; }

        [Dependency]
        public IRegionRepository RegionRepository { get; set; }

        UploadBll Upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string OriginRegion, string PathName, string TargetRegion, string State)
        {
          
           IQueryable<Path> pathQuery = PathRepository.GetQueryable();
           IQueryable<Region> regionQuery = RegionRepository.GetQueryable();
           var path = pathQuery.Join(regionQuery,
                       p => p.OriginRegionID,
                       r => r.ID,
                       (p, r) => new
                       {
                           p.ID,
                           p.PathName,
                           p.OriginRegionID,
                           p.TargetRegionID,
                           p.Description,
                           p.State,
                          
                           OriginRegionName = r.RegionName
                       })
                      .Join(regionQuery,
                      p => p.TargetRegionID,
                      r => r.ID,
                      (p, r) => new
                      {
                          p.ID,
                          p.PathName,
                          p.OriginRegionID,
                          p.TargetRegionID,
                          p.Description,
                          p.State,
                          p.OriginRegionName,
                          TargetRegionName=r.RegionName
                      })
               .Where(p => p.PathName.Contains(PathName) && p.State.Contains(State) && p.TargetRegionName.Contains(TargetRegion) 
                   && p.OriginRegionName.Contains(OriginRegion) && p.State.Contains(State))
                .OrderBy(p => p.ID).AsEnumerable()
                 .Select(p => new
                 {
                     p.ID,
                     p.PathName,
                     p.OriginRegionID,
                     p.OriginRegionName,
                     p.TargetRegionID,
                     p.TargetRegionName,
                     p.Description,
                     State = p.State == "01" ? "可用" : "不可用",
                 });
           int total = path.Count();
           path = path.Skip((page - 1) * rows).Take(rows);
           return new { total, rows = path.ToArray() };
        }


        public bool Add(Path path, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var emp = new Path();
          
          
                if (emp != null)
                {
                    try
                    {
                        emp.ID = path.ID;
                        emp.PathName = path.PathName;
                        emp.Description = path.Description;
                        emp.OriginRegionID = path.OriginRegionID;
                        emp.TargetRegionID = path.TargetRegionID;
                        emp.State = path.State;
                        PathRepository.Add(emp);

                        PathRepository.SaveChanges();
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
           
        public bool Save(Path path, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var emp = PathRepository.GetQueryable().FirstOrDefault(p => p.ID == path.ID);
            

       //public object GetPath(int page, int rows,string value)
       //{
       //    IQueryable<Path> pathQuery = PathRepository.GetQueryable();
       //    var path = pathQuery.Where(p => p.PathName.Contains(value) && p.State == "01")
       //        .OrderBy(p => p.ID).AsEnumerable().
       //        Select(p => new
       //        {
       //            p.ID,
       //            p.PathName,
       //            State = p.State == "01" ? "可用" : "不可用",
       //        });
       //    int total = path.Count();
       //    path = path.Skip((page - 1) * rows).Take(rows);
       //    return new { total, rows = path.ToArray() };
       //}
            if (emp != null)
            {
                try
                {
                    emp.ID = path.ID;
                    emp.PathName = path.PathName;
                    emp.Description = path.Description;
                    emp.OriginRegionID = path.OriginRegionID;
                    emp.TargetRegionID = path.TargetRegionID;
                    emp.State = path.State;
                    PathRepository.SaveChanges();
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

        public object GetPath(int page, int rows, string queryString, string value)
        {
            string id = "", pathName = "";


            if (queryString == "PathName")
            {
                id = value;
            }
            else
            {
                pathName = value;
            }
            IQueryable<Path> paQuery = PathRepository.GetQueryable();
            var path = paQuery.Where(p => p.PathName.Contains(pathName) && p.State == "01")
                .OrderBy(p => p.ID).AsEnumerable()
                .Select(p => new
                {
                    p.ID,
                    p.PathName,
                    p.Description,
                    State = p.State == "01" ? "可用" : "不可用",
                   
                });
            int total = path.Count();
            path = path.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = path.ToArray() };
        }

        public DataTable GetPath(int page, int rows, string Id, string pathName, string originId, string targetId, string state)
        {
            IQueryable<Path> pathQuery = PathRepository.GetQueryable();
             IQueryable<Region> regionQuery = RegionRepository.GetQueryable();
           var path = pathQuery.Join(regionQuery,
                       p => p.OriginRegionID,
                       r => r.ID,
                       (p, r) => new
                       {
                           p.ID,
                           p.PathName,
                           p.OriginRegionID,
                           p.TargetRegionID,
                           p.Description,
                           p.State,
                           
                           OriginRegionName = r.RegionName
                       })
                      .Join(regionQuery,
                      p => p.TargetRegionID,
                      r => r.ID,
                      (p, r) => new
                      {
                          p.ID,
                          p.PathName,
                          p.OriginRegionID,
                          p.TargetRegionID,
                          p.Description,
                          p.State,
                          p.OriginRegionName,
                          TargetRegionName=r.RegionName
                      })
               
                .OrderBy(p => p.ID).AsEnumerable()
                 .Select(p => new
                 {
                     p.ID,
                     p.PathName,
                     p.OriginRegionName,
                     p.TargetRegionName,
                     p.Description,
                     State = p.State == "01" ? "可用" : "不可用",
                 });
       
         
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("路径编号", typeof(int));
            dt.Columns.Add("路径名称", typeof(string));
            dt.Columns.Add("起始区域", typeof(string));
            dt.Columns.Add("目标区域", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("状态", typeof(string));
           foreach (var item in path)
            {
                dt.Rows.Add
                    (
                        item.ID,
                        item.PathName,
                        item.OriginRegionName,
                        item.TargetRegionName,
                        item.Description,
                        item.State
                       
                    );   
            }
            return dt;
        }
        

        public bool Delete(int pathId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var path = PathRepository.GetQueryable().FirstOrDefault(p => p.ID == pathId);
            if (path != null)
            {
                try
                {
                    PathRepository.Delete(path);
                    PathRepository.SaveChanges();
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




       
    }
}


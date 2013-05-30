using System;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using System.Linq;

namespace THOK.Wms.Bll.Service
{
    public class PathNodeService : ServiceBase<PathNode>, IPathNodeService
    {
        [Dependency]
        public IPathNodeRepository PathNodeRepository { get; set; }
        [Dependency]
        public IPathRepository PathRepository { get; set; }
        [Dependency]
        public IPositionRepository PositionRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string PathName, string PositionName, string PathNodeOrder)
        {
            //引入表
            IQueryable<PathNode> pathNodeQuery = PathNodeRepository.GetQueryable();
            IQueryable<Path> pathQuery = PathRepository.GetQueryable();
            IQueryable<Position> positionQuery = PositionRepository.GetQueryable();
            //关联表
            var pathNode = pathNodeQuery.Join(pathQuery,
                             pn => pn.PathID,
                             p => p.ID,
                             (pn, p) => new { pn.ID, pn.PathID, pn.PositionID, pn.PathNodeOrder, p.PathName })
                             .Join(positionQuery,
                             pn => pn.PositionID,
                             p => p.ID,
                             (pn, p) => new { pn.ID, pn.PathID, pn.PositionID, pn.PathNodeOrder, pn.PathName,p.PositionName })
                .Where(p => p.PathName.Contains(PathName) && p.PositionName.Contains(PositionName))
                .OrderBy(p => p.ID).AsEnumerable()
                .Select(p => new
                {
                    p.ID,
                    p.PathID,
                    p.PathName,
                    p.PositionID,
                    p.PositionName,
                    p.PathNodeOrder,
                });
            int Order = -1;
            if (PathNodeOrder != "" && PathNodeOrder != null)
            {
                try { Order = Convert.ToInt32(PathNodeOrder); }
                catch { Order = -1; }
                finally { 
                    pathNode=pathNode.Where(p =>p.PathNodeOrder==Order)
                            .OrderBy(p => p.ID).AsEnumerable()
                            .Select(p => new
                            {
                                p.ID,
                                p.PathID,
                                p.PathName,
                                p.PositionID,
                                p.PositionName,
                                p.PathNodeOrder,
                            });
                }
            }
            int total = pathNode.Count();
            pathNode = pathNode.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = pathNode.ToArray() };
        }

        public bool Add(PathNode pathNode, string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var p = new PathNode();
            if (p != null)
            {
                try
                {
                    p.PathID = pathNode.PathID;
                    p.PositionID = pathNode.PositionID;
                    p.PathNodeOrder = pathNode.PathNodeOrder;

                    PathNodeRepository.Add(p);
                    PathNodeRepository.SaveChanges();
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

        public bool Save(PathNode pathNode, string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var p = PathNodeRepository.GetQueryable().FirstOrDefault(pn => pn.ID == pathNode.ID);
            if (p != null)
            {
                try
                {
                    p.PathID = pathNode.PathID;
                    p.PositionID = pathNode.PositionID;
                    p.PathNodeOrder = pathNode.PathNodeOrder;

                    PathNodeRepository.SaveChanges();
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

        public bool Delete(PathNode pathNode, string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var p = PathNodeRepository.GetQueryable().FirstOrDefault(pn => pn.ID == pathNode.ID);
            if (p != null)
            {
                try
                {
                    PathNodeRepository.Delete(p);
                    PathNodeRepository.SaveChanges();
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

        public object GetPathNode(int page, int rows, string queryString, string value)
        {
            string id = "", PathID = "";

            if (queryString == "ID")
            {
                id = value;
            }
            else
            {
                PathID = value;
            }
            IQueryable<PathNode> PathNodeQuery = PathNodeRepository.GetQueryable();
            int Id = Convert.ToInt32(id);
            var PathNode = PathNodeQuery.Where(p => p.ID == Id)
                .OrderBy(p => p.ID).AsEnumerable().
                Select(p => new
                {
                    p.ID,
                    p.PathID,
                    p.PositionID,
                    p.PathNodeOrder,
                });
            int total = PathNode.Count();
            PathNode = PathNode.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = PathNode.ToArray() };
        }

        public System.Data.DataTable GetPathNode(int page, int rows, string id)
        {
            int t=-1;

            IQueryable<PathNode> pathNodeQuery = PathNodeRepository.GetQueryable();
            IQueryable<Path> pathQuery = PathRepository.GetQueryable();
            IQueryable<Position> positionQuery = PositionRepository.GetQueryable();

            var pathNode = pathNodeQuery.Join(pathQuery,
                             pn => pn.PathID,
                             p => p.ID,
                             (pn, p) => new { pn.ID, pn.PathID, pn.PositionID, pn.PathNodeOrder, PathName = p.PathName })
                             .Join(positionQuery,
                             pn => pn.PositionID,
                             p => p.ID,
                             (pn, p) => new { pn.ID, pn.PathID, pn.PositionID, pn.PathNodeOrder, pn.PathName, PositionName = p.PositionName })
                //.Where(p => p.ID == PathID)
              //  .Where(p => p.ID == t)
                .OrderBy(p => p.ID).AsEnumerable()
                .Select(p => new
                {
                    p.ID,
                    p.PathName,
                    p.PositionName,
                    p.PathNodeOrder,
                });
            try { t = Convert.ToInt32(id); }
            catch { t = -1; }
            finally
            {
                pathNode = pathNode.Where(p => p.ID == t)
                        .OrderBy(p => p.ID).AsEnumerable()
                        .Select(p => new
                        {
                            p.ID,
                            p.PathName,
                            p.PositionName,
                            p.PathNodeOrder,
                        });
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("路径名称", typeof(string));
            dt.Columns.Add("位置名称", typeof(string));
            dt.Columns.Add("路径节点顺序", typeof(string));
            foreach (var item in pathNode)
            {
                dt.Rows.Add
                    (
                        item.ID,
                        item.PathName,
                        item.PositionName,
                        item.PathNodeOrder
                    );
            }
            return dt;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IPathService : IService<Path>
    {
        object GetDetails(int page, int rows, string OriginRegion, string PathName, string TargetRegion, string State);

        bool Add(Path path, out string strResult);

        bool Save(Path path, out string strResult);

        bool Delete(int pathId, out string strResult);

        object GetPath(int page, int rows, string queryString, string value);

        System.Data.DataTable GetPath(int page, int rows, string Id, string pathName, string originId, string targetId, string state);
    }
}

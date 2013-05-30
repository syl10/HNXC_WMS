using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IPositionService : IService<Position>
    {

        object GetDetails(int page, int rows,string PositionName, string PositionType, string SRMName,string State);

        bool Add(Position position, out string strResult);

        bool Save(Position position, out string strResult);

        bool Delete(int positionId, out string strResult);

        object GetPosition(int page, int rows, string queryString, string value);

        System.Data.DataTable GetPosition(int page, int rows, string positionName, string srmName,string t);
    }
}

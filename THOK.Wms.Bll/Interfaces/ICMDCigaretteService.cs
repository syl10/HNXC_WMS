using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;


namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDCigaretteService: IService<CMD_CIGARETTE>
    {
        object GetDetails(int page, int rows, string CIGARETTE_NAME, string CIGARETTE_MOMO);

        bool Add(string CIGARETTE_NAME, string CIGARETTE_MEMO);

        bool Delete(string Cigarette_Code);

        bool Save(string CIGARETTE_CODE, string CIGARETTE_NAME, string CIGARETTE_MEMO);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class CMDCigaretteService : ServiceBase<CMD_CIGARETTE>, ICMDCigaretteService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDCigaretteRepository CMDCigaretteRepository { get; set; }

        public object GetDetails(int page, int rows, string CIGARETTE_NAME, string CIGARETTE_MEMO)
        {
            IQueryable<CMD_CIGARETTE> query = CMDCigaretteRepository.GetQueryable();
            var CMDCigarettes = query.OrderBy(i => i.CIGARETTE_CODE).Select(i => new { i.CIGARETTE_CODE, i.CIGARETTE_NAME, i.CIGARETTE_MEMO });
            if(!string.IsNullOrEmpty(CIGARETTE_NAME))
            {
               CMDCigarettes= CMDCigarettes.Where(i => i.CIGARETTE_NAME.Contains(CIGARETTE_NAME));
            }
            if (!string.IsNullOrEmpty(CIGARETTE_MEMO))
            {
                CMDCigarettes = CMDCigarettes.Where(i => i.CIGARETTE_MEMO.Contains(CIGARETTE_MEMO));
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(CMDCigarettes);
            }
            int total = 0;
            try
            {
                 total = CMDCigarettes.Count();
                CMDCigarettes = CMDCigarettes.Skip((page - 1) * rows).Take(rows);
            }
            catch (Exception ex) { }
            return new { total, rows = CMDCigarettes.ToArray() };
        }

        public bool Add(string CIGARETTE_NAME, string CIGARETTE_MEMO)
        {
            var city = new THOK.Wms.DbModel.CMD_CIGARETTE()
            {
                CIGARETTE_CODE = CMDCigaretteRepository.GetNewID("CMD_CIGARETTE", "CIGARETTE_CODE"),
                CIGARETTE_NAME = CIGARETTE_NAME,
                CIGARETTE_MEMO = CIGARETTE_MEMO
            };
            CMDCigaretteRepository.Add(city);
            CMDCigaretteRepository.SaveChanges();
            return true;
        }

        public bool Delete(string CIGARETTE_CODE)
        {
            var Cigarette = CMDCigaretteRepository.GetQueryable()
            .FirstOrDefault(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            CMDCigaretteRepository.Delete(Cigarette);
           int rejust= CMDCigaretteRepository.SaveChanges();
            if (rejust == -1) return false;
            else return true;

        }

        public bool Save(string CIGARETTE_CODE, string CIGARETTE_NAME, string CIGARETTE_MEMO)
        {
            var Cigarette = CMDCigaretteRepository.GetQueryable()
             .FirstOrDefault(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            Cigarette.CIGARETTE_NAME = CIGARETTE_NAME;
            Cigarette.CIGARETTE_MEMO = CIGARETTE_MEMO;
            CMDCigaretteRepository.SaveChanges();
            return true;
        }
    }
}

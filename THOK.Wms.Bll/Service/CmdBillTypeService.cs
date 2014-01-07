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
    public class CmdBillTypeService:ServiceBase<CMD_BILL_TYPE>,ICmdBillTypeService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICmdBillTypeRepository CmdBillTypeRepository { get; set; }

        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ISysBillTargetRepository SysBillTargetRepository { get; set; }


        [Dependency]
        public ISysTableStateService SysTableStateService { get; set; }

        public object GetDetails(int page, int rows, string BTYPE_NAME, string BILL_TYPE, string TASK_LEVEL, string Memo, string TARGET_CODE)
        {
            IQueryable<CMD_BILL_TYPE> query = CmdBillTypeRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> StateQuery=SysTableStateRepository.GetQueryable();
            IQueryable<SYS_BILL_TARGET> BillTargerQuery = SysBillTargetRepository.GetQueryable();
            //var CmdBillTypes = query.OrderBy(i => i.BTYPE_CODE).Select(i => new { i.BTYPE_CODE, i.BTYPE_NAME, i.ALLOW_EDIT, i.MEMO, i.TASK_LEVEL, i.BILL_TYPE, BILLTYPENAME = SysTableStateService.GetDescByState("CMD_BILL_TYPE", "BILL_TYPE", i.BILL_TYPE) });

            var CmdBillTypes = from a in query
                      join b in StateQuery on a.BILL_TYPE equals b.STATE
                      join c in BillTargerQuery on a.TARGET_CODE equals c.TARGET_CODE into Targer
                      from c in Targer.DefaultIfEmpty()

                      where b.TABLE_NAME=="CMD_BILL_TYPE" && b.FIELD_NAME== "BILL_TYPE" 
                               select new { a.BTYPE_CODE, a.BTYPE_NAME, a.ALLOW_EDIT, a.MEMO, a.TASK_LEVEL, a.BILL_TYPE, a.TARGET_CODE, BILLTYPENAME = b.STATE_DESC, TARGETNAME=c.TARGET_NAME };
                     



            if (!string.IsNullOrEmpty(BTYPE_NAME))
            {
                CmdBillTypes = CmdBillTypes.Where(i => i.BTYPE_NAME.Contains(BTYPE_NAME));
            }
            if (!string.IsNullOrEmpty(BILL_TYPE))
            {
                if (BILL_TYPE.Contains(','))
                {
                    CmdBillTypes = CmdBillTypes.Where(i => BILL_TYPE .Contains (i.BILL_TYPE ));
                }
              //else if(BILL_TYPE =="2")
              //  {
              //      CmdBillTypes = CmdBillTypes.Where(i => i.BILL_TYPE == BILL_TYPE&&i.BTYPE_CODE !="005");
              //   }
                else
                    CmdBillTypes = CmdBillTypes.Where(i => i.BILL_TYPE == BILL_TYPE);
            }
            if (!string.IsNullOrEmpty(TASK_LEVEL))
            {
                CmdBillTypes = CmdBillTypes.Where(i => i.TASK_LEVEL==TASK_LEVEL);
            }
            if (!string.IsNullOrEmpty(Memo))
            {
                CmdBillTypes = CmdBillTypes.Where(i => i.MEMO.Contains(Memo));
            }
            if (!string.IsNullOrEmpty(TARGET_CODE))
            {
                CmdBillTypes = CmdBillTypes.Where(i => i.TARGET_CODE==TARGET_CODE);
            }
            CmdBillTypes = CmdBillTypes.OrderBy(i => i.BTYPE_CODE).Select(i => i);
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(CmdBillTypes);
            }
            int total = CmdBillTypes.Count();
            CmdBillTypes = CmdBillTypes.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = CmdBillTypes.ToArray() };
        }

        public new bool Add(CMD_BILL_TYPE BillType)
        {
            BillType.BTYPE_CODE = CmdBillTypeRepository.GetNewID("CMD_BILL_TYPE", "BTYPE_CODE");
            BillType.ALLOW_EDIT = "1";
            if (BillType.BILL_TYPE != "2")
                BillType.TARGET_CODE = "";
            IQueryable<CMD_BILL_TYPE> query = CmdBillTypeRepository.GetQueryable();
            var taskType = query.OrderBy(i=>i.BTYPE_CODE).AsEnumerable().Select(b => new {b.BILL_TYPE,b.TARGET_CODE, b.TASK_TYPE });
            taskType = taskType.Where(i => i.BILL_TYPE == BillType.BILL_TYPE );
            if(!string.IsNullOrEmpty(BillType.TARGET_CODE))
                taskType = taskType.Where(i => i.TARGET_CODE == BillType.TARGET_CODE);
            BillType.TASK_TYPE = taskType.FirstOrDefault().TASK_TYPE;
           
            CmdBillTypeRepository.Add(BillType);
            CmdBillTypeRepository.SaveChanges();
            return true;
        }

        public bool Delete(string BTYPE_CODE)
        {
            var BillType = CmdBillTypeRepository.GetQueryable()
           .FirstOrDefault(i => i.BTYPE_CODE == BTYPE_CODE);
            CmdBillTypeRepository.Delete(BillType);
            int rejust= CmdBillTypeRepository.SaveChanges();
            if (rejust == -1) return false;
            else return true;
        }

        public bool Save(CMD_BILL_TYPE BillType)
        {
            var BillNewType = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == BillType.BTYPE_CODE);
            BillNewType.BTYPE_NAME = BillType.BTYPE_NAME;
            BillNewType.BILL_TYPE = BillType.BILL_TYPE;
            BillNewType.ALLOW_EDIT = "1";
            BillNewType.MEMO = BillType.MEMO;
            BillNewType.TASK_LEVEL = BillType.TASK_LEVEL;
            BillNewType.TARGET_CODE = BillType.TARGET_CODE;
            if (BillNewType.BILL_TYPE != "2")
                BillNewType.TARGET_CODE = "";
            IQueryable<CMD_BILL_TYPE> query = CmdBillTypeRepository.GetQueryable();
            var taskType = query.OrderBy(i => i.BTYPE_CODE).AsEnumerable().Select(b => new { b.BILL_TYPE, b.TARGET_CODE, b.TASK_TYPE });
            taskType = taskType.Where(i => i.BILL_TYPE == BillType.BILL_TYPE);
            if (!string.IsNullOrEmpty(BillType.TARGET_CODE))
                taskType = taskType.Where(i => i.TARGET_CODE == BillType.TARGET_CODE);
            BillType.TASK_TYPE = taskType.FirstOrDefault().TASK_TYPE;
            CmdBillTypeRepository.SaveChanges();

            
            return true;
        }
    }
}

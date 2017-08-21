using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace THOK.Common
{
    public   class PrintHandle
    {
        public static DataTable dt;
        public static DataTable searchdt;
        public static bool issearch=false;  //判断用户是否点击了  综合查询中的打印
        public static bool isbase = false;//判断用户是否点击了  基本资料中的打印
        public static DataTable baseinfoprint;
        //public static bool issuccess = false;
        /// <summary>
        /// 设置表结构,并
        /// </summary>
        /// <param name="tablestructstr">表结构字符串。</param>
        public static void setbaseinfodata(string tablestructstr) {
            //DataTable data = new DataTable();
            if (!string.IsNullOrEmpty(tablestructstr)) {
                string[] colums = tablestructstr.Split(';');
                for (int i = 0; i <baseinfoprint .Columns .Count ; i++) {
                    if (tablestructstr.Contains(baseinfoprint.Columns[i].ColumnName+":"))
                    {
                        for (int n = 0; n < colums.Length; n++) {
                            if (colums[n].Split(':')[0] == baseinfoprint.Columns[i].ColumnName) {
                                baseinfoprint.Columns[i].ColumnName = colums[n].Split(':')[1];
                                break;
                            }
                        }
                    }
                    else {
                        baseinfoprint.Columns.Remove(baseinfoprint.Columns[i].ColumnName);
                        i--;
                    }
                }
            }
        }
        //public  delegate void StockinprintEventHandler( DataTable dt,string soursename);
        //public static  event StockinprintEventHandler Stockinprint;
        //public static  void Onstockinprint(DataTable dt, string soursename)
        //{
        //    if (Stockinprint != null) {
        //        Stockinprint(dt,soursename);
        //    }
        //}
    }
}

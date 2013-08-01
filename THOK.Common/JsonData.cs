using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace THOK.Common
{
    public static class JsonData
    {
         public static DataTable JsonToDataTable(string strJson)
         {
             //取出表名  
            System.Text.RegularExpressions.Regex rg = new Regex(@"(?<={)[^:]+(?=:/\[)", RegexOptions.IgnoreCase);
             string strName = rg.Match(strJson).Value;
             DataTable tb = null;
             //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
             strJson = strJson.Substring(0, strJson.IndexOf("]"));

             //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
             MatchCollection mc = rg.Matches(strJson);
             for (int i = 0; i < mc.Count; i++)
             {
                 string strRow = mc[i].Value;
                 string[] strRows = strRow.Split(',');

                 //创建表  
                if (tb == null)
                 {
                     tb = new DataTable();
                     tb.TableName = strName;
                     foreach (string str in strRows)
                     {
                         DataColumn dc = new DataColumn();
                         string[] strCell = str.Split(':');
                         dc.ColumnName = strCell[0].ToString().Replace("\"", "");

                         tb.Columns.Add(dc);
                     }
                     tb.AcceptChanges();
                 }

                 //增加内容  
                DataRow dr = tb.NewRow();
                 for (int r = 0; r < strRows.Length; r++)
                 {
                     //dr[r] = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                     dr[strRows[r].Split(':')[0].Trim().Replace("\"", "")] = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                     //if(dr.ItemArray [r]=="")
                      
                 }
                 tb.Rows.Add(dr);
                 tb.AcceptChanges();
             }

             return tb;
         }

         /// <summary>
         /// 根据一行数据，绑定指定实体值；
         /// 此方法需要注意，DataRow的列名要与实体类属性名称一致
         /// </summary>
         /// <param name="entity">实体</param>
         /// <param name="row">一行数据</param>
         public static void DataBind(object entity, DataRow row)
         {
             //获取实体类类型
             Type type = entity.GetType();
             //获取实体类所有公共属性
             PropertyInfo[] infors = type.GetProperties();
             for (int i = 0; i < infors.Length; i++)
             {
                 //如果DataRow列名包含此属性
                 if (row.Table.Columns.Contains(infors[i].Name))
                 {
                     //获取值
                     object value = Convert.ChangeType(row[infors[i].Name], infors[i].PropertyType);
                     infors[i].SetValue(entity, value, null);
                 }
             }
         }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.WMS.Upload.Dao
{
    public class UploadDao:BaseDao
    {
        #region 查询分拣情况表数据，上报中烟

        string date = Convert.ToDateTime(DateTime.Now).ToString("yyyyMMdd");
        /// <summary>
        /// 查询分拣情况表【DWV_IORD_SORT_STATUS】，上报中烟
        /// </summary>
        /// <returns></returns>
        public DataTable QuerySortStatus()
        {
            string sql = "SELECT * FROM V_DWV_IORD_SORT_STATUS WHERE IS_IMPORT ='0'";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 插入分拣情况表【DWV_IORD_SORT_STATUS】，中烟数据库
        /// </summary>
        /// <param name="orderDetailTable"></param>
        public void InsertSortStatus(DataTable sortStatusTable)
        {
            foreach (DataRow row in sortStatusTable.Rows)
            {
                string sql = string.Format("INSERT INTO DWV_IORD_SORT_STATUS(SORT_BILL_ID,ORG_CODE,SORTING_CODE,SORT_DATE,SORT_SPEC," +
                    "SORT_QUANTITY,SORT_ORDER_NUM,SORT_BEGIN_DATE,SORT_END_DATE,SORT_COST_TIME,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}',{4},{5},{6},'{7}','{8}',{9},'{10}')",
                    row["SORT_BILL_ID"], row["ORG_CODE"], row["SORTING_CODE"], row["SORT_DATE"], row["SORT_SPEC"], row["SORT_QUANTITY"], row["SORT_ORDER_NUM"],
                    row["SORT_BEGIN_DATE"], row["SORT_END_DATE"], row["SORT_COST_TIME"], row["IS_IMPORT"]);
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 修改分拣情况表信息上报状态
        /// </summary>
        /// <param name="productCode"></param>
        public void UpdateSortStatus(string sortStatusCode)
        {
            string sql = "UPDATE DWV_IORD_SORT_STATUS SET IS_IMPORT='1' WHERE IS_IMPORT='0'";
            this.ExecuteNonQuery(sql);
        }


        #endregion

        #region 查询分拣线信息表数据，上报中烟

        /// <summary>
        /// 查询分拣线信息表【DWV_IDPS_SORTING】，上报中烟
        /// </summary>
        /// <returns></returns>
        public DataTable QueryIdpsSorting()
        {
            string sql = "SELECT * FROM V_DWV_IDPS_SORTING WHERE IS_IMPORT ='0'";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 插入分拣线信息表【DWV_IORD_SORT_STATUS】，中烟数据库
        /// </summary>
        /// <param name="orderDetailTable"></param>
        public void InsertIdpsSorting(DataTable SortingTable)
        {
            foreach (DataRow row in SortingTable.Rows)
            {
                string sql = string.Format("INSERT INTO DWV_IDPS_SORTING(SORTING_CODE,SORTING_NAME,SORTING_TYPE,ISACTIVE,UPDATE_DATE," +
                    "IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}')",
                    row["SORTING_CODE"], row["SORTING_NAME"], row["SORTING_TYPE"], row["ISACTIVE"], row["UPDATE_DATE"], row["IS_IMPORT"]);
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 修改分拣线信息表上报状态
        /// </summary>
        /// <param name="productCode"></param>
        public void UpdateSorting(string sortingCode)
        {
            string sql = string.Format("UPDATE DWV_DPS_SORTING SET IS_IMPORT='1' WHERE IS_IMPORT='0'");
            this.ExecuteNonQuery(sql);
        }

        #endregion

        #region 上报数据

        /// <summary>
        /// 上报卷烟信息
        /// </summary>
        /// <param name="brandSet"></param>
        public void InsertProduct(DataSet brandSet)
        {
            DataTable brandTable = brandSet.Tables["WMS_PRODUCT"];
            DataTable brandCode = this.ExecuteQuery("SELECT BRAND_CODE FROM ms.DWV_IINF_BRAND ").Tables[0];
            string brand_code = "''";
            string sql;
            for (int i = 0; i < brandCode.Rows.Count; i++)
            {
                brand_code += ",'" + brandCode.Rows[i]["BRAND_CODE"] + "'";
            }
            foreach (DataRow row in brandTable.Rows)
            {
                //string Sql = "select unit_code01 from wms_unit_list where unit_list_code='" + row["unit_list_code"] + "'";
                //string qtyUnit = this.ExecuteQuery(Sql).Tables[0].ToString();
                if (brand_code != "''" && brand_code.Contains(row["product_code"].ToString()))
                {
                    //sql = string.Format("UPDATE  ms.DWV_IINF_BRAND SET BRAND_CODE='{0}',BRAND_TYPE='{1}',BRAND_NAME='{2}',UP_CODE='{3}',BARCODE_BAR='{4}',PRICE_LEVEL_CODE='{5}',IS_FILTERTIP='{6}',IS_NEW='{7}',IS_FAMOUS='{8}'" +
                    //  " ,IS_MAINPRODUCT='{9}',IS_MAINPROVINCE='{10}',BELONG_REGION='{11}',IS_ABNORMITY_BRAND='{12}',BUY_PRICE={13},TRADE_PRICE={14},RETAIL_PRICE={15},COST_PRICE={16},QTY_UNIT={17}" +
                    //  ",BARCODE_ONE_PROJECT='{18}' ,UPDATE_DATE='{19}',ISACTIVE='{20}',N_UNIFY_CODE='{21}',IS_CONFISCATE='{22}',IS_IMPORT='{23}')", row["product_code"], row["product_type_code"], row["product_name"], row["product_code"], row["bar_barcode"], row["price_level_code"],
                    //  row["is_filter_tip"], row["is_new"], row["is_famous"], row["is_main_product"], row["is_province_main_product"], row["belong_region"],
                    //  row["is_abnormity"], row["buy_price"], row["trade_price"], row["retail_price"], row["cost_price"], qtyUnit, row["one_project_barcode"], date, row["is_active"], row["uniform_code"], row["is_confiscate"], "0");
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO ms.DWV_IINF_BRAND(BRAND_CODE,BRAND_TYPE,BRAND_NAME,UP_CODE,BARCODE_BAR,PRICE_LEVEL_CODE,IS_FILTERTIP,IS_NEW,IS_FAMOUS" +
                       " ,IS_MAINPRODUCT,IS_MAINPROVINCE,BELONG_REGION,IS_ABNORMITY_BRAND,BUY_PRICE,TRADE_PRICE,RETAIL_PRICE,COST_PRICE,QTY_UNIT" +
                       ",BARCODE_ONE_PROJECT ,UPDATE_DATE,ISACTIVE,N_UNIFY_CODE,IS_CONFISCATE,IS_IMPORT)" +
                       " VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},{14},{15},{16},{17},'{18}','{19}','{20}','{21}','{22}','{23}')", row["product_code"], row["product_type_code"], row["product_name"], row["product_code"], row["bar_barcode"], row["price_level_code"],
                       row["is_filter_tip"], row["is_new"], row["is_famous"], row["is_main_product"], row["is_province_main_product"], row["belong_region"],
                       row["is_abnormity"], row["buy_price"], row["trade_price"], row["retail_price"], row["cost_price"],row["qtyUnit"], row["one_project_barcode"], date, row["is_active"], row["uniform_code"], row["is_confiscate"], "0");
                    this.ExecuteNonQuery(sql);
                }
            }
        }
        /// <summary>
        /// 插入组织结构表【DWV_IORG_ORGANIZATION】，中烟数据库
        /// </summary>
        /// <param name="organTable"></param>
        public void InsertCompany(DataSet organSet)
        {
            DataTable organTable = organSet.Tables["wms_company"];
            DataTable custCode = this.ExecuteQuery("SELECT ORGANIZATION_CODE FROM DWV_IORG_ORGANIZATION ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["ORGANIZATION_CODE"] + "'";
            }
            foreach (DataRow row in organTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["company_code"].ToString()))
                {
                   // sql = string.Format("update DWV_IORG_ORGANIZATION SET ORGANIZATION_CODE='{0}',ORGANIZATION_NAME='{1}',ORGANIZATION_TYPE='{2}',UP_CODE='{3}'" +
                   // " ,N_ORGANIZATION_CODE='{4}',STORE_ROOM_AREA={5},STORE_ROOM_NUM={6},STORE_ROOM_CAPACITY={7},SORTING_NUM={8},UPDATE_DATE='{9}',ISACTIVE='{10}',IS_IMPORT='{11}')" 
                   //, row["company_code"], row["company_name"], row["company_type"], row["parent_company_id"],
                   // row["uniform_code"], row["warehouse_space"], row["warehouse_count"], row["warehouse_capacity"], row["sorting_count"],
                   // date, row["is_active"],'0');
                   // this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IORG_ORGANIZATION(ORGANIZATION_CODE,ORGANIZATION_NAME,ORGANIZATION_TYPE,UP_CODE" +
                   " ,N_ORGANIZATION_CODE,STORE_ROOM_AREA,STORE_ROOM_NUM,STORE_ROOM_CAPACITY,SORTING_NUM,UPDATE_DATE,ISACTIVE,IS_IMPORT)" +
                   "VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},'{9}','{10}','{11}')", row["company_code"], row["company_name"], row["company_type"], row["parent_company_id"],
                   row["uniform_code"] ?? "", row["warehouse_space"], row["warehouse_count"], row["warehouse_capacity"], row["sorting_count"],
                   date, row["is_active"],"0");
                    this.ExecuteNonQuery(sql);
                }
            }
            }
        /// <summary>
        /// 员工信息上报
        /// </summary>
        /// <param name="employeeSet"></param>
        public void InsertEmployee(DataSet employeeSet)
        {
            DataTable employeeTable = employeeSet.Tables["wms_employee"];
            DataTable custCode = this.ExecuteQuery("SELECT PERSON_CODE FROM DWV_IORG_PERSON ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["PERSON_CODE"] + "'";
            }
            foreach (DataRow row in employeeTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["employee_code"].ToString()))
                {
                     sql = string.Format("UPDATE DWV_IORG_PERSON SET PERSON_CODE='{0}',PERSON_N='{1}',PERSON_NAME='{2}',SEX='{3}'," +
                    " UPDATE_DATE='{4}',ISACTIVE='{5}',IS_IMPORT='{6}'", row["employee_code"],
                    row["employee_no"], row["employee_name"], row["sex"], date, row["is_active"],"0");
                    this.ExecuteNonQuery(sql);
                }
                else
                {
                     sql = string.Format("INSERT INTO DWV_IORG_PERSON(PERSON_CODE,PERSON_N,PERSON_NAME,SEX," +
                    " UPDATE_DATE,ISACTIVE,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", row["employee_code"],
                    row["employee_code"], row["employee_name"], row["sex"], date, row["is_active"],"0");
                    this.ExecuteNonQuery(sql);
                }
            }
        }
        /// <summary>
        ///  上报客户信息
        /// </summary>
        /// <param name="customSet"></param>
        public void InsertCustom(DataSet customSet)
        {
            DataTable customTable = customSet.Tables["DWV_IORG_CUSTOMER"];
            DataTable custCode = this.ExecuteQuery("SELECT CUST_CODE FROM ms.DWV_IORG_CUSTOMER ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["CUST_CODE"] + "'";
            }
            foreach (DataRow row in customTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["customer_code"].ToString()))
                {
                    //sql = string.Format("update DWV_IORG_CUSTOMER SET CUST_CODE='{0}',CUST_N='{1}',CUST_NAME='{2}',ORG_CODE='{3}',SALE_REG_CODE='{4}',CUST_TYPE='{5}',RTL_CUST_TYPE_CODE='{6}'," +
                    //   "CUST_GEO_TYPE_CODE='{7}',DIST_ADDRESS='{8}',DIST_PHONE='{9}',LICENSE_CODE='{10}',PRINCIPAL_NAME='{11}',UPDATE_DATE='{12}',ISACTIVE='{13}',IS_IMPORT='{14}',N_CUST_CODE='{15}',SEFL_CUST_FLG='{16}',FUNC_CUST_FLG='{17}',BUSI_CIRC_TYPE='{18}',CHAI_FLG='{19}',ALL_DAY_FLG='{20}',BUSI_HOUR='{21}',INFO_TERM='{22}',HEAD_LOGO='{23}',DELI_TIME='{24}',DELIVER_WAY='{25}',PAY_TYPE='{26}' WHERE CUST_CODE='{0}'"
                    //   , row["customer_code"], row["custom_code"],
                    //   row["customer_name"], row["company_code"], row["sale_region_code"], row["customer_type"], row["industry_type"], row["city_or_countryside"], row["address"], row["phone"],
                    //   row["license_code"], row["principal_name"], date, row["is_active"], "0", row["uniform_code"], 1, 0, 1, 2, 2, 1, 3, 3, 2, 1, 2);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IORG_CUSTOMER(CUST_CODE,CUST_N,CUST_NAME,ORG_CODE,SALE_REG_CODE,CUST_TYPE,RTL_CUST_TYPE_CODE," +
                       "CUST_GEO_TYPE_CODE,DIST_ADDRESS,DIST_PHONE,LICENSE_CODE,PRINCIPAL_NAME,UPDATE_DATE,ISACTIVE,IS_IMPORT,N_CUST_CODE,SEFL_CUST_FLG,FUNC_CUST_FLG,BUSI_CIRC_TYPE,CHAI_FLG,ALL_DAY_FLG,BUSI_HOUR,INFO_TERM,HEAD_LOGO,DELI_TIME,DELIVER_WAY,PAY_TYPE)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}' ,'{21}','{22}','{23}','{24}','{25}','{26}')", row["customer_code"], row["custom_code"],
                       row["customer_name"], row["company_code"], row["sale_region_code"], row["customer_type"], row["industry_type"], row["city_or_countryside"], row["address"], row["phone"],
                       row["license_code"], row["principal_name"], date, row["is_active"], "0", row["uniform_code"], 1, 0, 1, 2, 2, 1, 3, 3, 2, 1, 2);
                    this.ExecuteNonQuery(sql);
                }
            }
        }
        /// <summary>
        /// 仓储信息上报
        /// </summary>
        /// <param name="employeeSet"></param>
        public void InsertCell(DataSet cellSet)
        {
            DataTable cellTable = cellSet.Tables["wms_cell"];
            DataTable custCode = this.ExecuteQuery("SELECT STORAGE_CODE FROM DWV_IBAS_STORAGE ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["STORAGE_CODE"] + "'";
            }
            foreach (DataRow row in cellTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["employee_code"].ToString()))
                {
                    sql = string.Format("UPDATE DWV_IBAS_STORAGE SET STORAGE_TYPE='{1}',ORDER_NUM='{2}',CONTAINER='{3}',STORAGE_NAME='{4}',UP_CODE='{5}',DIST_CTR_CODE='{6}',N_ORG_CODE='{7}',N_STORE_ROOM_CODE='{8}',CAPACITY='{9}'," +
                 "HORIZONTAL_NUM='{10}',VERTICAL_NUM='{11}',AREA_TYPE='{12}',UPDATE_DATE='{13}',ISACTIVE='{14}',IS_IMPORT='{15}' where STORAGE_CODE='{0}'",
                 row["STORAGE_CODE"], row["STORAGE_TYPE"], row["ORDER_NUM"], row["CONTAINER"], row["STORAGE_NAME"], row["UP_CODE"], row["DIST_CTR_CODE"], row["N_ORG_CODE"], row["N_STORE_ROOM_CODE"], row["CAPACITY"], row["HORIZONTAL_NUM"], row["VERTICAL_NUM"], row["AREA_TYPE"],
                 date, row["ISACTIVE"],"0");
                    this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IBAS_STORAGE(STORAGE_CODE,STORAGE_TYPE,ORDER_NUM,CONTAINER,STORAGE_NAME,UP_CODE,DIST_CTR_CODE,N_ORG_CODE,N_STORE_ROOM_CODE,CAPACITY," +
                 "HORIZONTAL_NUM,VERTICAL_NUM,AREA_TYPE,UPDATE_DATE,ISACTIVE,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},{11},'{12}','{13}','{14}','{15}')",
                 row["STORAGE_CODE"], row["STORAGE_TYPE"], row["ORDER_NUM"], row["CONTAINER"], row["STORAGE_NAME"], row["UP_CODE"], row["DIST_CTR_CODE"], row["N_ORG_CODE"], row["N_STORE_ROOM_CODE"], row["CAPACITY"], row["HORIZONTAL_NUM"], row["VERTICAL_NUM"], row["AREA_TYPE"],
                 date, row["ISACTIVE"],"0");
                    this.ExecuteNonQuery(sql);
                }
            }
        }
        /// <summary>
        /// 仓库库存表
        /// </summary>
        /// <param name="stockTable"></param>
        public void InsertStoreStock(DataSet stockSet)
        {
            DataTable stockTable = stockSet.Tables["wms_storage"];
            string sql = "DELETE FROM DWV_IWMS_STORE_STOCK";
            this.ExecuteNonQuery(sql);
            foreach (DataRow row in stockTable.Rows)
            {
                sql = string.Format("INSERT INTO DWV_IWMS_STORE_STOCK(STORE_PLACE_CODE,BRAND_CODE,AREA_TYPE,BRAND_BATCH,DIST_CTR_CODE,QUANTITY,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                 "10002", row["product_code"], row["area_type"], date, row["dist_ctr_code"], row["quantity"], "0");
                this.ExecuteNonQuery(sql);
            }
        }
        /// <summary>
        /// 业务库存表
        /// </summary>
        /// <param name="stockTable"></param>
        public void InsertBustStock(DataSet stockSet)
        {
            DataTable stockTable = stockSet.Tables["wms_busistorage"];
            string sql = "DELETE FROM DWV_IWMS_BUSI_STOCK";
            this.ExecuteNonQuery(sql);
            foreach (DataRow row in stockTable.Rows)
            {
                sql = string.Format("INSERT INTO DWV_IWMS_BUSI_STOCK(ORG_CODE,BRAND_CODE,DIST_CTR_CODE,QUANTITY,IS_IMPORT)VALUES('{0}','{1}','{2}',{3},'{4}')",
                 "01", row["BRAND_CODE"],row["DIST_CTR_CODE"], row["QUANTITY"],"0");
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 入库单据主表
        /// </summary>
        /// <param name="inMasterTable"></param>
        public void InsertInMasterBill(DataSet inMasterSet)
        {
            DataTable inMasterTable = inMasterSet.Tables["WMS_IN_BILLMASTER"];
            DataTable custCode = this.ExecuteQuery("SELECT STORE_BILL_ID FROM DWV_IWMS_IN_STORE_BILL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["STORE_BILL_ID"] + "'";
            }
            foreach (DataRow row in inMasterTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["STORE_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_IN_STORE_BILL SET RELATE_STORE_BILL_ID='{1}',RELATE_BUSI_BILL_NUM={2},DIST_CTR_CODE='{3}',AREA_TYPE='{4}',QUANTITY_SUM={5}," +
                    //   "AMOUNT_SUM={6},DETAIL_NUM={7},CREATOR_CODE='{8}',CREATE_DATE='{9}',AUDITOR_CODE='{10}',AUDIT_DATE='{11}',ASSIGNER_CODE='{12}',ASSIGN_DATE='{13}',AFFIRM_CODE='{14}',AFFIRM_DATE='{15}'," +
                    //   "IN_OUT_TYPE='{16}',BILL_TYPE='{17}',BILL_STATUS='{18}',DISUSE_STATUS='{19}',IS_IMPORT='{20}' where STORE_BILL_ID='{0}'",
                    //   row["STORE_BILL_ID"], row["STORE_BILL_ID"], row["RELATE_BUSI_BILL_NUM"], row["DIST_CTR_CODE"], row["AREA_TYPE"], row["QUANTITY_SUM"], row["AMOUNT_SUM"], row["DETAIL_NUM"],
                    //   row["CREATOR_CODE"], row["CREATE_DATE"], row["AUDITOR_CODE"], row["AUDIT_DATE"], row["ASSIGNER_CODE"], row["ASSIGN_DATE"], row["AFFIRM_CODE"],
                    //   row["AFFIRM_DATE"], row["IN_OUT_TYPE"], row["BILL_TYPE"], row["BILL_STATUS"], row["DISUSE_STATUS"], row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_IN_STORE_BILL(STORE_BILL_ID,RELATE_STORE_BILL_ID,RELATE_BUSI_BILL_NUM,DIST_CTR_CODE,AREA_TYPE,QUANTITY_SUM," +
                         "AMOUNT_SUM,DETAIL_NUM,CREATOR_CODE,CREATE_DATE,AUDITOR_CODE,AUDIT_DATE,ASSIGNER_CODE,ASSIGN_DATE,AFFIRM_CODE,AFFIRM_DATE," +
                         "IN_OUT_TYPE,BILL_TYPE,BILL_STATUS,DISUSE_STATUS,IS_IMPORT)VALUES('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                         row["STORE_BILL_ID"], row["STORE_BILL_ID"], row["RELATE_BUSI_BILL_NUM"], row["DIST_CTR_CODE"], row["AREA_TYPE"], row["QUANTITY_SUM"], row["AMOUNT_SUM"], row["DETAIL_NUM"],
                         row["CREATOR_CODE"], row["CREATE_DATE"], row["AUDITOR_CODE"], row["AUDIT_DATE"], row["ASSIGNER_CODE"], row["ASSIGN_DATE"], row["AFFIRM_CODE"],
                         row["AFFIRM_DATE"], row["IN_OUT_TYPE"], row["BILL_TYPE"], row["BILL_STATUS"], row["DISUSE_STATUS"], row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
            sql = "UPDATE DWV_IWMS_IN_STORE_BILL SET DIST_CTR_CODE='0101',STORAGE_TYPE='4',STORAGE_CODE='10002',STORAGE_NAME='平顶山储位'";
            this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 入库单据细表
        /// </summary>
        /// <param name="inDetailTable"></param>
        public void InsertInDetailBill(DataSet inDetailSet)
        {
            DataTable inDetailTable = inDetailSet.Tables["WMS_IN_BILLDETAIL"];
            DataTable custCode = this.ExecuteQuery("SELECT DISTINCT STORE_BILL_ID FROM DWV_IWMS_IN_STORE_BILL_DETAIL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["STORE_BILL_ID"] + "'";
            }
            foreach (DataRow row in inDetailTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["STORE_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_IN_STORE_BILL_DETAIL SET STORE_BILL_ID='{1}',BRAND_CODE='{2}',BRAND_NAME='{3}',QUANTITY={4},IS_IMPORT='{5}'where STORE_BILL_DETAIL_ID='{0}'",
                    //   row["STORE_BILL_DETAIL_ID"], row["STORE_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"], row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_IN_STORE_BILL_DETAIL(STORE_BILL_DETAIL_ID,STORE_BILL_ID,BRAND_CODE,BRAND_NAME,QUANTITY,IS_IMPORT" +
                        ")VALUES('{0}','{1}','{2}','{3}',{4},'{5}')",
                        row["STORE_BILL_DETAIL_ID"], row["STORE_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"], row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
        }

        /// <summary>
        /// 入库业务单据表
        /// </summary>
        /// <param name="busiTable"></param>
        public void InsertInBusiBill(DataSet inBusiSet)
        {
            DataTable inBusiTable = inBusiSet.Tables["WMS_IN_BILLALLOT"];
            DataTable custCode = this.ExecuteQuery("SELECT DISTINCT BUSI_BILL_ID FROM DWV_IWMS_IN_BUSI_BILL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["BUSI_BILL_ID"] + "'";
            }
            foreach (DataRow row in inBusiTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["BUSI_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_IN_BUSI_BILL SET BUSI_BILL_DETAIL_ID='{1}',BUSI_BILL_ID='{2}',RELATE_BUSI_BILL_ID='{3}',STORE_BILL_ID='{4}',BRAND_CODE='{5}'," +
                    //"BRAND_NAME='{6}',QUANTITY='{7}',DIST_CTR_CODE='{8}',ORG_CODE='{9}',STORE_ROOM_CODE='{10}',STORE_PLACE_CODE='{11}',TARGET_NAME='{12}',IN_OUT_TYPE='{13}',BILL_TYPE='{14}',BEGIN_STOCK_QUANTITY={15}," +
                    //"END_STOCK_QUANTITY={16},DISUSE_STATUS='{17}',RECKON_STATUS='{18}',RECKON_DATE='{19}',UPDATE_CODE='{20}',UPDATE_DATE='{21}',IS_IMPORT='{22}'where BUSI_ACT_ID='{0}'",
                    //row["BUSI_ACT_ID"], row["BUSI_BILL_DETAIL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"],
                    //row["DIST_CTR_CODE"], row["ORG_CODE"], row["STORE_ROOM_CODE"], row["STORE_PLACE_CODE"], row["TARGET_NAME"], row["IN_OUT_TYPE"], row["BILL_TYPE"],
                    //row["BEGIN_STOCK_QUANTITY"], row["END_STOCK_QUANTITY"], row["DISUSE_STATUS"], row["RECKON_STATUS"], row["RECKON_DATE"], row["UPDATE_CODE"], date, row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_IN_BUSI_BILL(BUSI_ACT_ID,BUSI_BILL_DETAIL_ID,BUSI_BILL_ID,RELATE_BUSI_BILL_ID,STORE_BILL_ID,BRAND_CODE," +
                     "BRAND_NAME,QUANTITY,DIST_CTR_CODE,ORG_CODE,STORE_ROOM_CODE,STORE_PLACE_CODE,TARGET_NAME,IN_OUT_TYPE,BILL_TYPE,BEGIN_STOCK_QUANTITY," +
                     "END_STOCK_QUANTITY,DISUSE_STATUS,RECKON_STATUS,RECKON_DATE,UPDATE_CODE,UPDATE_DATE,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},'{17}','{18}','{19}','{20}','{21}','{22}')",
                     row["BUSI_ACT_ID"], row["BUSI_BILL_DETAIL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"],
                     row["DIST_CTR_CODE"], row["ORG_CODE"], row["STORE_ROOM_CODE"], row["STORE_PLACE_CODE"], row["TARGET_NAME"], row["IN_OUT_TYPE"], row["BILL_TYPE"],
                     row["BEGIN_STOCK_QUANTITY"], row["END_STOCK_QUANTITY"], row["DISUSE_STATUS"], row["RECKON_STATUS"], row["RECKON_DATE"], row["UPDATE_CODE"], date, row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
            sql = "update  DWV_IWMS_IN_BUSI_BILL set STORE_ROOM_CODE='1002',STORE_PLACE_CODE='10002' ";
            this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 出库单据主表
        /// </summary>
        /// <param name="inMasterTable"></param>
        public void InsertOutMasterBill(DataSet outMasterSet)
        {
            DataTable outMasterTable = outMasterSet.Tables["WMS_OUT_BILLMASTER"];
            DataTable custCode = this.ExecuteQuery("SELECT STORE_BILL_ID FROM DWV_IWMS_OUT_STORE_BILL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["STORE_BILL_ID"] + "'";
            }
            foreach (DataRow row in outMasterTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["STORE_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_OUT_STORE_BILL RELATE_STORE_BILL_ID='{1}',RELATE_BUSI_BILL_NUM={2},DIST_CTR_CODE='{3}',AREA_TYPE='{4}',QUANTITY_SUM={5}," +
                    //   "AMOUNT_SUM={6},DETAIL_NUM={7},CREATOR_CODE='{8}',CREATE_DATE='{9}',AUDITOR_CODE='{10}',AUDIT_DATE='{11}',ASSIGNER_CODE='{12}',ASSIGN_DATE='{13}',AFFIRM_CODE='{14}',AFFIRM_DATE='{15}'," +
                    //   "IN_OUT_TYPE='{16}',BILL_TYPE='{17}',BILL_STATUS='{18}',DISUSE_STATUS='{19}',IS_IMPORT='{20}' where STORE_BILL_ID='{0}'",
                    //   row["STORE_BILL_ID"], row["STORE_BILL_ID"], row["RELATE_BUSI_BILL_NUM"], row["DIST_CTR_CODE"], row["AREA_TYPE"], row["QUANTITY_SUM"], row["AMOUNT_SUM"], row["DETAIL_NUM"],
                    //   row["CREATOR_CODE"], row["CREATE_DATE"], row["AUDITOR_CODE"], row["AUDIT_DATE"], row["ASSIGNER_CODE"], row["ASSIGN_DATE"], row["AFFIRM_CODE"],
                    //   row["AFFIRM_DATE"], row["IN_OUT_TYPE"], row["BILL_TYPE"], row["BILL_STATUS"], row["DISUSE_STATUS"], row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_OUT_STORE_BILL(STORE_BILL_ID,RELATE_STORE_BILL_ID,RELATE_BUSI_BILL_NUM,DIST_CTR_CODE,AREA_TYPE,QUANTITY_SUM," +
                         "AMOUNT_SUM,DETAIL_NUM,CREATOR_CODE,CREATE_DATE,AUDITOR_CODE,AUDIT_DATE,ASSIGNER_CODE,ASSIGN_DATE,AFFIRM_CODE,AFFIRM_DATE," +
                         "IN_OUT_TYPE,BILL_TYPE,BILL_STATUS,DISUSE_STATUS,IS_IMPORT)VALUES('{0}','{1}',{2},'{3}','{4}',{5},{6},{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}')",
                         row["STORE_BILL_ID"], row["STORE_BILL_ID"], row["RELATE_BUSI_BILL_NUM"], row["DIST_CTR_CODE"], row["AREA_TYPE"], row["QUANTITY_SUM"], row["AMOUNT_SUM"], row["DETAIL_NUM"],
                         row["CREATOR_CODE"], row["CREATE_DATE"], row["AUDITOR_CODE"], row["AUDIT_DATE"], row["ASSIGNER_CODE"], row["ASSIGN_DATE"], row["AFFIRM_CODE"],
                         row["AFFIRM_DATE"], row["IN_OUT_TYPE"], row["BILL_TYPE"], row["BILL_STATUS"], row["DISUSE_STATUS"], row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
            sql = "UPDATE DWV_IWMS_OUT_STORE_BILL SET DIST_CTR_CODE='0101',STORAGE_TYPE='4',STORAGE_CODE='10002',STORAGE_NAME='平顶山储位'";
            this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 出库单据细表
        /// </summary>
        /// <param name="inDetailTable"></param>
        public void InsertOutDetailBill(DataSet outDetailSet)
        {
            DataTable outDetailTable = outDetailSet.Tables["WMS_OUT_BILLDETAIL"];
            DataTable custCode = this.ExecuteQuery("SELECT DISTINCT STORE_BILL_ID FROM DWV_IWMS_OUT_STORE_BILL_DETAIL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["STORE_BILL_ID"] + "'";
            }
            foreach (DataRow row in outDetailTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["STORE_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_OUT_STORE_BILL_DETAIL set STORE_BILL_ID='{1}',BRAND_CODE='{2}',BRAND_NAME='{3}',QUANTITY={4},IS_IMPORT='{5}' where STORE_BILL_DETAIL_ID='{0}'",
                    //   row["STORE_BILL_DETAIL_ID"], row["STORE_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"], row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_OUT_STORE_BILL_DETAIL(STORE_BILL_DETAIL_ID,STORE_BILL_ID,BRAND_CODE,BRAND_NAME,QUANTITY,IS_IMPORT" +
                        ")VALUES('{0}','{1}','{2}','{3}',{4},'{5}')",
                        row["STORE_BILL_DETAIL_ID"], row["STORE_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"], row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
        }

        /// <summary>
        /// 出库业务单据表
        /// </summary>
        /// <param name="busiTable"></param>
        public void InsertOutBusiBill(DataSet outBusiSet)
        {
            DataTable outBusiTable = outBusiSet.Tables["WMS_OUT_BILLALLOT"];
            DataTable custCode = this.ExecuteQuery("SELECT DISTINCT BUSI_BILL_ID FROM DWV_IWMS_OUT_BUSI_BILL ").Tables[0];
            string cust_code = "''";
            string sql;
            for (int i = 0; i < custCode.Rows.Count; i++)
            {
                cust_code += ",'" + custCode.Rows[i]["BUSI_BILL_ID"] + "'";
            }
            foreach (DataRow row in outBusiTable.Rows)
            {
                if (cust_code != "''" && cust_code.Contains(row["BUSI_BILL_ID"].ToString()))
                {
                    //sql = string.Format("UPDATE DWV_IWMS_OUT_BUSI_BILL set BUSI_BILL_DETAIL_ID='{1}',BUSI_BILL_ID='{2}',RELATE_BUSI_BILL_ID='{3}',STORE_BILL_ID='{4}',BRAND_CODE='{5}'," +
                    //"BRAND_NAME='{6}',QUANTITY='{7}',DIST_CTR_CODE='{8}',ORG_CODE='{9}',STORE_ROOM_CODE='{10}',STORE_PLACE_CODE='{11}',TARGET_NAME='{12}',IN_OUT_TYPE='{13}',BILL_TYPE='{14}',BEGIN_STOCK_QUANTITY={15}," +
                    //"END_STOCK_QUANTITY={16},DISUSE_STATUS='{17}',RECKON_STATUS='{18}',RECKON_DATE='{19}',UPDATE_CODE='{20}',UPDATE_DATE='{21}',IS_IMPORT='{22}' where  BUSI_ACT_ID='{0}'",
                    //row["BUSI_ACT_ID"], row["BUSI_BILL_DETAIL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"],
                    //row["DIST_CTR_CODE"], row["ORG_CODE"], row["STORE_ROOM_CODE"], row["STORE_PLACE_CODE"], row["TARGET_NAME"], row["IN_OUT_TYPE"], row["BILL_TYPE"],
                    //row["BEGIN_STOCK_QUANTITY"], row["END_STOCK_QUANTITY"], row["DISUSE_STATUS"], row["RECKON_STATUS"], row["RECKON_DATE"], row["UPDATE_CODE"], date, row["IS_IMPORT"]);
                    //this.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = string.Format("INSERT INTO DWV_IWMS_OUT_BUSI_BILL(BUSI_ACT_ID,BUSI_BILL_DETAIL_ID,BUSI_BILL_ID,RELATE_BUSI_BILL_ID,STORE_BILL_ID,BRAND_CODE," +
                     "BRAND_NAME,QUANTITY,DIST_CTR_CODE,ORG_CODE,STORE_ROOM_CODE,STORE_PLACE_CODE,TARGET_NAME,IN_OUT_TYPE,BILL_TYPE,BEGIN_STOCK_QUANTITY," +
                     "END_STOCK_QUANTITY,DISUSE_STATUS,RECKON_STATUS,RECKON_DATE,UPDATE_CODE,UPDATE_DATE,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}','{10}','{11}','{12}','{13}','{14}',{15},{16},'{17}','{18}','{19}','{20}','{21}','{22}')",
                     row["BUSI_ACT_ID"], row["BUSI_BILL_DETAIL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BUSI_BILL_ID"], row["BRAND_CODE"], row["BRAND_NAME"], row["QUANTITY"],
                     row["DIST_CTR_CODE"], row["ORG_CODE"], row["STORE_ROOM_CODE"], row["STORE_PLACE_CODE"], row["TARGET_NAME"], row["IN_OUT_TYPE"], row["BILL_TYPE"],
                     row["BEGIN_STOCK_QUANTITY"], row["END_STOCK_QUANTITY"], row["DISUSE_STATUS"], row["RECKON_STATUS"], row["RECKON_DATE"], row["UPDATE_CODE"], date, row["IS_IMPORT"]);
                    this.ExecuteNonQuery(sql);
                }
            }
            sql = "update  DWV_IWMS_OUT_BUSI_BILL set STORE_ROOM_CODE='1002',STORE_PLACE_CODE='10002' ";
            this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 上报分拣订单主表
        /// </summary>
        /// <param name="orderMasterSet"></param>
        public void UploadIordOrder(DataSet orderMasterSet)
        {
            DataTable orderMasterTable = orderMasterSet.Tables["DWV_OUT_ORDER"];
            foreach (DataRow row in orderMasterTable.Rows)
            {
                string sql = string.Format("INSERT INTO DWV_IORD_ORDER(ORDER_ID,ORG_CODE,SALE_REG_CODE,ORDER_DATE,ORDER_TYPE," +
                    "CUST_CODE,CUST_NAME,QUANTITY_SUM,AMOUNT_SUM,DETAIL_NUM,DELIVER_ORDER,ISACTIVE,UPDATE_DATE,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10},'{11}','{12}','{13}')",
                    row["ORDER_ID"], row["COMPANY_CODE"], row["SALE_REGION_CODE"], row["ORDER_DATE"], row["ORDER_TYPE"], row["CUSTOMER_CODE"], row["CUSTOMER_NAME"],
                    row["QUANTITY_SUM"], row["AMOUNT_SUM"], row["DETAIL_NUM"], row["DELIVER_ORDER"], row["ISACTIVE"], row["UPDATE_DATE"], "0");
                this.ExecuteNonQuery(sql);
            }
        }
        /// <summary>
        /// 上报分拣订单细表
        /// </summary>
        /// <param name="orderDetailTable"></param>
        public void UploadIordOrderDetail(DataSet orderDetailSet)
        {
            DataTable orderDetailTable = orderDetailSet.Tables["DWV_OUT_ORDER_DETAIL"];
            foreach (DataRow row in orderDetailTable.Rows)
            {
                string sql = string.Format("INSERT INTO DWV_IORD_ORDER_DETAIL(ORDER_DETAIL_ID,ORDER_ID,BRAND_CODE,BRAND_NAME,BRAND_UNIT_NAME," +
                    "QTY_DEMAND,QUANTITY,PRICE,AMOUNT,IS_IMPORT)VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8},'{9}')",
                    row["ORDER_DETAIL_ID"], row["ORDER_ID"], row["PRODUCT_CODE"], row["PRODUCT_NAME"], row["UNIT_NAME"], row["DEMAND_QUANTITY"], row["REAL_QUANTITY"],
                    row["PRICE"], row["AMOUNT"], "0");
                this.ExecuteNonQuery(sql);
            }
        }
        /// <summary>
        /// 插入同步状态表【DWV_IOUT_SYNCHRO_INFO】
        /// </summary>
        /// <param name="synchroTable"></param>
        public void InsertSynchro()
        {
            string date = Convert.ToDateTime(DateTime.Now).ToString("yyyyMMddHHmmss");
            string sql = string.Format("UPDATE DWV_IOUT_SYNCHRO_INFO SET UPDATE_DATE='" + date + "'");
            this.ExecuteNonQuery(sql);
        }
        #endregion
    }
}

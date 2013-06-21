using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.Bll.Models
{
    public class NewWareTree
    {
        //public string Code { get; set; }
        //public string Name { get; set; }
        //public string WarehouseCode { get; set; }
        //public string WarehouseName { get; set; }
        //public string AreaCode { get; set; }
        //public string AreaName { get; set; }
        //public string ShelfCode { get; set; }
        //public string ShelfName { get; set; }
        //public string CellCode { get; set; }
        //public string CellName { get; set; }

        //public int AllotInOrder{ get; set; }
        //public int AllotOutOrder { get; set; }

        //public int MaxQuantity { get; set; }
        //public int Layer { get; set; }
        //public string ProductName { get; set; }

        //public string ShortName { get; set; }       
        //public string Type { get; set; }
        //public string Description { get; set; }
        //public string IsActive { get; set; }
        //public string UpdateTime { get; set; }
        public string CODE { get; set; }
        public string NAME { get; set; }

        public string WAREHOUSE_CODE { get; set; }
        public string WAREHOUSE_NAME { get; set; }
        public string MEMO { get; set; }

        public string AREA_CODE { get; set; }
        public string ROW_COUNT { get; set; }
        public string COLUMN_COUNT { get; set; }
        public string AREA_NAME { get; set; }
        //public string WAREHOUSE_CODE { get; set; }
        //public string MEMO { get; set; }

        public string SHELF_CODE { get; set; }
        public string SHELF_NAME { get; set; }
        //public string ROW_COUNT { get; set; }
        //public string COLUMN_COUNT { get; set; }
        //public string WAREHOUSE_CODE { get; set; }
        //public string AREA_CODE { get; set; }
        public string CRANE_NO { get; set; }
        //public string STATION_NO { get; set; }
        //public string MEMO { get; set; }


        public string CELL_CODE { get; set; }
        public string CELL_NAME { get; set; }
        public string IS_ACTIVE { get; set; }

        //public string AREA_CODE { get; set; }
        //public string SHELF_CODE { get; set; }
        //public string CELL_COLUMN { get; set; }
        //public string CELL_ROW { get; set; }

        //public string PRODUCT_BARCODE { get; set; }
        //public string REAL_WEIGHT { get; set; }
        //public string SCHEDULE_NO { get; set; }
        //public string IS_LOCK { get; set; }
        //public string PALLET_RFID { get; set; }
        //public string BILL_NO { get; set; }
        //public string IN_DATE { get; set; }
        //public string MEMO { get; set; }
        //public string PRIORITY_LEVEL { get; set; }
        //public string ERROR_FLAG { get; set; }



        public string  ATTRIBUTES { get; set; }
        public NewWareTree[] children { get; set; }
    }
}

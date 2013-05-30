using System;
using System.Collections.Generic;
using System.Text;

namespace THOK.Wms.AutomotiveSystems.Models
{
    public class BillDetail
    {
        public string BillNo = string.Empty;
        public string BillType = string.Empty;

        public int DetailID = 0;
        public string StorageName = string.Empty;
        public string StorageRfid = string.Empty;
        public string CellRfid = string.Empty;
        public string TargetStorageName = string.Empty;
        public string TargetStorageRfid = string.Empty;

        public string ProductCode = string.Empty;
        public string ProductName = string.Empty;

        public decimal PieceQuantity = 0;
        public decimal BarQuantity = 0;
        public decimal OperatePieceQuantity = 0;
        public decimal OperateBarQuantity = 0;

        public string OperatorCode = string.Empty;
        public string Operator = string.Empty;
        public string Status = string.Empty;
        public int PalletTag = 0;
    }
}

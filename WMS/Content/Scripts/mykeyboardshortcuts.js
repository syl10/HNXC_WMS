$(document).keydown(function (e) {
    e = e ? e : window.event;
    var keyCode = e.keyCode ? e.keyCode : e.which ? e.which : e.charCode;
    if (keyCode == 13) {
        /*----------------- enter 查找页 -------------------*/
        if (searchKey == true) {
            searchKey = false;
            switch (module) {
                //组织结构管理
                case "Company":
                case "Department":
                case "Employee":
                case "Job":
                    GetQueryParams();
                    break;
                //仓库信息管理
                case "Warehouse2":
                case "DefaultProductSet":
                    select();
                    break;
                //卷烟信息管理
                case "Product":
                case "Supplier":
                case "Brand":
                case "UnitList":
                case "Unit":
                //入库单据管理
                case "StockInBillType":
                    select();
                    break;
                case "StockInBill":
                    GetQueryParams()
                    break;
                //出库单据管理
                case "StockOutBillType":
                case "StockOutBill":
                //移库单据管理
                case "StockMoveBillType":
                case "StockMoveBill":
                //盘点单据管理
                case "CheckBillType":
                case "CheckBill":
                //损益单据管理
                case "ProfitLossBillType":
                case "ProfitLossBill":
                //库存信息管理
                case "DailyBalance":
                case "CurrentStock":
                case "Distribution":
                case "Cargospace":
                case "StockLedger":
                case "HistoricalDetail":
                case "CellHistorical":
                //分拣信息管理
                case "SortingLine":
                case "SortingLowerLimit":
                case "SortingOrder":
                case "SortOrderDispatch":
                case "SortWorkDispatch":
                //综合数据查询
                case "StockIntoSearch":
                case "StockOutSearch":
                case "StockMoveSearch":
                case "StockCheckSearch":
                case "StockDifferSearch":
                case "SortOrderSearch":
                //产品质量管理
                case "ProductWarning":
                case "QuantityLimits":
                case "ProductTimeOut":
                    select();
                    break;
            }
        }
        /*----------------- enter 增、删、改 页 -------------------*/
        if (addKey == true || editKey == true || deleteKey == true) {
            deleteKey = false;
            switch (module) {
                //组织结构管理
                case "Company":
                case "Department":
                case "Employee":
                case "Job":
                //仓库信息管理
                case "Warehouse2":
                    save();
                    break;
                case "DefaultProductSet":
                    enterSave();
                    break;
                //卷烟信息管理
                case "Product":
                case "Supplier":
                case "Brand":
                case "UnitList":
                case "Unit":
                //入库单据管理
                case "StockInBillType":
                case "StockInBill":
                //出库单据管理
                case "StockOutBillType":
                case "StockOutBill":
                //移库单据管理
                case "StockMoveBillType":
                case "StockMoveBill":
                //盘点单据管理
                case "CheckBillType":
                case "CheckBill":
                //损益单据管理
                case "ProfitLossBillType":
                case "ProfitLossBill":
                //库存信息管理
                case "DailyBalance":
                case "CurrentStock":
                case "Distribution":
                case "Cargospace":
                case "StockLedger":
                case "HistoricalDetail":
                case "CellHistorical":
                //分拣信息管理
                case "SortingLine":
                case "SortingLowerLimit":
                case "SortingOrder":
                case "SortOrderDispatch":
                case "SortWorkDispatch":
                //综合数据查询
                case "StockIntoSearch":
                case "StockOutSearch":
                case "StockMoveSearch":
                case "StockCheckSearch":
                case "StockDifferSearch":
                case "SortOrderSearch":
                //产品质量管理
                case "ProductWarning":
                case "QuantityLimits":
                case "ProductTimeOut":
                    save();
                    break;
            }
        }
        if (productKey = true) {
            switch (module) {
                case "SortingLowerLimit":
                    ProductQueryClick();
                    break;
            }
        }
    }
    if (keyCode == 27) {
        /*------------------ esc 查找页 -------------------*/
        if (searchKey == true) {
            searchKey = false;
            switch (module) {
                //组织结构管理
                case "Company":
                case "Department":
                case "Employee":
                case "Job":
                    searchDialog.dialog('close');
                    break;
                //仓库信息管理
                case "Warehouse2":
                case "DefaultProductSet":
                    $('#dlgSearch').dialog('close')
                    break;
                //卷烟信息管理
                case "Product":
                case "Supplier":
                case "Brand":
                case "UnitList":
                case "Unit":
                //入库单据管理
                case "StockInBillType":
                    $('#dlg-search').dialog('close');
                    break;
                case "StockInBill":
                    searchDialog.dialog('close')
                    break;
                //出库单据管理
                case "StockOutBillType":
                case "StockOutBill":
                //移库单据管理
                case "StockMoveBillType":
                case "StockMoveBill":
                //盘点单据管理
                case "CheckBillType":
                case "CheckBill":
                //损益单据管理
                case "ProfitLossBillType":
                case "ProfitLossBill":
                //库存信息管理
                case "DailyBalance":
                case "CurrentStock":
                case "Distribution":
                case "Cargospace":
                case "StockLedger":
                case "HistoricalDetail":
                case "CellHistorical":
                    $('#dlg-search').dialog('close');
                    break;
                //分拣信息管理
                case "SortingLine":
                    $('#searchdlg').dialog('close');
                    break;
                case "SortingLowerLimit":
                case "SortingOrder":
                    $('#dlg-search').dialog('close');
                    break;
                case "SortOrderDispatch":
                case "SortWorkDispatch":
                    $('#searchdlg').dialog('close');
                    break;
                //综合数据查询
                case "StockIntoSearch":
                    $('#dlg-search').dialog('close');
                    break;
                case "StockOutSearch":
                case "StockMoveSearch":
                case "StockCheckSearch":
                case "StockDifferSearch":
                case "SortOrderSearch":
                //产品质量管理
                case "ProductWarning":
                case "QuantityLimits":
                case "ProductTimeOut":
                    $('#dlg-search').dialog('close');
                    break;
            }
        }
        /*----------------- esc 增、删、改 页 -------------------*/
        if (addKey == true || editKey == true || deleteKey == true) {
            addKey = false;
            editKey = false;
            deleteKey = false;
            switch (module) {
                //组织结构管理
                case "Company":
                case "Department":
                case "Employee":
                case "Job":
                    addDialog.dialog('close');
                    break;
                //仓库信息管理
                case "Warehouse2":
                case "DefaultProductSet":
                //卷烟信息管理
                case "Product":
                case "Supplier":
                case "Brand":
                    $('#dlg').dialog('close');
                    break;
                case "UnitList":
                    $('#dlg-add').dialog('close');
                    break;
                case "Unit":
                //入库单据管理
                case "StockInBillType":
                case "StockInBill":
                //出库单据管理
                case "StockOutBillType":
                case "StockOutBill":
                //移库单据管理
                case "StockMoveBillType":
                case "StockMoveBill":
                //盘点单据管理
                case "CheckBillType":
                    $('#dlg').dialog('close');
                    break;
                case "CheckBill":
                    $('#addCheck').dialog('close');
                    break;
                //损益单据管理
                case "ProfitLossBillType":
                case "ProfitLossBill":
                //库存信息管理
                case "DailyBalance":
                case "CurrentStock":
                case "Distribution":
                case "Cargospace":
                case "StockLedger":
                case "HistoricalDetail":
                case "CellHistorical":
                //分拣信息管理
                case "SortingLine":
                case "SortingLowerLimit":
                case "SortingOrder":
                case "SortOrderDispatch":
                case "SortWorkDispatch":
                //综合数据查询
                case "StockIntoSearch":
                case "StockOutSearch":
                case "StockMoveSearch":
                case "StockCheckSearch":
                case "StockDifferSearch":
                case "SortOrderSearch":
                //产品质量管理
                case "ProductWarning":
                case "QuantityLimits":
                case "ProductTimeOut":
                    $('#dlg').dialog('close');
                    break;
            }
        }
    }
});
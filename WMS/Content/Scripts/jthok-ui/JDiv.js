$(function () {
    
});
function show(div_id, height, width) {
    $(div_id).show().css("top", ($(document).height() - height) / 2).css("left", ($(document).width() - width) / 2);
}

function hide(div_id) {
    $(div_id).hide();
}

//设置只读控件的背景色.
function setbg() {
    $(":input").each(function (index, eleme) {
        if ($(eleme).attr("readonly") == "true" || $(eleme).attr("readonly") == "readonly") {
            $(eleme).addClass("readonlybgcolor");
        }
        else {
            $(eleme).removeClass("readonlybgcolor");
        }
    });
}

//用于按钮样式的改变
function btmouseup(obj) {
    $(obj).addClass('btclasschange');
}
function btmouseout(obj) {
    $(obj).removeClass("btclasschange");
}
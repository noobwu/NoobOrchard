//**************// 
//* EasyUI扩展 *//
//**************// 

//扩展外键字段关联
$.extend($.fn.datagrid.defaults.view, {
    renderRow: function (target, fields, frozen, rowIndex, rowData) {
        var opts = $.data(target, 'datagrid').options;
        var cc = [];
        if (frozen && opts.rownumbers) {
            var rownumber = rowIndex + 1;
            if (opts.pagination) {
                rownumber += (opts.pageNumber - 1) * opts.pageSize;
            }
            cc.push('<td class="datagrid-td-rownumber"><div class="datagrid-cell-rownumber">' + rownumber + '</div></td>');
        }
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var col = $(target).datagrid('getColumnOption', field);
            if (col) {
                //start 处理多级数据
                var fieldSp = field.split(".");
                var dta = rowData[fieldSp[0]];
                for (var j = 1; j < fieldSp.length; j++) {
                    dta = dta != null ? dta[fieldSp[j]] : "";
                }
                //end 处理多级数据
                // get the cell style attribute
                var styleValue = col.styler ? (col.styler(dta, rowData, rowIndex) || '') : '';
                var style = col.hidden ? 'style="display:none;' + styleValue + '"' : (styleValue ? 'style="' + styleValue + '"' : '');
                cc.push('<td field="' + field + '" ' + style + '>');
                if (col.checkbox) {
                    var style = '';
                } else {
                    var style = '';
                    //var style = 'width:' + (col.boxWidth) + 'px;';
                    style += 'text-align:' + (col.align || 'left') + ';';
                    if (!opts.nowrap) {
                        style += 'white-space:normal;height:auto;';
                    } else if (opts.autoRowHeight) {
                        style += 'height:auto;';
                    }
                }

                cc.push('<div style="' + style + '" ');
                if (col.checkbox) {
                    cc.push('class="datagrid-cell-check ');
                } else {
                    cc.push('class="datagrid-cell ' + col.cellClass);
                }
                cc.push('">');

                if (col.checkbox) {
                    cc.push('<input type="checkbox" name="' + field + '" value="' + (dta != undefined ? dta : '') + '"/>');
                } else if (col.formatter) {
                    cc.push(col.formatter(dta, rowData, rowIndex));
                } else {
                    cc.push(dta);
                }

                cc.push('</div>');
                cc.push('</td>');
            }
        }
        return cc.join('');
    }
});

function NoneData(obj) {
    if ($(obj).datagrid("getRows").length == 0) {
        $.messager.alert("提示", "未找到任何数据！");
        return false;
    }
    return true;
}

//Json时间转换公用函数
function formatJsonDate(obj) {
    var date = new Date(parseInt(obj.replace("/Date(", "").replace(")/", ""), 10));
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
}

//Json时间转行函数，返回Date对象
function getFormatJsonDate(obj) {
    var date = new Date(parseInt(obj.replace("/Date(", "").replace(")/", ""), 10));
    return date;
}

//Json时间转换格式为 2010-10-02 23:59:59
function formatJsonDateTime(obj, format) {
    obj = obj + "";
    if (obj.indexOf("/Date(", 0) == -1) {
        return obj;
    }
    var date = new Date(parseInt(obj.replace("/Date(", "").replace(")/", ""), 10));

    //    if (date.getMonth() < 11) {
    //        date.setMonth(date.getMonth() + 1);
    //    }

    if (format) {
        return date.Format(format);
    }
    else {
        return date.Format("yyyy-MM-dd hh:mm:ss");
    }
}

//---------------------------------------------------  
// 日期格式化  
// 格式 YYYY/yyyy/YY/yy 表示年份  
// MM/M 月份  
// W/w 星期  
// dd/DD/d/D 日期  
// hh/HH/h/H 时间  
// mm/m 分钟  
// ss/SS/s/S 秒  
//---------------------------------------------------  
Date.prototype.Format = function (formatStr) {
    var str = formatStr;
    var Week = ['日', '一', '二', '三', '四', '五', '六'];

    str = str.replace(/yyyy|YYYY/, this.getFullYear());
    str = str.replace(/yy|YY/, (this.getYear() % 100) > 9 ? (this.getYear() % 100).toString() : '0' + (this.getYear() % 100));

    str = str.replace(/MM/, this.getMonth() + 1 > 9 ? (this.getMonth() + 1).toString() : '0' + (this.getMonth() + 1));
    str = str.replace(/M/g, this.getMonth() + 1);

    str = str.replace(/w|W/g, Week[this.getDay()]);

    str = str.replace(/dd|DD/, this.getDate() > 9 ? this.getDate().toString() : '0' + this.getDate());
    str = str.replace(/d|D/g, this.getDate());

    str = str.replace(/hh|HH/, this.getHours() > 9 ? this.getHours().toString() : '0' + this.getHours());
    str = str.replace(/h|H/g, this.getHours());
    str = str.replace(/mm/, this.getMinutes() > 9 ? this.getMinutes().toString() : '0' + this.getMinutes());
    str = str.replace(/m/g, this.getMinutes());

    str = str.replace(/ss|SS/, this.getSeconds() > 9 ? this.getSeconds().toString() : '0' + this.getSeconds());
    str = str.replace(/s|S/g, this.getSeconds());

    return str;
}
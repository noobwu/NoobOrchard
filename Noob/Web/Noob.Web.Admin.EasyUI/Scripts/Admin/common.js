/*
js配置模块
*/
var fastdfsgroupurl = 'http://192.168.16.218/pm1/';


/*
页面公共常量字段
*/
var constfield = { pageSize: 20, pageList: [10, 20, 40, 50, 100] };



function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var urlParms = window.location.search.substr(1);
    var r = urlParms.match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
}

function GetStrLen(s) {
    var l = 0;
    var a = s.split("");
    for (var i = 0; i < a.length; i++) {
        if (a[i].charCodeAt(0) < 299) {
            l++;
        }
        else
            l += 2;
    }
    return l;
}
//调用： 
String.prototype.IsNullOrEmpty = function (fmt) {
    if (this == undefined||this == null || this == "") {
        return  true;
    }
    return false;
}
//调用： 
String.prototype.DateFormat = function (fmt) {
    if (this == null || this == "")
    {
        return "";
    }
    var date = new Date(this);
    return date.Format(fmt);
}
//var time1 = new Date().Format("yyyy-MM-dd");
//var time2 = new Date().Format("yyyy-MM-dd HH:mm:ss");
// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    //console.log("year:" + this.getFullYear());
    if (this.getFullYear() <= 1900) return "";
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
//Json日期转换
function ConvertJsonDate(jsonDate) {
    if (jsonDate == null || jsonDate.length == 0) return "";
    var year = (new Date(parseInt(jsonDate.substr(6)))).getFullYear();
    var month = (new Date(parseInt(jsonDate.substr(6)))).getMonth() + 1;
    var day = (new Date(parseInt(jsonDate.substr(6)))).getDate();
    if (year + "-" + month + "-" + day == "1900-1-1") {
        return "";
    }
    if (month < 10) {
        month = "0" + month;
    }
    if (day < 10) {
        day = "0" + day;
    }
    return year + "-" + month + "-" + day;
}

function jsonDateFormat(jsonDate) {//json日期格式转换为正常格式
    if (jsonDate == null || jsonDate.length == 0) return "";
    try {
        var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
        var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
        var hours = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
        var minutes = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
        var seconds = date.getSeconds() < 10 ? "0" + date.getSeconds() : date.getSeconds();
        return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
    } catch (ex) {
        return "";
    }
}

//是否包含汉字
function funcChina(obj) {
    return /.*[\u4e00-\u9fa5]+.*$/.test(obj);
}

//当在弹出框里再弹出窗体时，通过传不同参数的id
function addDialog(option, id) {
    var divId = id || "div_easyuiDialog";
    if (!option.url) return;
    var defaultPama = {
        width: 400, height: 210, iconCls: 'icon-save', closed: false, modal: true, autoCloseOnEsc: true,
        url: "", title: 'Add'
    }
    var pamaOption = $.extend(defaultPama, option);
    if (!document.getElementById(id)) {
        $("body").append('<div id="' + divId + '"/>');
    }
    $('#' + divId).dialog({
        iconCls: pamaOption.iconCls,
        title: pamaOption.title,
        width: pamaOption.width,
        height: pamaOption.height,
        closed: pamaOption.closed,
        cache: false,
        href: pamaOption.url,
        modal: pamaOption.modal,
        autoCloseOnEsc: pamaOption.autoCloseOnEsc
    });
}
function closeDialog(id) {
    var divId = id || "div_easyuiDialog";
    $("#" + divId).dialog("close");
}

//提交去除前后空格
function triminput(id) {
    var obj = $(id + " input[type='text']");
    $(obj).each(function () {
        $(this).val($.trim($(this).val()));
    });
}

//回车事件
function enterSearch(obj) {
    document.onkeydown = function (e) {
        var theEvent = window.event || e;
        var code = theEvent.keyCode || theEvent.which;
        if (code == 13) {
            //document.getElementById(obj).focus();
            document.getElementById(obj).click();
        }
    }
}


//easyui 验证扩展
$.extend($.fn.validatebox.defaults.rules, {
    equals: {
        validator: function (value, param) {
            return value == $(param[0]).val();
        },
        message: '确认密码不一样.'
    },
    minLength: {
        validator: function (value, param) {
            return GetStrLen($.trim(value)) >= param[0];
        },
        message: '请输入至少{0}个字符.'
    },
    maxLength: {
        validator: function (value, param) {
            return GetStrLen($.trim(value)) <= param[0];
        },
        message: '请输入最大{0}个字符.'
    },
    phone: {// 验证电话号码
        validator: function (value) {
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '格式不正确,请使用下面格式:020-88888888'
    },
    mobile: {// 验证手机号码
        validator: function (value) {
            return /^1|2[3|4|5|7|8][0-9]\d{8}$/i.test(value);
        },
        message: '手机号码格式不正确'
    },
    ddlRequire: {// 验证下拉列表为必选    class="easyui-validatebox" validtype="ddlRequire['#ddl_SPID']"
        validator: function (value, param) {
            var val = $(param[0]).combobox("getValue");
            return val != "-1";
        },
        message: '下拉列表为必选'
    },
    idcard: {// 验证身份证 
        validator: function (value) {
            //return /^\d{15}(\d{2}[A-Za-z0-9])?$/i.test(value);
            return /^[a-zA-Z|0-9]{3,15}(\d|\()+[\d]+(\d|X|x|\))$/i.test(value);
        },
        message: '身份证号码格式不正确'
    },
    intOrFloat: {// 验证整数或小数 
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '请输入数字，并确保格式正确'
    },
    currency: {// 验证货币 
        validator: function (value) {
            return /^\d+(\.\d+)?$/i.test(value);
        },
        message: '货币格式不正确'
    },
    qq: {// 验证QQ,从10000开始 
        validator: function (value) {
            return /^[1-9]\d{4,9}$/i.test(value);
        },
        message: 'QQ号码格式不正确'
    },
    integer: {// 验证整数 
        validator: function (value) {
            return /^[+]?[1-9]+\d*$/i.test(value);
        },
        message: '请输入整数'
    },
    age: {// 验证年龄
        validator: function (value) {
            return /^(?:[1-9][0-9]?|1[01][0-9]|120)$/i.test(value);
        },
        message: '年龄必须是0到120之间的整数'
    },
    chinese: {// 验证中文 
        validator: function (value) {
            return /^[\Α-\￥]+$/i.test(value);
        },
        message: '请输入中文'
    },
    english: {// 验证英语 
        validator: function (value) {
            return /^[A-Za-z]+$/i.test(value);
        },
        message: '请输入英文'
    },
    realname:{  
     validator: function (value) {
    return /^([\u4E00-\u9FA5]+|[a-zA-Z]+)$/i.test(value);
   },
   message: '请输入中文或英文'
    },
    unnormal: {// 验证是否包含空格和非法字符 
        validator: function (value) {
            return /.+/i.test(value);
        },
        message: '输入值不能为空和包含其他非法字符'
    },
    username: {// 验证用户名 
        validator: function (value) {
            return /^[a-zA-Z][a-zA-Z0-9_]{5,15}$/i.test(value);
        },
        message: '用户名不合法（字母开头，允许6-16字节，允许字母数字下划线）'
    },
    faxno: {// 验证传真 
        validator: function (value) {
            //            return /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/i.test(value); 
            return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
        },
        message: '传真号码不正确'
    },
    zip: {// 验证邮政编码 
        validator: function (value) {
            return /^[1-9]\d{5}$/i.test(value);
        },
        message: '邮政编码格式不正确'
    },
    ip: {// 验证IP地址 
        validator: function (value) {
            //return /d+.d+.d+.d+/i.test(value);
            return /^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$/.test(value);
        },
        message: 'IP地址格式不正确'
    },
    name: {// 验证姓名，可以是中文或英文 
        validator: function (value) {
            return /^[\Α-\￥]+$/i.test(value) | /^\w+[\w\s]+\w+$/i.test(value);
        },
        message: '请输入姓名'
    },
    date: {// 验证姓名，可以是中文或英文 
        validator: function (value) {
            //格式yyyy-MM-dd或yyyy-M-d
            return /^(?:(?!0000)[0-9]{4}([-]?)(?:(?:0?[1-9]|1[0-2])\1(?:0?[1-9]|1[0-9]|2[0-8])|(?:0?[13-9]|1[0-2])\1(?:29|30)|(?:0?[13578]|1[02])\1(?:31))|(?:[0-9]{2}(?:0[48]|[2468][048]|[13579][26])|(?:0[48]|[2468][048]|[13579][26])00)([-]?)0?2\2(?:29))$/i.test(value);
        },
        message: '请输入合适的日期格式'
    },
    dateCompare: {  //验证开始时间小于结束时间，added by Michael in 2014/8/15
        validator: function (value, param) {
            startTime2 = $(param[0]).datetimebox('getValue');
            var d1 = $.fn.datebox.defaults.parser(startTime2);
            var d2 = $.fn.datebox.defaults.parser(value);
            return d2 >= d1;
        },
        message: '结束时间要大于或等于开始时间！'
    },
    msn: {
        validator: function (value) {
            return /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value);
        },
        message: '请输入有效的msn账号(例：abc@hotnail(msn/live).com)'
    },
    same: {
        validator: function (value, param) {
            if ($("#" + param[0]).val() != "" && value != "") {
                return $("#" + param[0]).val() == value;
            } else {
                return true;
            }
        },
        message: '两次输入的密码不一致！'
    },
    diffs: {
        validator: function (value, param) {
            return value != $(param[0]).val();
        },
        message: '新密码和旧密码不能相同！'
    },
    urlvalid: { // url:http:// or https://
        validator: function (value) {
            return /(http:|https:)\/\/.+/i.test(value);
        },
        message: '输入http://或https://格式url地址'
    }
});


//显示遮罩 
function loading(opt) {
    var defaultOptions = { loadMsg: '操作进行中，请耐心等待...', width: 100, height: 100, isHtml: true, injectedBody: 'body', mask_z_index: 100000, mask_z_index_msg: 100001, buttonDisabled: 'disabled' };
    $.extend(defaultOptions, opt);

    //禁用按钮
    if (defaultOptions.btn) {
        $("#" + opt.btn).attr({ "disabled": defaultOptions.buttonDisabled });
    }

    //传入参数是Html标签还是控件ID
    var divMask;
    if (defaultOptions.isHtml == true) { //Html标签
        divMask = $(defaultOptions.injectedBody);
    }
    else {   //控件ID
        divMask = $('#' + defaultOptions.injectedBody);
    }

    if (defaultOptions.loadMsg) {
        $("<div class=\"datagrid-mask\" style='z-index:" + defaultOptions.mask_z_index + ";'></div>").css({ display: "block", width: divMask.width(), height: divMask.height() }).appendTo(divMask);
        $("<div class=\"datagrid-mask-msg\" style='z-index:" + defaultOptions.mask_z_index_msg + ";'></div>").html(defaultOptions.loadMsg).appendTo(divMask).css({ display: "block", left: (divMask.width() - $("div.datagrid-mask-msg", divMask).outerWidth()) / 2, top: (divMask.height() - $("div.datagrid-mask-msg", divMask).outerHeight()) / 2 });
    }
}

//隐藏遮罩 
function loaded(opt) {
    $("div.datagrid-mask").remove();
    $("div.datagrid-mask-msg").remove();

    var defaultOptions = { loadMsg: '操作进行中，请耐心等待...', width: 100, height: 100, isHtml: true, injectedBody: 'body', mask_z_index: 100000, mask_z_index_msg: 100001, buttonDisabled: 'disabled' };
    $.extend(defaultOptions, opt);

    //禁用按钮
    if (defaultOptions.btn) {
        $("#" + defaultOptions.btn).removeAttr("disabled");
    }
}

//超时，指向的登入页面。在异步执行时用到
function logouToLogOnPage() {
    top.location.pathname = "/Account/Login";
}

//ajax超时，指向的登入页面。在异步执行时用到 <head id="logon_head">
function ajaxErrorLogOn(XMLHttpRequest, textStatus, errorThrown) {
    //if (textStatus == "parsererror")
    if (XMLHttpRequest.responseText.indexOf('head id="logon_head"') != -1)
        logouToLogOnPage();
}

//重写控件的ajax事件，捕获错误或超时。start ---
$.extend($.fn.datagrid.defaults, {
    loader: function (_76c, _76d, _76e) {
        var opts = $(this).datagrid("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _76c, dataType: "json", success: function (data) {
                _76d(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                ajaxErrorLogOn(XMLHttpRequest, textStatus, errorThrown);
                _76e.apply(this, arguments);
            }
        });
    }
});
$.extend($.fn.treegrid.defaults, {
    loader: function (_8c8, _8c9, _8ca) {
        var opts = $(this).treegrid("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _8c8, dataType: "json", success: function (data) {
                _8c9(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                ajaxErrorLogOn(XMLHttpRequest, textStatus, errorThrown);
                _8ca.apply(this, arguments);
            }
        });
    }
});
$.extend($.fn.combobox.defaults, {
    loader: function (_95e, _95f, _960) {
        var opts = $(this).combobox("options");
        if (!opts.url) {
            return false;
        }
        $.ajax({
            type: opts.method, url: opts.url, data: _95e, dataType: "json", success: function (data) {
                _95f(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                ajaxErrorLogOn(XMLHttpRequest, textStatus, errorThrown);
                _960.apply(this, arguments);
            }
        });
    }
});


//日期控件扩展(开始-结束：限制)
$.extend($.fn.datebox.defaults, {
    onChange: function (t) {
        var date = null;
        if (t)
        {
            var ts = t.split('-');
            date = new Date(ts[0], ts[1]*1-1, ts[2]);
        }

        var opts = $(this).datebox("options");
        if (opts.less) {

            $('#' + opts.less).datebox('calendar').calendar({
                validator: function (mdate) {
                    return date ? mdate >= date : true;
                }
            });
        }

        if (opts.more) {
            $('#' + opts.more).datebox('calendar').calendar({
                validator: function (mdate) {

                    return date ? mdate <= date : true;
                }
            });
        }
    }
});

//日期时间控件扩展(开始-结束：限制)
$.extend($.fn.datetimebox.defaults, {
    onChange: function (t)
    {
        var date = null;

        if (t) {
            var ts = t.split(' ')[0].split('-');
            date = new Date(ts[0], ts[1] * 1 - 1, ts[2]);
        }

        var opts = $(this).datetimebox("options");
        if (opts.less) {
            $('#' + opts.less).datetimebox('calendar').calendar({
                validator: function (mdate) {
                    return date ? mdate >= date : true;
                }
            });
        }

        if (opts.more) {
            $('#' + opts.more).datetimebox('calendar').calendar({
                validator: function (mdate) {
                    return date ? mdate <= date : true;
                }
            });
        }
    }
});
/*处理内容过长之简略显示的问题-----Begin---------,by michael in 2014/6/26*/
$.extend($.fn.datagrid.methods, {
    DataGridLoad: function (data) {
        LoadSuccess(data);
    }
});

function LoadSuccess(data) {
    $("div.tipcontent").each(function () {
        $(this).attr("title", $(this).attr("title") || $(this).attr("cc"));
        $(this).poshytip({ offoffsetY: 16, allowTipHover: true });
    });
}

/**
   * EasyUI DataGrid根据字段动态合并单元格
   * param tableID 要合并table的id
   * param colList 要合并的列,用逗号分隔(例如："name,department,office");
   * param mainColIndex 要合并的主列索引，即colList的索引
   */
function mergeCellsByField(tableID, colList, mainColIndex) {
    var ColArray = colList.split(",");
    var tTable = $('#' + tableID);
    var TableRowCnts = tTable.datagrid("getRows").length;
    var tmpA;
    var tmpB;
    var PerTxt = "";
    var CurTxt = "";
    var alertStr = "";
    for (var i = 0; i <= TableRowCnts ; i++) {
        if (i == TableRowCnts) {
            CurTxt = "";
        }
        else {
            CurTxt = tTable.datagrid("getRows")[i][ColArray[mainColIndex]];
        }
        if (PerTxt == CurTxt) {
            tmpA += 1;
        }
        else {
            tmpB += tmpA;
            for (var j = 0; j < ColArray.length; j++) {
                tTable.datagrid('mergeCells', {
                    index: i - tmpA,
                    field: ColArray[j],
                    rowspan: tmpA,
                    colspan: null
                });
            }
            tmpA = 1;
        }
        PerTxt = CurTxt;
    }
}


//datagrid返回记录为0时显示“没有找到相关记录”
var myview = $.extend({}, $.fn.datagrid.defaults.view, {
    onAfterRender: function (target) {
        $.fn.datagrid.defaults.view.onAfterRender.call(this, target);
        var opts = $(target).datagrid('options');
        var vc = $(target).datagrid('getPanel').children('div.datagrid-view');
        vc.children('div.datagrid-empty').remove();
        if (!$(target).datagrid('getRows').length) {
            var d = $('<div class="datagrid-empty"></div>').html(opts.emptyMsg || '没有找到相关记录').appendTo(vc);
            d.css({
                position: 'absolute',
                left: 0,
                top: 50,
                width: '100%',
                textAlign: 'center'
            });
        }
    }
});

$.extend($.fn.tree.methods, {
   
    enableCheck: function (jq, target) {
        var ckSpan = $(target).find(">span.tree-checkbox");
        if (ckSpan.hasClass('tree-checkbox-disabled1')) {
            return ckSpan.removeClass('tree-checkbox-disabled1');
        }

    },
   
    disableCheck: function (jq, target) {
        var ckSpan = $(target).find(">span.tree-checkbox");
        if (ckSpan.hasClass('tree-checkbox1')) {
            return ckSpan.addClass('tree-checkbox-disabled1');
        }
    }

});

function checkPass(pass) {
    if (pass.length < 8) {
        return 0;
    }
    return 5;
    var ls = 0;
    if (pass.match(/([a-z])+/)) {
        ls++;
    }
    if (pass.match(/([0-9])+/)) {
        ls++;
    }
    if (pass.match(/([A-Z])+/)) {
        ls++;
    }
    if (pass.match(/[^a-zA-Z0-9]+/)) {
        ls++;
    }
    return ls;
}

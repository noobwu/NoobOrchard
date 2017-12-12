/*
js配置模块
*/
var fastdfsGroupUrl = 'http://192.168.16.218/pm1/';


/*
页面公共常量字段
*/
var objPagging = { pageSize: 20, pageList: [10, 20, 40, 50, 100] };



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


//提交去除前后空格
function trimInput(id) {
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




//超时，指向的登入页面。在异步执行时用到
function logoutToUrl() {
    top.location.pathname = "/Account/Login";
}

//ajax超时，指向的登入页面。在异步执行时用到 <head id="logon_head">
function ajaxErrorLogOn(XMLHttpRequest, textStatus, errorThrown) {
    //if (textStatus == "parsererror")
    if (XMLHttpRequest.responseText.indexOf('head id="logon_head"') != -1)
        logoutToUrl();
}
//
function startPageLoading()
{
    App.startPageLoading({ animate: true });
}
function stopPageLoading()
{
    App.stopPageLoading();
}

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
function showMessage(msg, success) {
    debug('success:', success, 'msg:' + msg);
}
function debug()
{
    if (console && console.log) {
        console.log(arguments);
    }
}

function handleIdCheckAll(el,event,dgId)
{
    var oEvent = oEvent ? oEvent : window.event
    var oElem = oEvent.toElement ? oEvent.toElement : oEvent.relatedTarget; // 此做法是为了兼容FF浏览器
    //debug(oElem);
    var $oElem = $(oElem);
    var isInput = $oElem.is('input');
    //debug("handleIdCheckAll,$oElem.is('input'):", isInput);
    var checked = false; 
    var $checkboxAll = $(el).find('input[type=checkbox]');
    var $lblCheckAll = $checkboxAll.next().find('label');
    if (!dgId) dgId = 'dg';
    //debug('handleIdCheckAll,checked:', checked);
    if (isInput) {
        checked=$checkboxAll.is(":checked");
        setIdCheckAll(dgId, checked, $checkboxAll, $lblCheckAll);
    }
    else
    {
        if ($lblCheckAll.hasClass('checked2'))
        {
            checked = true;
            setIdCheckAll(dgId, checked, $checkboxAll, $lblCheckAll);
        }
    }
}
function setIdCheckAll(dgId, checked, checkboxAll, lblCheckAll) {
    checkboxAll.attr('checked', checked);
    if (checked) {
        clearIdCheckAll(lblCheckAll);
        lblCheckAll.addClass('checked');
        $("#" + dgId + " tbody .checkboxes:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var $lblCheck = $(this).next().find('lable');
            if (!$lblCheck.hasClass('checked')) {
                $lblCheck.addClass('checked');
            }
        });
    }
    else {
        clearIdCheckAll(lblCheckAll);
        $("#" + dgId + " tbody .checkboxes:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var $lblCheck = $(this).next().find('lable');
            $lblCheck.removeClass('checked');
        });
    }
    setIdCheck(dgId, checkboxAll);
}
function handleIdMultipleCheck(el, event, dgId) {
    var oEvent = oEvent ? oEvent : window.event
    var oElem = oEvent.toElement ? oEvent.toElement : oEvent.relatedTarget; // 此做法是为了兼容FF浏览器
    //debug(oElem);
    var $oElem = $(oElem);
    var isInput = $oElem.is('input');
    //debug("handleIdMultipleCheck,$oElem.is('input'):", isInput);
    if (!dgId) dgId = 'dg';
    if (isInput) {
        var $checkbox = $(el).find('input[type=checkbox]');
        var checked = $checkbox.is(":checked");
        $checkbox.attr('checked', checked);
        //debug('handleIdMultipleCheck,checked:', checked);
        setIdCheck(dgId);
    }
}
function setIdCheck(dgId,checkboxAll) {
    if (!dgId) dgId = 'dg';
    var chkTotalSize = $("#" + dgId + " tbody .checkboxes:input[type=checkbox]").size();//选项总个数
    var chkSize = $("#" + dgId + " tbody .checkboxes:input[type=checkbox]:checked").size();//已选中的个数;
    var $lblCheckAll = $("#" + dgId + " thead .mt-checkbox span label");
    if (chkTotalSize == chkSize) {
        if (!checkboxAll) {
            $("#" + dgId + " thead .group - checkable:input[type = checkbox]").attr('checked', true);
            clearIdCheckAll($lblCheckAll)
            $lblCheckAll.addClass('checked');
        }
    }
    else if (chkSize > 0)
    {
        if (!checkboxAll) {
            $("#" + dgId + " thead .group-checkable:input[type=checkbox]").attr('checked', false);
            clearIdCheckAll($lblCheckAll)
        }
        $lblCheckAll.addClass('checked2');
    }
    else {
        if (!checkboxAll) {
            $("#" + dgId + " thead .group-checkable:input[type=checkbox]").attr('checked', false);
            clearIdCheckAll($lblCheckAll);
        }

    }

    //debug('chkTotalSize:', chkTotalSize, ',', 'chkSize:', chkSize, ',', 'checked:', $("#dg thead .group-checkable:input[type=checkbox]").attr('checked'));
}

function clearIdCheckAll(el)
{
    if (!el) {
        var dgId = 'dg';
        el = $("#" + dgId + " thead .mt-checkbox span label");
    }
    clearIdCheck(el)
    el.removeClass('checked2');
}
function clearIdCheck(el)
{
    el.removeClass('checked');
}
function getIdCheckValues(dgId)
{
    if (!dgId) dgId = 'dg';
    var ids = [];
    $("#" + dgId + " tbody .checkboxes:input[type=checkbox]:checked").each(function () {
        ids.push(this.value)
    });
    return ids.join(",");
}
function goUrl(url)
{
    window.location.href = url;
}
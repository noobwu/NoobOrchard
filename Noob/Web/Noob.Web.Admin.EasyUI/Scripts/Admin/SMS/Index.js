
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '短信发送日志',
        url: rootUrl + 'SMS/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'LogID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'Mobile', title: '手机号码', width: 80, align: 'center' },
        {
            field: 'Status', title: '发送状态', width: 50, align: 'center',
            formatter: function (value, row, index) {
                //0:禁用,1:启用
                switch (value) {
                    case "0":
                        return "未发送";
                    case "1":
                        return "发送成功";
                    case "2":
                        return "发送失败";
                    case "3":
                        return "取消发送";
                    default:
                        return "";
                }

            }
        },
        { field: 'SMSContent', title: '短信内容', width: 80, align: 'center' },
        { field: 'UserIP', title: '用户IP', width: 80, align: 'center' },
        { field: 'SendTime', title: '发送时间', width: 80, align: 'center' }
        ]],
        onLoadSuccess: function () {
            $('#dg').datagrid("clearSelections");
        }
    });
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
                return value.length >= param[0];
            },
            message: '请输入至少{0}个字符.'
        },
        maxLength: {
            validator: function (value, param) {
                return value.length <= param[0];
            },
            message: '请输入小于{0}个字符.'
        },
        phone: {// 验证电话号码
            validator: function (value) {
                return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
            },
            message: '格式不正确,请使用下面格式:020-88888888'
        },
        mobile: {// 验证手机号码
            validator: function (value) {
                return /^(13|15|18)\d{9}$/i.test(value);
            },
            message: '手机号码格式不正确'
        }
    });
});

//重新加载
function reloadData() {
    $("#dg").datagrid("reload");
}


//搜索
function searchData() {
    var status = $("#sltSearchStatus").combobox("getValue");
    var startTime = $('#txtSearchStartTime').datetimebox('getValue');
    var endTime = $('#txtSearchEndTime').datetimebox('getValue');
    var mobile = $("#txtSearchMobile").val();
    $('#dg').datagrid('load', {
        status: status,
        startTime: startTime,
        endTime: endTime,
        mobile: mobile,
    });
}
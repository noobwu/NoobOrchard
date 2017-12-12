$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '手机验证码',
        url: rootUrl + 'SMS/GetAuthCodeList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'AuthCodeID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'Mobile', title: '手机号码', width: 80, align: 'center' },
        { field: 'AuthCode', title: '验证码', width: 80, align: 'center' },
        { field: 'AuthType', title: '验证码类型', width: 80, align: 'center' },
       {
              field: 'AuthStatus', title: '使用状态', width: 80, align: 'center',
              formatter: function (value, row, index) {
                  //验证状态(0:未验证,1:已验证)
                  switch (value) {
                      case "0":
                          return "未使用";
                      case "1":
                          return "已使用";
                      default:
                          return "";
                  }

              }
          },
        { field: 'UserIP', title: '用户IP', width: 80, align: 'center' },
        { field: 'AuthTime', title: '验证时间', width: 80, align: 'center' },
        { field: 'EndTime', title: '过期时间', width: 80, align: 'center' }
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



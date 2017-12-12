
//重新加载
function reloadData() {
    $("#dg").datagrid("reload");
}

//删除用户登录日志
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].LogID);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的用户登录日志', 'error');
        return;
    }

}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '用户登录日志',
        url: rootUrl + 'Customer/UserLoginLog/GetList',
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
        { field: 'UserName', title: '账户', width: 80, align: 'center' },
        {
            field: 'LoginSource', title: '来源', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //来源(1:网站,2:Android,3:IOS 4:微信 )
                switch (value) {
                    case "0":
                        return "未知";
                    case "1":
                        return "网站";
                    case "2":
                        return "Android";
                    case "3":
                        return "IOS";
                    case "4":
                        return "微信";
                    default:
                        return "";
                }

            }
        },
        { field: 'Ip', title: 'IP地址', width: 80, align: 'center' },
        { field: 'LoginTime', title: '登录时间', width: 80, align: 'center' },
        {
            field: 'LoginStatus', title: '登录状态', width: 100, align: 'center',
            formatter: function (value, row, index) {
                return value == "1" ? "登录成功" : "登录失败";
            }
        }
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

//删除数据
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Customer/UserLoginLog/Delete",
                data: "id=" + id,
                success: function (data) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            $.messager.alert('提示', '删除成功。', 'info', function () {
                                reloadData($('#hidPid').val());
                                closeDialog();
                            });
                        }
                        else {
                            $.messager.alert('提示', data.Msg, 'error');
                        }
                    } else {
                        $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(XMLHttpRequest.status);
                    //alert(XMLHttpRequest.readyState);
                    // alert(textStatus);
                    //console.log(errorThrown);
                },
                complete: function (XMLHttpRequest, textStatus) {
                    this; // 调用本次AJAX请求时传递的options参数
                }
            });
        }
    });
}

//搜索
function searchData() {
    var status = $("#sltSearchStatus").combobox("getValue");
    var startTime = $('#txtSearchStartTime').datetimebox('getValue');
    var endTime = $('#txtSearchEndTime').datetimebox('getValue');
    var userName = $("#txtSearchUserName").val();
    $('#dg').datagrid('load', {
        status: status,
        startTime: startTime,
        endTime: endTime,
        userName: userName,
    });
}


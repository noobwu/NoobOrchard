//打开窗体
function openDialog(id) {
    addDialog({ url: rootUrl + "Customer/Customer/Edit?id=" + id, title: '修改用户信息', width: 360, height: 260 });
}

//打开新的选项卡
function openDialogTab(id) {
    addDialog({ url: rootUrl + "Customer/Customer/Details/" + id, title: '查看用户', width: 410, height: 400 });
}
function openDialogPass(id) {
    addDialog({ url: rootUrl + "Customer/Customer/ResetPassWord/" + id, title: '重置密码', width: 310, height: 160 });
}
function updateStatus(status,id)
{
    var title = "您确认想更新吗?";
    if (status == 0) {
        title = "您确认禁用吗";
    }
    else {
        title = "您确认启用吗";
    }
    $.messager.confirm('确认', title, function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Customer/Customer/UpdateStatus",
                data: "id=" + id + "&status=" + status,
                success: function (data) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            $.messager.alert('提示', '更新成功。', 'info', function () {
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
//重新加载
function reloadData() {
    $("#dg").datagrid("reload");
}

//编辑用户信息
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.UserID);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的用户信息', 'info');
        return;
    }
}
//删除用户信息
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.UserID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].UserID);
    }
    id = ids.join(',');
    if (id!=null&&id!="") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的用户信息', 'info');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '用户信息',
        url: rootUrl + 'Customer/Customer/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'UserID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'UserName', title: '用户名', width: 120, align: 'center' },
        { field: 'RealName', title: '真实姓名', width: 120, align: 'center' },
        { field: 'Mobile', title: '手机号码', width: 120, align: 'center' },
        {
            field: 'Sex', title: '性别', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //性别(0：未设,1：男,2：女,3:保密)
                switch (value) {
                    case "0":
                        return "未设";
                    case "1":
                        return "男";
                    case "2":
                        return "女";
                    case "3":
                        return "保密";
                    default:
                        return "";
                }

            }
        },
        {
            field: 'Status', title: '账号状态', width: 100, align: 'center',
            formatter: function (value, row, index) {
                return value == "1" ? "启用" : "禁用";
            }
        },
        {
            field: 'RegisteSource', title: '来源', width: 80, align: 'center',
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
        { field: 'RegisteTime', title: '注册时间', width: 80, align: 'center' },
        { field: 'RegisteIP', title: '用户IP', width: 80, align: 'center' },
        {
            field: 'UserType', title: '用户类型', width: 100, align: 'center',
            formatter: function (value, row, index) {
                //0-会员 1-商家
                switch (value) {
                    case "0":
                        return "普通会员";
                    case "1":
                        return "门店";
                    default:
                        return "";
                }
            }
        },
        { field: 'NickName', title: '昵称', width: 80, align: 'center' },
        {
            field: 'Avatars', title: '头像图片', width: 160, align: 'center', formatter: function (value, row, index) {
                if (value == null || value == '') return '';
                var imgUrl= uploadUrl+value;
                return '<div class="avatars"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
            }
        },
        {
            field: 'x', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                return (isEdit == "1" ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.UserID + '\')">编辑</a>') : "") + '&nbsp;&nbsp;' +
                    '<a href="javascript:void(0)" onclick="openDialogTab(\'' + row.UserID + '\')">查看</a>' + '&nbsp;&nbsp;' +
                      (isChangePass == "1" ? ('<a href="javascript:void(0)" onclick="openDialogPass(\'' + row.UserID + '\')">重置密码</a>') : "");
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
    //$('.avatars').imgPreview({
    //    imgCSS: { width: 800 }
    //});
});

//删除数据
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Customer/Customer/Delete",
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
    var mobile = $("#txtSearchMobile").val();
    var userName = $("#txtSearchUserName").val();
    var realName = $("#txtSearchRealName").val();
    $('#dg').datagrid('load', {
        status: status,
        startTime: startTime,
        endTime: endTime,
        mobile: mobile,
        userName: userName,
        realName: realName
    });
}
 

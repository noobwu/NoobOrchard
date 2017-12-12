//打开窗体
function openDialog(id) {
    if (id) {
        addDialog({ url: rootUrl + "AdvPosition/Edit?id=" + id, title: '修改广告位', width: 360, height: 300 });
    }
    else {
        addDialog({ url: rootUrl + "AdvPosition/Create", title: '添加广告位', width: 360, height: 300 });
    }
}
function callAction(cmd, id) {
    switch (cmd) {
        case "Enable":
            updateStatus(1, id);
            return;
        case "Disable":
            updateStatus(0, id);
            return;
    }
}
function updateStatus(status, id) {
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
                url: rootUrl + "AdvPosition/UpdateStatus",
                data: "id=" + id + "&status=" + status,
                success: function (data) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            $.messager.alert('提示', '更新成功。', 'info', function () {
                                reloadData();
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

//新增广告位
function createModule() {
    openDialog();
}

//编辑广告位
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.AdvPositionId);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的广告位', 'info');
        return;
    }
}
//删除广告位
function deleteModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        deleteData(node.AdvPositionId);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的广告位', 'info');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '广告位',
        url: rootUrl + 'AdvPosition/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'AdvPositionId',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'AdvPositionCode', title: '广告位置编号', width: 80, align: 'center' },
        { field: 'AdvPositionName', title: '广告位名称', width: 150, align: 'center' },
        {
             field: 'Status', title: '启用状态', width: 50, align: 'center',
             formatter: function (value, row, index) {
                 //0:禁用,1:启用
                 switch (value) {
                     case "1":
                         return "启用";
                     case "0":
                         return "禁用";
                     default:
                         return "";
                 }

             }
         },
        { field: 'Width', title: '广告位宽度', width: 50, align: 'center' },
        { field: 'Height', title: '广告位高度', width: 50, align: 'center' },
        { field: 'SortOrder', title: '排序', width: 50, align: 'center' },
        { field: 'Remark', title: '描述', width: 100, align: 'center' },
        {
                field: 'x', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                    var statusText = "";
                    var cmd = "";
                    switch (row.Status) {
                        case "1":
                            statusText = "禁用";
                            cmd = "Disable";
                            break;
                        case "0":
                            statusText = "启用";
                            cmd = "Enable";
                            break;
                        default:
                            break;
                    }
                    return (isEdit ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.AdvPositionId + '\')">编辑</a>') : "") + '&nbsp;&nbsp;' 
                          +(isUpdateStatus?('<a href="javascript:void(0)"  onclick="callAction(\'' + cmd + '\',\'' + row.AdvPositionId + '\')">' + statusText + '</a>') : "");
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

//搜索
function searchData() {
    var status = $("#sltStatus").combobox("getValue");
    $('#dg').datagrid('load', {
        advPositionCode: $.trim($("#txtSearchAdvPositionCode").val()),
        advPositionName: $.trim($("#txtSearchAdvPositionName").val()),
        status: status
    });
}

//删除数据
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "AdvPosition/Delete",
                data: "id=" + id,
                success: function (data) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            $.messager.alert('提示', '删除成功。', 'info', function () {
                                reloadData();
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


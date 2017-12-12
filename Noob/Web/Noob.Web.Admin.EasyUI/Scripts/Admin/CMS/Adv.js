//打开窗体
function openDialog(id, pid) {
    if (id) {
        parent.openTab("修改广告", rootUrl + "Adv/Edit?id=" + id);
        //addDialog({ url: rootUrl + "Adv/Edit?id=" + id, title: '修改广告', width: 4200, height: 460 });
    }
    else {
        parent.openTab("添加广告", rootUrl + "Adv/Create?pid=" + (pid ? pid : '0'));
        //addDialog({ url: rootUrl + "Adv/Create?pid=" + (pid ? pid : '0'), title: '添加广告', width: 420, height: 460 });
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
                url: rootUrl + "Adv/UpdateStatus",
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

//新增广告
function createModule() {
    openDialog();
}

//编辑广告
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.AdvID);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的广告', 'info');
        return;
    }
}
//删除广告
function deleteModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        deleteData(node.AdvID);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的广告', 'info');
        return;
    }
}
$(function () {
    $('#sltSearchAdvPositionId').combobox({
        url: rootUrl + "AdvPosition/GetSubList?t=" + Math.random(),
        valueField: 'id',
        textField: 'text',
        editable: false,
        onLoadSuccess: function () {
        }
    });
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '广告',
        url: rootUrl + 'Adv/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'AdvID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'AdvName', title: '广告名称', width: 80, align: 'center' },
        { field: 'AdvCode', title: '广告标识', width: 80, align: 'center' },
        { field: 'AdvTitle', title: '广告标题', width: 80, align: 'center' },
        {
            field: 'ImageUrl', title: '广告图片', width: 160, align: 'center', formatter: function (value, row, index) {
                var imgUrl = uploadUrl + value;
                return '<div class="thumb"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
            }
        },
        { field: 'Url', title: '广告链接地址', width: 80, align: 'center' },
        {
            field: 'AdvType', title: '广告类型', width: 50, align: 'center',
            formatter: function (value, row, index) {
                //广告类型 (1:外部链接广告 2 :内部链接广告 3:Html广告)
                switch (value) {
                    case "1":
                        return "外部链接广告";
                    case "2":
                        return "内部链接广告";
                    case "3":
                        return "Html广告";
                    default:
                        return "";
                }

            }
        },
        { field: 'SortOrder', title: '排序', width: 80, align: 'center' },
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
                return (isEdit == "1" ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.AdvID + '\')">编辑</a>') : "") + '&nbsp;&nbsp;' 
                          +(isUpdateStatus?('<a href="javascript:void(0)"  onclick="callAction(\'' + cmd + '\',\'' + row.AdvID + '\')">' + statusText + '</a>') : "");
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
                url: rootUrl + "Adv/Delete",
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
    var advPositionId = $("#sltSearchAdvPositionId").combobox("getValue");
    if (!advPositionId || funcChina(advPositionId)) {
        advPositionId = -1;
    }
    var status = $("#sltSearchStatus").combobox("getValue");
    $('#dg').datagrid('load', {
        advCode: $.trim($("#txtSearchAdvCode").val()),
        advName: $.trim($("#txtSearchAdvName").val()),
        status: status,
        advPositionId: advPositionId
    });
}


//打开窗体
function openDialog(id) {
    addDialog({ url: rootUrl + "Barber/BarberShop/Edit?id=" + id, title: '修改理发师服务门店', width: 360, height: 260 });
}
//打开新的选项卡
function openDialogTab(id) {
    parent.openTab("查看门店", rootUrl + "Barber/BarberShop/Details/" + id);
}
function updateAuditStatus(id,type) {
    var title, msg,status;
    switch (type) {
        case 1:
            title = "门店审核通过";
            msg = "您确认该门店信息有效吗";
            status = 1;
            break;
        case 0:
            title = "门店审核不通过";
            msg = "您确认该门店信息无效吗";
            status = 2;
            break;
        default:
            break;
    }
    $.messager.prompt(title, msg, function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Barber/BarberShop/UpdateAuditStatus",
                data: "id=" + id + "&status=" + status+"&msg="+r,
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
                url: rootUrl + "Barber/BarberShop/UpdateStatus",
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


//编辑理发师服务门店
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.ShopId);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的理发师服务门店', 'info');
        return;
    }
}
//删除理发师服务门店
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].ShopId);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的理发师服务门店', 'error');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '理发师服务门店',
        url: rootUrl + 'Barber/BarberShop/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'ShopId',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'ShopName', title: '门店名称', width: 180, align: 'center' },
        { field: 'ShopPhone', title: '店铺电话', width: 140, align: 'center' },
        { field: 'ShopMobile', title: '店铺手机号码', width: 140, align: 'center' },
        {
            field: 'AuditStatus', title: '审核状态', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //审核状态(0:待审核,1:已审核,2:审核不通过)
                switch (value) {
                    case "0":
                        return "待审核";
                    case "1":
                        return "已审核";
                    case "2":
                        return "审核不通过";
                    default:
                        return "";
                }
            }
        },
        {
            field: 'BusinessYear', title: '营业年限', width: 80, align: 'center',
            formatter: function (value, row, index) {
                return value + ' 年';

            }
        },
        {
            field: 'BusinessLicense', title: '营业执照', width: 160, align: 'center', formatter: function (value, row, index) {
                var imgUrl = uploadUrl + value;
                return '<div class="thumb"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
            }
        },
        { field: 'TaxCode', title: '执业编号', width: 80, align: 'center' },
        { field: 'LegalPerson', title: '法人姓名', width: 80, align: 'center' },
        { field: 'LegalIdentityCard', title: '法人身份证号', width: 80, align: 'center' },
        { field: 'LegalMobile', title: '法人手机号', width: 80, align: 'center' },
        {
            field: 'DayOff', title: '休息日', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //休息日(1:星期一，2:星期二)
                switch (value) {
                    case "1":
                        return "星期一";
                    default:
                        return "";
                }

            }
        },
        {
            field: 'ShopType', title: '门店类型', width: 100, align: 'center',
            formatter: function (value, row, index) {
                //门店类型 (0:门店注册,1:固定门店, 2:兼职门店)
                switch (value) {
                    case "0":
                        return "加盟门店";
                    case "1":
                        return "固定门店";
                    case "2":
                        return "兼职门店";
                    default:
                        return "";
                }

            }
        },
        { field: 'ShopAddress', title: '门店地址', width: 80, align: 'center' },
        {
             field: 'Status', title: '状态', width: 80, align: 'center',
             formatter: function (value, row, index) {
                 return value == "1" ? "启用" : "禁用";
             }
         },
        { field: 'SortOrder', title: '排序', width: 80, align: 'center' },
        {
            field: 'x', title: '操作', width: 250, align: 'center', formatter: function (value, row, index) {
                //审核状态(0:待审核,1:已审核,2:审核不通过)
                var auditStatusAction = '';
                switch (row.AuditStatus) {
                    case "0":
                        auditStatusAction = '<a href="javascript:void(0)" onclick="updateAuditStatus(\'' + row.ShopId + '\',1)">审核通过</a>&nbsp;&nbsp;<a href="javascript:void(0)" onclick="updateAuditStatus(\'' + row.ShopId + '\',0)">审核不通过</a>';
                        break;
                    case "1":
                        auditStatusAction = '<a href="javascript:void(0)" onclick="updateAuditStatus(\'' + row.ShopId + '\',0)">审核不通过</a>';
                        break;
                    case "2":
                        auditStatusAction = '<a href="javascript:void(0)" onclick="updateAuditStatus(\'' + row.ShopId + '\',1)">审核通过</a>';
                        break;
                    default:
                        break;
                }
                return '<a href="javascript:void(0)" onclick="openDialogTab(\'' + row.ShopId + '\')">查看</a>' + '&nbsp;&nbsp;' + auditStatusAction;
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
                url: rootUrl + "Barber/BarberShop/Delete",
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
    $('#dg').datagrid('load', {
        Status: status
    });
}


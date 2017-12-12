//打开窗体
function openDialog(id) {
    addDialog({ url: rootUrl + "Order/Order/Edit?id=" + id, title: '修改用户订单信息', width: 360, height: 260 });
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
                url: rootUrl + "Order/Order/UpdateStatus",
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


//编辑用户订单信息
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.OrderId);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的用户订单信息', 'info');
        return;
    }
}
//删除用户订单信息
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].OrderId);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的用户订单信息', 'error');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '用户订单信息',
        url: rootUrl + 'Order/Order/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'OrderId',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'OrderNo', title: '订单编号', width: 160, align: 'center' },
        { field: 'UserName', title: '买家账户', width: 80, align: 'center' },
        {
            field: 'OrderStatus', title: '订单状态', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //订单状态（255:无效订单,0:待付款,1:已付款,9:已取消,10:已完成）
                switch (value) {
                    case "0":
                        return "待付款";
                    case "1":
                        return "已付款";
                    case "9":
                        return "已取消";
                    case "10":
                        return "已完成";
                    case "255":
                        return "无效订单";
                    default:
                        return "未知";
                }
            }
        },

        {
            field: 'PayStatus', title: '支付状态', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //支付状态（0:未支付,1:已支付,2:已取消,3:已退款）
                switch (value) {
                    case "0":
                        return "未支付";
                    case "1":
                        return "已支付";
                    case "2":
                        return "已取消";
                    case "3":
                        return "已退款";
                    default:
                        return "未知";
                }
            }
        },
        {
            field: 'Sex', title: '姓名', width: 100, align: 'center',
            formatter: function (value, row, index) {
                var sexTitle = "";
                //性别(1:男,2:女)
                switch (value) {
                    case "1":
                        sexTitle = "先生";
                    case "2":
                        sexTitle = "女士";
                    default:
                }
                return row.RealName + " " + sexTitle;
            }
        },
        { field: 'Mobile', title: '手机号码', width: 80, align: 'center' },
        {
             field: 'ReservationTime', title: '预约时间', width: 120, align: 'center',
             formatter: function (value, row, index) {
                 return value.DateFormat("yyyy-MM-dd hh:mm");
             }
         },
        { field: 'OrderAmount', title: '订单总金额', width: 80, align: 'center' },
        { field: 'DiscountAmount', title: '优惠金额', width: 80, align: 'center' },
        {
            field: 'PayType', title: '支付方式', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //支付方式(0:余额支付 1:在线支付)
                switch (value) {
                    case "0":
                        return "余额支付";
                    case "1":
                        return "在线支付";
                    default:
                        return "未知";
                }
            }
        },
        {
            field: 'PayDate', title: '支付时间', width: 80, align: 'center',
            formatter: function (value, row, index) {
                return value.DateFormat("yyyy-MM-dd hh:mm:s");
            }
        },
        //{ field: 'PayDays', title: '支付时限天数', width: 80, align: 'center' },
       
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
                url: rootUrl + "Order/Order/Delete",
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
    var startTime = $('#txtSearchStartTime').datetimebox('getValue');
    var endTime = $('#txtSearchEndTime').datetimebox('getValue');
    var mobile = $("#txtSearchMobile").val();
    var userName = $("#txtSearchUserName").val();
    var realName = $("#txtSearchRealName").val();
    //console.log(startTime + "," + endTime);
    $('#dg').datagrid('load', {
        startTime: startTime,
        endTime: endTime,
        mobile: mobile,
        userName: userName,
        realName: realName
    });
}


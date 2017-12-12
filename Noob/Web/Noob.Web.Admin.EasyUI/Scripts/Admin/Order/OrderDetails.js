//打开窗体
function openDialog(id) {
    addDialog({ url: rootUrl + "Order/OrderDetails/Edit?id=" + id, title: '修改订单详情表', width: 360, height: 260 });
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
                url: rootUrl + "Order/OrderDetails/UpdateStatus",
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


//编辑订单详情表
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.OrderDetailsId);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的订单详情表', 'info');
        return;
    }
}
//删除订单详情表
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].OrderDetailsId);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的订单详情表', 'error');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '订单详情表',
        url: rootUrl + 'Order/OrderDetails/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'OrderDetailsId',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'OrderNo', title: '订单号', width: 160, align: 'center' },
        { field: 'ProductId', title: '商品名称', width: 80, align: 'center' },
        { field: 'ProductPrice', title: '商品价格', width: 80, align: 'center' },
        { field: 'ProductNum', title: '商品数量', width: 80, align: 'center' },
        { field: 'DiscountAmount', title: '优惠金额', width: 80, align: 'center' },
        { field: 'BarberId', title: '发型师', width: 80, align: 'center' },
        { field: 'ShopId', title: '门店', width: 80, align: 'center' },
        {
            field: 'x', title: '金额', width: 160, align: 'center', formatter: function (value, row, index) {
                  
                return (row.ProductPrice * row.ProductNum - row.DiscountAmount);
            }
        }
        ]],
        onLoadSuccess: function (data) {
            $('#dg').datagrid("clearSelections");
            if (data.rows.length > 0) {
                //调用mergeCellsByField()合并单元格
                mergeCellsByField("dg", "OrderNo", "0");
            }
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
                url: rootUrl + "Order/OrderDetails/Delete",
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


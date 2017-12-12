//打开窗体
function openDialog(id, pid) {
    if (id) {
        parent.openTab("修改支付配置", rootUrl + "PayConfigs/Edit?id=" + id);
    }
    else {
        parent.openTab("添加支付配置", rootUrl + "PayConfigs/Create?pid=" + (pid ? pid : '0'));
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
                url: rootUrl + "PayConfigs/UpdateStatus",
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

//新增支付方式配置
function createModule() {
    openDialog();
}

//编辑支付方式配置
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.PayConfigID);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的支付方式配置', 'info');
        return;
    }
}
//删除支付方式配置
function deleteModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        deleteData(node.PayConfigID);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的支付方式配置', 'info');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '支付方式配置',
        url: rootUrl + 'PayConfigs/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'PayConfigID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'PayName', title: '支付方式名称', width: 80, align: 'center' },
        { field: 'PayCode', title: '支付编号', width: 80, align: 'center' },
        {
            field: 'PayType', title: '支付类型', width: 50, align: 'center',
            formatter: function (value, row, index) {
                //支付类型（0:后台充值 1:在线支付 ）
                switch (value) {
                    case "0":
                        return "后台充值";
                    case "1":
                        return "在线支付";
                    default:
                        return "";
                }

            }
        },
        {
          field: 'Status', title: '状态', width: 50, align: 'center',
          formatter: function (value, row, index) {
              //0支付方式状态(0，开发中 1 开启，2 关闭)
              switch (value) {
                  case "0":
                      return "开发中";
                  case "1":
                      return "启用";
                  case "2":
                      return "关闭";
                  default:
                      return "";
              }

          }
        },
        { field: 'SortOrder', title: '排序', width: 80, align: 'center' },
        { field: 'Remark', title: '描述', width: 120, align: 'center' },
        { field: 'Note', title: '支付帮助说明（提示用户）', width: 120, align: 'center' },
        {
            field: 'x', title: '操作', width: 100, align: 'center', formatter: function (value, row, index) {
                return "";
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
                url: rootUrl + "PayConfigs/Delete",
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


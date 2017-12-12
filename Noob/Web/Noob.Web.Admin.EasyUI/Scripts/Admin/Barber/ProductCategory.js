//打开窗体
function openDialog(id, pid) {
    if (id) {
        addDialog({ url: rootUrl + "Barber/ProductCategory/Edit?id=" + id, title: '修改服务项目类别', width: 360, height: 260 });
    }
    else {
        addDialog({ url: rootUrl + "Barber/ProductCategory/Create?pid=" + (pid ? pid : '0'), title: '添加理服务项目类别', width: 360, height: 260 });
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
                url: rootUrl + "Barber/ProductCategory/UpdateStatus",
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
function reloadData(id) {
    if (id > 0) {
        $("#dg").treegrid("update", {
            id: id,
            row: { state: 'closed' }
        });
        $("#dg").treegrid("reload", id);
    }
    else {
        $("#dg").treegrid("unselectAll");
        $("#dg").treegrid("reload");
    }
}

//新增理发服务项目类别
function createModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        var level = node.LevelNum ? (node.LevelNum + 1) : 1;
        openDialog(0, node.id, level);
    }
    else {
        openDialog();
    }
}
//新增子菜单
function createModuleChild() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        var level = node.LevelNum ? (node.LevelNum + 1) : 1;
        openDialog(0, node.id, level);
    } else {
        $.messager.alert('提示信息', '请选择要在哪个节点添加理发服务项目类别', 'info');
        return;
    }
}
//编辑菜单
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的理发服务项目类别', 'info');
        return;
    }
}
//删除菜单
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].id);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的理发服务项目类别', 'error');
        return;
    }
}
$(function () {
    $('#dg').treegrid({
        title: '理发服务项目类别',
        url: rootUrl + 'Barber/ProductCategory/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        toolbar: '#tbar',
        // onContextMenu: onContextMenu,
        columns: [[
           { field: 'name', title: '名称', width: 150 },
           { field: 'CategoryCode', title: '类别标识', width: 100, align: 'center' },
           { field: 'SortOrder', title: '排序', width: 50, align: 'center' },
           {
               field: 'x', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                   var statusText = "";
                   var cmd = "";
                   switch (row.Status) {
                       case 1:
                           statusText = "禁用";
                           cmd = "Disable";
                           break;
                       case 0:
                           statusText = "启用";
                           cmd = "Enable";
                           break;
                       default:
                           break;
                   }
                   return (isEdit == "1" ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.id + '\')">编辑</a>') : "") + '&nbsp;&nbsp;';
               }
           }
        ]],
        onLoadSuccess: function () {
            delete $(this).treegrid('options').queryParams['id'];
        }
    });
    function onContextMenu(e, row) {
        e.preventDefault();
        console.log(row.id);
        $(this).treegrid('select', row.id);
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    }
    $.extend($.fn.validatebox.defaults.rules, {
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
        }
    });
});

//删除数据
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Barber/ProductCategory/Delete",
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


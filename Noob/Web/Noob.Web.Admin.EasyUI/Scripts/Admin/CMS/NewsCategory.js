//打开窗体
function openDialog(id, pid) {
    if (id) {
        addDialog({ url: rootUrl + "NewsCategory/Edit?id=" + id, title: '修改资讯类别', width: 500, height: 420 });
    }
    else {
        addDialog({ url: rootUrl + "NewsCategory/Create?pid=" + (pid ? pid : '0'), title: '添加资讯类别', width: 500, height: 420 });
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
                url: rootUrl + "NewsCategory/UpdateStatus",
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

//新增资讯类别
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
        $.messager.alert('提示信息', '请选择要在哪个节点添加资讯类别', 'info');
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
        $.messager.alert('提示信息', '请选择要编辑的资讯类别', 'info');
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
        $.messager.alert('提示信息', '请选择要删除的资讯类别', 'error');
        return;
    }
}
$(function () {
    $('#dg').treegrid({
        title: '资讯类别',
        url: rootUrl + 'NewsCategory/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        toolbar: '#tbar',
        // onContextMenu: onContextMenu,
        columns: [[
           { field: 'name', title: '名称', width: 150 },
           { field: 'CategoryCode', title: '类别标识', width: 100, align: 'center' },
           {
               field: 'ImageUrl', title: '图片', width: 160, align: 'center', formatter: function (value, row, index) {
                   if (value == null || value == '') return '';
                    var imgUrl = uploadUrl + value;
                    return '<div class="thumb"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
                }
            },
           { field: 'SortOrder', title: '排序', width: 50, align: 'center' },
           {
               field: 'CategoryType', title: '类型', width: 50, align: 'center',
               formatter: function (value, row, index) {
                   //广告类型 (1:外部链接广告 2 :内部链接广告 3:Html广告)
                   switch (value) {
                       case "0":
                           return "资讯";
                       case "1":
                           return "视频资讯";
                       default:
                           return "";
                   }

               }
           },
           {
               field: 'x', title: '操作', width: 160, align: 'center',
               formatter: function (value, row, index) {
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
                url: rootUrl + "NewsCategory/Delete",
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


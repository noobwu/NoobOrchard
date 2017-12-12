//打开窗体
function openDialog(id, pid, level) {
    if (id) {
        addDialog({ url: rootUrl+"Group/Edit/" + id, title: '修改用户组', width: 410, height: 250 });
    }
    else {
        addDialog({
            url: rootUrl+"Group/Create?pid=" + (pid ? pid : '0') + "&level=" + (level ? level : '1'),
            title: (pid ? '添加用户组' : '添加用户组'), width: 410, height: 250
        });
    }
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

//新增用户组
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
//新增根用户组
function createModuleRoot() {
    openDialog();
}
//新增子用户组
function createModuleChild() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        var level = node.LevelNum ? (node.LevelNum + 1) : 1;
        openDialog(0, node.id, level);
    }
}
//编辑用户组
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的用户组', 'info');
        return;
    }
}
//删除用户组
function deleteModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        deleteData(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的用户组', 'info');
        return;
    }
}

//打开新的选项卡
function openDialogMenu(id) {

    addDialog({ url: rootUrl+"Authorize/Index?type=0&id="+id, title: '用户组授权', width:800, height: 400 });
}

$(function () {
    $.ajaxSetup({
        cache: false
    });
    $('#dg').treegrid({
        title: '用户组信息',
        url: rootUrl+'Group/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        //onContextMenu: onContextMenu,
        toolbar: '#tbar',
        columns: [[
        { field: 'name', title: '用户组名称', width: 150, halign: 'center', align: 'center' },
        {
            field: 'GroupType', title: '用户组类型', width: 50, halign: 'center', align: 'center',
            formatter: function (value, row, index) {
                return value == 1 ? "系统用户组" : "普通用户组";
            }
        },
        { field: 'SortOrder', title: '排序大小', width: 100, align: 'center' },
        { field: 'CreateTime', title: '创建时间', width: 100, align: 'center' },
        {
             field: 'id', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                 return '<a href="javascript:void(0)"  onclick="openDialog(\'' + row.id + '\')">编辑</a>'+ '&nbsp;&nbsp;' +
                        ('<a href="javascript:void(0)"  onclick="openDialogMenu(\'' + row.id + '\')">授权</a>') + '&nbsp;&nbsp;'
             }
         }
        ]],
        onLoadSuccess: function () {
            delete $(this).treegrid('options').queryParams['id'];
        }
    });
    function onContextMenu(e, row) {
        e.preventDefault();
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
                url: rootUrl+"Group/Delete",
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
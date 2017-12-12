//打开窗体
function openDialog(id, pid, level) {
    if (id) {
        addDialog({ url: rootUrl+"RightsType/Edit/" + id, title: '修改权限分类', width: 410, height: 210 });
    }
    else {
        addDialog({ url: rootUrl+"RightsType/Create?pid=" + (pid ? pid : '0') + "&level=" + (level ? level : '1'), title: (pid ? '添加子权限分类' : '添加权限分类'), width: 410, height: 210 });
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

//新增权限分类
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
//新增根权限分类
function createModuleRoot() {
    openDialog();
}
//新增子权限分类
function createModuleChild() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        var level = node.LevelNum ? (node.LevelNum + 1) : 1;
        openDialog(0, node.id, level);
    }
}
//编辑权限分类
function updateModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的权限分类', 'info');
        return;
    }
}
//删除权限分类
function deleteModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        deleteData(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的权限分类', 'info');
        return;
    }
}
$(function () {
    $.ajaxSetup({
        cache: false
    });
    $('#dg').treegrid({
        title: '权限分类信息',
        url: rootUrl+'RightsType/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        onContextMenu: onContextMenu,
        toolbar: '#tbar',
        columns: [[
        { field: 'name', title: '权限分类名称', width: 200, halign: 'center' },
        { field: 'SortOrder', title: '排序大小', width: 100, align: 'center' },
        { field: 'CreateTime', title: '创建时间', width: 100, align: 'center' }
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
                url: rootUrl+"RightsType/Delete",
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
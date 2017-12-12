//打开窗体
function openDialog(id, pid) {
    if (id&&id>0) {
        addDialog({ url: rootUrl+"Menu/Edit/" + id, title: '修改菜单', width: 500, height: 300 });
    }
    else {
        addDialog({ url: rootUrl+"Menu/Create?pid=" + (pid ? pid : '0'), title: "添加菜单", width: 500, height: 300 });
    }
}
//重新加载
function reloadData(id) {
    if (id > 0) {
        $("#tg").treegrid("update", {
            id: id,
            row: { state: 'closed' }
        });
        $("#tg").treegrid("reload", id);
    }
    else {
        $("#tg").treegrid("unselectAll");
        $("#tg").treegrid("reload");
    }
}

//新增菜单
function createModule() {
    var node = $('#tg').treegrid('getSelected');
    if (node) {
        var menuType = node.MenuType;
        switch (menuType) {
            case 0:
                return "菜单类别";
            case 1:
                $.messager.alert('提示信息', '该菜单无法添加子菜单', 'info');
                return;
            default:
                return "";
        }
        openDialog(0, node.id);
    }
    else {
        openDialog();
    }
}
//新增根菜单
function createModuleRoot() {
    openDialog();
}
//新增子菜单
function createModuleChild() {
    var node = $('#tg').treegrid('getSelected');
    if (node) {
        var level = node.LevelNum ? (node.LevelNum + 1) : 1;
        openDialog(0, node.id, level);
    }
}
//编辑菜单
function updateModule() {
    var node = $('#tg').treegrid('getSelected');
    if (node) {
        openDialog(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要编辑的菜单', 'info');
        return;
    }
}
//删除菜单
function deleteModule() {
    var node = $('#tg').treegrid('getSelected');
    if (node) {
        deleteData(node.id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的菜单', 'info');
        return;
    }
}
$(function () {
    $.ajaxSetup({
        cache: false
    });
    $('#tg').treegrid({
        title: '菜单信息',
        url: rootUrl+'Menu/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        expandLayer: 3,
       // onContextMenu: onContextMenu,
        toolbar: '#tbar',
        columns: [[
        { field: 'name', title: '菜单名称', width: 200, halign: 'center' },
        { field: 'MenuCode', title: '菜单代码', width: 100, align: 'center' },
        { field: 'SortOrder', title: '排序大小', width: 100, align: 'center' },
        { field: 'MenuUrl', title: 'Url地址', width: 100, align: 'center' },
        { field: 'CreateTime', title: '创建时间', width: 100, align: 'center' },
        {
            field: 'MenuType', title: '菜单类型', width: 50, halign: 'center', align: 'center',
            formatter: function (value, row, index) {
                switch (value) {
                    case 0:
                        return "菜单类别";
                    case 1:
                        return "菜单";
                    default:
                        return "";
                }
            }
        },
        ]],
        onLoadSuccess: function () {
            //$('#tg').treegrid('expandAll',2);
            //delete $(this).treegrid('options').queryParams['id'];
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
                url: rootUrl+"Menu/Delete",
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
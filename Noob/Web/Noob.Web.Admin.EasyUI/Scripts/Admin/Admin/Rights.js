$(function () {
    $.ajaxSetup({
        cache: false
    });
    setSize();
    $(window).resize(function () {
        setTimeout(setSize, 500);
    });
    bindTree();
});

function setSize() {
    var h = $(window).height();
    var treeh = h - 50;
    var tgh = h - 10;
    $("#treeH").css({ "height": treeh });
    $("#tg").datagrid({ height: tgh });
}

function bindTree() {
    $("#ModuleTree").tree({
        url: rootUrl+'Rights/GetRightsTypes',
        animate: true,
        onBeforeSelect: function (node) {
            if (!$(this).tree('isLeaf', node.target)) {
                return false;
            }
        },
        onClick: setActionList
    });
}


function setActionList(node) {
    if (!$(this).tree('isLeaf', node.target)) {
        if (node.state == "open") {
            $(this).tree("collapse", node.target);
        }
        else {
            $(this).tree("expand", node.target);
        }
        return false;
    }
    $("#tg").datagrid("unselectAll");
    $("#tg").datagrid({
        url: rootUrl+'Rights/GetList?tid=' + node.id
    });
}

function onContextMenu(e, rowIndex, rowData) {
    e.preventDefault();
    $(this).datagrid('selectRow', rowIndex);
    $('#mm').menu('show', {
        left: e.pageX,
        top: e.pageY
    });
}

//打开窗体
function openDialog(id) {
    if (id) {
        addDialog({ url: rootUrl+"Rights/Edit/" + id, title: '修改权限', width: 410, height: 260 });
    }
    else {
        var id = $("#hidid").val();
        addDialog({ url: rootUrl+"Rights/Create?tid=" + id, title: '新增权限', width: 410, height: 260 });
    }
}

//新增权限
function createData() {
    var node = $('#ModuleTree').tree('getSelected');   
    if (node) {
        $("#hidid").val(node.id);
        //alert(node.id)
        openDialog();
    }
    else {
        $.messager.alert('提示信息', '请选择一个菜单', 'info');
        return false;
    }
}

//修改权限
function updateData() {
    var selects = $("#tg").datagrid("getSelections");
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择要编辑的行', 'info');
        return false;
    }
    else {
        if (selects.length > 1) {
            $.messager.alert('提示', '请选择一行数据', 'info');
            return false;
        }
        //for (var item in selects[0]) {
        //    //遍历pp对象中的属性，只显示出 非函数的 属性，注意不能 遍历 selects这个类
        //    if (typeof (selects[0][item]) == "function") {
        //        continue;
        //    }
        //    else {
        //        console.log("selects对象中" + item + "的属性=" + selects[0][item]);
        //    }
        //}
        var id = selects[0].id;
        openDialog(id);
    }
}

//重新加载
function reloadData() {
    $("#tg").datagrid("unselectAll");
    $('#tg').datagrid("reload");
}

//获取选中行ID
function selectDatas() {
    var selects = $("#tg").datagrid("getSelections");
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择要删除的行', 'info');
        return 0;
    }
    else {
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            ids.push(selects[i].id)
        }
        return ids.join(",");
    }
}
//删除数据
function deleteData() {
    var datas = selectDatas();
    if (datas) {
        $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: rootUrl+"Rights/Delete",
                    data: "id=" + datas,
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
}
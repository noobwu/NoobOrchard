//打开窗体
function openDialog(id, pid) {
    if (id) {
        addDialog({ url: rootUrl+"Organization/Edit/" + id, title: '修改组织机构', width: 420, height: 300 });
    }
    else {
        addDialog({ url: rootUrl+"Organization/Create?pid=" + (pid ? pid : '0'), title: (pid ? '新增子机构' : '新增机构'), width: 420, height: 320 });
    }
}
//重新加载
function reloadData(id) {
    //$('#dg').treegrid("reload");

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

//新增机构
function createModule() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        var statusFlag = $('#dg').treegrid('getSelected').StatusFlag;
        if (statusFlag == 0) {
            $.messager.alert('提示', '无效机构下不能添加子机构', 'info');
            return;
        }
        else {
            openDialog(0, node.id);
        }
    }
    else {
        openDialog();
    }
}

//新增根机构
function createDept() {
    openDialog();
}
//新增子机构
function createDeptChild() {
    var node = $('#dg').treegrid('getSelected');         
    if (node) {
        var statusFlag = $('#dg').treegrid('getSelected').StatusFlag;
        if (statusFlag == 0)
        {
            $.messager.alert('提示', '无效机构下不能添加子机构', 'info');
            return;
        }
        else
        {
            openDialog(0, node.id);
        }
        
    }
}
//编辑机构
function updateDept() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        openDialog(node.id);
    } else {
        $.messager.alert('提示信息', '请选择要编辑的机构', 'info');
        return;
    }
}
//删除机构
function deleteDept() {
    var node = $('#dg').treegrid('getSelected');
    if (node) {
        if (node.state == "closed" || $('#dg').treegrid("getChildren", node.id).length > 0) {
            $.messager.alert('提示', '你选择的机构下有子机构，请先删除子机构', 'info');
            return;
            //$.messager.alert('提示', '删除该机构将删除机构下的子机构，是否继续执行。', 'info', function () {
            //    deleteData(node.id);
            //});
        }
        else {
            deleteData(node.id);
        }

    } else {
        $.messager.alert('提示信息', '请选择要删除的机构', 'info');
        return;
    }
}
$(function () {
    $('#dg').treegrid({
        title: '组织机构',
        url: rootUrl+'Organization/GetList',
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        toolbar: '#tbar',
       // onContextMenu: onContextMenu,
        columns: [[
        { field: 'name', title: '名称', width: 150 },
        { field: 'Address', title: '办公室地址', width: 150, align: 'center' },
        { field: 'OfficePhone', title: '办公室电话', width: 80, align: 'center' },
        { field: 'CreateTime', title: '创建时间', width: 100, align: 'center' },
        { field: 'SortOrder', title: '排序', width: 50, align: 'center' },
        {
            field: 'StatusFlag', title: '是否有效', width: 100, align: 'center',
            formatter: function (value, row, index) {
                return value == 1 ? "有效" : "无效";
            }
        }]],
        onLoadSuccess: function (row,data) {
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
//搜索
function searchUserData() {
    $('#dg').datagrid('load', {
        userName: $.trim($("#txt_name").val()),
        userNo: $.trim($("#txt_userNo").val()),
        deptId: dept
    });
}
//删除数据 
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除记录吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl+"Organization/Delete",
                data: "id=" + id,
                success: function (data) {
                    if (data.Code!=undefined) {
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
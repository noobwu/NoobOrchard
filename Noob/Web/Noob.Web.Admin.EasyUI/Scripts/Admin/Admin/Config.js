//打开窗体
function openDialog(id) {
    if (id) {
        addDialog({ url: rootUrl+"Configs/Edit/" + id, title: '修改配置值', width: 600, height: 400 });
    }
    else { addDialog({ url: rootUrl+"Configs/Create", title: '新增配置值', width: 600, height: 400 }); }
}
//重新加载
function reloadData() {
    $('#dg').datagrid("reload");
}
var clearContent = null;
$(function () {
    enterSearch("btn_search");
    var isEdit = $("#hid_isEdit").val();
    $('#dg').datagrid({
        title: '基础配置列表',
        url: rootUrl+'Configs/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        idField: 'ConfigID',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        emptyMsg: '没有找到相关记录',
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'ConfigGroupName', title: '配置组名称', width: 100, align: 'center' },
        { field: 'ConfigName', title: '配置名称', width: 150, align: 'center' },
        { field: 'ConfigCode', title: '配置标识名称', width: 150, align: 'center' },
        {
            field: 'ConfigValue', title: '值', width: 260, align: 'center', formatter: function (value, row, index) {
                return "<div title='" + decodeURI(value) + "'>" + decodeURI(value) + "</div>";
            }
        },
        {
            field: 'Remark', title: '备注', width: 150, halign: 'center', align: 'left', formatter: function (value, row, index) {
                return '<div title="' + value + '">' + value + '</div>';
            } },
        { field: 'x', title: '操作', width: 50, align: 'center', formatter: function (value, row, index) {
            return ('<a href="javascript:void(0)" onclick="openDialog(\'' + row.ConfigID + '\')">编辑</a>');
        }
        }
        ]],
        onLoadSuccess: function () {
            $('#dg').datagrid("clearSelections");
        },
        onSelect: function (rowIndex, rowData) {

        },
        onUnselect: function (rowIndex, rowData) {

        }
    });

    //呈现列表数据
    $('#dg').datagrid({
        pagination: true,
        onLoadSuccess: function (data) {
            if (data.rows.length > 0) {
                //调用mergeCellsByField()合并单元格
                mergeCellsByField("dg", "ConfigGroupName", "0");
            }
        }
    });
});
    //搜索
function searchCodeData() {
    $('#dg').datagrid('load', {
        ConfigGroupName: $.trim($("#txtSearchConfigGroupName").val()),
        ConfigCode: $.trim($("#txtSearchConfigCode").val()),
        ConfigName: $.trim($("#txtSearchConfigName").val())
    });
}

//获取选中行ID
function selectDatas() {
    var selects = $("#dg").datagrid("getSelections");
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择记录。', 'info');
        return 0;
    }
    else {
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            ids.push(selects[i].ConfigID)
        }
        return ids.join(",");
    }
}

//删除数据
function deleteData() {
    var datas = selectDatas();
    if (datas) {
        $.messager.confirm('确认', '您确认想要删除记录吗？', function (r) {
            if (r) {
                $.ajax({
                    type: "POST",
                    url: rootUrl+"Configs/Delete",
                    data: "id=" + datas,
                    success: function (data) {
                        if (data.Code != undefined) {
                            if (data.Code == 1) {
                                $.messager.alert('提示', '删除成功。', 'info', function () {
                                    reloadData();
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


  


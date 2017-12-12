//打开窗体
function openDialog(id) {
    if (id) {
        parent.openTab("修改短信配置", rootUrl + "SMS/EditConfig/" + id);
    }
    else {
        parent.openTab("新增短信配置", rootUrl + "SMS/CreateConfig");
    }
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
        title: '短信配置',
        url: rootUrl + 'SMS/GetConfigList',
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
        { field: 'ConfigName', title: '配置名称', width: 150, align: 'center' },
        { field: 'ConfigCode', title: '配置标识名称', width: 150, align: 'center' },
        {
            field: 'ConfigValue', title: '短信配置信息', width: 260, align: 'center', formatter: function (value, row, index) {
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
    $('#dg').datagrid('load', {});
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



  


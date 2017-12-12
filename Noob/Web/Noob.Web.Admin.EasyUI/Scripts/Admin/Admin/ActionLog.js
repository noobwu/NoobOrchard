$(function () {
    enterSearch("btn_search");
    bindMenuCate();
    bindActionCate();
    $('#dg').datagrid({
        title: '用户操作日志列表',
        url: rootUrl+'ActionLog/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        pageSize: constfield.pageSize,
        fit: true,
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'Account', title: '操作人账号', width: 100, align: 'center'},
        { field: 'MenuName', title: '菜单名称', width: 150, align: 'center' },
        { field: 'ShowName', title: '权限名称', width: 120, align: 'center' },
        { field: 'IP', title: 'IP地址', width: 100, align: 'center' },
        { field: 'LogContent', title: '日志内容', width: 550, align: 'left',halign:'center',formatter:contentFormatter },
        { field: 'CreateDT', title: '操作时间', width: 130, align: 'center' }
        ]],
        onLoadSuccess: function () {
            $('#dg').datagrid("clearSelections");
            $('#dg').datagrid("DataGridLoad");
        }
    });
});

function contentFormatter(value,row)
{
    var str = value;
    if(value && value.length>45)
    {
        str = str.substr(0, 40) + '...';
        var content = '<div class="tipcontent" title="' +value + '">' +str + '</div>';
        str = content;
    }
        return str;    
}

//搜索
function searchData() {
    var cValue = $("#menuTree").combotree("getValue");
    if (isNaN(cValue)) {
        cValue = ""
        $("#hidCateList").val(cValue);
    }
    else {
        if (cValue == "") {
            $("#hidCateList").val(cValue);
        }
    }
    $('#dg').datagrid('load', {
        Account: $.trim($("#txtAccount").val()),
        MenuCode: $("#hidCateList").val(),
        ActionName: $("#ddl_ActionCate").combobox("getValue"),
        content: $.trim($("#txtContent").val()),
        StartDT: $('#txt_startDT').datebox('getValue'),
        EndDT: $('#txt_endDT').datebox('getValue')
    });
}


function bindMenuCate() {
    $("#menuTree").combotree({
        url: rootUrl+"ActionLog/GetMenu?t=" + Math.random(),
        method: "get",
        required: false,
        panelHeight: 200,
        onClick: clickCate
    }).combobox("initClear");
}
function clickCate(node) {
    var cValue = node.id;
    var nodes = node.children;
    if (nodes != null || nodes != undefined) {
        for (var i = 0; i < nodes.length; i++) {
            cValue += "," + nodes[i].id;
            var nodes1 = nodes[i].children;
            if (nodes1 != null || nodes1 != undefined) {
                for (var j = 0; j < nodes1.length; j++) {
                    cValue += "," + nodes1[j].id;
                }
            }
        }
    }
    $("#hidCateList").val(cValue);
}

function bindActionCate() {   
    $('#ddl_ActionCate').combobox({
        url: rootUrl+"ActionLog/GetActionType",
        valueField: 'ShowValue',
        textField: 'ShowName',
        editable: false
    }).combobox("initClear");
}

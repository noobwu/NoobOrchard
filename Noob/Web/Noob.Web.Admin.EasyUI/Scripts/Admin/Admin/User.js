//打开窗体
function openDialog(id) {
    if (id) {
        addDialog({ url: rootUrl+"User/Edit/" + id, title: '修改用户', width: 410, height: 400 });
    }
    else { addDialog({ url: rootUrl+"User/Create", title: '新增用户', width: 410, height: 450 }); }
}
//打开新的选项卡
function openDialogTab(id) {
    addDialog({ url: rootUrl+"User/Details/" + id, title: '查看用户', width: 410, height: 400 });
}
function openDialogPass(id) {
    addDialog({ url: rootUrl+"User/ResetPassWord/" + id, title: '修改密码', width: 310, height: 160 });
}


//打开新的选项卡
function openDialogMenu(id) {
    addDialog({ url: rootUrl+"Authorize/Index?type=1&id="+id, title: '用户授权', width:800, height: 400 });
}
//重新加载
function reloadData() {
    $('#dg').datagrid("reload");
}

$(function () {
    enterSearch("btn_search");
    var deptValue = "";
    $("#txtSearchOrgID").combotree({
        url: rootUrl+"Organization/GetSubList?t=" + Math.random(),
        method: "get",
        required: false,
        panelHeight: 100,
        value: deptValue,
        onBeforeSelect: function (node) {
            if (!$(this).tree('isLeaf', node.target)) {
                $('#txtSearchOrgID').combotree('clear');
                return false;
            }
        },
        onClick: function (node) {
            if (!$(this).tree('isLeaf', node.target)) {
                $('#txtSearchOrgID').combo('showPanel');
                $(this).tree("expand", node.target);
            }

        }
    });
    $('#dg').datagrid({
        title: '用户信息',
        url: rootUrl+'User/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'id',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        { field: 'UserName', title: '用户账号', width: 80, align: 'center' },
        { field: 'TrueName', title: '真实姓名', width: 80, align: 'center' },
        { field: 'Mobile', title: '手机号码', width: 80, align: 'center' },
        { field: 'Phone', title: '办公电话', width: 80, align: 'center' },
        { field: 'Email', title: 'Email地址', width: 80, align: 'center' },
        { field: 'GroupName', title: '所属角色', width: 200, align: 'center' },
        { field: 'OrgName', title: '所属组织结构', width: 100, align: 'center' },
        {
            field: 'CreateTime', title: '注册时间', width: 100, align: 'center'
        },
        {
            field: 'Status', title: '账号状态', width: 100, align: 'center', formatter: function (value, row, index) {
                return value == 1 ? "启用" : "禁用";
            }
        },
        {
            field: 'x', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                return (isEdit == "1" ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.id + '\')">编辑</a>') : "") + '&nbsp;&nbsp;' +
                    '<a href="javascript:void(0)" onclick="openDialogTab(\'' + row.id + '\')">查看</a>' + '&nbsp;&nbsp;' +
                      (isAuthorization == "1" ? ('<a href="javascript:void(0)"  onclick="openDialogMenu(\'' + row.id + '\')">授权</a>') : "") + '&nbsp;&nbsp;' +
                      (isChangePass == "1" ? ('<a href="javascript:void(0)" onclick="openDialogPass(\'' + row.id + '\')">重置密码</a>') : "");
            }
        }
        ]],
        onLoadSuccess: function () {
            $('#dg').datagrid("clearSelections");
        }
    });
    //easyui 验证扩展
    $.extend($.fn.validatebox.defaults.rules, {
        equals: {
            validator: function (value, param) {
                return value == $(param[0]).val();
            },
            message: '确认密码不一样.'
        },
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
        },
        phone: {// 验证电话号码
            validator: function (value) {
                return /^((\(\d{2,3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}(\-\d{1,4})?$/i.test(value);
            },
            message: '格式不正确,请使用下面格式:020-88888888'
        },
        mobile: {// 验证手机号码
            validator: function (value) {
                return /^(13|15|18)\d{9}$/i.test(value);
            },
            message: '手机号码格式不正确'
        },
        deptRequire: {// 验证下拉列表部门为必选
            validator: function (value, param) {
                var val = $(param[0]).combobox("getValue");
                return val != "-1";
            },
            message: '下拉列表部门为必选'
        }
    });
});

//搜索
function searchData() {
    var OrgID = $("#txtSearchOrgID").combobox("getValue");
    if (!OrgID || funcChina(OrgID)) {
        dept = -1;
    }
    var status = $("#sltStatus").combobox("getValue");
    
    $('#dg').datagrid('load', {
        UserName: $.trim($("#txtSearchUserName").val()),
        TrueName: $.trim($("#txtSearchTrueName").val()),
        OrgID: OrgID,
        Status: status
    });
}
//获取选中行ID
function selectDatas() {
    var selects = $("#dg").datagrid("getSelections");
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择要删除的用户。', 'info');
        return 0;
    }
    else {
        var ids = [];
        for (var i = 0; i < selects.length; i++) {
            //console.log("selects[i].id:"+selects[i].id);
            ids.push(selects[i].id)
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
                    url: rootUrl+"User/Delete",
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



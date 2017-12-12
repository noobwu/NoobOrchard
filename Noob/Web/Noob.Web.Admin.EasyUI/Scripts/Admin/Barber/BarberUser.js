//打开窗体
function openDialog(id, action) {
    switch (action) {
        case "Recommend":
            addDialog({ url: rootUrl + "Barber/BarberUser/Recommend?id=" + id, title: '推荐发型师', width: 360, height: 260 });
            break;
        default:
            break;

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
                url: rootUrl + "Barber/BarberUser/UpdateStatus",
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
function reloadData() {
    $("#dg").datagrid("reload");
}


//删除发型师信息
function deleteModule() {
    var id = "";
    // var node = $('#dg').datagrid('getSelected');//单行
    //id=node.LogID;
    var ids = [];
    var rows = $('#dg').datagrid('getSelections');//多行
    for (var i = 0; i < rows.length; i++) {
        ids.push(rows[i].UserId);
    }
    id = ids.join(',');
    if (id != null && id != "") {
        deleteData(id);
    }
    else {
        $.messager.alert('提示信息', '请选择要删除的发型师信息', 'error');
        return;
    }
}
$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '发型师管理',
        url: rootUrl + 'Barber/BarberUser/GetList',
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'UserId',
        pageSize: constfield.pageSize,
        fit: true,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        { field: 'ck', checkbox: true },
        //{ field: 'IdentityCard', title: '身份证号', width: 80, align: 'center' },
        //{ field: 'IdentityCardFrontImg', title: '身份证正面照', width: 80, align: 'center' },
        //{ field: 'IdentityCardcontraryImg', title: '身份证反面照', width: 80, align: 'center' },
        //{ field: 'RealStatus', title: '实名认证状态(0未审核 1审核中 2审核通过 3审核未通过)', width: 80, align: 'center' },
        //{ field: 'RealNameTime', title: '实名认证时间', width: 80, align: 'center' },
        //{ field: 'RealRemark', title: '实名认证备注', width: 80, align: 'center' },
         {
             field: 'Avatars', title: '头像', width: 160, align: 'center', formatter: function (value, row, index) {
                 var imgUrl = uploadUrl + value;
                 return '<div class="thumb"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
             }
         },
        { field: 'NickName', title: '艺名', width: 80, align: 'center' },
        { field: 'Mobile', title: '联系方式', width: 80, align: 'center' },
        //{ field: 'StarSign', title: '星座', width: 80, align: 'center' },
        //{ field: 'Nationality', title: '国籍', width: 80, align: 'center' },
        {
            field: 'Sex', title: '性别', width: 80, align: 'center',
            formatter: function (value, row, index) {
                //性别(0：未设,1：男,2：女,3:保密)
                switch (value) {
                    case "0":
                        return "未设";
                    case "1":
                        return "男";
                    case "2":
                        return "女";
                    case "3":
                        return "保密";
                    default:
                        return "";
                }

            }
        },
         {
             field: 'WorkStartTime', title: '工作时间', width: 80, align: 'center',
             formatter: function (value, row, index) {
                 return row.WorkStartTime + '至' + row.WorkEndTime
             }
         },
        { field: 'WorkYear', title: '从业时间(年)', width: 80, align: 'center' },
        {
             field: 'ImageUrl', title: '工作照', width: 160, align: 'center', formatter: function (value, row, index) {
                 var imgUrl = uploadUrl + value;
                 return '<div class="thumb"><img src="' + imgUrl + '" alt="' + imgUrl + '"/></div>';
             }
         },
        { field: 'Recommend', title: '是否推荐', width: 80, align: 'center' },
        { field: 'RecommendTitles', title: '推荐标题', width: 80, align: 'center' },
        {
            field: 'PraiseCount', title: '数量', width: 100, align: 'center', formatter:
                function (value, row, index) {
                    var countHtml = '<div class="num">点赞数量:' + row.PraiseCount + '</div>'
                        + '<div class="num">浏览数量:' + row.BrowseCount + '</div>'
                       + '<div class="num">收藏数量:' + row.FavoriteCount + '</div>'
                     + '<div class="num">订单数量:' + row.OrderCount + '</div>';
                 return countHtml;
             }
        },
        {
            field: 'x', title: '操作', width: 160, align: 'center', formatter: function (value, row, index) {
                return (isRecommend ? ('<a href="javascript:void(0)"  onclick="openDialog(\'' + row.UserId + '\',\'Recommend\')">推荐</a>') : "");
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
        }
    });
});

//删除数据
function deleteData(id) {
    $.messager.confirm('确认', '您确认想要删除吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl + "Barber/BarberUser/Delete",
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

//搜索
function searchData() {
    var status = $("#sltSearchStatus").combobox("getValue");
    $('#dg').datagrid('load', {
        Status: status
    });
}


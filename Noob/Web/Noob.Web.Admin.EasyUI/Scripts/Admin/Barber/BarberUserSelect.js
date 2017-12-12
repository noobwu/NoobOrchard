//重新加载
function reloadData() {
    $("#dg").datagrid("reload");
}

$(function () {
    enterSearch("btn_search");
    $('#dg').datagrid({
        title: '发型师',
        url: rootUrl + 'Barber/BarberUser/GetList',
        autoRowHeight:true,
        pagination: true,
        rownumbers: true,
        fitColumns: true,
        singleSelect: false,
        idField: 'UserId',
        pageSize: constfield.pageSize,
        fit: false,
        toolbar: '#tbar',
        pageList: constfield.pageList,
        view: myview,
        columns: [[
        {
            field: 'ck', checkbox: true
        },
        {
            field: 'ImageUrl', title: '照片', width: 180, fixed: true, align: 'center', formatter: function (value, row, index) {
                var imgUrl = uploadUrl + value;
                var avatarsUrl = uploadUrl + row.Avatars;
                return '<div class="select_thumb" id="thumb_'+row.UserId+'"><div>工作照</div><div class="image"><img src="' + imgUrl + '" alt=""/></div></div>' + '<div class="select_thumb"><div>头像</div><div class="avatars"><img src="' + avatarsUrl + '" alt=""/></div></div>';
            }
        },
      {
          field: 'PraiseCount', title: '数量', width: 100, fixed: true, align: 'center', formatter:
                function (value, row, index) {
                    var countHtml = '<div class="num">点赞数量:' + row.PraiseCount + '</div>'
                        + '<div class="num">浏览数量:' + row.BrowseCount + '</div>'
                        + '<div class="num">收藏数量:' + row.FavoriteCount + '</div>'
                    + '<div class="num">订单数量:' + row.OrderCount + '</div>';
                    return countHtml;
                }
        },
        { field: 'RealName', title: '艺名', width: 120, fixed: true, align: 'center' },
        { field: 'NickName', title: '英文名', width: 120, fixed: true, align: 'center' },
        { field: 'Mobile', title: '联系方式', width: 120, fixed: true, align: 'center' }
      
        ]],
        onLoadSuccess: function (data) {
           // $.inArray(5 + 5, ["8", "9", "10", 10 + ""]);
            $('#dg').datagrid("clearSelections");
            var id = $("#hidBarberUserIds").val();
            console.log('id1:' + id);
            if (!id.IsNullOrEmpty())
            {
                var idArray = id.split(',');
                console.log('id2:'+idArray.join(','));
                $.each(data.rows, function (index, item) {
                    var selected = $.inArray(item.UserId, idArray)>=0?true:false;
                    //console.log('index:' + index + ',userId:' + item.UserId + ',select:' + selected);
                    if (selected) {
                        $('#dg').datagrid('selectRecord', item.UserId);
                    }
                });
            }
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


//搜索
function searchData() {
    $('#dg').datagrid('load', {
    });
}


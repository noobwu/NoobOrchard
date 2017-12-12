var pageSize = 30;
$(function () {
    //var onlineImage = new OnlineImage('imageList');
    //onlineImage.reset();
    initPagination();
    getImageData(1);
});
function initPagination()
{
    $('#pager').pagination({
        total:1,
        pageSize: pageSize,
        //当用户选择新的页面时触发。回调函数包含两个参数：
        onSelectPage: function (pageNumber, pageSize) {
            $(this).pagination('loading');
            getImageData(pageNumber, pageSize);
            //console.log('onSelectPage:pageNumber:' + pageNumber + ',pageSize:' + pageSize);
            $(this).pagination('loaded');
        },
        //刷新按钮点击之前触发，返回 false 就取消刷新动作。
        onBeforeRefresh: function (pageNumber, pageSize) {
            //return true;
            // console.log('onBeforeRefresh:pageNumber:' + pageNumber + ',pageSize:' + pageSize);
        },
        //刷新之后触发
        onRefresh: function (pageNumber, pageSize) {
            console.log('onRefresh:pageNumber:' + pageNumber + ',pageSize:' + pageSize);

        },
        //当用户改变页面尺寸时触发
        onChangePageSize: function (pageNumber, pageSize) {
            // console.log('onChangePageSize:pageNumber:' + pageNumber + ',pageSize:' + pageSize);
        }
    });
}
function getImageData(page, rows)
{
    if (page == undefined)
    {
        page = 1;
    }
    if (rows == undefined) {
        rows = pageSize;
    }
    $.ajax({
        type: "POST",
        url: rootUrl + "Barber/BarberWorks/GetList",
        data: {
            'rows': rows,
            'page': page,
        },
        success: function (data) {
            try {
                data = eval('(' + data + ')');
            } catch (e) {
                $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
                return;
            }
            if (data.rows != undefined) {
                var imageTemlate = doT.template($('#imageList_Template').html());
                $('#imageList_list').html(imageTemlate(data.rows));
                $('#pager').pagination('refresh', {	// 改变选项并刷新页面右栏的信息
                    total: data.total,
                    pageNumber: page
                });
            }
            else {
                $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
        },
        complete: function (XMLHttpRequest, textStatus) {
            // this; // 调用本次AJAX请求时传递的options参数
        }
    });
}
var imgHeight = 40;
var parentHeight = 210;
/* 改变图片大小 */
function scaleImage(image) {
    scale(image, parentHeight - imgHeight, parentHeight - imgHeight);
}
function scale(img, w, h) {
    var ow = img.width,
        oh = img.height;
    // console.log("ow:"+ow+",oh:"+oh);
    if (ow >= oh) {
        var tmpWidth = w * ow / oh;
        var tmpHeight = h;
        if (tmpWidth >= w)
        {
            tmpWidth = w;
            tmpHeight = h * oh / ow;;
        }
        img.width = tmpWidth;
        img.height = tmpHeight;
        img.style.marginLeft = (parentHeight-tmpWidth)/2+'px';
    } else {
        var tmpWidth = w;
        var tmpHeight = h * oh / ow;
        if (tmpHeight >= h) {
            tmpHeight = h;
            tmpHeight = w * ow / oh;;
        }
        img.width = tmpWidth;
        img.height = tmpHeight;
        img.style.marginLeft = (parentHeight - tmpWidth)/2 + 'px';
        //img.style.marginTop = '-' + parseInt((img.height - (h+imgHeight)) / 2) + 'px';
    }
    
}
$(function () {
    loadData();
    initModals();
    initToast();
});
function loadData() {
    $('#tg').treegridData({
        title: '组织机构管理',
        url: rootUrl + 'Organization/GetList',
        idField: 'id',
        parentIdField: '_parentId',
        striped: true,   //是否各行渐变色
        bordered: true,  //是否显示边框
        expandAll: true,  //是否全部展开
        columns: [
            { field: 'name', title: '名称', width: 150 },
            { field: 'Address', title: '办公室地址', width: 150, align: 'center' },
            { field: 'OfficePhone', title: '办公室电话', width: 80, align: 'center' },
            { field: 'SortOrder', title: '排序', width: 50, align: 'center' },
            {
                field: 'StatusFlag', title: '是否有效', width: 100, align: 'center',
                formatter: function (value, row, index) {
                    return value == 1 ? "有效" : "无效";
                }
            },
            {
                field: 'id', title: '操作',
                formatter: function (value, row, index) {
                    var actionHtml = '<a href="javascript:" onclick="openModalDialog(this);" data-url="../Organization/Edit/' + value + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>';
                    actionHtml += '&nbsp;&nbsp;<a href="javascript:" onclick="openModalDialog(this);" data-url="../Organization/Create?pid=' + value + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-plus"></i> 添加子权限分类</a>';
                    switch (row.state) {
                        case "open":
                            actionHtml += '&nbsp;&nbsp;<a  href="javascript:" onclick="openDeleteModalDialog(' + value + ');"  class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-trash"></i> 删除</a>';
                            break;
                        default:
                            break;
                    }

                    return actionHtml;
                }
            }

        ]
    });
}
//重新加载
function reloadData() {
    closeModal($ajaxModal);
    $("#tg").treegridData("reload");
}
var $ajaxModal;
var $smallModal;
function initModals() {
    // general settings
    $.fn.modal.defaults.spinner = $.fn.modalmanager.defaults.spinner =
        '<div class="loading-spinner" style="width: 200px; margin-left: -100px;">' +
        '<div class="progress progress-striped active">' +
        '<div class="progress-bar" style="width: 100%;"></div>' +
        '</div>' +
        '</div>';
    $.fn.modalmanager.defaults.resize = true;
    //ajax modal:
    $ajaxModal = $('#ajax-modal');
    $smallModal = $('#small-modal');

}
function initToast() {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "positionClass": "toast-top-center",//toast-top-right,toast-top-left,toast-top-center,toast-top-full-width,toast-bottom-right,toast-bottom-left,toast-bottom-center,toast-bottom-full-width
        "onclick": null,
        "showDuration": "1000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}
function openModalDialog(el) {
    // create the backdrop and wait for next modal to be triggered
    $('body').modalmanager('loading');
    var url = $(el).attr('data-url');
    //console.log('url:' + url)
    setTimeout(function () {
        $ajaxModal.load(url, '', function () {
            $ajaxModal.modal();
        });
    }, 1000);
}
function openDeleteModalDialog(id) {
    $('#btnDelete').attr('data-id', id);
    $smallModal.modal();
}
//删除数据
function deleteData(el) {
    closeModal($smallModal);
    var id = $(el).attr('data-id') * 1;
    //console.log('id:' + id);
    if (!id) return;
    $.ajaxPost({
        url: rootUrl + "Organization/Delete",
        data: "id=" + id,
        handleSuccess: function (data) {
            showMessage('删除成功。', true);
        },
    });
}
function closeModal(el) {
    el.modal('hide');
}

function showMessage(msg, success) {
    if (success) {
        showToast(msg, success);
        $("#tg").treegridData("reload");
    }
    else {
        //swal("提示!", msg, "error");  
        showToast(msg, success);
    }
}
function showToast(msg, success) {
    var title = '提示!';
    var toastType = 'error';//success,info,warning,error
    if (success) {
        toastType = 'success';
    }
    toastr[toastType](msg, title);
}
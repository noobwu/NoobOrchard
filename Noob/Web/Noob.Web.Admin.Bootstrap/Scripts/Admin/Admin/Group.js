var grid = null;
jQuery(document).ready(function () {
    initData();
    initModals();
    initToast();
    bindEvents();
});
function bindEvents() {
    $('#addAction').click(function () {
        openModalDialog(this);
    });
    $('#deleteAction').click(function () {
        var ids = getIdCheckValues();
        if (ids == "" || ids == '0') {
            showToast('请先选择要删除的数据');
            return;
        }
        $('#btnDelete').data('id', ids);
        $smallModal.modal();
    });
}
function initData()
{
    grid = new Datatable();
    grid.init({
        src: $("#dg"),
        onSuccess: function (grid, response) {
            //console.log('onSuccess');
            // grid:        grid object
            // response:    json object of server side ajax response
            // execute some code after table records loaded
        },
        onError: function (grid) {
            // execute some code on network or other general error  
            //console.log('onError');
        },
        onDataLoad: function (grid) {
            // execute some code on ajax data load
            //$.unblockUI();
            //console.log('onDataLoad');
            //$('.loading-message').remove();
        },
        loadingMessage: 'Loading...',
        dataTable: { // here you can define a typical datatable settings from http://datatables.net/usage/options 

            // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
            // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/scripts/datatable.js). 
            // So when dropdowns used the scrollable div should be removed. 
            //"dom": "<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'<'table-group-actions pull-right'>>r>t<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'>>",
            "ajax": {
                "url": rootUrl + 'Group/GetList', // ajax source
            },
            "columns": [
                { "data": 'id' },
                { "data": 'name' },
                { "data": 'GroupType' },
                { "data": 'SortOrder' },
                { "data": 'CreateTime' },
                { "data": 'id' },
            ],
            "columnDefs": [
                {
                    "targets": 0,
                    "data": "id",
                    "render": function (data, type, full, meta) {
                        var actionHtml = '<label class="mt-checkbox mt-checkbox-single mt-checkbox-outline" onclick="handleIdMultipleCheck(this,event)"><input name="id" type="checkbox" class="checkboxes" value="' + data + '" ><span></span></label>';
                        return actionHtml

                    }
                },
                {
                    "targets": 2,
                    "data": "GroupType",
                    "render": function (data, type, full, meta) {
                        switch (data) {
                            case 1:
                                return "系统用户组";
                            default:
                                return "普通用户组";
                        }
                    }
                },
                {
                    "targets": 5,
                    "data": "id",
                    "render": function (data, type, full, meta) {
                        var actionHtml = '<a href="javascript:" onclick="openModalDialog(this);"  data-url="../Group/Edit/' + data + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>';
                        actionHtml += '&nbsp;<a  href="../Authorize/Index?type=0&id=' + data + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-cogs"></i> 用户组授权</a>';
                        actionHtml += '&nbsp;&nbsp;<a  href="javascript:" onclick="openDeleteModalDialog(' + data + ');"  class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-trash"></i> 删除</a>';
                        return actionHtml;
                    }
                },
            ],
            "bStateSave": true, // save datatable state(pagination, sort, etc) in cookie.
            "lengthMenu": [
                [10, 20, 50, 100, 150, -1],
                [10, 20, 50, 100, 150, "All"] // change per page values here
            ],
            "order": [
                [1, "asc"]
            ],// set first column as a default sort by asc
            "pageLength": 10, // default record count per page
            "paging": true,//paging
            "ordering": false,//ordering
            "processing": false,// enable/disable display message box on record load
            "serverSide": true, // enable/disable server side ajax loading

        }
    });

   
}
//重新加载
function reloadData() {
    closeModal($ajaxModal);
    grid.reload();
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

function closeModal(el) {
    el.modal('hide');
}
function openDeleteModalDialog(id) {
    $('#btnDelete').data('id', id);
    $smallModal.modal();
}
//删除数据
function deleteData(el) {
    closeModal($smallModal);
    var id = $(el).data('id') * 1;
    //console.log('id:' + id);
    if (!id) return;
    $.ajaxPost({
        url: rootUrl + "Group/Delete",
        data: "id=" + id,
        handleSuccess: function (data) {
            showMessage('删除成功。', true);
        },
    });
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


function showMessage(msg, success) {
    if (success) {
        showToast(msg, success);
        grid.reload();
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
function showWarningToast(msg) {
    var title = '提示!';
    var toastType = 'warning';//success,info,warning,error
    toastr[toastType](msg, title);
}

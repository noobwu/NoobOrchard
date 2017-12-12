$(function () {
    bindRightsTypes();
    initModals();
    initToast();
    bindEvents();
});
var rightsTypeId = "";
function bindEvents()
{
    $('#addAction').click(function () {
        if (rightsTypeId == "" || rightsTypeId == '0')
        {
            showWarningToast('请先选择权限类别');
            return;
        }
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
    //$('#dg thead .mt-checkbox span label').unbind('click');
    //$("#dg thead .group - checkable:input[type = checkbox]").unbind('click');

}

function bindRightsTypes() {
    var treeRightsType = $('#treeRightsType').jstree({
        'core': {
            'data': {
                url: rootUrl + 'RightsType/GetTreeList',
                "dataType": "json" // needed only if you do not supply JSON headers
            }
        }
    });

    $(treeRightsType).bind("select_node.jstree", function (evt, data) {
        if (data && data.selected && data.node) {
            if (rightsTypeId == data.selected[0]) {
                //$('#treeRightsType').jstree("deselect_all");//取消所有选中的节点
                //$('#treeRightsType').jstree("deselect_node", data.node);//取消选中的当前节点
            }
            else {
                rightsTypeId = data.selected[0];
                $('#addAction').attr('data-url', '../Rights/Create?tid=' + rightsTypeId);
                clearIdCheckAll();
                bindRights();
            }
        }
        //console.log("selected!");

    });
    $(treeRightsType).bind("deselect_node.jstree", function (evt, data) {
        //console.log("deselected!");
    });
}
var grid =null;
function bindRights()
{
    if (grid == null) {
        initRights();
    }
    else
    {
        grid.setAjaxParam('tid', rightsTypeId);
        grid.reload(); 
    }
}
//重新加载
function reloadData() {
    closeModal($ajaxModal);
    grid.reload(); 
}
function initRights()
{
    grid = new Datatable();
    grid.setAjaxParam('tid',rightsTypeId);
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
            "bStateSave": true, // save datatable state(pagination, sort, etc) in cookie.
            "lengthMenu": [
                ["All"] // change per page values here
            ],
            "pageLength": 10, // default record count per page
            "ajax": {
                "url": rootUrl + 'Rights/GetList', // ajax source
            },
            "columns": [
                { "data": 'id' },
                { "data": 'name' },
                { "data": 'RightsCode' },
                { "data": 'TypeName' },
                { "data": 'RightsType' },
                { "data": 'SortOrder' },
                { "data": 'id' },
            ],
            "columnDefs": [
                {
                    "targets": 0,
                    "data": "id",
                    "render": function (data, type, full, meta) {
                        var actionHtml = '<label class="mt-checkbox mt-checkbox-single mt-checkbox-outline" onclick="handleIdMultipleCheck(this,event)" ><input name="id" type="checkbox" class="checkboxes" value="' + data + '"  /><span><label></label></span></label>';
                        return actionHtml

                    }
                },
                {
                    "targets": 6,
                    "data": "id",
                    "render": function (data, type, full, meta) {
                        var actionHtml = '<a href="javascript:" onclick="openModalDialog(this);" data-url="../Rights/Edit/' + data + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>';
                        actionHtml += '&nbsp;&nbsp;<a  href="javascript:" onclick="openDeleteModalDialog(' + data + ');"  class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-trash"></i> 删除</a>';
                        return actionHtml

                    }
                },
            ],
            "paging": false,//paging
            "ordering": false,//ordering
            "processing": false,// enable/disable display message box on record load
            "serverSide": false, // enable/disable server side ajax loading
        }
    });
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
        url: rootUrl + "Rights/Delete",
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
        grid.setAjaxParam('tid', rightsTypeId);
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

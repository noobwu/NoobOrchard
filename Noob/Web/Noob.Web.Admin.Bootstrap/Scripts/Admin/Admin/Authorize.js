

$(function () {
    loadData();
    initModals();
    initToast();
    bindEvents();
});
function loadData() {
    $('#tg').treegridData({
        title: '授权',
        url: rootUrl + 'Authorize/GetList?id=' + id + '&type=' + type,
        idField: 'id',
        parentIdField: '_parentId',
        striped: true,   //是否各行渐变色
        bordered: true,  //是否显示边框
        expandAll: true,  //是否全部展开
        columns: [
            //{field:'ck',checkbox:true},//需设置                                    
            {
                title: '名称', field: 'name',
                formatter: function (value, rowData, rowIndex) {
                    //debug('rowData:', rowData);
                    var lblClass = '';
                    var checked = '';
                    switch (rowData.check) {
                        case 1:
                            checked = 'checked="checked"';//全部选中
                            break;
                        case 2:
                            lblClass = 'class="checked2"';//部分选中
                            break;
                        default:
                            break;
                    }
                    var chkName = 'id';
                    var chkClass = '';
                    switch (rowData.type) {
                        case 0:
                            chkName = 'rightsType';//权限类别
                            chkClass = 'rightsType';
                            break;
                        default:
                            chkName = 'id';//权限
                            chkClass = 'rights';
                            break;
                    }
                    //return "<span class='tree-checkbox tree-checkbox" + rowData.check + "' onclick='setCheckbox(this);' node-id='" + rowData.id + "'></span>" + value;
                    var actionHtml = '<label id="lbl_' + rowData.id + '" class="mt-checkbox mt-checkbox-single mt-checkbox-outline ' + rowData.idPath + '" data-row=\'' + JSON.stringify(rowData) + '\' onclick="handleIdCheck(this,event)"><input name="' + chkName + '" type="checkbox" class="checkboxes ' + chkClass + '" value="' + rowData.id + '" ' + checked + '><span><label ' + lblClass + '></label></span></label><span>' + value + '</span>';
                    return actionHtml
                }, width: 130
            },
            { field: 'code', title: '代码', width: 80, halign: 'center', align: 'center' },
            {
                field: 'type', title: '是否是权限', width: 50, halign: 'center', align: 'center',
                formatter: function (value, row, index) {
                    return value == 0 ? "否" : "是";
                }
            },
            {
                field: 'rightsType', title: '权限类型', width: 50, halign: 'center', align: 'center',
                formatter: function (value, row, index) {
                    switch (value) {
                        case 0:
                            return "菜单权限";
                        case 1:
                            return "普通权限";
                        default:
                            return "";
                    }
                }
            },
        ],
    });
}
//重新加载
function reloadData() {
    closeModal($ajaxModal);
    $("#tg").treegridData("reload");
}
function handleIdCheck(el,event)
{
    var oEvent = oEvent ? oEvent : window.event
    var oElem = oEvent.toElement ? oEvent.toElement : oEvent.relatedTarget; // 此做法是为了兼容FF浏览器
    //debug(oElem);
    var $oElem = $(oElem);
    var isInput = $oElem.is('input');
    var $checkbox = $(el).find('input[type=checkbox]');
    var chkType = 'rights';
    if ($checkbox.hasClass('rightsType'))
    {
        chkType = 'rightsType';
    }
    var checked = false;
    var rowData = $(el).data('row');
    //debug('isInput:', isInput, 'chkType:', chkType, 'rowData:', rowData);
    var tbody = $(el).parent().parent().parent();
    if (isInput) {
        checked = $checkbox.is(":checked");
        if (chkType == 'rights') {
            $checkbox.attr('checked', checked);
            handleParentIdCheck(el, rowData, tbody);
        }
        else
        {
            handleChildIdCheck(el, rowData, tbody, checked);
            handleParentIdCheck(el, rowData, tbody);
        }
    }
    else
    {
        var lblChecked2 = $(el).find('span label');
        var isChecked2 = lblChecked2.hasClass('checked2');
        //debug('isChecked2:', isChecked2);
        if (isChecked2) {
            checked = true;
            handleChildIdCheck(el, rowData, tbody, checked);
        }
    }
}
function handleChildIdCheck(el, rowData, tbody, checked)
{
    if (!el || !rowData) return;
    var lblCheck = $(el).find("span label");
    var checkbox = $(el).find('input[type=checkbox]');
    //checkbox.attr('checked', checked);
    if (checked) {
        clearRightsTypeCheck(lblCheck);
        lblCheck.addClass('checked');
        tbody.find(".treegrid-parent-" + rowData.id + "  .rights:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var tmpCheckedLable = $(this).next().find('label');//设置checked样式的label
            if (!tmpCheckedLable.hasClass('checked')) {
                tmpCheckedLable.attr('class', 'checked');
                //debug("tmpCheckedLable.hasClass('checked'):", tmpCheckedLable.hasClass('checked'));
            }
        });

        tbody.find(".treegrid-parent-" + rowData.id + "  .rightsType:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var tmpCheckedLable = $(this).next().find('label');//设置checked样式的label
            if (!tmpCheckedLable.hasClass('checked')) {
                tmpCheckedLable.attr('class', 'checked');
                //debug("tmpCheckedLable.hasClass('checked'):", tmpCheckedLable.hasClass('checked'));
            }
            var tmpLabel = $(this).parent();
            var tmpRowData = tmpLabel.data('row');

            handleChildIdCheck(tmpLabel, tmpRowData, tbody, checked)
        });
    }
    else
    {
        clearRightsTypeCheck(lblCheck);
        tbody.find(".treegrid-parent-" + rowData.id + "  .rights:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var tmpCheckedLable = $(this).next().find('label');//设置checked样式的label
            clearRightsTypeCheck(tmpCheckedLable);
        });

        tbody.find(".treegrid-parent-" + rowData.id + "  .rightsType:input[type=checkbox]").each(function () {
            $(this).attr('checked', checked);
            var tmpCheckedLable = $(this).next().find('label');//设置checked样式的label
            clearRightsTypeCheck(tmpCheckedLable);
            var tmpLabel = $(this).parent();
            var tmpRowData = tmpLabel.data('row');

            handleChildIdCheck(tmpLabel, tmpRowData, tbody, checked)
        });
    }
}
function  handleParentIdCheck(el, rowData, tbody)
{
    if (!el || !rowData || rowData._parentId==0) return;
    var lblContainer = tbody.find('#lbl_' + rowData._parentId); //lable>td>tr>tbody
    var lblCheck = lblContainer.find("span label");
    var checkbox = lblContainer.find("input[type=checkbox]");

    var chkRightsTypeTotalSize = 0;// var chkRightsTypeTotalSize = tbody.find(".treegrid-parent-" + rowData._parentId + "  .rightsType:input[type=checkbox]").size();//选项总个数
    var chkRightsTypeSize = 0;//  var chkRightsTypeSize = tbody.find(".treegrid-parent-" + rowData._parentId + "  .rightsType:input[type=checkbox]:checked").size();//选项总个数
    tbody.find(".treegrid-parent-" + rowData._parentId + "  .rightsType:input[type=checkbox]").each(function () {
        chkRightsTypeTotalSize++;
        if ($(this).attr('checked')) {
            chkRightsTypeSize++;
        }
    });

    var chkRightsTotalSize = tbody.find(".treegrid-parent-" + rowData._parentId + "  .rights:input[type=checkbox]").size();//选项总个数
    var chkRightsSize = tbody.find(".treegrid-parent-" + rowData._parentId + "  .rights:input[type=checkbox]:checked").size();//选项总个数
   
    //debug('chkRightsTypeTotalSize:', chkRightsTypeTotalSize, 'chkRightsTypeSize:', chkRightsTypeSize, 'chkRightsTotalSize:', chkRightsTotalSize, 'chkRightsSize:', chkRightsSize, 'name:', rowData.name, 'id:', rowData.id, '_parentId:', rowData._parentId);
    /*全部选中*/
    if ((chkRightsTypeTotalSize + chkRightsTotalSize) == (chkRightsTypeSize + chkRightsSize)) {
        checkbox.attr('checked', true);
        clearRightsTypeCheck(lblCheck);
        lblCheck.addClass('checked');

    }
    /*全部没选*/
    else if (chkRightsTypeSize == 0 && chkRightsSize == 0) {
        checkbox.attr('checked', false);
        clearRightsTypeCheck(lblCheck);
    }
    else
    {
         /*部分选中*/
        checkbox.attr('checked', false);
        clearRightsTypeCheck(lblCheck);
        lblCheck.addClass('checked2');
    }
    var nextRowData = lblContainer.data('row');
    handleParentIdCheck(lblContainer, nextRowData, tbody);
}
function getRightsIdCheckValues() {
    var dgId = 'tg';
    var ids = [];
    $("#" + dgId + " .rights:input[type=checkbox]:checked").each(function () {
        var tmpRightsIdArray = this.value.split("_");
        if (tmpRightsIdArray.length == 2) {
            ids.push(tmpRightsIdArray[1])
        }
    });
    return ids.join(",");
}
function clearRightsTypeCheck(el)
{
    el.removeClass('checked');
    el.removeClass('checked2');
}
//编辑权限分类
function saveData() {
    var rightsId = getRightsIdCheckValues();
    if (rightsId == "") return false;
}
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
    $smallModal = $('#small-modal');

}
function closeModal(el) {
    el.modal('hide');
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
var rightsId;
function bindEvents() {
    $('#saveAction').click(function () {
        rightsId = getRightsIdCheckValues();
        if (rightsId == "" || rightsId == '0') {
            showToast('请先选择相应的权限');
            return;
        }
        $smallModal.modal();
    });
    $('#cancelAction').click(function () {
        goBack();
    });
}
function showMessage(msg, success) {
    if (success) {
        showToast(msg, success);

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
        setTimeout(function () {
            goBack();
        }, 1000);
       
        toastType = 'success';
    }
    toastr[toastType](msg, title);
}
function goBack()
{
    if (type == 0) {
        goUrl('../../Group/Index');
    }
    else {
        goUrl('../../User/Index');
    }
}
function showWarningToast(msg) {
    var title = '提示!';
    var toastType = 'warning';//success,info,warning,error
    toastr[toastType](msg, title);
}

//编辑授权信息
function saveData(el) {
    //debug('saveData')
    closeModal($smallModal);
    //console.log('id:' + id);
    $.ajaxPost({
        url: rootUrl + "Authorize/AuthorizeRights",
        data: "id=" + id + "&type=" + type + "&rightsId=" + rightsId,
        handleSuccess: function (data) {
            showMessage('授予权限成功。', true);
        },
    });

}
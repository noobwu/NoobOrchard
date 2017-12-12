var target;

//重新加载
function reloadData(id) {
    if (id > 0) {
        $("#tg").treegrid("update", {
            id: id,
            row: { state: 'closed' }
        });
        $("#tg").treegrid("reload", id);
    }
    else {
        $("#tg").treegrid("unselectAll");
        $("#tg").treegrid("reload");
    }
}


//编辑权限分类
function saveModule() {
    var rightsId = getChecked();
    if (rightsId == "") return false;
    $.messager.confirm('确认', '您确认授予这些权限吗？', function (r) {
        if (r) {
            $.ajax({
                type: "POST",
                url: rootUrl+"Authorize/AuthorizeRights",
                data: "id=" + id + "&type=" + type + "&rightsId=" + rightsId,
                success: function (data) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            $.messager.alert('提示', '授予权限成功。', 'info', function () {
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

$(function () {
    target = $('#tg');
    $.ajaxSetup({
        cache: false
    });
    $('#tg').treegrid({
        title: '授权',
        url: rootUrl+'Authorize/GetList?id='+id+'&type='+type,
        fitColumns: true,
        fit: true,
        idField: "id",
        treeField: 'name',
        //onContextMenu: onContextMenu,
        //toolbar: '#tbar',
        columns: [[
				 //{field:'ck',checkbox:true},//需设置                                    
				 {
				     title:'名称',field:'name',
				     formatter:function(value,rowData,rowIndex)
				     { 
				         return "<span class='tree-checkbox tree-checkbox" + rowData.check + "' onclick='setCheckbox(this);' node-id='" + rowData.id + "'></span>" + value;
				     },width:130}, 
				{ field: 'code', title: '代码', width: 80, halign: 'center',align: 'center'},
			    { field: 'type', title: '是否是权限', width: 50, halign: 'center',align: 'center', 
			        formatter: function (value, row, index) {
			            return value == 0 ? "否" : "是";
			        }
			    },
				{ field: 'rightsType', title: '权限类型', width: 50, halign: 'center',align: 'center', 
				    formatter: function (value, row, index) {
				        switch(value)
				        {
				            case 0:
				                return "菜单权限";
				            case 1:
				                return "普通权限";
				            default:
				                return "";
				        }
				    }
				},
        ]],
        onLoadSuccess: function () {
            //$('#tg').treegrid('expandAll');
            //$('#tg').treegrid('collapseAll');
        }
    });
    function onContextMenu(e, row) {
        e.preventDefault();
        $(this).treegrid('select', row.id);
        $('#mm').menu('show', {
            left: e.pageX,
            top: e.pageY
        });
    }
    $.extend($.fn.validatebox.defaults.rules, {
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
        }
    });
});

function setCheckbox(node) 
{
    if ($(node).hasClass('tree-checkbox0')){
        $(node).removeClass('tree-checkbox0').addClass('tree-checkbox1');
    } else if ($(node).hasClass('tree-checkbox1')){
        $(node).removeClass('tree-checkbox1').addClass('tree-checkbox0');
    } else if ($(node).hasClass('tree-checkbox2')){
        $(node).removeClass('tree-checkbox2').addClass('tree-checkbox1');
    }
    if($(node).parent().prev().hasClass('tree-folder'))
    {
        setChildCheckbox($(node).parent().parent().parent().parent());
        setParentCheckbox($(node).parent().parent().parent().parent());
    }
    else
    {
        setParentCheckbox($(node).parent());
    }
    // console.log("setCheckbox,node:"+$(node).parent().html());

}
	 
function setChildCheckbox(node){
    //console.log("setChildCheckbox:"+$(node).next().html());
    var childck = node.next().find('.tree-checkbox');
    childck.removeClass('tree-checkbox0 tree-checkbox1 tree-checkbox2')
    if (node.find('.tree-checkbox').hasClass('tree-checkbox1')){
        childck.addClass('tree-checkbox1');
    } else {
        childck.addClass('tree-checkbox0');
    }
}
/**
* get the parent node
* param: DOM object, from which to search it's parent node 
*/
function getParentNode(target, param){
    //var node = $(param).parent().parent().prev();
    var node=param;
    //console.log("class:"+$(param).attr("class"));
    if($(param).prev().hasClass('tree-file'))
    {
        node=$(param).parent().parent().parent().parent().parent().parent().parent().parent().prev();	 
    }
    else
    {
        node=$(param).parent().parent().parent().parent().parent().prev();	  
    }
    //console.log("param:"+$(param).html());
    if (node.length){
        //console.log("target:"+$(node[0]).html());
        return $.extend({}, $.data(node[0], 'tree-node'), {
            target: node[0],
            checked: node.find('.tree-checkbox').hasClass('tree-checkbox1')
        });
    } else {
        //console.log("getParentNode is null:"+$(node).html());
        return null;
    }
}	
function setParentCheckbox(node) {
    //var tmpNode=$(param).parent().parent().parent().parent().parent().parent().prev();
    //console.log($(tmpNode).html());
    //console.log($(node).parent().parent().html());
    var pnode = getParentNode(target, node);
    if (pnode) {
        var ck = $(pnode.target).find('.tree-checkbox');
        ck.removeClass('tree-checkbox0 tree-checkbox1 tree-checkbox2');
        //console.log("node:"+$(node).parent().parent().html());
        if (isAllSelected($(node).parent().parent())) {
            ck.addClass('tree-checkbox1');
        } else if (isAllNull($(node).parent().parent())) {
            ck.addClass('tree-checkbox0');
        } else {
            ck.addClass('tree-checkbox2');
        }
        // console.log("pnode.target:" + $(pnode.target).html());
        setParentCheckbox($(pnode.target));
    }
    else {
        //var tmpNode=$(param).parent().parent().parent().parent().parent().parent().prev();
        //console.log($(tmpNode).html());
        //console.log("pnode is null:" + $(node).html());
    }

    function isAllSelected(n) {
        var ck = n.find('.tree-checkbox');
        if (ck.hasClass('tree-checkbox0') || ck.hasClass('tree-checkbox2')) {
            //for (var item in ck) {
            //    //遍历pp对象中的属性，只显示出 非函数的 属性，注意不能 遍历 selects这个类
            //    if (typeof (ck[item]) == "function") {
            //        continue;
            //    }
            //    else {
            //        if (item == "context") {
            //            //console.log("ck_" + item + "_的属性=" + $(ck[item]).html());
            //        }
            //    }
            //}
            return false;
        }
        var b = true;
        n.parent().siblings().each(function () {
            if (!$(this).find('.tree-checkbox').hasClass('tree-checkbox1')) {
                var tmpCK1 = $(this).find('.tree-checkbox1');
                if (tmpCK1 != null || tmpCK1 != undefined) {
                    //for (var item in tmpCK1) {
                    //    //遍历pp对象中的属性，只显示出 非函数的 属性，注意不能 遍历 selects这个类
                    //    if (typeof (tmpCK1[item]) == "function") {
                    //        continue;
                    //    }
                    //    else {
                    //        if (item == "context") {
                    //            //console.log("tmpCK1_" + item + "_的属性=" + $(tmpCK1[item]).html());
                    //        }
                    //    }
                    //}
                }
                b = false;
            }
        });
        return b;
    }
    function isAllNull(n) {
        var ck = n.find('.tree-checkbox');
        if (ck.hasClass('tree-checkbox1') || ck.hasClass('tree-checkbox2')) return false;
        var b = true;
        n.parent().siblings().each(function () {
            if (!$(this).find('.tree-checkbox').hasClass('tree-checkbox0')) {
                b = false;
            }
        });
        return b;
    }
}

function getSelects() {
    var selects = $("#tg").datagrid("getSelections");
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择权限', 'error');
        return "";
    }
    else {
        var rightsId = "";
        if (selects.length == 1) {
            rightsId = selects[0].id;
        }
        else {
            for (var i = 0; i < selects.length; i++) {

                if (i == 0)
                {
                    rightsId = selects[i].id;
                }
                else if (i != selects.length - 1) {
                    rightsId = rightsId + "," + selects[i].id
                }
            }
        }
        return rightsId;
    }
}

function getChecked() {
    var selects = $("span .tree-checkbox1");
   // console.log(selects.length);
    if (selects.length <= 0) {
        $.messager.alert('提示', '请选择权限', 'error');
        return "";
    }
    else {
        var rightsId = "";
        if (selects.length == 1) {
            var tmpRightsIdArray = $(selects[0]).attr("node-id").split("_");
            if (tmpRightsIdArray.length != 2)
            {
                return "";
            }
           rightsId = tmpRightsIdArray[1];
        }
        else {
            for (var i = 0; i < selects.length; i++) {
                var tmpRightsIdArray = $(selects[i]).attr("node-id").split("_");
                if (tmpRightsIdArray.length != 2) continue;
                if (rightsId=="") {
                    rightsId = tmpRightsIdArray[1];
                }
                else {
                    rightsId = rightsId + "," + tmpRightsIdArray[1];
                }
            }
        }
        return rightsId;
    }
}
/*
 http://www.cnblogs.com/landeanfen/p/6776152.html
 */
(function ($) {
    "use strict";
     function getHtml(item,column,index)
     {
     	if(column.formatter&& (typeof column.formatter == 'function'))
     	{
             return column.formatter.call(this,item[column.field], item,index);     		
     	}
     	else
     	{
     		return item[column.field];
     	}
     };
     //得到根节点
     function getRootNodes(data,options) {
        var result = [];
        $.each(data, function (index, item) {
            if (!item[options.parentIdField]) {
                result.push(item);
            }
        });
        return result;
     }
     //递归获取子节点并且设置子节点
     function getChildNodes(data, parentNode, tbody,options) {
        $.each(data, function (i, item) {
            if (item[options.parentIdField] == parentNode[options.idField]) {
            	var display=parentNode.state=="closed"?"none":"";
            	var className=item.state=="closed"?"treegrid-collapsed":"treegrid-expanded";
                var tr = $('<tr></tr>');
                tr.addClass('treegrid-' + item[options.idField]);
                tr.addClass('treegrid-parent-' + parentNode[options.idField]);
                //tr.css('display',display);
                $.each(options.columns, function (index, column) {
                    var td = $('<td></td>');
                    td.html(getHtml(item,column,index));
                    tr.append(td);
                });
                tbody.append(tr);
                getChildNodes(data, item, tbody,options)
            }
        });
    }
    function reload(target)
    {
      var options = $.data(target, 'treegridData').options;
      //console.log('reload,options：',JSON.stringify(options))
       if (options.url) {
            handleRequest($(target),options,false);
        }
        else if(options.data&&options.data.length) {
            //也可以通过defaults里面的data属性通过传递一个数据集合进来对组件进行初始化
        	loadData(target,options.data,options,false);
        }
    }
    function handleRequest(target,options,append)
    {
    	  $.ajax({
            type: options.type,
            url: options.url,
            data: options.ajaxParams,
            dataType: "JSON",
            success: function (data, textStatus, jqXHR) {
            	//console.log('handleRequest:','success');
            	var result=data.rows;
            	loadData(target,result,options,append);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(XMLHttpRequest.status);
                //alert(XMLHttpRequest.readyState);
                // alert(textStatus);
                //console.log(errorThrown);
               //console.log('handleRequest:','error');
            },
            complete: function (XMLHttpRequest, textStatus) {
               // 调用本次AJAX请求时传递的options参数
               //console.log('handleRequest:','complete');
            }
        });
    }
    function loadData(target,data,options,append)
    {
        //debugger;
        //构造表体
        var tbody;
        if(!append)
        {
           tbody=target.find('tbody').html('');
        }	
        else
        {
        	tbody = $('<tbody></tbody>');
        }
        var rootNode = getRootNodes(data,options);
        $.each(rootNode, function (i, item) {
        	var className=item.state=="closed"?"treegrid-collapsed":"treegrid-expanded";
            var tr = $('<tr></tr>');
            tr.addClass('treegrid-' + item[options.idField]);
            //tr.addClass(className);
            $.each(options.columns, function (index, column) {
                var td = $('<td></td>');
                td.html(getHtml(item,column,index));
                tr.append(td);
            });
            tbody.append(tr);
            getChildNodes(data, item, tbody,options);
        });
        target.append(tbody);
        target.treegrid({
            expanderExpandedClass: options.expanderExpandedClass,
            expanderCollapsedClass: options.expanderCollapsedClass
        });
        if (!options.expandAll) {
            target.treegrid('collapseAll');
        }
    }
    function buildTreegrid(target,options)
    {
    	//构造表头
        var thr = $('<tr></tr>');
        $.each(options.columns, function (i, item) {
            var th = $('<th style="padding:10px;"></th>');
            th.text(item.title);
            thr.append(th);
        });
        var thead = $('<thead></thead>');
        thead.append(thr);
        target.append(thead);
    	target.addClass('table');
        if (options.striped) {
            target.addClass('table-striped');
        }
        if (options.bordered) {
            target.addClass('table-bordered');
        }
    }
        
    $.fn.treegridData = function (options, param) {
    	//console.log('treegridData');
        //如果是调用方法
        if (typeof options == 'string') {
           switch(options){
				case 'reload':
					return this.each(function(){
						reload(this);	// param: the row id value
					});
				default:
				    return $.fn.treegridData.methods[options](this, param);
			}

        }
        
        //如果是初始化组件,并用$.data保存options数据
        var $this=this;
        //如果是初始化组件,并用$.data保存options数据
        options = options || {};
	    this.each(function(){
			var state = $.data(this, 'treegridData');
			if (state){
				options=$.extend(state.options, options);
			} else {
			    options=$.extend({}, $.fn.treegridData.defaults, options);
				$.data(this, 'treegridData', {
					options:options
				});
			}
		});
		//return;
        var target = $(this);
		//console.log('options：',JSON.stringify(options));
        //debugger;
        buildTreegrid(target,options);
        if (options.url) {
            handleRequest(target,options,true);
        }
        else if(options.data&&options.data.length) {
            //也可以通过defaults里面的data属性通过传递一个数据集合进来对组件进行初始化
        	loadData(target,options.data,options,true);
        }
        return target;
    };

    $.fn.treegridData.methods = {
        getAllNodes: function (target, data) {
            return target.treegrid('getAllNodes');
        },
        //组件的其他方法也可以进行类似封装........
    };

    $.fn.treegridData.defaults = {
        idField: 'id',
        parentIdField: '_parentId',
        data: [],    //构造table的数据集合
        type: "GET", //请求数据的ajax类型
        url: null,   //请求数据的ajax的url
        ajaxParams: {}, //请求数据的ajax的data属性
        expandColumn: null,//在哪一列上面显示展开按钮
        expandAll: false,  //是否全部展开
        striped: false,   //是否各行渐变色
        bordered: false,  //是否显示边框
        columns: [],
        expanderExpandedClass: 'glyphicon glyphicon-chevron-down',//展开的按钮的图标
        expanderCollapsedClass: 'glyphicon glyphicon-chevron-right'//缩起的按钮的图标
        
    };
})(jQuery);
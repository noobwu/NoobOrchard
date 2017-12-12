var TableDatatablesAjax = function () {

    var handleTreeGrid = function () {

        var grid = new Datatable();
        grid.init({
            src: $("#tg"),
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
            onDataLoad: function(grid) {
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
                    "url": rootUrl+'Menu/GetList', // ajax source
                },
               "columns": [
                    { "data": 'id' },
                    { "data": 'name' },
                    { "data": 'MenuCode' },
                    { "data": 'SortOrder' },
                    { "data": 'MenuUrl' },
                    { "data": 'CreateTime' },
                    { "data": 'MenuType' },
                    { "data": 'id' },
                ],
                "columnDefs": [
                      {
                        "targets": 6,
                        "data": "MenuType",
                        "render": function(data, type, full, meta ) {
                            switch (data) {
                                case 0:
                                    return "菜单类别";
                                case 1:
                                    return "菜单";
                                default:
                                    return "";
                            }
                        }
                      },
                      {
                        "targets": 7,
                        "data": "id",
                        "render": function(data, type, full, meta ) {
                            return '<a href="../Menu/Edit/' + data +'" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>'
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

 

    return {

        //main function to initiate the module
        init: function () {
            handleTreeGrid();
        }

    };

}();

jQuery(document).ready(function() {
    //TableDatatablesAjax.init();
    loadData();
});
function loadData() {
    $('#tg').treegridData({
        title: '权限类别管理',
        url: rootUrl + 'Menu/GetList',
        idField: 'id',
        parentIdField: '_parentId',
        striped: true,   //是否各行渐变色
        bordered: true,  //是否显示边框
        expandAll: true,  //是否全部展开
        columns: [
            { field: 'name', title: '菜单名称', width: 200, halign: 'center' },
            { field: 'MenuCode', title: '菜单代码', width: 100, align: 'center' },
            { field: 'SortOrder', title: '排序大小', width: 100, align: 'center' },
            { field: 'MenuUrl', title: 'Url地址', width: 100, align: 'center' },
            {
                field: 'MenuType', title: '菜单类型', width: 50, halign: 'center', align: 'center',
                formatter: function (value, row, index) {
                    switch (value) {
                        case 0:
                            return "菜单类别";
                        case 1:
                            return "菜单";
                        default:
                            return "";
                    }
                }
            },
            {
                field: 'id', title: '操作',
                formatter: function (value, row, index) {
                    var actionHtml = '<a  href="../Menu/Edit/' + value + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>';
                    switch (row.MenuType) {
                        case 0:
                            actionHtml += '&nbsp;&nbsp;<a  href="../Menu/Create?pid=' + value + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-plus"></i> 添加子菜单</a>';
                            break;
                        case 1:
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


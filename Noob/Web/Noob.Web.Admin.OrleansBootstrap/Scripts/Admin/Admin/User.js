
jQuery(document).ready(function () {
    loadData();
    bind();
});
var grid;
function loadData()
{
    grid = new Datatable();
    grid.init({
        src: $("#dg"),
        loadingMessage: 'Loading...',
        dataTable: { // here you can define a typical datatable settings from http://datatables.net/usage/options 

            // Uncomment below line("dom" parameter) to fix the dropdown overflow issue in the datatable cells. The default datatable layout
            // setup uses scrollable div(table-scrollable) with overflow:auto to enable vertical scroll(see: assets/global/scripts/datatable.js). 
            // So when dropdowns used the scrollable div should be removed. 
            //"dom": "<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'<'table-group-actions pull-right'>>r>t<'row'<'col-md-8 col-sm-12'pli><'col-md-4 col-sm-12'>>",
            "ajax": {
                "url": rootUrl + 'User/GetList', // ajax source
            },
            "columns": [
                { "data": 'id' },
                { "data": 'UserName' },
                { "data": 'TrueName' },
                { "data": 'Mobile' },
                { "data": 'Phone' },
                { "data": 'Email' },
                { "data": 'TrueName' },
                { "data": 'OrgName' },
                { "data": 'CreateTime' },
                { "data": 'Status' },
                { "data": 'id' },
            ],
            "columnDefs": [
                {
                    "className": 'control',
                    "targets": 9,
                    "data": "Status",
                    "render": function (data, type, full, meta) {
                        switch (data) {
                            case 1:
                                return "启用";
                            default:
                                return "禁用";
                        }
                    }
                },
                {
                    "targets": 10,
                    "data": "id",
                    "render": function (data, type, full, meta) {
                        var actionHtml = '<a href="../User/Edit/' + data + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-edit"></i> 编辑</a>';
                        actionHtml += '&nbsp;<a href="../Authorize/Index?type=1&id=' + data + '" class="btn btn-outline btn-circle btn-sm purple"> <i class="fa fa-cogs"></i>授权</a>';
                        return actionHtml;
                    }
                },
            ],
            "bStateSave": true, // save datatable state(pagination, sort, etc) in cookie.
            "lengthMenu": [
                [10, 20, 50, 100, 150, -1],
                [10, 20, 50, 100, 150, "All"] // change per page values here
            ],
            "pageLength": 10, // default record count per page
            "paging": true,//paging
            "ordering": false,//ordering
            "processing": false,// enable/disable display message box on record load
            "serverSide": true, // enable/disable server side ajax loading
            // setup responsive extension: http://datatables.net/extensions/responsive/
            "responsive": true
        }
    });
}
function bind()
{
   // handle group actionsubmit button click
    $('#btnSearch').click(function (e) {
        e.preventDefault();
        grid.setAjaxParam("userName", $('#txtSearchUserName').val());
        grid.setAjaxParam("trueName", $('#txtSearchTrueName').val());
        grid.setAjaxParam("status", $('#sltSearchStatus').val());
        grid.getDataTable().ajax.reload();
        grid.clearAjaxParams();
        /*
         App.alert({
                type: 'danger',
                icon: 'warning',
                message: 'Please select an action',
                container: grid.getTableWrapper(),
                place: 'prepend'
            });
        */
    });
  
}
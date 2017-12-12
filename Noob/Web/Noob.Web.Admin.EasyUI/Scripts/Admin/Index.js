$(function () {
    runTick();
    loadTopMenu();

    initContextMenu();
});

function initContextMenu()
{
    //绑定tabs的右键菜单
    $("#tt").tabs({
        onContextMenu: function (e, title) {
            e.preventDefault();
            $('#tabsMenu').menu('show', {
                left: e.pageX,
                top: e.pageY
            }).data("tabTitle", title);
        }
    });

    //实例化menu的onClick事件
    $("#tabsMenu").menu({
        onClick: function (item) {
            //console.log("name:"+item.name);
            closeTab(this, item.name);
        }
    });
}
function openTab(title, url) {
    //console.log("title:" + title + ",url:" + url);
    var tabDiv = $("#tt");
    if (tabDiv.tabs('exists', title)) {
        tabDiv.tabs('select', title);
        tabDiv.tabs('update', {
            tab: tabDiv.tabs('getSelected'), options: {
                content: '<iframe scrolling="auto" src="' + url + '" frameborder="0" width="100%" height="98%"></iframe>'
            }
        });
    } else {
        tabDiv.tabs('add', {
            title: title,
            fit: true,
            content: '<iframe scrolling="auto" src="' + url + '" frameborder="0" width="100%" height="98%"></iframe>',
            closable: true
        });
    }
}
function closedTabs(tabTitle) {
    if (tabTitle != "首页") {
        $("#tt").tabs("close", tabTitle);
    }
}
function selectTabs(tabTitle) {
    if (tabTitle != "首页") {
        $("#tt").tabs("select", tabTitle);
    }
}

//几个关闭事件的实现
function closeTab(menu, type) {
    var curTabTitle = $(menu).data("tabTitle");
    var tabs = $("#tt");

    if (type === "closeCurrent") {
        if (curTabTitle != "首页")
            tabs.tabs("close", curTabTitle);
        return;
    }

    var allTabs = tabs.tabs("tabs");
    var closeTabsTitle = [];

    $.each(allTabs, function () {
        var opt = $(this).panel("options");
        if (opt.closable && opt.title != curTabTitle && type === "closeOther") {
            closeTabsTitle.push(opt.title);
        } else if (opt.closable && type === "closeAll") {
            closeTabsTitle.push(opt.title);
        }
    });

    for (var i = 0; i < closeTabsTitle.length; i++) {
        tabs.tabs("close", closeTabsTitle[i]);
    }
}

//关闭指定的选项卡并且刷新指定的选项卡
function goTargetIframeAndReloadData(options) {
    try {
        $("#tt").tabs("select", options.targetTabName);
        var currIfram = $("#tt").tabs("getSelected").find("iframe")[0].contentWindow;
        options.reloAdmethod.call(currIfram);
        closeTab(options.closeTabName);
    } catch (e) { }
}

//刷新当前打开的选项卡
function reloadTabData() {
    var tab = $("#tt").tabs("getSelected");
    var index = $('#tt').tabs('getTabIndex', tab);
    //$("#tt").tabs("getSelected").panel('refresh');
    window.frames[index].location.reload();
}

function showLocale(objD) {
    var str, colorhead, colorfoot;
    var yy = objD.getYear();
    if (yy < 1900) yy = yy + 1900;
    var MM = objD.getMonth() + 1;
    if (MM < 10) MM = '0' + MM;
    var dd = objD.getDate();
    if (dd < 10) dd = '0' + dd;
    var hh = objD.getHours();
    if (hh < 10) hh = '0' + hh;
    var mm = objD.getMinutes();
    if (mm < 10) mm = '0' + mm;
    var ss = objD.getSeconds();
    if (ss < 10) ss = '0' + ss;
    var ww = objD.getDay();
    if (ww == 0) colorhead = "<font color=\"#ffffff\">";
    if (ww > 0 && ww < 6) colorhead = "<font color=\"#ffffff\">";
    if (ww == 6) colorhead = "<font color=\"#ffffff\">";
    if (ww == 0) ww = "星期日";
    if (ww == 1) ww = "星期一";
    if (ww == 2) ww = "星期二";
    if (ww == 3) ww = "星期三";
    if (ww == 4) ww = "星期四";
    if (ww == 5) ww = "星期五";
    if (ww == 6) ww = "星期六";
    colorfoot = "</font>"
    str = colorhead + yy + "-" + MM + "-" + dd + " " + hh + ":" + mm + ":" + ss + "  " + ww + colorfoot;
    return (str);
}

//运行时间
function runTick() {
    var today = new Date();
    document.getElementById("lbl_tick").innerHTML = showLocale(today);
    window.setTimeout("runTick()", 1000);
}

//改变密码
function changePassword() {
    addDialog({ url: rootUrl + "Home/ChangePassword", title: '修改密码', width: 410, height: 180 });
}

//退出系统
function logout() {
    $.messager.confirm('提示信息', "确认要退出系统吗？", function (r) {
        if (r) {
            window.location.href = rootUrl+"Account/Logout";
        }
    });
}

//获取顶级菜单
function loadTopMenu() {
    if (menuDatas != undefined) {
        var html = [];
        for (var item in menuDatas) {
            //var itemArray = item.split("_");
            //var menuId = itemArray[itemArray.length - 1];
            var menuId = item;
            html.push('<a href="javascript:void(0)" name="' + menuDatas[item].name + '"code="' + menuDatas[item].code + '" id="' + menuId + '" onclick="selectMenus(this)">'
            + menuDatas[item].name + '</a>&nbsp;&nbsp;');
        }
        var topMenu = $("#div_topMenu");
        topMenu.html(html.join(''));
        topMenu.children("a")[0].click();
        $("#div_topMenu a").each(function (index) {
            if (index == 0) {
                $(this).linkbutton({
                    toggle: true,
                    group: "topMenuGroup",
                    plain: true,
                    selected: true
                });
                $(this).css({ color: "#0092DC" });
            }
            else {
                $(this).linkbutton({
                    toggle: true,
                    group: "topMenuGroup",
                    plain: true
                });
            }
            $(this).hover(function () {
                $(this).css({ color: "#0092DC" });
            }, function () {
                var flag = $(this).linkbutton("options").selected;
                if (!flag) {
                    $(this).css({ color: "#fff" });
                }
            });
        });
    }
}
//获取子菜单
function selectMenus(obj) {
    $("#div_topMenu a").css({ color: "#fff" });
    $(obj).css({ color: "#0092DC" });
    var id = $(obj).attr("id");
    var name = $(obj).attr("name");
    $("#div_panelWest").prev().children('div.panel-title').html(name);
    createMenusHtml(id);

}
function createMenusHtml(id) {
    //console.log("id:" + id);
    var accor = $('#nav');
    var p = accor.accordion("panels");
    for (var k = 0, len = p.length; k < len; k++) {
        accor.accordion("remove", 0);
    }
    accor.accordion({
        animate: false
    });
    if (menuDatas[id] != null && menuDatas[id].menus != undefined) {
        for (var i = 0; i < menuDatas[id].menus.length; i++) {
            var html = [];
            html.push('<ul>');
            //for (var item in menuDatas[id].menus[i]) {
            //    //遍历pp对象中的属性，只显示出 非函数的 属性，注意不能 遍历 selects这个类
            //    if (typeof (menuDatas[id].menus[i][item]) == "function") {
            //        continue;
            //    }
            //    else {
            //        console.log("menuDatas[id].menus[i]_" + item + "_的属性=" + menuDatas[id].menus[i][item]);
            //    }
            //}
            for (var j = 0; j < menuDatas[id].menus[i].menus.length; j++) {
                html.push('<li><div><a ref="' + menuDatas[id].menus[i].menus[j].id + '" href="javascript:void(0)" rel="' + menuDatas[id].menus[i].menus[j].url
                      + '" ><span class="" >&nbsp;</span><span class="nav">' + menuDatas[id].menus[i].menus[j].name + '</span></a></div></li> ');
            }
            html.push('</ul>');
            var title = menuDatas[id].menus[i].name;
            var cont = html.join("");
            //console.log("cont:"+cont);
            $('#nav').accordion('add', {
                title: title,
                content: cont,
                iconCls: 'icon11'
            });
        }
    }

    $('div.easyui-accordion li a').click(function () {
        var thisObj = $(this);
        var tabTitle = thisObj.children('.nav').text();
        var url = thisObj.attr("rel");
        var menuid = thisObj.attr("ref");
        openTab(tabTitle, url);
        $('div.easyui-accordion li div').removeClass("selected");
        thisObj.parent().addClass("selected");
    }).hover(function () {
        $(this).parent().addClass("hover");
    }, function () {
        $(this).parent().removeClass("hover");
    });

    //选中第一个
    try {
        var panels = $('#nav').accordion('panels');
        var t = panels[0].panel('options').title;
        $('#nav').accordion('select', t);
    }
    catch (e) {

    }
}
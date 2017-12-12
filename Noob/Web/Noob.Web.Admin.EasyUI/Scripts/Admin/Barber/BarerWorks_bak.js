
/* 在线图片 */
function OnlineImage(target) {
    this.container = document.getElementById(target);
    this.init;
}
OnlineImage.prototype = {
    init: function () {
        this.reset();
        this.initEvents();
    },
    /* 初始化容器 */
    initContainer: function () {
        this.container.innerHTML = '';
        this.list = document.createElement('ul');
        this.clearFloat = document.createElement('li');
        $(this.list).addClass('list')
        $(this.clearFloat).addClass('clearFloat')
        // domUtils.addClass(this.list, 'list');
        //domUtils.addClass(this.clearFloat, 'clearFloat');

        this.list.appendChild(this.clearFloat);
        this.container.appendChild(this.list);
    },
    /* 初始化滚动事件,滚动到地步自动拉取数据 */
    initEvents: function () {
        var _this = this;

        /* 滚动拉取图片 */
        $(this.container).scroll(function (e) {
            var panel = this;
            if (panel.scrollHeight - (panel.offsetHeight + panel.scrollTop) < 10) {
                _this.getImageData();
            }
        });
        /* 选中图片 */
        $(this.container).click(function (e) {
            var target = e.target || e.srcElement,
               li = target.parentNode;
            //if (li.tagName.toLowerCase() == 'li') {
            //    if (domUtils.hasClass(li, 'selected')) {
            //        domUtils.removeClasses(li, 'selected');
            //    } else {
            //        domUtils.addClass(li, 'selected');
            //    }
            //}
        });
    },
    /* 初始化第一次的数据 */
    initData: function () {
        /* 拉取数据需要使用的值 */
        this.state = 0;
        this.listSize = 100;
        this.listIndex = 0;
        this.listEnd = false;

        /* 第一次拉取数据 */
        this.getImageData();
    },
    /* 重置界面 */
    reset: function () {
        this.initContainer();
        this.initData();
    },
    /* 向后台拉取图片列表数据 */
    getImageData: function () {
        var _this = this;
        if (!_this.listEnd && !this.isLoadingData) {
            this.isLoadingData = true;
            $.ajax({
                type: "POST",
                url: rootUrl + "BarberWorks/GetList",
                data: {
                    page: this.listIndex,
                    rows: this.listSize
                },
                success: function (data) {
                    try {
                        data = eval('(' + data + ')');
                    } catch (e) {
                        $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
                    }

                    if (data.rows != undefined) {
                        _this.pushData(data.rows);
                        //_this.listIndex = parseInt(json.start) + parseInt(json.list.length);
                        if (_this.listIndex >= 10) {
                            _this.listEnd = true;
                        }
                        _this.isLoadingData = false;
                    }
                    else {
                        _this.isLoadingData = false;
                        $.messager.alert('提示', '系统错误，请联系管理人员！', 'error');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(XMLHttpRequest.status);
                    //alert(XMLHttpRequest.readyState);
                    // alert(textStatus);
                    //console.log(errorThrown);
                    _this.isLoadingData = false;
                },
                complete: function (XMLHttpRequest, textStatus) {
                    // this; // 调用本次AJAX请求时传递的options参数
                }
            });
        }
    },
    /* 添加图片到列表界面上 */
    pushData: function (list) {
        var i, item, img, icon, _this = this,
            urlPrefix = uploadUrl;
        for (i = 0; i < list.length; i++) {
            if (list[i] && list[i].ImageUrl) {
                item = document.createElement('li');
                img = document.createElement('img');
                icon = document.createElement('span');
                img.onload = (function (image) {
                    return function () {
                        _this.scale(image, image.parentNode.parentNode.offsetWidth, image.parentNode.parentNode.offsetHeight);
                    }
                })(img);
                img.width = 113;
                img.setAttribute('src', urlPrefix + list[i].ImageUrl + (list[i].ImageUrl.indexOf('?') == -1 ? '?noCache=' : '&noCache=') + (+new Date()).toString(36));
                img.setAttribute('_src', urlPrefix + list[i].ImageUrl);
                $(icon).addClass('icon');
                item.appendChild(img);
                item.appendChild(icon);
                this.list.insertBefore(item, this.clearFloat);
            }
        }
    },
    /* 改变图片大小 */
    scale: function (img, w, h, type) {
        var ow = img.width,
            oh = img.height;

        if (type == 'justify') {
            if (ow >= oh) {
                img.width = w;
                img.height = h * oh / ow;
                img.style.marginLeft = '-' + parseInt((img.width - w) / 2) + 'px';
            } else {
                img.width = w * ow / oh;
                img.height = h;
                img.style.marginTop = '-' + parseInt((img.height - h) / 2) + 'px';
            }
        } else {
            if (ow >= oh) {
                img.width = w * ow / oh;
                img.height = h;
                img.style.marginLeft = '-' + parseInt((img.width - w) / 2) + 'px';
            } else {
                img.width = w;
                img.height = h * oh / ow;
                img.style.marginTop = '-' + parseInt((img.height - h) / 2) + 'px';
            }
        }
    },
    getInsertList: function () {
        var i, lis = this.list.children, list = [], align = getAlign();
        for (i = 0; i < lis.length; i++) {
            if ($(lis[i]).hasClass('selected')) {
                var img = lis[i].firstChild,
                    src = img.getAttribute('_src');
                list.push({
                    src: src,
                    _src: src,
                    alt: src.substr(src.lastIndexOf('/') + 1),
                    floatStyle: align
                });
            }

        }
        return list;
    }
};
initUploads = function (initOpts) {
    initOpts = $.extend({
        //id名称索引 如:_0
        index: '',
        //{Boolean} [可选] [默认值：false] 设置为 true 后，不需要手动调用上传，有文件选择即开始上传。
        auto: false,
        //{Boolean} 是否开起同时选择多个文件能力。
        multiple: false,
        // 添加的文件数量
        fileCount: 0,
        // 添加的文件总大小
        fileSize: 0,
        //图片文件对应hidden的name名称
        imgUrlName: 'ImageUrl',
        //图片上传服务器相对应的目录
        imgDir: 'Images',
        //限制图片文件数量
        fileNumLimit: 1,
         //限制单个图片文件大小 4M
        fileSingleSizeLimit: 4 * 1024 * 1024,
        //apiUrl
        apiUrl: rootUrl + 'api/WebUploader',

    }, initOpts);
    //console.log(JSON.stringify(initOpts))
    // 当domReady的时候开始初始化
    //http://fex.baidu.com/webuploader/doc/index.html#WebUploader_Uploader
    var $wrap = $('#uploader' + initOpts.index),

        // 图片容器
        $queue = $('<ul class="filelist"></ul>')
            .appendTo($wrap.find('.queueList')),

        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),

        // 文件总体选择信息。
        $info = $statusBar.find('.info'),

        // 上传按钮
        $upload = $wrap.find('.uploadBtn'),

        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),

        $progress = $statusBar.find('.progress').hide(),

        // 添加的文件数量
        fileCount = initOpts.fileCount,

        // 添加的文件总大小
        fileSize = initOpts.fileSize,

        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,

        // 缩略图大小
        thumbnailWidth = 220 * ratio,
        thumbnailHeight = 220 * ratio,

        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // 所有文件的进度信息，key为file id
        percentages = {},
        // 判断浏览器是否支持图片的base64
        isSupportBase64 = (function () {
            var data = new Image();
            var support = true;
            data.onload = data.onerror = function () {
                if (this.width != 1 || this.height != 1) {
                    support = false;
                }
            }
            data.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
            return support;
        })(),

        // 检测是否已经安装flash，检测flash的版本
        flashVersion = (function () {
            var version;

            try {
                version = navigator.plugins['Shockwave Flash'];
                version = version.description;
            } catch (ex) {
                try {
                    version = new ActiveXObject('ShockwaveFlash.ShockwaveFlash')
                        .GetVariable('$version');
                } catch (ex2) {
                    version = '0.0';
                }
            }
            version = version.match(/\d+/g);
            return parseFloat(version[0] + '.' + version[1], 10);
        })(),

        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                    'WebkitTransition' in s ||
                    'MozTransition' in s ||
                    'msTransition' in s ||
                    'OTransition' in s;
            s = null;
            return r;
        })(),
        $initOpts = initOpts,
        // WebUploader实例
        uploader;

    if (!WebUploader.Uploader.support('flash') && WebUploader.browser.ie) {

        // flash 安装了但是版本过低。
        if (flashVersion) {
            (function (container) {
                window['expressinstallcallback'] = function (state) {
                    switch (state) {
                        case 'Download.Cancelled':
                            alert('您取消了更新！')
                            break;

                        case 'Download.Failed':
                            alert('安装失败')
                            break;

                        default:
                            alert('安装已成功，请刷新！');
                            break;
                    }
                    delete window['expressinstallcallback'];
                };

                var swf = 'expressInstall.swf';
                // insert flash object
                var html = '<object type="application/' +
                    'x-shockwave-flash" data="' + swf + '" ';

                if (WebUploader.browser.ie) {
                    html += 'classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" ';
                }

                html += 'width="100%" height="100%" style="outline:0">' +
                    '<param name="movie" value="' + swf + '" />' +
                    '<param name="wmode" value="transparent" />' +
                    '<param name="allowscriptaccess" value="always" />' +
                    '</object>';

                container.html(html);

            })($wrap);

            // 压根就没有安转。
        } else {
            $wrap.html('<a href="http://www.adobe.com/go/getflashplayer" target="_blank" border="0"><img alt="get flash player" src="http://www.adobe.com/macromedia/style_guide/images/160x41_Get_Flash_Player.jpg" /></a>');
        }

        return;
    } else if (!WebUploader.Uploader.support()) {
        alert('Web Uploader 不支持您的浏览器！');
        return;
    }

    // 实例化
    uploader = WebUploader.create({
        /**
     * 调试开关
     */
        debug: true,
        /**
         * 上传调试输出
         */
        dump: function (object) {
            if (this.debug) {
                if (typeof object == 'object') {
                    console.dir(object);
                } else {
                    console.log(object);
                }
            }
        },
        /*
id {Seletor|dom} 指定选择文件的按钮容器，不指定则不创建按钮。注意 这里虽然写的是 id, 但是不是只支持 id, 还支持 class, 或者 dom 节点。
label {String} 请采用 innerHTML 代替
innerHTML {String} 指定按钮文字。不指定时优先从指定的容器中看是否自带文字。
multiple {Boolean} 是否开起同时选择多个文件能力。
*/
        pick: {
            id: '#filePicker' + $initOpts.index,
            label: '点击选择图片',
            multiple: $initOpts.multiple
        },
        // formData {Object} [可选] [默认值：{}] 文件上传请求的参数表，每次发送都会发送此对象中的参数。
        formData: {
            uid: $initOpts.uid,
            dir: $initOpts.imgDir
        },
        fileVal: $initOpts.imgUrlName,        //fileVal {Object} [可选] [默认值：'file'] 设置文件上传域的name。
        dnd: '#dndArea' + $initOpts.index,//dnd {Selector} [可选] [默认值：undefined] 指定Drag And Drop拖拽的容器，如果不指定，则不启动。
        paste: '#uploader' + $initOpts.index,// {Selector} [可选] [默认值：undefined] 指定监听paste事件的容器，如果不指定，不启用此功能。此功能为通过粘贴来添加截屏的图片。建议设置为document.body.
        swf: rootUrl + 'Scripts/webuploader/Uploader.swf',
        chunked: false,
        chunkSize: 512 * 1024,
        server: $initOpts.apiUrl + '/UploadImage',
        runtimeOrder: 'html5,flash',//runtimeOrder {Object} [可选] [默认值：html5,flash] 指定运行时启动顺序。默认会想尝试 html5 是否支持，如果支持则使用 html5, 否则则使用 flash.可以将此值设置成 flash，来强制使用 flash 运行时。

        /*
         {Arroy} [可选] [默认值：null] 指定接受哪些类型的文件。 由于目前还有ext转mimeType表，所以这里需要分开指定。
title {String} 文字描述
extensions {String} 允许的文件后缀，不带点，多个用逗号分割。
mimeTypes {String} 多个用逗号分割。
        */
        // accept: {
        //     title: 'Images',
        //     extensions: 'gif,jpg,jpeg,bmp,png',
        //     mimeTypes: 'image/*'
        // },
        /*
    thumb {Object} [可选] 配置生成缩略图的选项。
默认为：
{
width: 110,
height: 110,

// 图片质量，只有type为`image/jpeg`的时候才有效。
quality: 70,

// 是否允许放大，如果想要生成小图的时候不失真，此选项应该设置为false.
allowMagnify: true,

// 是否允许裁剪。
crop: true,

// 为空的话则保留原有图片格式。
// 否则强制转换成指定的类型。
type: 'image/jpeg'
}
        */
        thumb: {
            width: 220,
            height: 220,
            // 图片质量，只有type为`image/jpeg`的时候才有效。
            quality: 70,
            // 是否允许放大，如果想要生成小图的时候不失真，此选项应该设置为false.
            allowMagnify: true,
            // 是否允许裁剪。
            crop: true,
            // 为空的话则保留原有图片格式。
            // 否则强制转换成指定的类型。
            type: 'image/jpeg'
        },
        /*
        compress {Object} [可选] 配置压缩的图片的选项。如果此选项为false, 则图片在上传前不进行压缩。
        默认为：

        {
            width: 1600,
            height: 1600,

            // 图片质量，只有type为`image/jpeg`的时候才有效。
            quality: 90,

            // 是否允许放大，如果想要生成小图的时候不失真，此选项应该设置为false.
            allowMagnify: false,

            // 是否允许裁剪。
            crop: false,

            // 是否保留头部meta信息。
            preserveHeaders: true,

            // 如果发现压缩后文件大小比原来还大，则使用原来图片
            // 此属性可能会影响图片自动纠正功能
            noCompressIfLarger: false,

            // 单位字节，如果图片大小小于此值，不会采用压缩。
            compressSize: 0
        }
        */

        // 禁掉全局的拖拽功能。这样不会出现图片拖进页面的时候，把图片打开。
        disableGlobalDnd: true,
        fileNumLimit: $initOpts.fileNumLimit,//300 图片文件数量
        fileSizeLimit: $initOpts.fileNumLimit * $initOpts.fileSingleSizeLimit,    // 200 M
        fileSingleSizeLimit: $initOpts.fileSingleSizeLimit,    // 50 M
        auto: $initOpts.auto//{Boolean} [可选] [默认值：false] 设置为 true 后，不需要手动调用上传，有文件选择即开始上传。
        /*
prepareNextFile {Boolean} [可选] [默认值：false] 是否允许在文件传输时提前把下一个文件准备好。 对于一个文件的准备工作比较耗时，比如图片压缩，md5序列化。 如果能提前在当前文件传输期处理，可以节省总体耗时。
chunked {Boolean} [可选] [默认值：false] 是否要分片处理大文件上传。
chunkSize {Boolean} [可选] [默认值：5242880] 如果要分片，分多大一片？ 默认大小为5M.
chunkRetry {Boolean} [可选] [默认值：2] 如果某个分片由于网络问题出错，允许自动重传多少次？
threads {Boolean} [可选] [默认值：3] 上传并发数。允许同时最大上传进程数。
formData {Object} [可选] [默认值：{}] 文件上传请求的参数表，每次发送都会发送此对象中的参数。
fileVal {Object} [可选] [默认值：'file'] 设置文件上传域的name。
method {Object} [可选] [默认值：'POST'] 文件上传方式，POST或者GET。
sendAsBinary {Object} [可选] [默认值：false] 是否已二进制的流的方式发送文件，这样整个上传内容php://input都为文件内容， 其他参数在$_GET数组中。
fileNumLimit {int} [可选] [默认值：undefined] 验证文件总数量, 超出则不允许加入队列。
fileSizeLimit {int} [可选] [默认值：undefined] 验证文件总大小是否超出限制, 超出则不允许加入队列。
fileSingleSizeLimit {int} [可选] [默认值：undefined] 验证单个文件大小是否超出限制, 超出则不允许加入队列。
duplicate {Boolean} [可选] [默认值：undefined] 去重， 根据文件名字、文件大小和最后修改时间来生成hash Key.
disableWidgets {String, Array} [可选] [默认值：undefined] 默认所有 Uploader.register 了的 widget 都会被加载，如果禁用某一部分，请通过此 option 指定黑名单
        */
    });

    // 拖拽时不接受 js, txt 文件。
    uploader.on('dndAccept', function (items) {
        var denied = false,
            len = items.length,
            i = 0,
            // 修改js类型
            unAllowed = 'text/plain;application/javascript ';

        for (; i < len; i++) {
            // 如果在列表里面
            if (~unAllowed.indexOf(items[i].type)) {
                denied = true;
                break;
            }
        }

        return !denied;
    });

    uploader.on('dialogOpen', function () {
        //console.log('here');
    });

    // uploader.on('filesQueued', function() {
    //     uploader.sort(function( a, b ) {
    //         if ( a.name < b.name )
    //           return -1;
    //         if ( a.name > b.name )
    //           return 1;
    //         return 0;
    //     });
    // });

    // 添加“添加文件”的按钮，
    uploader.addButton({
        id: '#filePicker2' + $initOpts.index,
        label: '继续添加'
    });

    uploader.on('ready', function () {
        window.uploader = uploader;
    });

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {
        var $li = $('<li id="' + file.id + '">' +
            '<input type="hidden" name="' + $initOpts.imgUrlName + '" id="' + $initOpts.imgUrlName + '_' + file.id + '" />' +
            '<p class="title">' + file.name + '</p>' +
            '<p class="imgWrap"></p>' +
            '<p class="progress"><span></span></p>' +
            '</li>'),

            $btns = $('<div class="file-panel">' +
                '<span class="cancel">删除</span>' +
                '<span class="rotateRight">向右旋转</span>' +
                '<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>'),

            showError = function (code) {
                switch (code) {
                    case 'exceed_size':
                        text = '文件大小超出';
                        break;

                    case 'interrupt':
                        text = '上传暂停';
                        break;

                    default:
                        text = '上传失败，请重试';
                        break;
                }

                $info.text(text).appendTo($li);
            };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            // @todo lazyload
            $wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                var img;

                if (error) {
                    $wrap.text('不能预览');
                    return;
                }

                if (isSupportBase64) {
                    img = $('<img src="' + src + '">');
                    $wrap.empty().append(img);
                } else {
                    $.ajax($initOpts.apiUrl + '/PreviewImage', {
                        method: 'POST',
                        data: src,
                        dataType: 'json'
                    }).done(function (response) {
                        if (response.code == 1 && response.rs.filePath) {
                            img = $('<img src="' + response.rs.filePath + '">');
                            $wrap.empty().append(img);
                        } else {
                            $wrap.text("预览出错");
                        }
                    });
                }
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            //console.log("cur:" + cur + ",prev:" + prev);
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                //$li.off('mouseenter mouseleave');
                //$btns.remove();
            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                //console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                $info.remove();
                $prgress.css('display', 'block');
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $prgress.hide().width(0);
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.on('mouseenter', function () {
            $btns.stop().animate({ height: 30 });
        });

        $li.on('mouseleave', function () {
            $btns.stop().animate({ height: 0 });
        });

        $btns.on('click', 'span', function () {
            var index = $(this).index(),
                deg;
            switch (index) {
                case 0:
                    {
                        $.messager.confirm('确认', "你确认要删除该图片吗？", function (r) {
                            if (r) {
                                $.ajax({
                                    type: "POST",
                                    url: $initOpts.apiUrl + "/DeleteFile",
                                    data: { "": $("#" + $initOpts.imgUrlName + "_" + file.id).val() },
                                    success: function (data) {
                                        if (data.code != undefined) {
                                            if (data.code == 1) {
                                                uploader.removeFile(file);
                                            }
                                            else {
                                                $.messager.alert('提示', data.msg, 'error');
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
                    return;

                case 1:
                    file.rotation += 90;
                    break;

                case 2:
                    file.rotation -= 90;
                    break;
            }

            if (supportTransition) {
                deg = 'rotate(' + file.rotation + 'deg)';
                $wrap.css({
                    '-webkit-transform': deg,
                    '-mos-transform': deg,
                    '-o-transform': deg,
                    'transform': deg
                });
            } else {
                $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                // use jquery animate to rotation
                // $({
                //     rotation: rotation
                // }).animate({
                //     rotation: file.rotation
                // }, {
                //     easing: 'linear',
                //     step: function( now ) {
                //         now = now * Math.PI / 180;

                //         var cos = Math.cos( now ),
                //             sin = Math.sin( now );

                //         $wrap.css( 'filter', "progid:DXImageTransform.Microsoft.Matrix(M11=" + cos + ",M12=" + (-sin) + ",M21=" + sin + ",M22=" + cos + ",SizingMethod='auto expand')");
                //     }
                // });
            }


        });

        $li.appendTo($queue);
    }

    // 负责view的销毁
    function removeFile(file) {
        var $li = $('#' + file.id);

        delete percentages[file.id];
        updateTotalProgress();
        $li.off().find('.file-panel').off().end().remove();
    }

    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;


        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        if (state === 'ready') {
            text = '选中' + fileCount + '张图片。';
        } else if (state === 'confirm') {
            stats = uploader.getStats();
            if (stats.uploadFailNum) {
                var uploadedNum = stats.successNum > fileCount ? fileCount : stats.successNum;
                text = '已成功上传' + uploadedNum + '张照片至XX相册，' +
                    stats.uploadFailNum + '张照片上传失败，<a class="retry" href="#">重新上传</a>失败图片或<a class="ignore" href="#">忽略</a>'
            }

        } else {
            stats = uploader.getStats();
            //console.log('updateStatus,stats:' + JSON.stringify(stats) + ",fileCount:" + fileCount);
            if (fileCount > 0) {
                if (fileCount > 1) {
                    var uploadedNum = stats.successNum > fileCount ? fileCount : stats.successNum;
                    text = '共' + fileCount + '张（' +
                        WebUploader.formatSize(fileSize) +
                        '），已上传' + uploadedNum + '张';

                    if (stats.uploadFailNum) {
                        text += '，失败' + stats.uploadFailNum + '张';
                    }
                }
                else
                {
                    text = '上传成功（' + WebUploader.formatSize(fileSize) + '）';

                    if (stats.uploadFailNum) {
                        text += '很抱歉！上传失败，请重新上传';
                    }
                }
            }
            if (stats.cancelNum > 0) {
                if ($initOpts.fileNumLimit > fileCount) {
                    $statusBar.find('.btns').show();
                }
                else {
                    $statusBar.find('.btns').hide();
                }
            }
        }

        $info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }

        $upload.removeClass('state-' + state);
        $upload.addClass('state-' + val);
        state = val;
        //console.log('setState,state:' + state);
        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');
                $('#filePicker2' + $initOpts.index).removeClass('element-invisible');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':

                $statusBar.find('.btns').show();

                $('#filePicker2' + $initOpts.index).addClass('element-invisible');
                $progress.show();
                $upload.text('暂停上传');
                break;

            case 'paused':
                $progress.show();
                $upload.text('继续上传');
                break;

            case 'confirm':
                $progress.hide();
                $('#filePicker2' + $initOpts.index).removeClass('element-invisible');
                $upload.text('开始上传');

                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                if ($initOpts.fileNumLimit == fileCount) {
                    $statusBar.find('.btns').hide();
                }
                else {
                    $statusBar.find('.btns').show();
                }

                stats = uploader.getStats();
                if (stats.successNum) {
                    //alert('上传成功');
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        updateStatus();
    }

    uploader.onUploadProgress = function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        $percent.css('width', percentage * 100 + '%');
        percentages[file.id][1] = percentage;
        updateTotalProgress();
    };

    uploader.onFileQueued = function (file) {
        fileCount++;
        fileSize += file.size;

        if (fileCount >= 1) {
            $placeHolder.addClass('element-invisible');
            $statusBar.show();
        }

        addFile(file);
        setState('ready');
        updateTotalProgress();
    };

    uploader.onFileDequeued = function (file) {
        fileCount--;
        fileSize -= file.size;

        if (!fileCount) {
            setState('pedding');
        }

        removeFile(file);
        updateTotalProgress();

    };

    uploader.on('all', function (type) {
        var stats;
        switch (type) {
            case 'uploadFinished':
                setState('confirm');
                break;

            case 'startUpload':
                setState('uploading');
                break;

            case 'stopUpload':
                setState('paused');
                break;

        }
    });

    uploader.onError = function (code) {
        $.messager.alert('提示', code, 'info');
    };

    // 文件上传成功，给item添加成功class, 用样式标记上传成功。
    uploader.on('uploadSuccess', function (file, response) {
        //console.log("uploadSuccess,file:" + file.id + ",name:" + file.name + ",response:" + JSON.stringify(response));
        if (response.code == 1 && response.rs.filePath) {
            //console.log(response.rs.id+':' + response.rs.filePath);
            $("#" + $initOpts.imgUrlName + "_" + response.rs.id).val(response.rs.filePath);
        }

    });

    // 文件上传失败，给item添加成功class, 用样式标记上传完成。
    uploader.on('uploadError', function (file, response) {
        //console.log("uploadError,file:" + file.id + ",name:" + file.name + ",response:" + JSON.stringify(response));

    });
    // 文件上传完成，给item添加成功class, 用样式标记上传完成。
    uploader.on('uploadComplete', function (file) {
        //console.log("uploadComplete,file:" + file.id + ",name:" + file.name);

    });

    $upload.on('click', function () {
        if ($(this).hasClass('disabled')) {
            return false;
        }

        if (state === 'ready') {
            uploader.upload();
        } else if (state === 'paused') {
            uploader.upload();
        } else if (state === 'uploading') {
            uploader.stop();
        }
    });

    $info.on('click', '.retry', function () {
        uploader.retry();
    });

    $info.on('click', '.ignore', function () {
        $.messager.alert('提示', 'todo', 'info');
    });

    $upload.addClass('state-' + state);

    initImages();
    function getImageName(imgPath)
    {
        if (!imgPath || imgPath.length < 1) return imgPath;
        return imgPath.substring(imgPath.lastIndexOf('/') + 1);
    }
    /*
    
     */
    function initImages() {
        var imgFiles = initOpts.fileDatas;
        if (!imgFiles || imgFiles == null || imgFiles.length == 0) {
            updateTotalProgress();
            return;
        }
        fileCount = imgFiles.length;
        if (fileCount >= 1) {
            $placeHolder.addClass('element-invisible');
            $statusBar.show();
            if ($initOpts.fileNumLimit == fileCount) {
                $statusBar.find('.btns').hide();
            }
        }
        
        fileSize = 0;
        $(imgFiles).each(function (i) {
            var fileData = this;
            fileSize += fileData.fileSize;
            var fileId = 'WU_FILE_' + i;
            var $li = $('<li id="' + fileId + '">' +
                '<input type="hidden" name="' + $initOpts.imgUrlName + '" id="' + $initOpts.imgUrlName + '_' + fileId + '" value="' + fileData.filePath + '" />' +
                '<p class="title">' + getImageName(fileData.filePath) + '</p>' +
                '<p class="imgWrap nailthumb-container">' +
                '<img src="' + uploadUrl + "/" + fileData.filePath + '" /></p>' +
                '<p class="progress"><span></span></p>' +
                '</li>'),

                $btns = $('<div class="file-panel">' +
                    '<span class="cancel">删除</span></div>').appendTo($li),
                $prgress = $li.find('p.progress span'),
                $wrap = $li.find('p.imgWrap'),
                $info = $('<p class="error"></p>'),

                showError = function (code) {
                    switch (code) {
                        case 'exceed_size':
                            text = '文件大小超出';
                            break;

                        case 'interrupt':
                            text = '上传暂停';
                            break;

                        default:
                            text = '上传失败，请重试';
                            break;
                    }

                    $info.text(text).appendTo($li);
                };

            $li.on('mouseenter', function () {
                $btns.stop().animate({ height: 30 });
            });

            $li.on('mouseleave', function () {
                $btns.stop().animate({ height: 0 });
            });

            $btns.on('click', 'span', function () {
                var index = $(this).index(),
                    deg;
                switch (index) {
                    case 0:
                        {
                            //console.log("fileId:" + $initOpts.imgUrlName + "_" + fileId);
                            $.messager.confirm('确认', "你确认要删除该图片吗？", function (r) {
                                if (r) {
                                    $.ajax({
                                        type: "POST",
                                        url: $initOpts.apiUrl + "/DeleteFile",
                                        data: { "": $("#" + $initOpts.imgUrlName + "_" + fileId).val() },
                                        success: function (data) {
                                            if (data.code != undefined) {
                                                if (data.code == 1) {
                                                    removeImgFile(fileData, i);
                                                }
                                                else {
                                                    $.messager.alert('提示', data.msg, 'error');
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
                        return;

                    case 1:
                        //file.rotation += 90;
                        break;

                    case 2:
                        //file.rotation -= 90;
                        break;
                }

                if (supportTransition) {
                    deg = 'rotate(' + file.rotation + 'deg)';
                    $wrap.css({
                        '-webkit-transform': deg,
                        '-mos-transform': deg,
                        '-o-transform': deg,
                        'transform': deg
                    });
                } else {
                    $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                    // use jquery animate to rotation
                    // $({
                    //     rotation: rotation
                    // }).animate({
                    //     rotation: file.rotation
                    // }, {
                    //     easing: 'linear',
                    //     step: function( now ) {
                    //         now = now * Math.PI / 180;

                    //         var cos = Math.cos( now ),
                    //             sin = Math.sin( now );

                    //         $wrap.css( 'filter', "progid:DXImageTransform.Microsoft.Matrix(M11=" + cos + ",M12=" + (-sin) + ",M21=" + sin + ",M22=" + cos + ",SizingMethod='auto expand')");
                    //     }
                    // });
                }


            });
            $li.append('<span class="success"></span>');
            $li.appendTo($queue);
        });
        var text = '共' + fileCount + '张（' +
            WebUploader.formatSize(fileSize) +
            '），已上传' + fileCount + '张';
        $info.html(text);
    }
    // 负责view的销毁
    function removeImgFile(fileData, index) {
        fileCount = fileCount - 1;
        fileSize = fileSize - fileData.fileSize;
        var fileId = 'WU_FILE_' + index;
        var text = '共' + fileCount + '张（' +
            WebUploader.formatSize(fileSize) +
            '），已上传' + fileCount + '张';
        $info.html(text);
        var $li = $('#' + fileId);
        $li.off().find('.file-panel').off().end().remove();
        if (fileCount == 0)
        {
            $placeHolder.removeClass('element-invisible');
            $statusBar.hide();
            uploader.refresh();
        }
        else if($initOpts.fileNumLimit > fileCount) {
            $statusBar.find('.btns').show();
        }
    }
}

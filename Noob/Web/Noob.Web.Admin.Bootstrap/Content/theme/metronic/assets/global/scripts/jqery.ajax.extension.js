(function ($) {
    //首先备份下jquery的ajax方法
    var $ajax = $.ajax;
    //重写jquery的ajax方法
    $.ajax = function (opt) {
        //备份opt中error和success方法
        var fn = {
            error: function (jqXHR, textStatus, errorThrown) {
                showMessage('很抱歉！保存失败，请重新提交！', false);
            },
            success: function (data, textStatus, jqXHR) {
                //showDebugMsg('data:', data, 'textStatus:', textStatus, 'jqXHR:', jqXHR);
                if (jqXHR) {
                    var contentType = jqXHR.getResponseHeader("Content-Type");
                    //showDebugMsg('contentType:', contentType);
                }
            }
        }
        if (opt.error) {
            fn.error = opt.error;
        }
        if (opt.success) {
            fn.success = opt.success;
        }
        //扩展增强处理
        var $opt = $.extend(opt, {
            error: function (jqXHR, textStatus, errorThrown) {
                //错误方法增强处理
                fn.error(jqXHR, textStatus, errorThrown);
            },
            success: function (data, textStatus,jqXHR) {
                //成功回调方法增强处理
                fn.success(data, textStatus,jqXHR);
            },
            beforeSend: function (jqXHR) {
                //提交前回调方法
                startPageLoading();
            },
            complete: function (jqXHR, textStatus) {
                //请求完成后回调函数 (请求成功或失败之后均调用)。
                stopPageLoading();
            }
        });
        return $ajax($opt);
    };

    //定义jquery的ajaxPost方法
    $.ajaxPost = function (opt) {
        //备份opt中error和success方法
        var fn = {
            error: function (jqXHR, textStatus, errorThrown) {
                showMessage('很抱歉！保存失败，请重新提交！', false);
            },
            handleSuccess: function (data)
            {
                showMessage('操作成功。', true);
            },
            success: function (data, textStatus, jqXHR) {
                var isJson = false;
                if (jqXHR) {
                    var contentType = jqXHR.getResponseHeader("Content-Type");
                    var jsonType = /\bjson\b/;
                    isJson = jsonType.test(contentType);
                    //showDebugMsg('contentType:', contentType, 'isJson:', isJson);
                }
                if (isJson) {
                    if (data.Code != undefined) {
                        if (data.Code == 1) {
                            this.handleSuccess(data);
                        }
                        else {
                            showMessage(data.Msg, false);
                        }
                    } else {
                        showMessage('系统错误，请联系管理人员！', false);
                    }
                }
            }
        }
        if (opt.error) {
            fn.error = opt.error;
        }
        if (opt.success) {
            fn.success = opt.success;
        }
        if (opt.handleSuccess) {
            fn.handleSuccess = opt.handleSuccess;
        }
        //扩展增强处理
        var $opt = $.extend(opt, {
            type: "POST",
            error: function (jqXHR, textStatus, errorThrown) {
                //错误方法增强处理
                fn.error(jqXHR, textStatus, errorThrown);
            },
            success: function (data, textStatus,jqXHR) {
                //成功回调方法增强处理
                fn.success(data, textStatus,jqXHR);
            },
            beforeSend: function (jqXHR) {
                //提交前回调方法
                startPageLoading();
            },
            complete: function (jqXHR, textStatus) {
                //请求完成后回调函数 (请求成功或失败之后均调用)。
                stopPageLoading();
            }
        });
        return $ajax($opt);
    };

})(jQuery);
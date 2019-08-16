layui.define(function (exports) {

    var config = {
        pageTabs: true,       // 是否开启多标签
        ajax: function (url, options, callFun, errCallback) {
            $.ajax(url, {
                data: options,
                async: true,
                dataType: 'json', //服务器返回json格式数据
                type: 'post', //HTTP请求类型
                timeout: 50 * 1000, //超时时间设置为50秒；
                success: function (data) {
                    callFun(data);
                },
                error: function (data) {
                    errCallback ? errCallback(data) : alert(data);
                }
            });
        },
        getUrlParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]);
            return null;
        },
        SetSession: function (key, options) {
            localStorage.setItem(key, JSON.stringify(options));
        },
        GetSession: function (key) {
            var obj = localStorage.getItem(key);
            if (obj != null) {
                return JSON.parse(obj);
            }
            return null;
        },
        SessionRemove: function (key) {
            localStorage.removeItem(key);
        },
    };
    if (localStorage.getItem("config_pageTabs") == "false")
        config.pageTabs = false; 

    exports('config', config);
});

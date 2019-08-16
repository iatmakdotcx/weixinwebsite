layui.define(['config', 'admin', 'layer', 'element', 'form'], function (exports) {
    var $ = layui.$;
    var config = layui.config;
    var admin = layui.admin;
    var element = layui.element;

    var index_events = {
        flexible: function (e) {  // 折叠侧导航
            var fb = $('.layui-layout-admin');
            if (fb.hasClass('admin-nav-mini')) {
                fb.removeClass('admin-nav-mini');
            } else {
                fb.addClass('admin-nav-mini');
            }
          //  admin.onResize();
        },
        refresh: function () {  // 刷新主体部分
            location.reload();
        },
        messages: function () {
            index.popupRight('admin/components/message');
        },
        fullScreen: function (e) {  // 全屏
            var ac = 'layui-icon-screen-full', ic = 'layui-icon-screen-restore';
            var ti = $(this).find('i');

            var isFullscreen = document.fullscreenElement || document.msFullscreenElement || document.mozFullScreenElement || document.webkitFullscreenElement || false;
            if (isFullscreen) {
                var efs = document.exitFullscreen || document.webkitExitFullscreen || document.mozCancelFullScreen || document.msExitFullscreen;
                if (efs) {
                    efs.call(document);
                } else if (window.ActiveXObject) {
                    var ws = new ActiveXObject('WScript.Shell');
                    ws && ws.SendKeys('{F11}');
                }
                ti.addClass(ac).removeClass(ic);
            } else {
                var el = document.documentElement;
                var rfs = el.requestFullscreen || el.webkitRequestFullscreen || el.mozRequestFullScreen || el.msRequestFullscreen;
                if (rfs) {
                    rfs.call(el);
                } else if (window.ActiveXObject) {
                    var ws = new ActiveXObject('WScript.Shell');
                    ws && ws.SendKeys('{F11}');
                }
                ti.addClass(ic).removeClass(ac);
            }
        },
        profile: function () {  //个人信息
            
        },
        chgpwd: function () {  //修改密码
            index.popupRight('admin/components/chgpassword');
        },
        logout: function () {  //注销登录
            location.href = "admin/login?logout=1";
        },
        theme: function () {  // 设置主题
            index.popupRight('admin/components/theme');
        },
        locale: function () {
            index.popupRight('admin/components/locale', "100px");
        },



        // 左滑动tab
        leftPage: function () {
            admin.rollPage("left");
        },
        // 右滑动tab
        rightPage: function () {
            admin.rollPage();
        },
        // 关闭当前选项卡
        closeThisTabs: function () {
            var $title = $('.layui-layout-admin .layui-body .layui-tab .layui-tab-title');
            if ($title.find('li').first().hasClass('layui-this')) {
                return;
            }
            $title.find('li.layui-this').find(".layui-tab-close").trigger("click");
            $(this).parent().removeClass("layui-show");
        },
        // 关闭其他选项卡
        closeOtherTabs: function () {
            $('.layui-layout-admin .layui-body .layui-tab .layui-tab-title li:gt(0):not(.layui-this)').find(".layui-tab-close").trigger("click");
            $(this).parent().removeClass("layui-show");
        },
        // 关闭所有选项卡
        closeAllTabs: function () {
            $('.layui-layout-admin .layui-body .layui-tab .layui-tab-title li:gt(0)').find(".layui-tab-close").trigger("click");
            $(this).parent().removeClass("layui-show");
        },
        // 关闭所有弹窗
        closeDialog: function () {
            layer.closeAll('page');
        }

    }
    var index = {
        init: function () {
            //绑定按钮事件
            $('body').on('click', '*[idx-event]', function () {
                var event = $(this).attr('idx-event');
                var te = index_events[event];
                te && te.call(this, $(this));
            });
            //是否启用选项卡
            if (1||!config.pageTabs) {
                $('.layui-body').html("");
                $(".open-tab").removeClass("open-tab");
            }
            if (location.hash) {
                index.loadurl(location.hash.substr(1));
            } else {
                //加载首页
                index.loadurl("page/home");
            }
            //切换页面事件
            window.onhashchange = function () {
                index.loadurl(location.hash.substr(1));
            }
        },
        reload: function () {
            index.loadurl(location.hash.substr(1));
        },
        // 右侧弹出
        popupRight: function (path, w) {
            var param = new Object();
            param.path = path;
            param.id = 'adminPopupR';
            param.title = false;
            param.anim = 2;
            param.isOutAnim = false;
            param.closeBtn = false;
            param.offset = 'r';
            param.shadeClose = true;
            param.area = w || '336px';
            param.skin = 'layui-layer-adminRight';
            param.end = function () {
                layer.closeAll('tips');
            };
            popupRightIndex = this.open(param);
            return popupRightIndex;
        },
        open: function (param) {
            var sCallBack = param.success;
            param.type = 1;
            param.area = param.area ? param.area : '450px';
            param.offset = param.offset ? param.offset : '120px';
            param.resize ? param.resize : false;
            param.shade ? param.shade : .2;
            param.success = function (layero, index) {
                sCallBack ? sCallBack(layero, index) : '';
                $(layero).children('.layui-layer-content').load(param.path);
            };
            return layer.open(param);
        },
        popupCenter: function (title, url, width, height, fun) {            
            layer.open({
                type: 1,
                title: title,
                shadeClose: false,
                shade: 0.2,
                skin: 'layer-cur-open',
                maxmin: false,
                area: [width, height],               
                zIndex: "1000",
                success : function (layero) {                    
                    $(layero).children('.layui-layer-content').load(url);
                },
                end: function () {
                    if (fun) fun();
                }
            });
        },
        // 页面元素绑定事件监听
        bindEvent: function () {
            $(".layui-side a[lay-href]").click(function () {
                index.openTabWindow(this.innerText, this.getAttribute("lay-href"), this.innerText);
            });
        },
        loadurl: function (url) {
            if (!url) return;
            
            if (url[0] != "/") {
                url = "/admin/" + url;
            }
            layui.layer.load(2);
            $.ajax({
                url: url
                , type: 'get'
                , dataType: 'html'
                , data: {
                    v: layui.cache.version
                }
                , success: function (html) {                    
                    html = '<div>' + html + '</div>';

                    var elemTitle = $(html).find('title')
                        , title = elemTitle.text() || (html.match(/\<title\>([\s\S]*)\<\/title>/) || [])[1];
                    elemTitle.remove();
                    index.render(html, title);
                    layer.closeAll('loading');
                }
                , error: function (e) {
                    layer.closeAll('loading');
                    index.render(e.responseText, e);
                    //view.removeLoad();
                    //if (that.render.isError) {
                    //    return view.error('请求视图文件异常，状态：' + e.status);
                    //};
                    if (e.status === 401) {
                        location.href = "/admin/login"
                    //} else {
                    //    that.render('template/tips/error');
                    }
                    //that.render.isError = true;
                }
            });
        },
        render: function (html, title) {
            $('.layui-body').html(html);
        },
        openTabWindow: function (menuId, menuPath, menuName) {
            layer.load(2);
            if (config.pageTabs) {
                if ($(".layui-body ul.layui-tab-title>li[lay-id='" + menuId + "']").length == 0) {
                    element.tabAdd('admin-pagetabs', {
                        title: menuName,
                        content: '<iframe src="' + menuPath + '" frameborder=\"0\" class=\"layadmin-iframe\" id="' + menuId + '"></iframe>',
                        id: menuId
                    });
                    $("iframe#" + menuId).on("load", function () {
                        layer.closeAll('loading');
                    });
                } else {
                    layer.closeAll('loading');
                }
                // 切换tab关闭表格内浮窗
                $('.layui-table-tips-c').trigger('click');
                element.tabChange('admin-pagetabs', menuId);
            } else {
                var mainiframe = $(".layadmin-iframe");
                if (mainiframe.length == 0) {
                    mainiframe = $("<iframe frameborder=\"0\" class=\"layadmin-iframe\"></iframe>")
                    $(".layui-body").append(mainiframe);
                    mainiframe.on("load",function () {
                        layer.closeAll('loading');
                    });
                }
                mainiframe.attr("src", menuPath);
            }
        },
        // 关闭选项卡
        closeTab: function (menuId) {
            element.tabDelete('admin-pagetabs', menuId);
        }
    };    
    exports('index', index);
});

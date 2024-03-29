layui.define(['config', 'layer'], function (exports) {
    var config = layui.config;
    var layer = layui.layer;
    var popupRightIndex, popupCenterIndex, popupCenterParam;

    var admin = {
        isRefresh: false,
        // 设置侧栏折叠
        //flexible: function (expand) {
        //    var isExapnd = $('.layui-layout-admin').hasClass('admin-nav-mini');
        //    if (isExapnd == !expand) {
        //        return;
        //    }
        //    if (expand) {
        //        $('.layui-layout-admin').removeClass('admin-nav-mini');
        //    } else {
        //        $('.layui-layout-admin').addClass('admin-nav-mini');
        //    }
        //    admin.onResize();
        //},
        // 设置导航栏选中
        activeNav: function (url) {
            $('.layui-layout-admin .layui-side .layui-nav .layui-nav-item .layui-nav-child dd').removeClass('layui-this');
            $('.layui-layout-admin .layui-side .layui-nav .layui-nav-item').removeClass('layui-this');
            if (url && url != '') {
                $('.layui-layout-admin .layui-side .layui-nav .layui-nav-item').removeClass('layui-nav-itemed');
                var $a = $('.layui-layout-admin .layui-side .layui-nav a[href="#!' + url + '"]');
                $a.parent('li').addClass('layui-this');  // 一级菜单
                $a.parent('dd').addClass('layui-this');  // 二级菜单
                $a.parent('dd').parent('.layui-nav-child').parent('.layui-nav-item').addClass('layui-nav-itemed');  // 二级菜单
                $a.parent('dd').parent('.layui-nav-child').parent('dd').addClass('layui-nav-itemed');  // 三级菜单
                $a.parent('dd').parent('.layui-nav-child').parent('dd').parent('.layui-nav-child').parent('.layui-nav-item').addClass('layui-nav-itemed');  // 三级菜单
            }
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
            popupRightIndex = admin.open(param);
            return popupRightIndex;
        },
        // 关闭右侧弹出
        closePopupRight: function () {
            layer.close(popupRightIndex);
        },
        // 中间弹出
        popupCenter: function (param) {
            param.id = 'adminPopupC';
            popupCenterParam = param;
            popupCenterIndex = admin.open(param);
            return popupCenterIndex;
        },
        // 关闭中间弹出并且触发finish回调
        finishPopupCenter: function () {
            layer.close(popupCenterIndex);
            popupCenterParam.finish ? popupCenterParam.finish() : '';
        },
        // 关闭中间弹出
        closePopupCenter: function () {
            layer.close(popupCenterIndex);
        },
        // 封装layer.open
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
        // 滑动选项卡
        rollPage: function (d) {
            var $tabTitle = $('.layui-layout-admin .layui-body .layui-tab .layui-tab-title');
            var left = $tabTitle.scrollLeft();
            if ('left' === d) {
                $tabTitle.scrollLeft(left - 120);
            } else if ('auto' === d) {
                var autoLeft = 0;
                $tabTitle.children("li").each(function () {
                    if ($(this).hasClass('layui-this')) {
                        return false;
                    } else {
                        autoLeft += $(this).outerWidth();
                    }
                });
                // console.log(autoLeft);
                $tabTitle.scrollLeft(autoLeft - 47);
            } else {
                $tabTitle.scrollLeft(left + 120);
            }
        },
        //refresh: function () {
        //    admin.isRefresh = true;
        //    Q.refresh();
        //},
        // 判断是否为json
        parseJSON: function (str) {
            if (typeof str == 'string') {
                try {
                    var obj = JSON.parse(str);
                    if (typeof obj == 'object' && obj) {
                        return obj;
                    }
                } catch (e) {
                }
            }
        },
        refresh: function () {
            if (config.pageTabs) {
                var ifif = $(".layui-body .layui-tab-item.layui-show>.layadmin-iframe");
                if (ifif.length > 0) {
                    this.showLoading();
                    ifif.attr("src", ifif.attr("src"));
                }
            } else {
                var ifif = $(".layui-body>.layadmin-iframe");
                if (ifif.length > 0) {
                    this.showLoading();
                    ifif.attr("src", ifif.attr("src"));
                }
            }
        }
    };

    // ewAdmin提供的事件
    admin.events = {

        
    };

    //// 所有ew-event
    //$('body').on('click', '*[ew-event]', function () {
    //    var event = $(this).attr('ew-event');
    //    var te = admin.events[event];
    //    te && te.call(this, $(this));
    //});

    //// 移动设备遮罩层点击事件
    //$('.site-mobile-shade').click(function () {
    //    admin.flexible(true);
    //});

    //// 侧导航折叠状态下鼠标经过显示提示
    //$('body').on('mouseenter', '.layui-layout-admin.admin-nav-mini .layui-side .layui-nav .layui-nav-item>a', function () {
    //    var tipText = $(this).find('cite').text();
    //    if (document.body.clientWidth > 750) {
    //        layer.tips(tipText, this);
    //    }
    //}).on('mouseleave', '.layui-layout-admin.admin-nav-mini .layui-side .layui-nav .layui-nav-item>a', function () {
    //    layer.closeAll('tips');
    //});

    //// 侧导航折叠状态下点击展开
    //$('body').on('click', '.layui-layout-admin.admin-nav-mini .layui-side .layui-nav .layui-nav-item>a', function () {
    //    if (document.body.clientWidth > 750) {
    //        layer.closeAll('tips');
    //        admin.flexible(true);
    //    }
    //});

    //// 所有lay-tips处理
    //$('body').on('mouseenter', '*[lay-tips]', function () {
    //    var tipText = $(this).attr('lay-tips');
    //    var dt = $(this).attr('lay-direction');
    //    layer.tips(tipText, this, {tips: dt || 1, time: -1});
    //}).on('mouseleave', '*[lay-tips]', function () {
    //    layer.closeAll('tips');
    //});

    //$(".layui-tab-title").on("mousedown", function (a) {
    //    if (a.button == 1 && $(a.target).is("li") && $(a.target).find(".layui-tab-close").is(":visible")) {
    //        $(a.target).find(".layui-tab-close").trigger("click");
    //    }
    //});


    exports('admin', admin);
});

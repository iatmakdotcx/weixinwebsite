layui.config({
    base: '/layui/mod/'
});
layui.use(['jquery', 'form', 'config'], function () {
    var form = layui.form,
        $ = layui.jquery,
        os = layui.config;
    $(".layui-btn-danger").click(function () {
        document.getElementById("forms").reset();
    });
    //清空token
    os.SessionRemove('ADMIN_ACCESS_TOKEN');
    form.on('submit(loginsub)', function (data) {
        //var crypt = new JSEncrypt();
        //crypt.setPrivateKey(data.field.privateKey);
        //var enc = crypt.encrypt(data.field.password);
        //$("#password").val(enc);
        //data.field.password = enc;
        layui.layer.load(2);
        var btns = $(".layui-btn-normal");
        btns.html('<i class="layui-icon layui-anim layui-anim-rotate layui-anim-loop"></i>');
        btns.attr('disabled', 'disabled');
        os.ajax('/api/admin/login', data.field, function (res) {
            layer.closeAll('loading');
            if (res.ok) {
                os.SetSession('ADMIN_ACCESS_TOKEN', res.data);
                setTimeout(function () {
                    var rurl = os.getUrlParam('ReturnUrl');
                    if (!rurl) {
                        window.location.href = '/admin/index';
                    }
                    else {
                        window.location.href = rurl;
                    }
                }, 1000);
            } else {
                $(".login-tip span").html(res.message);
                $("#password").val('');
                $(".login-tip").animate({ 'height': '30px' });
                setTimeout(function () {
                    $(".login-tip").animate({ 'height': 0 });
                    $(".login-tip span").html('');
                }, 2500);
            }
            btns.attr('disabled', false);
            setTimeout(function () {
                btns.html('登录');
            }, 1000);
        }, function () {
            layer.closeAll('loading');
            alert("登录失败！")
        });
        return false;
    });
    $(window).resize(
        bodysize
    );
    bodysize();
    function bodysize() {
        $("body").height($(window).height());
    }
});
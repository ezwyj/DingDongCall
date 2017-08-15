define(['common', 'util','tlayer'], function ($, util) {
    var rootUrl = OP_CONFIG.rootUrl;

    $("#phone").focus();

    $('#ok').on('click', function () {
        var phone = $('#phone').val().trim();
        var myreg = /^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$/; 
        if (!myreg.test(phone)) {
            $.tips('不是有效的的11位手机号', 8);
            return false;
        }
        $.post(rootUrl + 'Home/SavePhone', {
            userId: $('#userid').val(),
            phone:phone
        })
        .success(function (res) {
            if (res.State) {
                $.tips('操作成功,应用开通',8);
                $.tlayer('close');
                location.href = 'linglong://back';

            } else {
                $.tips('操作失败：' + res.Msg, 0);
            }
        });
    });
});
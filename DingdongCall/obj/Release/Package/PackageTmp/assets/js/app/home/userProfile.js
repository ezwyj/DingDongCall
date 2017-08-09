define(['common', 'util','tlayer'], function ($, util) {
    var rootUrl = OP_CONFIG.rootUrl;

    $('#ok').on('click', function () {
        var phone = $('#phone').val();
        if (!(/^(13[0-9]|15[012356789]|17[0678]|18[0-9]|14[57])[0-9]{8}$/.test(phone))) {
            $.tips('不是完整的11位手机号', 5);
            return false;
        }
        $.post(rootUrl + 'Home/SavePhone', {
            userId: $('#userid').val(),
            phone:phone
        })
        .success(function (res) {
            if (res.State) {
                $.tips('操作成功',5);
                $.tlayer('close');

            } else {
                $.tips('操作失败：' + res.Msg, 0);
            }
        });
    });
});
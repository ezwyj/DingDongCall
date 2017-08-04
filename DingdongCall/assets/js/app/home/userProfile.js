define(['common', 'util','tlayer'], function ($, util) {
    var rootUrl = OP_CONFIG.rootUrl;

    $('#ok').on('click', function () {
        var phone = $('#phone').val();
        if (!(/^1[3|4|5|8][0-9]\d{4,8}$/.test(phone))) {
            $.tips('不是完整的11位手机号或者正确的手机号前七位', 2);
            return false;
        }
        $.post(rootUrl + 'Home/UserProfile', {
            userId: $('#userid').val(),
            phone:phone
        })
        .success(function (res) {
            if (res.State) {
                $.tips('操作成功', 3);
                $.tlayer('close');

            } else {
                $.tips('操作失败：' + res.Msg, 0);
            }
        });
    });
});
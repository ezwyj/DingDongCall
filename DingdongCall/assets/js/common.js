/**
 * 公用js文件
 */
define('common', ['jquery','util','tlayer'], function ($,  util) {
    //导航栏选中
    $('#menu-' + OP_CONFIG.module + '-' + OP_CONFIG.page).addClass('active');

    //设置弹出框图片路径
    $.tlayer('global', { imgPath: OP_CONFIG.rootUrl + 'assets/img/', theme: 'blue' });


    //禁用ajax缓存
    $.ajaxSetup({ cache: false });

    //设置页面最小高度
    var minHeight = $(window).height() - $('#footer').outerHeight(true);
    $('#main').css('min-height', minHeight);
    
    //panel展开/收起
    $(document).on('click', '.panel[data-expand] .panel-heading', function () {
        var panel = $(this).parent();
        var expand = panel.attr('data-expand');

        panel.children('.panel-body').slideToggle('fast', function () {
            if (expand == 'Y') {
                panel.attr('data-expand', 'N');
            } else {
                panel.attr('data-expand', 'Y');
            }
        });
    });

    $(document).on('click', '.panel[data-expand] .btn', function (e) {
        e.stopPropagation();
    });    

   
   
    Date.prototype.toCSharpTime = function () {
        return '/Date(' + this.getTime() + ')/';
    }



    return $;
});
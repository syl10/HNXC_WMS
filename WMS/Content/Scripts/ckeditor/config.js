/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';

    config.language = 'zh-cn'; //语言

    config.width = 800; //宽

    config.height = 1200; //高

    config.resize_enabled = false; // 取消 “拖拽以改变尺寸”功能

    config.toolbarLocation = 'top'; //工具栏的位置 可选：bottom(下方)

    config.skin = 'office2003'; // 编辑器样式，有三种：'kama'（默认）、'office2003'、'v2'

    config.toolbarStartupExpanded = true; //工具栏默认是否展开

    config.uiColor = '#CD8500'; // 背景颜色

//    config.uiColor = '#00ccff'; // 背景颜色

    config.disableObjectResizing = true; //是否开启 图片和表格 的改变大小的功能 

    config.font_names = 'Arial;Times New Roman;Verdana';
//    config.keystrokes = [
//                            [CKEDITOR.ALT + 121 /*F10*/, 'toolbarFocus'], //获取焦点
//                            [CKEDITOR.ALT + 122 /*F11*/, 'elementsPathFocus'], //元素焦点
//                            [CKEDITOR.SHIFT + 121 /*F10*/, 'contextMenu'], //文本菜单
//                            [CKEDITOR.CTRL + 90 /*Z*/, 'undo'], //撤销
//                            [CKEDITOR.CTRL + 89 /*Y*/, 'redo'], //重做
//                            [CKEDITOR.CTRL + CKEDITOR.SHIFT + 90 /*Z*/, 'redo'], //重做
//                            [CKEDITOR.CTRL + 76 /*L*/, 'link'], //链接
//                            [CKEDITOR.CTRL + 66 /*B*/, 'bold'], //粗体
//                            [CKEDITOR.CTRL + 73 /*I*/, 'italic'], //斜体
//                            [CKEDITOR.CTRL + 85 /*U*/, 'underline'], //下划线
//                            [CKEDITOR.ALT + 109 /*-*/, 'toolbarCollapse']
//]
        //工具栏
        config.toolbar_Full = [
                                ['Source', 'Save', 'NewPage', 'Preview', 'Templates'],
                                ['Cut', 'Copy', 'Paste','Print'],
                                ['Undo', 'Redo', 'Find', 'Replace', 'SelectAll', 'RemoveFormat'],
                                ['Form', 'Checkbox', 'Radio', 'TextField', 'Textarea', 'Select', 'Button', 'ImageButton', 'HiddenField'],
                                '/',
                                ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
                                ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
                                ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'],
                                ['Link', 'Unlink', 'Anchor'],
                                ['Image', 'Flash', 'Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak'],
                                '/',
                                ['Styles', 'Format', 'Font', 'FontSize'],
                                ['TextColor', 'BGColor']
                               ]
        //页面载入时，编辑框是否立即获得焦点     
        config.startupFocus = true;

        //在 CKEditor 中集成 CKFinder
        var ckfinderPath = "/Content/Scripts"; //ckfinder路径
        config.filebrowserBrowseUrl = ckfinderPath + '/ckfinder/ckfinder.html';
        config.filebrowserImageBrowseUrl = ckfinderPath + '/ckfinder/ckfinder.html?type=Images';
        config.filebrowserFlashBrowseUrl = ckfinderPath + '/ckfinder/ckfinder.html?type=Flash';
        config.filebrowserUploadUrl = ckfinderPath + '/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
        config.filebrowserImageUploadUrl = ckfinderPath + '/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images';
        config.filebrowserFlashUploadUrl = ckfinderPath + '/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Flash';
        //在 CKEditor提交时验证不通过问题
        config.protectedSource.push( /<\s*iframe[\s\S]*?>/gi ) ; // <iframe> tags. 
        config.protectedSource.push( /<\s*frameset[\s\S]*?>/gi ) ; // <frameset> tags. 
        config.protectedSource.push( /<\s*frame[\s\S]*?>/gi ) ; // <frame> tags. 
        config.protectedSource.push( /<\s*script[\s\S]*?\/script\s*>/gi ) ; // <SCRIPT> tags. 
        config.protectedSource.push( /<%[\s\S]*?%>/g ) ; // ASP style server side code 
        config.protectedSource.push( /<\?[\s\S]*?\?>/g ) ; // PHP style server side code 
        config.protectedSource.push( /(<asp:[^\>]+>[\s|\S]*?<\/asp:[^\>]+>)|(<asp:[^\>]+\/>)/gi ) ; 
};

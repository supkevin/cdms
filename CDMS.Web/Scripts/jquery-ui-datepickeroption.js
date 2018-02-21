﻿jQuery(function ($) {
    $.datepicker.regional['zh-TW'] = {
        closeText: '關閉',
        prevText: '&#x3C;上月',
        nextText: '下月&#x3E;',
        currentText: '今天',
        monthNames: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        monthNamesShort: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月'],
        dayNames: ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'],
        dayNamesShort: ['週日', '週一', '週二', '週三', '週四', '週五', '週六'],
        dayNamesMin: ['日', '一', '二', '三', '四', '五', '六'],
        weekHeader: '週',
        dateFormat: 'yy-mm-dd',
        showMonthAfterYear: true,
        changeMonth: true,
        changeYear: true,
        // yearSuffix: '年',
        showAnim: 'show'
    };
    $.datepicker.setDefaults($.datepicker.regional['zh-TW']);
});

/// <reference path="jquery-1.6.2-vsdoc.js" />
/// <reference path="jquery-ui.js" />
$(document).ready(function () {
    $('*[data-autocomplete-url]')
        .each(function () {
            // 原始寫法
            //$(this).autocomplete({
            //    source: $(this).data("autocomplete-url")
            //});

            // 改良寫法
            var self = $(this); // 目前控制項            
            self.autocomplete({
                source: function (request, response) {
                    // 捉取資料的action
                    let action = self.data("autocomplete-url");
                    $.get(action,
                        { term: request.term },
                        function (data) {
                            response($.map(data, function (item, index) {
                                console.log('item', item);
                                return {
                                    id: index,
                                    label: item,
                                    value: item
                                }
                            }))
                        });
                }// source
            });// self.autocomplete
        });
});
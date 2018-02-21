$(document).ready(function () {
    var globalUserData = [];

    var userOptions = {
        ajax: {          
            data: function () {
                var params = {
                    term: '{{{q}}}'
                };
                //if (gModel.selectedGroup().hasOwnProperty('ContactGroupID')) {
                //    params.GroupID = gModel.selectedGroup().ContactGroupID;
                //}
                return params;
            }
        },
        clearOnEmpty: false,
        preserveSelected: true,
        locale: {
            emptyTitle: '請選擇',
            currentlySelected: '目前選取',
            searchPlaceholder: '關鍵字',
            statusInitialized: '請輸入關鍵字查詢',
            statusNoResults: '查無資料',
        },
        preprocessData: function (data) {
            var contacts = [];
                   
            if (data) {
                $.each(data, function (index, item) {
                    //
                    globalUserData.push(item);

                    contacts.push(
                        {
                            'value': item.Value,
                            'text': item.Value + '-' + item.Label,
                            'data': {
                                'icon': 'icon-person',
                                'subtext': ''
                            },
                            'disabled': false
                        }
                        );
                });
            }
            return contacts;
        },
    };

    $('.selectpicker.after-init.user').each(function (index, value) {
        var self = $(value);

        var selected = self.val();
        self.find('[value != "' + selected + '"]').remove();

        self.selectpicker('refresh');
    });

    $('.selectpicker.user').selectpicker().filter('.with-ajax').ajaxSelectPicker(userOptions);

    $('.selectpicker.user').on('changed.bs.select', function (e) {

    });
});
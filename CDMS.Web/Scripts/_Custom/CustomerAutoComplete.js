$(document).ready(function () {
    var globalCustomerData = [];
    var customerOptions = {
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
            globalCustomerData = [];

            // 清空聯絡人等資料
            ClearCustomer();

            if (data) {
                $.each(data, function (index, item) {
                    //
                    globalCustomerData.push(item);

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

    $('.selectpicker.after-init.customer').each(function (index, value) {
        var self = $(value);       
        var selected = self.val();
        self.find('[value != "' + selected + '"]').remove();

        self.selectpicker('refresh');
    });

    $('.selectpicker.customer').selectpicker().filter('.with-ajax').ajaxSelectPicker(customerOptions);

    $('.selectpicker.customer').on('changed.bs.select', function (e) {
        //console.log('customer');
        var value = $(this).val();

        var wanted = $.grep(globalCustomerData, function (item) {
            //console.log('v', item.Value, item.Value == value);
            return item.Value == value;
        });

        if (wanted.length == 0) {
            ClearCustomer();
            return;
        }

        var source = wanted[0].Source;
        SetCustomer(source);     
    });

    function ClearCustomer(){    
        $('#txtContactPerson').val('');
        $('#txtContactPhone').val('');
        $('#txtShippingAddress').val('');
        $('#txtInvoiceAddress').val('');
    }

    function SetCustomer(info) {
        $('#txtContactPerson').val(info.ContactPerson);
        $('#txtContactPhone').val(info.Telephone1);
        $('#txtShippingAddress').val(info.ShippingAddress);
        $('#txtInvoiceAddress').val(info.InvoiceAddress);
    }
});
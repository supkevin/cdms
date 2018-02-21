// 單價數量計算總價
var Calculator = function (parent) {
    var price = $(parent).find('input[data-key="Price"]').val() || 0;
    var qty = $(parent).find('input[data-key="Qty"]').val() || 0;

    console.log('Calculator', price, qty);

    this.Parent = parent;
    this.Price = price;
    this.Qty = qty;
};

Calculator.prototype = {
    Parent: null,
    Price: 0,
    Qty: 0,
    DoCalculate: function () {
        var num = new Number(this.Price * this.Qty);
        // 四捨五入到整數
        $(this.Parent).find('input[data-key="Amount"]').val(Math.round(num));
    }
};


$('#table').on('keydown', '.form-control', function () {
    var self = $(this);
    var parent = self.closest('tr').first();
    parent.find(".dirty-item").first().val(true);
});//$('#table').on('keydown'

$('#table').on('change', 'select', function () {
    var self = $(this);
    var parent = self.closest('tr').first();
    parent.find(".dirty-item").first().val(true);
});//$('#table').on('keydown'

// 數字欄位才需做
$('#table').on('keyup', '.form-control', function () {
    if ($(this).is('[readonly]')) return;

    // 判斷是否是數字欄位
    if (typeof ($(this).attr('data-val-number')) == "undefined") return;

    var self = $(this);
    if (self.val().trim() == '') self.val('0');
    var v = self.val();
    self.val(parseInt(v));

    var parent = self.closest('tr').first();
    var myCalculator = new Calculator(parent);
    myCalculator.DoCalculate();
});//$('#table').on('keyup'

// 價別下拉選單
$('#table').on('change', 'select[data-key="PriceKindID"]', function () {
    console.log('this-change', $(this));
    var parent = $(this).closest('tr').first();
    var txtProduct = parent.find('input.auto-complete');
    GetOriginalPrice(parent, txtProduct.val() || '');
});

// Discount
$('#table').on('input', 'input[data-key="Discount"]', function () {
    console.log('this-input', $(this));

    //var parent = $(this).closest('tr').first();
    //var txtOriginalPrice = parent.find('input[data-key="OriginalPrice"]');
    //var txtPrice = parent.find('input[data-key="Price"]');

    var discount = $(this).val() || 0;

    console.log('discount', discount);


    //var originalPrice = txtOriginalPrice.val() || 0;

    //txtPrice.val(originalPrice * discount / 100);

    //var myCalculator = new Calculator(parent);
    //myCalculator.DoCalculate();
});

// ExchangeRate
$('input[data-key="ExchangeRate"]').on('input', function () {
    console.log('ExchangeRate', $(this).val());
    var exchangeRate = $(this).val() || 0;

    $('#table > tbody  > tr').each(function (index, myItem) {
        console.log('item', item);
        var item = $(myItem);
        var txtPrice = item.find('input[data-key="Price"]');
        var txtForeignPrice = item.find('input[data-key="ForeignPrice"]');
        var price = txtPrice.val() || 0;

        console.log('txtForeignPrice', txtForeignPrice);

        if (txtForeignPrice) {
            txtForeignPrice.val(price * exchangeRate);
        }
    });
});

// Price
$('#table').on('input', 'input[data-key="Price"]', function () {
    console.log('this-input', $(this));
    var parent = $(this).closest('tr').first();

    var txtOriginalPrice = parent.find('input[data-key="OriginalPrice"]');
    var txtDiscount = parent.find('input[data-key="Discount"]');

    var txtForeignPrice = parent.find('input[data-key="ForeignPrice"]');
    var txtExchangeRate = $('input[data-key="ExchangeRate"]');

    var exchangeRate = txtExchangeRate.val() || 0;
    var price = $(this).val() || 0;
    var originalPrice = txtOriginalPrice.val() || 0;

    console.log('exchangeRate', txtExchangeRate, exchangeRate);
    console.log('price', price);

    // 不一定有國外金額這欄位
    if (txtForeignPrice) {
        txtForeignPrice.val(price * exchangeRate);
    }
    var num = new Number(price / originalPrice * 100);
    txtDiscount.val(num.toFixed(2));

    var myCalculator = new Calculator(parent);
    myCalculator.DoCalculate();
});

// autocomplete 選取
$('#table').on('autocompleteselect', '.auto-complete', function (event, node) {
    console.log('this', $(this));

    // 往後找第一個input
    var parent = $(this).closest('tr').first();
    var txtProductName = parent.find('input.auto-complete-name');
    var source = node.item.source;
    var action = $(this).data("price-url");

    console.log('action' ,action);

    txtProductName.val(source.ProductName);

    GetOriginalPrice(action, parent, source.ProductID);
});//$('#table').on('autocompleteselect'

function Clear(parent) {
    var txtProductName = parent.find('input.auto-complete-name');
    var txtOriginalPrice = parent.find('input[data-key="OriginalPrice"]');
    var txtPrice = parent.find('input[data-key="Price"]');

    txtProductName.val('');
    txtOriginalPrice.val(0);
    txtPrice.val(0);

    var myCalculator = new Calculator(parent);
    myCalculator.DoCalculate();
}// Clear

function GetOriginalPrice(action, parent, product) {

    console.log('GetOriginalPrice');

    var txtOriginalPrice = parent.find('input[data-key="OriginalPrice"]');    
    var txtPrice = parent.find('input[data-key="Price"]');
    var txtDiscount = parent.find('input[data-key="Discount"]');
    var txtPriceKind = parent.find('select[data-key="PriceKindID"]');
   
    var discount = txtDiscount.val() || 0;
    var priceKind = txtPriceKind.val() || '';
    var data = { productID: product, priceKind: priceKind };

    console.log('data', data);

    // 捉單價
    $.get(action, data,
        function (response) {
            console.log('data', response);
            var result = response || 0;
            txtOriginalPrice.val(result);
            txtPrice.val(result * discount / 100);

            var myCalculator = new Calculator(parent);
            myCalculator.DoCalculate();
        });
}//GetProductPrice

$('#table').on('keydown', '.auto-complete', function (event, node) {
    console.log('keydown', $(this));
    // 清空
    var parent = $(this).closest('tr').first();
    Clear(parent);
});//$('#table').on('autocompleteselect'

// 新增的控制項註冊為autocomplete
$(document).on('keydown.autocomplete', '.auto-complete', function () {
    // console.log('document');
    var self = $(this); // 目前控制項
    self.autocomplete({
        source: function (request, response) {
            // 捉取資料的action
            let action = self.data("autocomplete-url");
            $.get(action, { term: request.term },
                function (data) {
                    console.log('autocomplete', data);
                    response($.map(data, function (item, index) {
                        console.log('index', item);

                        return {
                            id: index,
                            label: item.Value + '-' + item.Label,
                            value: item.Value,
                            source: item.Source,
                        }
                    }))
                });
        }// source
    });// self.autocomplete
});//$(document).on(
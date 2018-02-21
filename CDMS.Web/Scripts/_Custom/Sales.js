
$(document).ready(function () {
    // 上下左右
    $('#table').enableCellNavigation();
    //$("#table :input").attr("disabled", "disabled");
    //$("#btnAdd").attr("disabled", "disabled");
    Init();
});//$(document).ready(

function Init() {
    //客戶資料有異動
    $('select[data-key="txtCustomerID"]').on('change', function (e) {
        console.log('txtCustomerID', $(this).val());

        //if ($(this).val() == '') {
        //    $("#table :input").attr("disabled", "disabled");
        //} else {
        //    $("#table :input").removeAttr("disabled");
        //}
    });

    $('#table').on('input', 'input[data-key],select[data-key]', function () {
        console.log('data-key', $(this), $(this).data('key'));

        var key = $(this).data('key');
        var row = $(this).closest('tr').first();

        row.find(".dirty-item").first().val(true); // 標記這筆資料已調整

        var txtOriginalPrice = row.find('input[data-key="OriginalPrice"]');
        var txtDiscount = row.find('input[data-key="Discount"]');
        var txtPrice = row.find('input[data-key="Price"]');


        switch (key) {
            case "PriceKindID": //價別
                GetProductPrice(row);
            case "Discount":
                var discount = txtDiscount.val() || 0;
                var originalPrice = txtOriginalPrice.val() || 0;
                var num = new Number(originalPrice * discount / 100);
                // 單價計算至小數2位123
                txtPrice.val(num.toFixed(2));
                break;
            case "Price":
                var price = txtPrice.val() || 0;
                var originalPrice = txtOriginalPrice.val() || 0;
                if (originalPrice != 0) {
                    var num = new Number(price / originalPrice * 100);
                    txtDiscount.val(num.toFixed(2));
                } else {
                    txtDiscount.val(100);
                }
                break;
            case "Qty":
                break;
        }

        var myCalculator = new Calculator(row);
        myCalculator.DoCalculate();
    });
}

//
function AutoCompleteSelect(row, node) {
    console.log('AutoCompleteSelect', row, node);
    var txtProductName = row.find('input.auto-complete-name');
    var selectedProduct = node.item.source;
    txtProductName.val(selectedProduct.ProductName);

    GetLatestSales(row, selectedProduct);

    // 最近3筆銷售資料
    GetHistory(selectedProduct);
}

// AutoCompleteKeyDown
function AutoCompleteKeyDown(row) {
    console.log('AutoCompleteKeyDown', row);

    var txtProductName = row.find('input.auto-complete-name');
    var txtOriginalPrice = row.find('input[data-key="OriginalPrice"]');
    var txtPrice = row.find('input[data-key="Price"]');

    txtProductName.val('');
    txtOriginalPrice.val(0);
    txtPrice.val(0);

    var myCalculator = new Calculator(row);
    myCalculator.DoCalculate();
} //AutoCompleteKeyDown
// 捉產品過去報價資料

// 捉銷售記錄
function GetHistory(selectedProduct) {
    console.log('GetHistory', selectedProduct);

    //var action = '@Url.Action("_History", new { area = "" })';
    var action = _GlobalSetting.GetHistory;
    var data = { key: selectedProduct.ProductID };

    $.post(action, data,
      function (response) {
          //console.log('response', response);
          $("#MyHistory").html(response);
      });
}//GetHistory

function GetProductPrice(row) {
    console.log('GetProductPrice', parent);

    var txtPriceKind = row.find('select[data-key="PriceKindID"]');
    var txtProductID = row.find('input.auto-complete');
    var txtOriginalPrice = row.find('input[data-key="OriginalPrice"]');

    //var action = '@Url.Action("GetPrice","Product")';
    var action = _GlobalSetting.GetPrice;
    var data = { productID: txtProductID.val(), priceKind: txtPriceKind.val() };

    // 捉單價要用同步不能非同步         
    $.ajax({
        url: action,
        data: data,
        success: function (response) {
            //console.log('response', response);
            var result = response || 0;
            txtOriginalPrice.val(result);
        },
        async: false
    });
}//GetProductPrice

function GetLatestSales(row, selectedProduct) {
    console.log('GetLatestSales', row);

    var txtCustomerID = $('select[data-key="txtCustomerID"]');

    var txtOriginalPrice = row.find('input[data-key="OriginalPrice"]');
    var txtPrice = row.find('input[data-key="Price"]');
    var txtPriceKind = row.find('select[data-key="PriceKindID"]');
    var txtCondition = row.find('select[data-key="ConditionID"]');
    var txtDiscount = row.find('input[data-key="Discount"]');

    //var action = '@Url.Action("GetLatestSales", "Sales")';
    var action = _GlobalSetting.GetLatestSales;
    var data = { customerID: txtCustomerID.val(), productID: selectedProduct.ProductID };

    $.get(action, data,
      function (response) {
          console.log('response', response);
          if (response) {

              txtOriginalPrice.val(response.LatestPrice || 0);
              txtPrice.val(response.LatestPrice || 0);
              txtPriceKind.val(response.PriceKindID || '');
              txtCondition.val(response.ConditionID || '');
              txtDiscount.val(response.Discount || 100);
          }

          var myCalculator = new Calculator(row);
          myCalculator.DoCalculate();
      });
}//  GetLatestSales

// 單價數量計算總價
var Calculator = function (row) {
    var price = $(row).find('input[data-key="Price"]').val() || 0;
    var qty = $(row).find('input[data-key="Qty"]').val() || 0;

    console.log('Calculator', price, qty);

    this.Row = row;
    this.Price = price;
    this.Qty = qty;
};

Calculator.prototype = {
    Row: null,
    Price: 0,
    Qty: 0,
    DoCalculate: function () {
        var num = new Number(this.Price * this.Qty);
        // 四捨五入到整數
        $(this.Row).find('input[data-key="Amount"]').val(Math.round(num));
    }
};
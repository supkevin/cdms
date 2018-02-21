// 前端呼叫Ajax
function postAjax(url, data, success) {
    $.ajax({
        cache: false,
        type: "Post",
        url: url,
        async: false,
        data: data,
        beforeSend: function () {

        },
        complete: function () {

        },
        success: function (response) {
            success(response);
        },
        error: function (xhr, status, error) {
            alert('xhr:' + xhr.status + 'status:' + status + ' ,error:' + status);
        }
    });
}

// 將陣列物件轉為屬性不重複的陣列
function GetParameters(source) {
    var result = [];
    $.each(source, function (index, item) {
        //console.log(index, item);
        var target = _.find(result, { name: item.name });
        //console.log('target', target);

        if (!target) {
            result.push(item);
        } else {
            target.value = target.value + ',' + item.value;
        }
    });

    return result;
}
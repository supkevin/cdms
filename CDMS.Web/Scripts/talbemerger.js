////合併上下欄位(colIdx)
jQuery.fn.rowspan = function (colIdx) {
    return this.each(function () {
        var that;
        $('tr', this).each(function (row) {
            var thisRow = $('td:eq(' + colIdx + '),th:eq(' + colIdx + ')', this);
            if ((that != null) && ($(thisRow).html() == $(that).html())) {
                rowspan = $(that).attr("rowSpan");
                if (rowspan == undefined) {
                    $(that).attr("rowSpan", 1);
                    rowspan = $(that).attr("rowSpan");
                }
                rowspan = Number(rowspan) + 1;
                $(that).attr("rowSpan", rowspan);
                $(thisRow).remove(); ////$(thisRow).hide();
            } else {
                that = thisRow;
            }
            that = (that == null) ? thisRow : that;
        });
        alert('1');
    });
}

////當指定欄位(colDepend)值相同時，才合併欄位(colIdx)
jQuery.fn.rowspan = function (colIdx, colDepend) {
    return this.each(function () {
        var that;
        var depend;
        $('tr', this).each(function (row) {
            var thisRow = $('td:eq(' + colIdx + '),th:eq(' + colIdx + ')', this);
            var dependCol = $('td:eq(' + colDepend + '),th:eq(' + colDepend + ')', this);
            if ((that != null) && (depend != null) && ($(thisRow).html() == $(that).html()) && ($(depend).html() == $(dependCol).html())) {
                rowspan = $(that).attr("rowSpan");
                if (rowspan == undefined) {
                    $(that).attr("rowSpan", 1);
                    rowspan = $(that).attr("rowSpan");
                }
                rowspan = Number(rowspan) + 1;
                $(that).attr("rowSpan", rowspan);
                $(thisRow).remove(); ////$(thisRow).hide();

            } else {
                that = thisRow;
                depend = dependCol;
            }
            that = (that == null) ? thisRow : that;
            depend = (depend == null) ? dependCol : depend;
        });
    });
}

////合併左右欄位
jQuery.fn.colspan = function (rowIdx) {
    return this.each(function () {
        var that;
        $('tr', this).filter(":eq(" + rowIdx + ")").each(function (row) {
            $(this).find('th,td').each(function (col) {
                if ((that != null) && ($(this).html() == $(that).html())) {
                    colspan = $(that).attr("colSpan");
                    if (colspan == undefined) {
                        $(that).attr("colSpan", 1);
                        colspan = $(that).attr("colSpan");
                    }
                    colspan = Number(colspan) + 1;
                    $(that).attr("colSpan", colspan);
                    $(this).remove();
                } else {
                    that = this;
                }
                that = (that == null) ? this : that;
            });
        });
    });
}
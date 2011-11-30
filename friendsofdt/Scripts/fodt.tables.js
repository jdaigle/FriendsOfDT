fodt.paginatedTable = function (tableSelector) {
    var controller = this;
    controller.tableSelector = tableSelector;
    $(tableSelector).addClass("paginated");
    $(tableSelector).on("fodt.dataLoaded", function (e, page, itemsPerPage, totalItems) {
        $(controller.tableSelector).data("page-current", page);
        $(controller.tableSelector).data("page-itemsperpage", itemsPerPage);
        $(controller.tableSelector).data("items-total", totalItems);
        var lastPage = Math.floor(totalItems / itemsPerPage) + ((totalItems % itemsPerPage) != 0 ? 1 : 0);
        if (page <= 1) {
            $(tableSelector).find("caption > .page.prev").hide();
        } else {
            $(tableSelector).find("caption > .page.prev").show();
        }
        if (page >= lastPage) {
            $(tableSelector).find("caption > .page.next").hide();
        } else {
            $(tableSelector).find("caption > .page.next").show();
        }
        $(tableSelector).find("caption > .page-number").html("Page " + page + " of " + lastPage);
    });
    $(tableSelector).find("caption > .page.prev").on("click", function (e) {
        e.preventDefault();
        var prevPage = $(controller.tableSelector).data("page-current") - 1;
        controller.onPrev(prevPage, $(controller.tableSelector).data("page-itemsperpage"));
    });
    $(tableSelector).find("caption > .page.next").on("click", function (e) {
        e.preventDefault();
        var nextPage = $(controller.tableSelector).data("page-current") + 1;
        controller.onNext(nextPage, $(controller.tableSelector).data("page-itemsperpage"));
    });
};

$.extend(fodt.paginatedTable.prototype, {
    tableSelector: null,
    onPrev: function (page, itemsPerPage) { return; },
    onNext: function (page, itemsPerPage) { return; }
});
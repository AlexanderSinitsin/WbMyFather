var ColumnsFilters = (function () {

  var timeout = 1000;

  var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
      clearTimeout(timer);
      timer = setTimeout(callback, ms);
    };
  })();

  function intMatchFilterAttributeInit(table, filterTd, column) {
    $('input', filterTd).on('keyup change', function () {
      delay(function () {
        var value = $('input', filterTd).val();
        if (column.search() !== value) {
          column.search(value).draw();
        }
      }, timeout);
    });
  }

  function stringContainsFilterAttributeInit(table, filterTd, column) {
    $('input', filterTd).on('keyup change', function () {
      delay(function () {
        var value = $('input', filterTd).val();
        if (column.search() !== value) {
          column.search(value).draw();
        }
      }, timeout);
    });
  }

  function dateRangeFilterAttributeInit(table, filterTd, column) {
    var onChange = function () {
      var value = "" + $('#from', filterTd).val() + "|" + $('#to', filterTd).val();
      if (column.search() !== value) {
        column.search(value).draw();
      }
    }

    $('#from', filterTd).change(function () {
      delay(onChange, timeout);
    }
    );
    $('#to', filterTd).change(function () {
      delay(onChange, timeout);
    });
  }

  function decimalRangeFilterAttributeInit(table, filterTd, column) {
    var onChange = function () {
      var value = "" + $('#from', filterTd).val() + "|" + $('#to', filterTd).val();
      if (column.search() !== value) {
        column.search(value).draw();
      }
    }

    $('#from', filterTd).change(function () {
      delay(onChange, timeout);
    });
    $('#to', filterTd).change(function () {
      delay(onChange, timeout);
    });
  }

  function roadSectionServicesCategoriesFilterAttributeInit(table, filterTd, column) {
    $('input', filterTd).on('keyup change', function () {
      delay(function () {
        var value = $('input', filterTd).val();
        if (column.search() !== value) {
          column.search(value).draw();
        }
      }, timeout);
    });
  }



  return {
    intMatchFilterAttributeInit: intMatchFilterAttributeInit,
    stringContainsFilterAttributeInit: stringContainsFilterAttributeInit,
    dateRangeFilterAttributeInit: dateRangeFilterAttributeInit,
    decimalRangeFilterAttributeInit: decimalRangeFilterAttributeInit,
    roadSectionServicesCategoriesFilterAttributeInit: roadSectionServicesCategoriesFilterAttributeInit
  }
})();

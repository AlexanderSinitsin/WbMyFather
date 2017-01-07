var Word = (function () {

  var showUrl,
    editUrl,
    tableSelector;

  var containerHeight = $(window).height() * 0.65;
  var selectedRoadCoordinates = [];

  function initTable(option) {

    var table = $('#' + option.id).DataTable();

    if (option.row) {
      table.row('.table-data-empty').remove().draw(false);
      var controlRow = table.row('.locality-road-table-tr').data();

      table.row('.locality-road-table-tr').remove().draw(false);
      table.row.add(option.row).draw(false);
      table.row.add(controlRow).draw();

      table.rows().every(function (rowIdx, tableLoop, rowLoop) {
        var control = this.node();
        var data = this.data();

        $(control).attr('id', 'row_' + rowIdx);

        if (data == controlRow) {
          $(control).addClass('locality-road-table-tr');
        }
      });
    }
  }

  function deleteObject(id) {
    if (confirm('Вы действительно хотите удалить этот объект?')) {
      $.ajax({
        url: deleteObjectUrl,
        type: 'POST',
        dataType: 'json',
        data: {
          id: id
        }
      })
        .success(function (result) {
          if (result.result) {
            if (tableSelector != null) {
              $(tableSelector).DataTable().draw();
            }
          } else {
            alert("Ошибка удаления");
          }
        })
        .error(function (xhr, status, statusCode) {
          console.log(status + ': ' + statusCode);
        });
    }
  };

  function deleteObjects(ids) {
    if (ids.length > 0) {
      if (confirm('Вы действительно хотите удалить ' + ids.length + ' записей?')) {
        $.ajax({
          url: deleteObjectsUrl,
          type: 'POST',
          dataType: 'json',
          data: {
            ids: ids
          }
        })
          .success(function (result) {
            if (result.result) {
              if (tableSelector != null) {
                $(tableSelector).DataTable().draw();
              }
            } else {
              alert("Ошибка удаления");
            }
          })
          .error(function (xhr, status, statusCode) {
            console.log(status + ': ' + statusCode);
          });
      }
    }
  };

  function init(option) {
    showUrl = option.showUrl;
    editUrl = option.editUrl;
    tableSelector = option.tableSelector;

    $('#object-edit').on('hidden.bs.modal', function () {
      var id = $('#object-save').data('id');

      var isAdd = $('#object-save').data('is-add');
      if (isAdd === 'False') {
        Popups.showPopup(showUrl.replace('0', id), {}, $('#object-show-content'), $('#object-show'), function () { });
      }
    });

    $(document)
    .on('click', '#object-edit-btn', function () {
      Popups.showPopup(editUrl.replace('0', $(this).data('id')), {}, $('#object-edit-content'), $('#object-edit'), function () {

      });
    });
  }

  function show(option) {
    Popups.showPopup(showUrl.replace('0', option.id), {}, $('#object-show-content'), $('#object-show'));
  }

  function add(option) {

  }

  return {
    init: init,
    show: show,
    add: add,
    del: deleteObjects
  };
}())

String.prototype.format = function () {
  var args = arguments;
  return this.replace(/{([\d|\w]+)}/g, function (match, key) {
    var value = isNaN(key) ? args[0][key] : args[key];
    return typeof value != 'undefined' ? value : match;
  });
};

/**
 * Представляет методы для работы с URL
 */
var UrlHelper = (function () {

  function UrlHelper(baseUrl) {
    this.baseUrl = baseUrl;
  }

  UrlHelper.prototype.api = function (relativePath) {
    return this.baseUrl + relativePath;
  };

  UrlHelper.prototype.action = function (relativePath) {
    return this.baseUrl + relativePath;
  };

  UrlHelper.prototype.content = function (relativePath) {
    return this.baseUrl + relativePath;
  };

  UrlHelper.prototype.icon = function (iconName) {
    return this.baseUrl + 'Content/Icons/' + iconName;
  };

  UrlHelper.prototype.getQueryStringParams = function (url) {
    var m = url.match(/\?([^#]*)/);
    if (m === null)
      return {};
    var a = m[1].split('&');
    var b = {};


    for (var i = 0; i < a.length; ++i) {
      var p = a[i].split('=', 2);
      var key = p[0];
      var value = p.length > 1 ? decodeURIComponent(p[1].replace(/\+/g, " ")) : '';

      if (key in b) {
        if (!Array.isArray(b[key])) {
          b[key] = [b[key]];
        }
        b[key].push(value);
      } else {
        b[key] = value;
      }
    }
    return b;
  };

  UrlHelper.prototype.map = function (objectType, ids) {
    if (typeof ids.join === "function") {
      ids = ids.join(',');
    }
    return this.baseUrl + '?show=' + objectType + '.' + ids;
  };

  return UrlHelper;
}());

function disableSubmitOnEditForm() {
  $('#object-edit-content button[type="submit"]').attr('disabled', 'disabled');
  $('#recovery-password-content button[type="submit"]').attr('disabled', 'disabled');
  $('#change-password-content button[type="submit"]').attr('disabled', 'disabled');
  $('#login-content button[type="submit"]').attr('disabled', 'disabled');
}

//TODO нужно переименовать и в объект сложить
function removeDisabledSubmit() {
  $('#object-edit-content button[type="submit"]').removeAttr('disabled');
  $('#recovery-password-content button[type="submit"]').removeAttr('disabled');
  $('#change-password-content button[type="submit"]').removeAttr('disabled');
  $('#login-content button[type="submit"]').removeAttr('disabled');
}

var Popups = {

  showPopup: function (url, data, container, modalWindow, success) {
    container.html('');
    var containerHeight = $(window).height() * 0.65;
    $('#modal-spinner').attr('style', 'height:' + containerHeight + 'px;');
    var spinnerHtml = $('#modal-spinner-container').html();

    modalWindow.modal({
      backdrop: 'static'
    });
    container.html(spinnerHtml);

    $.ajax({
      url: url,
      contentType: 'application/html; charset=utf-8',
      type: 'GET',
      dataType: 'html',
      data: data
    }).success(function (result) {
      container.html(result);
      if (success != null) {
        success(result);
      }
    }).error(function (xhr, status) {
      container.html('<div class="modal-header modal-header--custom"> <button type="button" class="close pull-right" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button><div><div class="modal-body"> Ошибка открытия формы <div>');
      console.log(status);
    });
  },

  showPopupForModelId: function (url, id, modalControl, modalbodyControl, success) {
    showPopup(url, { id: id }, modalControl, modalbodyControl, success);
  },

  showPopupWithAction: function (url, action, id, modalControl, modalContentControl, success) {
    showPopup(url, { partialName: action, id: id }, modalControl, modalContentControl, success);
  },

  showPopupPost: function (url, formData, container, modalWindow, success) {
    container.html('');
    var containerHeight = $(window).height() * 0.65;
    $('#modal-spinner').attr('style', 'height:' + containerHeight + 'px;');
    var spinnerHtml = $('#modal-spinner-container').html();

    modalWindow.modal({
      backdrop: 'static'
    });
    container.html(spinnerHtml);

    $.ajax({
      url: url,
      contentType: false,
      type: 'POST',
      dataType: 'html',
      data: formData,
      processData: false
    }).success(function (result) {
      container.html(result);
      if (success != null) {
        success(result);
      }
    }).error(function (xhr, status) {
      container.html('<div class="modal-header modal-header--custom"> <button type="button" class="close pull-right" data-dismiss="modal"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button><div><div class="modal-body"> Ошибка открытия формы <div>');
      console.log(status);
    });
  },

  /**
   * Показывает диалоговое окно
   * @param {string} htmlContent Контент диалога
   * @param {string} caption Заголовок диалога
   * @returns {} jQuery объект диалога
   */
  show: function (htmlContent, caption) {

    var $popup = $('#popup');
    if ($popup.length === 0) {
      //если диалог не найден на странице, создаем его
      var tmpl = $('#popup-tmpl').html();
      if (!tmpl) {
        throw {
          message: 'Не найден шаблон диалога #popup-tmpl'
        };
      }
      $popup = $(tmpl).appendTo('body');
    }

    $popup.find('.modal-headding').html(caption);
    $popup.find('.modal-body').html(htmlContent);
    $popup.modal();

    return $popup;
  }
}

var Select2Helper = (function () {
  function findOption(choiseElemet) {
    var title = choiseElemet.attr("title");
    var options = choiseElemet.closest("td").children().first().find("option");
    var option = $.grep(options, function (value) {
      return $(value).html() === title;
    })[0];

    return $(option);
  }

  return { findOption: findOption }
})

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

//# sourceMappingURL=site.js.map

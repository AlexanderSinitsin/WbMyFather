Promise = function (asyncFunction) {

  var states = {
    PENDING: 0,
    FULFILLED: 1,
    REJECTED: 2
  };

  this.state = states.PENDING;
  this.result = null;
  var self = this;

  this.resolve = function () {
    self.state = states.FULFILLED;
    self.result = [].slice.call(arguments, 0);

    if (self.onFulfilled) {
      self.onFulfilled.apply(self, self.result);
    }
  }

  this.reject = function () {
    self.state = states.REJECTED;
    self.result = [].slice.call(arguments, 0);

    if (self.onRejected) {
      self.onRejected.apply(self, self.result);
    }
  }

  this.setOnFulfilledHandler = function (onFulfilled) {
    if (this.state === states.FULFILLED && onFulfilled) {
      onFulfilled.apply(this, this.result);
    } else {
      this.onFulfilled = onFulfilled;
    }
  }

  this.setOnRejectedHandler = function (onRejected) {
    if (this.state === states.REJECTED && onRejected) {
      onRejected.apply(this, this.result);
    } else {
      this.onRejected = onRejected;
    }
  }

  try {
    asyncFunction(this.resolve, this.reject);
  } catch (ex) {
    this.reject(ex);
  }

  return this;
}

Promise.prototype.then = function (onFulfilled, onRejected) {

  this.setOnFulfilledHandler(onFulfilled);
  this.setOnRejectedHandler(onRejected);
  return this;
};

Promise.prototype.catch = function (onRejected) {
  this.setOnRejectedHandler(onRejected);
  return this;
};

var Route = (function () {

  function Route(test, exec) {

    this.routeValues = {};

    if (typeof test === 'string') {
      this.testFunc = getTestFunction(test);
    } else {
      this.testFunc = test;
    }

    this.execFunc = exec;
  };

  Route.prototype.test = function (url) {
    return this.testFunc.call(this, url);
  };

  Route.prototype.exec = function () {
    return this.execFunc.call(this);
  };

  function getTestFunction(urlPattern) {
    var keys = [];

    var regexpStr = urlPattern
        .replace(/{(.+?)}/g, function (match, group) {
          var pair = group.split(':'); // key:type
          keys.push(pair[0]);

          if (pair[1] === 'Guid') {
            return '([0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12})';
          }
          return '([^/]+)';
        })
        .replace(/\//g, '\\/');

    var regexp = new RegExp(regexpStr, 'i');

    return function (url) {
      var result = regexp.exec(url);
      if (result) {
        for (var i = 1; i < result.length; ++i) {
          this.routeValues[keys[i - 1]] = result[i];
        }
        return true;
      }
      return false;
    }
  }
  return Route;
}());

var App = (function () {

  var app = {};

  // Маршрутизация
  app.routes = [];
  app.routes.mapRoute = function (path, exec) {
    this.push(new Route(path, exec));
    return this;
  };

  app.navigate = function (url) {
    history.pushState({}, null, url);
    window.onpopstate();
  };

  window.onpopstate = function () {
    for (var i = 0; i < app.routes.length; ++i) {
      if (app.routes[i].test(window.location.href)) {
        app.routes[i].exec();
        return;
      }
    }
  };


  return app;
}());

/**
 * @type {UrlHelper}
 */
var Url;

var Book = (function () {
  var
    addUrl,
    showUrl,
    editUrl,
    deleteUrl;

  function delete_objects(url, ids, table, options) {
    if (ids.length > 0) {
      if (confirm('Вы действительно хотите удалить ' + options.mess + '?')) {
        $.ajax({
          url: url,
          type: 'DELETE',
          data: {
            ids: ids
          }
        })
          .success(function (result) {
            if (result.result) {
              if (options.modalWindow != null) {
                options.modalWindow.modal('hide');
              }
              table.draw();
            } else {
              alert("Ошибка удаления");
            }
          })
          .error(function (xhr, status, statusCode) {
            console.log(status + ': ' + statusCode);
          });
      }
    }
  }

  function onSaved() {

  }

  function navigateFromEditWindow(id) {
    if (id && id > 0) {
      Popups.showPopup(showUrl.format(id), {}, $('#object-show-content'), $('#object-show'));
    }
  }

  function init() {
    addUrl = Url.action('api/books/add');
    showUrl = Url.action('api/books/{0}');
    editUrl = Url.action('api/books/{0}/edit');
    deleteUrl = Url.action('api/books');

    $(document)
      .on('keydown.dismiss.bs.modal', '#object-edit', function (e) {
        if (e.keyCode === 27) {
          var id = $('#object-save').data('id');
          navigateFromEditWindow(id);
          e.preventDefault();
        }
      })
      .on('click', '#object-edit-close', function () {
        var id = $('#object-save').data('id');
        navigateFromEditWindow(id);
      })
      .on('click', '#object-del', function () {
        var ids = new Array();
        ids.push($(this).data('id'));
        delete_objects(deleteUrl, ids, $('#BookListItemViewModelTable').DataTable(), { modalWindow: $('#object-show'), mess: 'этоу книгу' });
      })
    .on('click', '#object-edit-btn', function () {
      Popups.showPopup(editUrl.format($(this).data('id')), null, $('#object-edit-content'), $("#object-edit"));
    });
  }

  function show(option) {
    Popups.showPopup(showUrl.format(option.id), {}, $('#object-show-content'), $('#object-show'));
  }

  function del(option) {
    if (option != null && option.ids.length > 0) {
      delete_objects(deleteUrl, $.merge([], option.ids), option.table, { mess: option.ids.length + ' записей' });
    }
  }

  function add() {
    Popups.showPopup(addUrl, null, $('#object-edit-content'), $("#object-edit"));
  }

  return {
    init: init,
    show: show,
    del: del,
    add: add,
    OnSaved: onSaved
  };
}())

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

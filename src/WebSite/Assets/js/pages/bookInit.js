var Book = (function () {
  var
    addUrl,
    showUrl,
    editUrl,
    deleteUrl,
    tableId;

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

  function onSaved(data, status, xhr) {
    if (data.id) {
      $('.modal').modal('hide');
      navigateFromEditWindow(data.id);
    }
    $(tableId).DataTable().ajax.reload(null, false);
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
    tableId = '#BookListItemViewModelTable';

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
        delete_objects(deleteUrl, ids, $(tableId).DataTable(), { modalWindow: $('#object-show'), mess: 'этоу книгу' });
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

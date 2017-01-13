var Word = (function () {

  var addUrl,
    showUrl,
    editUrl,
    deleteUrl,
    tableId;

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

  function deleteObjects(ids, mess) {
    if (ids.length > 0) {
      if (confirm('Вы действительно хотите удалить ' + mess + '?')) {
        $.ajax({
          url: deleteUrl,
          type: 'POST',
          dataType: 'json',
          data: {
            ids: ids
          }
        })
        .success(function (result) {
          if (result.result) {
            if (tableId != null) {
              $(tableId).DataTable().draw();
            }
          } else {
            alert("Ошибка удаления");
          }
        })
        .error(function (xhr, status, statusCode) {
          console.log(status + ': ' + statusCode, xhr);
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

  function init(option) {
    addUrl = Url.action('api/words/add');
    showUrl = Url.action('api/words/{0}');
    editUrl = Url.action('api/words/{0}/edit');
    deleteUrl = Url.action('api/words/del');
    tableId = '#WordListItemViewModelTable';

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
        deleteObjects([$(this).data('id')], 'это слово');
      })
    .on('click', '#object-edit-btn', function () {
      Popups.showPopup(editUrl.format($(this).data('id')), null, $('#object-edit-content'), $("#object-edit"));
    });
  }

  function show(option) {
    Popups.showPopup(showUrl.format(option.id), {}, $('#object-show-content'), $('#object-show'));
  }

  function add() {
    Popups.showPopup(addUrl, null, $('#object-edit-content'), $("#object-edit"));
  }

  return {
    init: init,
    show: show,
    del: deleteObjects,
    add: add,
    OnSaved: onSaved
  };
}())

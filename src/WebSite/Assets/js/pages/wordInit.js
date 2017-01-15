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
    delWordBookUrl = Url.action('api/words/edit/book/delete');
    addWordBookUrl = Url.action('api/words/edit/book/add');

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
      })
      .on('click', '#addWordBook', function () {
        var control = $(this);
        var table = $('#wordbooksTable tbody');
        var inputChell = $('#wordbooksTable tbody tr').last();
        var first = $('#wordbooksTable tbody tr').first().text().indexOf('Записи отсутствуют')>-1;

        $('#wordbooksTable_processing').attr('style', 'display: block;');
        $.ajax({
          url: addWordBookUrl,
          type: 'POST',
          data: {
            SelectedBookId: $('#SelectedWordBook_SelectedBookId').val(),
            Book: $('#SelectedWordBook_Book').val(),
            Number: $('#SelectedWordBook_Number').val(),
            DateRecord: $('#SelectedWordBook_DateRecord').val(),
            SelectedRowId: $('#SelectedWordBook_SelectedRowId').val(),
            Up: $('#SelectedWordBook_Up').val(),
            LineNumber: $('#SelectedWordBook_LineNumber').val()
          }
        })
        .success(function (result) {
          console.log(result.result);
          if (result.result) {
            var addedTr = '';
            $.each(result.result, function (idx, wb) {
              var rows = 1;
              var pages = '';
              $.each(wb.Pages, function (idx, pg) {
                var lines = '';
                var rowspanLine = pg.Lines.length + 1;
                rows += (rowspanLine);
                var date;
                var colspan = 1;
                var rowName = pg.RowId;
                if (pg.Row) {
                  rowName = pg.Row.Name;
                }
                var number = pg.Number + " " + rowName;
                var minus = '';
                if (pg.DateRecord) {
                  date = new Date(pg.DateRecord.replace('/Date(-', '').replace(')/', ''));
                  colspan = 2;
                  number = date.toString();
                  minus = '<span class="input-group-btn"><a class="pull-right btn btn-sm btn-default no-borders" id="delWordBook"><i class="fa fa-minus"></i></a></span>';
                } else {
                  $.each(pg.Lines, function (idx, line) {
                    var data = line.Up ? "<i class='fa fa-long-arrow-down' />" + line.Number : "<i class='fa fa-long-arrow-up' />" + line.Number;
                    lines += '<tr><td class="text-center"><div class="input-group">' + data + '<span class="input-group-btn">' +
                      '<a class="pull-right btn btn-sm btn-default no-borders" id="delWordBook"><i class="fa fa-minus"></i></a>' +
                      '</span></div></td></tr>';
                  });
                }
                pages += '<tr><td class="text-center" rowspan="' + rowspanLine + '" colspan="' + colspan + ')"> <div class="input-group"><div class="input-group">' + number + ' </div>' + minus + ' </div></tr>' +
                  lines;
              });
              addedTr += '<tr><td class="text-center" rowspan="' + rows + '">' + wb.Book.Name + ' </td></tr>' + pages;
            });

            $('#wordbooksTable tbody').html(addedTr + '<tr>' + inputChell.html() + '</tr>');

          }
        })
        .error(function (xhr, status, statusCode) {
          $('#wordbooksTable_processing').attr('style', 'display: none;');
          console.log(status + ': ' + statusCode);
        });
      })
      .on('click', '#delWordBook', function () {
        var control = $(this);
        var table = $('#wordbooksTable').DataTable();

        $('#wordbooksTable_processing').attr('style', 'display: block;');
        $.ajax({
          url: delWordBookUrl,
          type: 'POST',
          data: {
            SelectedBookId: $('#SelectedWordBook_SelectedBookId').val(),
            Book: $('#SelectedWordBook_Book').val(),
            Number: $('#SelectedWordBook_Number').val(),
            DateRecord: $('#SelectedWordBook_DateRecord').val(),
            SelectedRowId: $('#SelectedWordBook_SelectedRowId').val(),
            Up: $('#SelectedWordBook_Up').val(),
            LineNumber: $('#SelectedWordBook_LineNumber').val()
          }
        })
        .success(function (result) {
          console.log(result);
        })
        .error(function (xhr, status, statusCode) {
          $('#wordbooksTable_processing').attr('style', 'display: none;');
          console.log(status + ': ' + statusCode);
        });
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

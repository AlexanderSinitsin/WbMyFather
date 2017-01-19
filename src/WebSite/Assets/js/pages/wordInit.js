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
          console.error(status + ': ' + statusCode, xhr);
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
        $.ajax({
          url: addWordBookUrl,
          type: 'POST',
          data: {
            SelectedBookId: $('#SelectedWordBook_SelectedBookId').val(),
            Book: $('#SelectedWordBook_Book').val(),
            Number: $('#SelectedWordBook_Number').val(),
            DateRecord: $('#SelectedWordBook_DateRecord').val(),
            SelectedRowId: $('#SelectedWordBook_SelectedRowId').val(),
            Up: $('#SelectedWordBook_Up').prop('checked'),
            LineNumber: $('#SelectedWordBook_LineNumber').val()
          }
        })
        .success(function (result) {
          if (result.result) {
            var books = $('#SelectedWordBook_SelectedBookId option');
            var rows = $('#SelectedWordBook_SelectedRowId option');
            generateTable(result.result, books, rows);
          }
        })
        .error(function (xhr, status, statusCode) {
          console.error(status + ': ' + statusCode);
        });
      })
      .on('click', '#delWordBook', function () {
        var control = $(this);
        $.ajax({
          url: delWordBookUrl,
          type: 'POST',
          data: {
            WbId: control.data('wbid'),
            SelectedBookId: control.data('bid'),
            Book: control.data('book'),
            Number: control.data('page'),
            PageId: control.data('pgid'),
            DateRecord: control.data('daterecord'),
            SelectedRowId: control.data('rowid'),
            Up: control.data('up'),
            LineId: control.data('lineid'),
            LineNumber: control.data('line')
          }
        })
        .success(function (result) {
          if (result.result) {
            control.closest('tr').remove();
          } else {
            console.error('Ошибка удаления записи');
          }
        })
        .error(function (xhr, status, statusCode) {
          console.error(status + ': ' + statusCode);
        });
      });
  }

  function generateTable(objects, books, rows) {
    var addedTr = '';
    var inputChell = $('#wordbooksTable tbody tr').last();
    // Счетчик по книгам
    $.each(objects, function (idx, wb) {
      // Название книги
      var book = '';
      if (wb.BookId) {
        books.each(function (idx, bookItem) {
          if (bookItem.value == wb.BookId) {
            book = bookItem.text;
          }
        });
      } else if (wb.Book && wb.Book.Name) {
        book = wb.Book.Name;
      }
      // Счетчик по страницам
      $.each(wb.Pages, function (idx, pg) {
        // Дата записи
        var date;
        // Объединение ячеек страниц и линий
        var colspan = 1;
        // Название колонки страницы
        var row = '';
        if (pg.RowId) {
          rows.each(function (idx, rowItem) {
            if (rowItem.value == pg.RowId) {
              row = rowItem.text;
            }
          });
        } else if (pg.Row && pg.Row.Name) {
          row = pg.Row.Name;
        }
        // Полное название страницы
        var number = pg.Number + " " + row;
        // Контрол удаления записи
        var minus = '';
        if (pg.DateRecord) {
          // Парсим дату из строки в число
          date = Number(pg.DateRecord.replace('/Date(', '').replace(')/', ''));
          colspan = 2;
          number = new Date(date).toLocaleDateString("ru-RU");
          minus = '<span class="input-group-btn"><a class="pull-right btn btn-sm btn-default no-borders" id="delWordBook" ' +
            'data-wbid="' + wb.Id + '" data-bid="' + wb.BookId + '" data-book="' + book + '" data-pgid="' + pg.Id + '" data-daterecord="' + number + '" data-page="' + pg.Number + '"' +
            '"><i class="fa fa-minus"></i></a></span>';

          addedTr += '<tr><td class="text-center">' + book + ' </td>' + // Книги
              '<td class="text-center" colspan="' + colspan + '"> <div class="input-group"><div class="input-group">' + number + ' </div>' + minus + ' </div></td>' + // Страницы
              '<td class="hidden"></td></tr>'; // Строки
        } else {
          // Счетчик по строкам
          $.each(pg.Lines, function (idx, line) {
            var data = line.Up ? "<i class='fa fa-long-arrow-down' />" + line.Number : "<i class='fa fa-long-arrow-up' />" + line.Number;
            addedTr += '<tr><td class="text-center">' + book + ' </td>' + // Книги
              '<td class="text-center" colspan="' + colspan + '"> <div class="input-group"><div class="input-group">' + number + ' </div> </div></td>' + // Страницы
              '<td class="text-center"><div class="input-group">' + data + '<span class="input-group-btn">' + // Строки
              '<a class="pull-right btn btn-sm btn-default no-borders" id="delWordBook" ' +
              'data-wbid="' + wb.Id + '" data-bid="' + wb.BookId + '" data-book="' + book + '" data-pgid="' + pg.Id + '" data-lineid="' + line.Id + '"' + '" data-page="' + pg.Number + '"' + '" data-rowid="' + pg.RowId + '"' + '" data-up="' + line.Up + '"' + '" data-line="' + line.Number + '"' +
              '><i class="fa fa-minus"></i></a>' +
              '</span></div></td></tr>';
          });
        }
      });
    });

    $('#wordbooksTable tbody').html(addedTr + '<tr>' + inputChell.html() + '</tr>');
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

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

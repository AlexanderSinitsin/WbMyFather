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

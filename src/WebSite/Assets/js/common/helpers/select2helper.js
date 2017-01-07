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

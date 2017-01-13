String.prototype.format = function () {
  var args = arguments;
  return this.replace(/{([\d|\w]+)}/g, function (match, key) {
    var value = isNaN(key) ? args[0][key] : args[key];
    return typeof value != 'undefined' ? value : match;
  });
};

String.format = function (format) {
  var args = Array.prototype.slice.call(arguments, 1);
  return format.replace(/{(\d+)}/g, function (match, number) {
    return typeof args[number] != 'undefined'
      ? args[number]
      : match;
  });
};

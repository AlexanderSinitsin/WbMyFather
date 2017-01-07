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

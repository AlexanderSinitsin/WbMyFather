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

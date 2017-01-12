var Route = (function () {

  function Route(test, exec) {

    this.routeValues = {};

    if (typeof test === 'string') {
      this.testFunc = getTestFunction(test);
    } else {
      this.testFunc = test;
    }

    this.execFunc = exec;
  };

  Route.prototype.test = function (url) {
    return this.testFunc.call(this, url);
  };

  Route.prototype.exec = function () {
    return this.execFunc.call(this);
  };

  function getTestFunction(urlPattern) {
    var keys = [];

    var regexpStr = urlPattern
        .replace(/{(.+?)}/g, function (match, group) {
          var pair = group.split(':'); // key:type
          keys.push(pair[0]);

          if (pair[1] === 'Guid') {
            return '([0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12})';
          }
          return '([^/]+)';
        })
        .replace(/\//g, '\\/');

    var regexp = new RegExp(regexpStr, 'i');

    return function (url) {
      var result = regexp.exec(url);
      if (result) {
        for (var i = 1; i < result.length; ++i) {
          this.routeValues[keys[i - 1]] = result[i];
        }
        return true;
      }
      return false;
    }
  }
  return Route;
}());

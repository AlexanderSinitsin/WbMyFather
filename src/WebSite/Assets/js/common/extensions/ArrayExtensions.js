Array.prototype.contains = function (value) {
  return this.indexOf(value) > -1;
};

Array.prototype.removeValue = function (value) {
  var index = this.indexOf(value);
  if (index > -1) {
    this.splice(index, 1);
  }
};

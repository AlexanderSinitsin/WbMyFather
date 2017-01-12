Promise = function (asyncFunction) {

  var states = {
    PENDING: 0,
    FULFILLED: 1,
    REJECTED: 2
  };

  this.state = states.PENDING;
  this.result = null;
  var self = this;

  this.resolve = function () {
    self.state = states.FULFILLED;
    self.result = [].slice.call(arguments, 0);

    if (self.onFulfilled) {
      self.onFulfilled.apply(self, self.result);
    }
  }

  this.reject = function () {
    self.state = states.REJECTED;
    self.result = [].slice.call(arguments, 0);

    if (self.onRejected) {
      self.onRejected.apply(self, self.result);
    }
  }

  this.setOnFulfilledHandler = function (onFulfilled) {
    if (this.state === states.FULFILLED && onFulfilled) {
      onFulfilled.apply(this, this.result);
    } else {
      this.onFulfilled = onFulfilled;
    }
  }

  this.setOnRejectedHandler = function (onRejected) {
    if (this.state === states.REJECTED && onRejected) {
      onRejected.apply(this, this.result);
    } else {
      this.onRejected = onRejected;
    }
  }

  try {
    asyncFunction(this.resolve, this.reject);
  } catch (ex) {
    this.reject(ex);
  }

  return this;
}

Promise.prototype.then = function (onFulfilled, onRejected) {

  this.setOnFulfilledHandler(onFulfilled);
  this.setOnRejectedHandler(onRejected);
  return this;
};

Promise.prototype.catch = function (onRejected) {
  this.setOnRejectedHandler(onRejected);
  return this;
};

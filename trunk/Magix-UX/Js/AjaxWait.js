/*
* Magix - A Managed Ajax Library for ASP.NET
* Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
* Magix is licensed as GPLv3.
*/

(function(){

// Creating class
MUX.AjaxWait = MUX.klass();


// Inheriting from MUX.Control
MUX.extend(MUX.AjaxWait.prototype, MUX.Control.prototype);

MUX.AjaxWait._current = null;
MUX.AjaxWait._widget = null;


// Creating IMPLEMENTATION of class
MUX.extend(MUX.AjaxWait.prototype, {

  init: function(el, opt) {
    this.initControl(el, opt);
    this.options = MUX.extend({
      delay:1000,
      maxOpacity:1
    }, this.options || {});
    if( MUX.AjaxWait._current ) {
      throw "You can only have one AjaxWait control per page ...!";
    }
    MUX.AjaxWait._current = this;
    this._oldCallback = MUX.Form.prototype.callback;
    MUX.Form.prototype.callback = this.callback;
  },

  callback: function() {
    var T = MUX.AjaxWait._current;
    MUX.AjaxWait._widget = this;
    T._previousTimeout = setTimeout(function() {
      T.startAnimation();
    }, T.options.delay);
    T._active = true;
    T._oldCallback.apply(this, [T.onFinished, T.onError]);
  },

  startAnimation: function() {
    this._previousTimeout = null;
    if(this._active) {
      var T = this;
      this._effect = new MUX.Effect.Opacity(T.element, {
        duration: 1000,
        from: 0,
        to: T.options.maxOpacity
      });
    }
  },

  onFinished: function(response) {
    var T = MUX.AjaxWait._current;
    T._active = false;
    var widget = MUX.AjaxWait._widget;
    if( T._previousTimeout !== null) {
      clearTimeout(T._previousTimeout);
      T._previousTimeout = null;
    }
    T.element.setStyle('display', 'none');
    T.element.setOpacity(0);
    if( !widget.options.callingContext ) {
      widget.options.onFinished(response);
    } else {
      widget.options.onFinished.call(widget.options.callingContext, response);
    }
  },

  onError: function(status, response) {
    var T = MUX.AjaxWait._current;
    T._active = false;
    var widget = MUX.AjaxWait._widget;
    if( T._previousTimeout !== null) {
      clearTimeout(T._previousTimeout);
      T._previousTimeout = null;
    }
    if( T._effect ) {
      T._effect.stopped = true;
    }
    T.element.setStyle('display', 'none');
    T.element.setOpacity(0);
    if( !widget.options.callingContext ) {
      widget.options.onError(status, response);
    } else {
      widget.options.onError.call(widget.options.callingContext, status, response);
    }
  },

  Delay: function(value) {
    this.options.delay = value;
  },

  MaxOpacity: function(value) {
    this.options.maxOpacity = value;
  },

  destroyThis: function() {

    // Restoring old callback
    MUX.Form.prototype.callback = this._oldCallback;
    MUX.AjaxWait._current = null;

    // Forward call to allow overriding in inherited classes...
    this._destroyThisControl();
  }
});
})();

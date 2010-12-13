/*
* MagicUX - A Managed Ajax Library for ASP.NET
* Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
* MagicUX is licensed as GPLv3.
*/

(function() {

  MUX.Aspect = MUX.klass();

  // Retrieve function
  MUX.Aspect.$ = function(id) {
    var ctrls = MUX.Control._ctrls;
    var idxCtrl = ctrls.length;
    while (idxCtrl--) {
      var behvs = ctrls[idxCtrl].options.aspects;
      var idxBeha = behvs.length;
      while (idxBeha--) {
        if (behvs[idxBeha].id == id) {
          return behvs[idxBeha];
        }
      }
    }
  };


  MUX.Aspect.prototype = {
    init: function(id, opt) {
      this.options = opt;
      this.id = id;
    },

    initBehavior: function(parent) {
      throw "Abstract base class [MUX.Aspect] used directly from JS. Something is really wrong...";
    },

    initBehaviorBase: function(parent) {
      this.parent = parent;
    },

    JSON: function(json) {
      for (var idxKey in json) {
        this[idxKey](json[idxKey]);
      }
    }
  };





  /*
  * AspectAjaxWait client-side logic
  */
  MUX.AspectAjaxWait = MUX.klass();
  MUX.extend(MUX.AspectAjaxWait.prototype, MUX.Aspect.prototype);
  MUX.extend(MUX.AspectAjaxWait.prototype, {

    initBehavior: function(parent) {
      this.initBehaviorBase(parent);
      this.initUpdater();
    },

    initUpdater: function() {
      this.options = MUX.extend({
        element: null,
        opacity: 0.5,
        delay: 500
      }, this.options || {});

      if (!this.options.element) {
        // No element given, need to explicitly create one...
        this.createObscurer();
      } else {
        this.el = MUX.$(this.options.element);
      }

      // We have to intercept the callback for the parent control for this behavior...
      var T = this;
      T._origCallback = T.parent.callback;

      T.parent.callback = function() {
        T._finished = false;
        if (T.options.delay == 0) {
          T.onStart.apply(T, arguments);
        } else {
          setTimeout(function() {
            T.onStart.apply(T, arguments);
          }, T.options.delay);
        }
        T._origCallback.apply(T.parent, arguments);
      };

      T._originalOnFinished = T.parent.onFinishedRequest;
      T.parent.onFinishedRequest = function() {
        T.onFinished.apply(T, arguments);
        T._originalOnFinished.apply(T.parent, arguments);
      };
    },

    createObscurer: function() {
      this.el = document.createElement('div');
      var el = this.el;
      MUX.extend(el, MUX.Element.prototype);
      el.id = this.id;
      el.setStyles({
        position: 'fixed',
        left: '0px',
        top: '0px',
        width: '100%',
        height: '100%',
        zIndex: 10000,
        backgroundColor: '#000',
        display: 'none'
      });
      this.parent.element.parentNode.appendChild(el);
    },

    Opacity: function(val) {
      this.options.opacity = val;
    },

    Delay: function(val) {
      this.options.delay = val;
    },

    Element: function(val) {
      this.el = MUX.$(val);
      delete this.options.element;
    },

    onStart: function() {

      // Checking to see if request finished before we could execute the "wait time"...
      if (this._finished) {
        return;
      }
      this.effect = new MUX.Effect.Opacity(this.el, {
        duration: this.options.delay,
        from: 0,
        to: this.options.opacity
      });
    },

    onFinished: function() {

      // Signalizing that request has finished in case obscurer effect haven't started yet...
      this._finished = true;
      if (this.effect) {
        this.effect.stopped = true;
        this.effect = new MUX.Effect.Opacity(this.el, {
          duration: this.options.delay / 5,
          from: parseFloat(this.el.getStyle('opacity')),
          to: 0
        });
      }
    },

    destroy: function() {
      if (this.effect) {
        this.effect.stopped = true;
        delete this.effect;
      }
      if (!this.options.element) {
        // This one was created by the JS DOM logic...
        this.el.parentNode.removeChild(this.el);
      }
    }
  });




  MUX.AspectScreenSaver = MUX.klass();
  MUX.AspectScreenSaver._current = null;
  MUX.extend(MUX.AspectScreenSaver.prototype, MUX.Aspect.prototype);
  MUX.extend(MUX.AspectScreenSaver.prototype, {

  initBehavior: function(parent) {
    this.initBehaviorBase(parent);
    this.options = MUX.extend({
      delay:15000,
      images:''
    }, this.options || {});
    
    this.images = this.options.images.split('|');
    this.active = 0;
    
    var el = MUX.$(document.body);
    el.observe('click', this.clicked, this);
    this.parent.element.observe('click', this.clicked, this);
    el.observe('keydown', this.clicked, this);
    el.observe('mousemove', this.clicked, this);
    MUX.extend(window, MUX.Element.prototype);
    window.observe('scroll', this.clicked, this);
    this.startTimer();
    this.active = -1;
    this.swapImages();
  },

  startRollingTimer: function() {
    var T = this;
    setTimeout(function(){
      T.swapImages();
    }, 10000);
  },

  swapImages: function() {
    this.active += 1;
    if( this.active >= this.images.length) {
      this.active = 0;
    }
    this.parent.element.setStyle('backgroundImage', 'url(' + this.images[this.active] + ')');
    this.startRollingTimer();
  },

  startTimer: function() {
    var T = this;
    this._timer = setTimeout(function(){
      T.tick();
    }, this.options.delay);
  },

  clicked: function() {
    if(this.parent.element.getStyle('display') != 'none') {
      var T = this;
      if(!T.animating) {
        T.animating = true;
        new MUX.Effect(this.parent.element, {
          duration: 300,
          onStart: function() {
            T.parent.element.setOpacity(1);
          },
          onFinished: function() {
            T.parent.element.setOpacity(0);
            T.parent.element.setStyle('display', 'none');
            T.animating = false;
          },
          onRender: function(pos) {
            T.parent.element.setOpacity(1 - pos);
          },
          sinoidal: true
        });
        this.startTimer();
      }
    }
    this._clicked = true;
  },

  tick: function() {
    if(!this._clicked) {
      var T = this;
      new MUX.Effect(this.parent.element, {
        duration: 300,
        onStart: function() {
          T.parent.element.setOpacity(0);
          T.parent.element.setStyle('display', 'block');
        },
        onFinished: function() {
          T.parent.element.setOpacity(1);
        },
        onRender: function(pos) {
          T.parent.element.setOpacity(pos);
        },
        sinoidal: true
      });
    } else {
      this.startTimer();
    }
    this._clicked = false;
  },

  Delay: function(value) {
    this.options.delay = value;
  },

  destroyThis: function() {
    // Forward call to allow overriding in inherited classes...
    this._destroyThisControl();
  }
});







  /*
  * AspectModal client-side logic
  */
  MUX.AspectModal = MUX.klass();
  MUX.extend(MUX.AspectModal.prototype, MUX.Aspect.prototype);

  MUX.extend(MUX.AspectModal.prototype, {

    initBehavior: function(parent) {
      this.initBehaviorBase(parent);
      this.initObscurer();
    },

    initObscurer: function() {
      this.options = MUX.extend({
        color: '#000',
        bottomColor: '#000',
        opacity: 0.5
      }, this.options || {});

      this.zIndex = parseInt(this.parent.element.style.zIndex || 100, 10) - 1;

      // Creating obscurer (from base class)
      this.createObscurer();

      // Then we can run the animation effect which will slowly show the obscurer...
      var T = this;
      var dummy = new MUX.Effect(this.el, {
        duration: 300,
        onStart: function() {
          T.el.setOpacity(0);
          T.el.setStyle('display', 'block');
        },
        onFinished: function() {
          T.el.setOpacity(T.options.opacity);
        },
        onRender: function(pos) {
          T.el.setOpacity(pos * T.options.opacity);
        },
        sinoidal: true
      });
    },

    createObscurer: function() {
      this.el = document.createElement('div');
      var el = this.el;
      MUX.extend(el, MUX.Element.prototype);
      el.id = this.id;
      el.setStyles({
        position: 'fixed',
        left: '0px',
        top: '0px',
        width: '100%',
        height: '100%',
        zIndex: this.zIndex,
        textAlign: 'center',
        display: 'none'
      });
      this.setColor();

      // We must add the node to the DOM before we can begin computing its offset according to the browser...
      this.parent.element.parentNode.appendChild(el);
    },

    setColor: function() {
      if (this.options.color == this.options.bottomColor || MUX.Browser.IE || MUX.Browser.Opera) {
        // IE + Opera
        this.el.setStyle('backgroundColor', this.options.color);
      } else {
        if (MUX.Browser.Gecko) {
          this.el.setStyle(
            'backgroundImage',
            '-moz-linear-gradient(' + this.options.color + ' 0%, ' + this.options.bottomColor + ' 100%)');
        } else {
          // Safari
          var val = '-webkit-gradient(linear, 0% 0%, 0% 100%, from(' + this.options.color + '), to(' + this.options.bottomColor + '))';
          this.el.setStyle(
            'backgroundImage',
            val);
        }
      }
    },

    Color: function(val) {
      this.options.color = val;
      this.setColor();
    },

    BottomColor: function(val) {
      this.options.bottomColor = val;
      this.setColor();
    },

    Opacity: function(val) {
      this.el.setStyle('opacity', val);
    },

    destroy: function() {
      var T = this;
      var dummy = new MUX.Effect(this.el, {
        duration: 300,
        onFinished: function() {
          T.el.repl();
        },
        onRender: function(pos) {
          this.element.setOpacity((1.0 - pos) * T.options.opacity);
        },
        sinoidal: true
      });
    }
  });








  /*
  * AspectDraggable client-side logic
  */
  MUX.AspectDraggable = MUX.klass();
  MUX.extend(MUX.AspectDraggable.prototype, MUX.Aspect.prototype);
  MUX.extend(document.body, MUX.Element.prototype);

  MUX.extend(MUX.AspectDraggable.prototype, {

    initBehavior: function(parent) {
      this.initBehaviorBase(parent);
      this.initDraggable();
    },

    initDraggable: function() {

      this._caption = false;
      this.options = MUX.extend({
        bounds: null,
        callback: false,
        snap: null,
        handle: this.parent.element,
        enabled: true
      }, this.options || {});

      this.options.handle.observe('mousedown', this.onMouseDown, this);
      document.body.observe('mousemove', this.onMouseMove, this);
      document.body.observe('mouseup', this.onMouseUp, this);
    },

    Bounds: function(rc) {
      this.options.bounds = rc;
    },

    Handle: function(handle) {
      this.options.handle.stopObserving('mousedown', this.onMouseDown, this);
      this.options.handle = MUX.$(handle);
      handle.observe('mousedown', this.onMouseDown, this);
    },

    Enabled: function(value) {
      this.options.enabled = value;
    },

    Snap: function(pt) {
      this.options.snap = pt;
    },

    onMouseDown: function(event) {
      if (this.options.enabled) {
        this._caption = true;
        this._beginPtrPos = this.pointer(event);
        this._beginX = parseInt(this.parent.element.getStyle('left'), 10);
        this._beginY = parseInt(this.parent.element.getStyle('top'), 10);
      }
    },

    onMouseUp: function(event) {
      if (!this._caption || !this.options.enabled) {
        return;
      }
      delete this._caption;
      delete this._beginPtrPos;
      var x = parseInt(this.parent.element.getStyle('left'), 10);
      var y = parseInt(this.parent.element.getStyle('top'), 10);
      if (this.options.callback) {
        var dummy = new MUX.Ajax({
          args: '__MUX_CONTROL_CALLBACK=' + this.id + '&__MUX_EVENT=Moved' + '&x=' + x + '&y=' + y,
          onSuccess: this.onFinishedRequest,
          onError: this.onFailedRequest,
          callingContext: this
        });
      }
    },

    onMouseMove: function(event) {
      if (this._caption && this.options.enabled) {
        var ptrPos = this.pointer(event);
        var delX = this._beginX + ptrPos.x - this._beginPtrPos.x;
        var delY = this._beginY + ptrPos.y - this._beginPtrPos.y;
        if (this.options.snap != null) {
          delX -= delX % this.options.snap.x;
          delY -= delY % this.options.snap.y;
        }
        if (this.options.bounds != null) {
          delX = Math.min(
            Math.max(delX, this.options.bounds.left),
            this.options.bounds.width + this.options.bounds.left);
          delY = Math.min(
            Math.max(delY, this.options.bounds.top),
            this.options.bounds.height + this.options.bounds.top);
        }
        this.parent.element.setStyles({
          left: delX + 'px',
          top: delY + 'px'
        });
      }
    },

    onFinishedRequest: function(response) {
      eval(response);
    },

    onFailedRequest: function(status, fullTrace) {
      MUX.Control.errorHandler(status, fullTrace);
    },

    pointer: function(event) {
      return {
        x: event.pageX ||
          (event.clientX + (document.documentElement.scrollLeft || document.body.scrollLeft)),
        y: event.pageY ||
          (event.clientY + (document.documentElement.scrollTop || document.body.scrollTop))
      };
    },

    destroy: function() {
      this.options.handle.stopObserving('mousedown', this.onMouseDown, this);
      document.body.stopObserving('mouseup', this.onMouseUp, this);
      document.body.stopObserving('mousemove', this.onMouseMove, this);
    }
  });





  /*
  * AspectFixated client-side logic
  */
  MUX.AspectFixated = MUX.klass();
  MUX.extend(MUX.AspectFixated.prototype, MUX.Aspect.prototype);
  MUX.extend(document.body, MUX.Element.prototype);

  MUX.extend(MUX.AspectFixated.prototype, {

    initBehavior: function(parent) {
      this.initBehaviorBase(parent);
      this.initFixated();
    },

    initFixated: function() {
      MUX.extend(window, MUX.Element.prototype);
      window.observe('scroll', this.onScroll, this);
      this._top = parseInt(this.parent.element.getStyle('top'), 10);
      this._lastScroll = this.scroll();
    },

    onScroll: function(event) {
      this._lastTick = new Date().getTime();
      this.tick();
    },

    tick: function() {
      if(this._destroyed) {
        return;
      }
      if (!this._isRunning && this._lastTick + 250 < new Date().getTime()) {
        this.runEffect();
      } else {
        var T = this;
        setTimeout(function() { T.tick(); }, 250);
      }
    },

    runEffect: function() {
      this._isRunning = true;
      if(this._destroyed) {
        return;
      }
      var T = this;
      var scr = this.scroll();
      var prevScroll = this._lastScroll;
      new MUX.Effect.Generic(this.parent.element, {
        transition: 'Explosive',
        loop: function(pos) {
          var nPos = (prevScroll.y + parseInt(((scr.y - prevScroll.y) * pos), 10) + T._top);
          T.parent.element.setStyle('top', nPos + 'px');
        },
        end: function() {
          T.parent.element.setStyle('top', scr.y + T._top + 'px');
          T._lastScroll = scr;
          T._isRunning = false;
        }
      });
    },

    scroll: function(event) {
      return {
        x: window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft,
        y: window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop
      };
    },

    destroy: function() {
      window.stopObserving('scroll', this.onScroll, this);
      this._destroyed = true;
    }
  });
})();

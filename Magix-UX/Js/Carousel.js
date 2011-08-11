/*
* Magix - A Managed Ajax Library for ASP.NET
* Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
* Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
*/

(function() {

  MUX.Carousel = MUX.klass();

  MUX.extend(MUX.Carousel.prototype, MUX.Control.prototype);

  MUX.extend(MUX.Carousel.prototype, {
    init: function(el, opt) {
      this.initControl(el, opt);
      this.initCarousel();
    },

    initCarousel: function() {
      // Initializing the child Carousel items...
      this._position = 0;
      this._length = 0;
      this._height = 0;
      this._width = 0;
      this._parentWidth = parseInt(MUX.$(this.element).getStyle('width'), 10);
      for (var idx = 0; idx < this.element.childNodes.length; idx++) {
        var cur = MUX.$(this.element.childNodes[idx]);
        if(!cur || !cur.id) {
          continue;
        }
        cur.setStyle('position', 'absolute');
        cur._pos = this._length;
        cur.observe('click', this.click, {T:this, C:cur});
        if(!this._height) {
          this._height = parseInt(cur.getStyle('height'), 10);
          this._width = parseInt(cur.getStyle('width'), 10);
        }
        this._length += 1;
      }

      // Settings the Carousel items to their initial placement
      this.rearrange(this._position);

      new MUX.Effect.Opacity(this.element, {
        from:0,
        to:1,
        duration:500
      });
      this.element.observe('mousedown', this.mouseDown, this);
      MUX.$(document.body).observe('mousemove', this.mouseMove, this);
      MUX.$(document.body).observe('mouseup', this.mouseUp, this);
    },

    click: function() {
      var el = this.C._pos;
      var T = this.T;
      var old = T._position;
      var toMove = el - old;
      new MUX.Effect.Generic(this.C, {
        transition: 'Explosive',
        loop: function(pos) {
          T._position = parseInt(old + (pos * toMove), 10);
          T.rearrange(T._position);
        },
        end: function() {
          T._position = el;
          T.rearrange(el);
        },
        duration:500
      });
    },

    pointer: function(event) {
      return {
        x: event.pageX ||
          (event.clientX + (document.documentElement.scrollLeft || document.body.scrollLeft)),
        y: event.pageY ||
          (event.clientY + (document.documentElement.scrollTop || document.body.scrollTop))
      };
    },

    mouseDown: function(event) {
      this._mouseDown = this.pointer(event);
    },

    mouseMove: function(event) {
      if(this._mouseDown) {
        var pointer = this.pointer(event);
        var offset = this._mouseDown.x - pointer.x;
        this._lastOffset = offset;
        var oldOffset = parseInt(this._position, 10);
        this._position += offset / 30;
        this._position = Math.round(this._position, 10);
        if(this._position < 0) {
          this._position = 0;
        } else if( this._position >= this._length) {
          this._position = this._length - 1;
        }
        if(oldOffset != this._position) {
          this.rearrange(this._position);
          this._mouseDown = pointer;
        }
      }
    },

    mouseUp: function() {
      this._mouseDown = false;
    },

    rearrange: function(active) {
      var idxNo = 0;
      var left = 0;
      var extra = 0;

      var allItemsCount = this.element.childNodes.length;
      for (var idx = 0; idx < allItemsCount; idx++) {
        var cur = MUX.$(this.element.childNodes[idx]);

        // Items inside of the carousel need to have ID's
        if(!cur || !cur.id) {
          continue;
        }

        var offsetFromMain = Math.abs(idxNo - active);
        cur.setStyle('zIndex', 500 - offsetFromMain);
        if(offsetFromMain == 0) {
          cur.className = 'item active';
        } else {
          cur.className = 'item nonactive';
        }

        cur.setStyle('height', Math.max(25, this._height - (Math.abs(idxNo - (active - 0.5)) * 4)) + 'px');
        cur.setStyle('top', Math.min(50, Math.abs(idxNo - (active - 0.5)) * 2) + 'px');
        cur.setStyle('left', left + 'px');
        idxNo += 1;
        left += ((this._parentWidth - this._width) / this._length);
      }
    }
  });
})();

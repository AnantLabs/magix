// ==============================================================================
//
// This is the BehaviorFingerScroll
// Mimics the Apple way of scrolling so that you can scroll by dragging
// your finger...
//
// ==============================================================================
MUX.SmartScroll = MUX.klass();


// Inheriting from Behavior base class
MUX.extend(MUX.SmartScroll.prototype, MUX.Aspect.prototype);


// Creating IMPLEMENTATION of class
MUX.extend(MUX.SmartScroll.prototype, {

  // Delayed CTOR, actually called by the MUX.Control class
  // for all Behaviors within the Control
  initBehavior: function(parent) {

    MUX.extend(document.body, MUX.Element.prototype);
    document.body.observe('touchmove', function(e) {
      e.preventDefault();
    }, this);

    this.parent = parent;

    var T = this;
    setTimeout(function() {
      T.parent.element.observe('touchmove', T.onTouchMove, T);
    }, 1);
  },

  pointer: function(event) {
    return {
      y: event.clientY
    };
  },

  onTouchMove: function(e) {
    if (e.targetTouches.length != 1) {
      return false;
    }
    var topDelta = e.targetTouches[0].clientY - this.startY;
    if (this.position > 0 || this.position < this.maxScroll) {
      return;
    }
    this.position = this.position + topDelta;
    this.parent.element.style.webkitTransform = 'translate3d(0, ' + this.position + 'px, 0)';
  },

  scrollTo: function(dest) {
    this.parent.element.style.webkitTransitionDuration = runtime ? runtime : '300ms';
    this.parent.element.style.webkitTransform = 'translate3d(0, ' + this.position + 'px, 0)';
  },

  destroy: function() {
    this.parent.element.stopObserving('touchmove', this.onTouchMove, this);
  }
});

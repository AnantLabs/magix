/*
* Magix - A Managed Ajax Library for ASP.NET
* Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
* Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
*/

(function() {

  MUX.ColorPicker = MUX.klass();

  MUX.extend(MUX.ColorPicker.prototype, MUX.Control.prototype);

  MUX.extend(MUX.ColorPicker.prototype, {
    init: function(el, opt) {
      this.initControl(el, opt);
      this.options = MUX.extend({
        img:'media/images/spectrum.png'
      }, this.options || {});
      this._cnv = MUX.$(this.element.id + '__cnv');
      this.drawCanvas();
      this._cnv.observe('click', this.clicked, this);
      this._cnv.observe('mousemove', this.moved, this);
      this._cnv.observe('mouseout', this.out, this);
      this.preview = MUX.$(this.element.id + '__prev');
      this.selected = MUX.$(this.element.id + '__selected');
      this.value = MUX.$(this.element.id + '__value');
      this.value.observe('change', this.valueChange, this);
      this.valueHex = MUX.$(this.element.id + '__valueHex');
      this.valueHex.observe('change', this.valueHexChange, this);
      this.valueHex.observe('focus', this.valueHexFocus, this);
      this.value.observe('focus', this.valueFocus, this);
    },

    valueHexFocus: function(e) {
      this.valueHex.select();
    },

    valueFocus: function(e) {
      this.value.select();
    },

    valueHexChange: function(e) {
      if(this.valueHex.value.length == 0) {
        this.value.value = '';
      } else {
        var red = Math.max(0, Math.min(255, parseInt(this.valueHex.value.substring(1,3), 16)));
        var green = Math.max(0, Math.min(255, parseInt(this.valueHex.value.substring(3,5), 16)));
        var blue = Math.max(0, Math.min(255, parseInt(this.valueHex.value.substring(5,7), 16)));
        this.selected.setStyle('backgroundColor', 'rgba('+red+','+green+','+blue+', 1)');
        this.selected.innerHTML = 'R: ' + red + '<br/>G: ' + green + '<br/>B: ' + blue;
        this.value.value = 'rgb('+red+', '+green+', '+blue+')';
        this.valueHex.value = '#'+
          (red < 16 ? '0' : '')+
          red.toString(16)+
          (green < 16 ? '0' : '')+
          green.toString(16)+
          (blue < 16 ? '0' : '')+
          blue.toString(16);
      }
    },

    valueChange: function(e) {
      if(this.value.value.length == 0) {
        this.valueHex.value = '';
      } else {
        var red = Math.max(0, Math.min(255, parseInt(this.value.value.split(',')[0].split('(')[1], 10)));
        var green = Math.max(0, Math.min(255, parseInt(this.value.value.split(',')[1], 10)));
        var blue = Math.max(0, Math.min(255, parseInt(this.value.value.split(',')[2].split(')')[0], 10)));
        this.selected.setStyle('backgroundColor', 'rgba('+red+','+green+','+blue+', 1)');
        this.selected.innerHTML = 'R: ' + red + '<br/>G: ' + green + '<br/>B: ' + blue;
        this.valueHex.value = '#'+
          (red < 16 ? '0' : '')+
          red.toString(16)+
          (green < 16 ? '0' : '')+
          green.toString(16)+
          (blue < 16 ? '0' : '')+
          blue.toString(16);
        this.value.value = 'rgb('+red+', '+green+', '+blue+')';
      }
    },

    clicked: function(e) {
      var ct = this._cnv.getContext('2d');
      var imageData = ct.getImageData(e.offsetX || e.layerX, e.offsetY || e.layerY, 1, 1);
      var data = imageData.data;
      var red = data[0];
      var green = data[1];
      var blue = data[2];
      this.selected.setStyle('backgroundColor', 'rgba('+red+','+green+','+blue+', 1)');
      this.selected.innerHTML = 'R: ' + red + '<br/>G: ' + green + '<br/>B: ' + blue;
      this.value.value = 'rgb('+red+', '+green+', '+blue+')';
      this.valueHex.value = '#'+
        (red < 16 ? '0' : '')+
        red.toString(16)+
        (green < 16 ? '0' : '')+
        green.toString(16)+
        (blue < 16 ? '0' : '')+
        blue.toString(16);
      this.valueHex.select();
      this.valueHex.focus();
    },

    moved: function(e) {
      var ct = this._cnv.getContext('2d');
      var imageData = ct.getImageData(e.offsetX || e.layerX, e.offsetY || e.layerY, 1, 1);
      var data = imageData.data;
      var red = data[0];
      var green = data[1];
      var blue = data[2];
      this.preview.setStyle('backgroundColor', 'rgba('+red+','+green+','+blue+', 1)');
      this.preview.innerHTML = 'R: ' + red + '<br/>G: ' + green + '<br/>B: ' + blue;
    },

    out: function(e) {
      this.preview.setStyle('backgroundColor', 'Transparent');
      this.preview.innerHTML = '';
    },

    ColorWheelImage: function(value) {
      this.options.img = value;
      this.drawCanvas();
    },

    drawCanvas: function() {
      var img = new Image();
      var T = this;
      img.onload = function() {
        var ct = T._cnv.getContext('2d');
        T._cnv.width = img.width;
        T._cnv.height = img.height;
        ct.drawImage(img, 0, 0);
      };
      img.src = this.options.img;
    },

    destroyThis: function() {
      this._destroyThisControl();
    }
  });
})();

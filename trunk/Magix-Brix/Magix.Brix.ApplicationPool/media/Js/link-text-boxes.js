/*
 * Magix - A Managed Ajax Library for ASP.NET, among other things ...
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

(function(){

MUX.linkTextBoxes = function(source, dest) {
  this.source = MUX.$(source);
  if(!this.source) {
    return;
  }
  this.dest = MUX.$(dest);
  this.source.observe('keypress', 
    function() {
      setTimeout(function() {
        MUX.linkedTextChanged();
      }, 1)}, this);
  this.source.observe('blur', MUX.linkedTextChanged, this);
  var T = this;
  MUX.C(this.source.id).preDestroyer = function() {
    T.source.stopObserving('keypress', MUX.linkedTextChanged, T);
    T.source.stopObserving('blur', MUX.linkedTextChanged, T);
  }
};

MUX.linkedTextChanged = function(e) {
  if(!this.source) {
    return;
  }
  var txt = this.source.value;

  if(!txt) {
    this.dest.value = '';
    return;
  }

  // Making sure the '@' parts are gone ...
  if(txt.indexOf('@') > 0) {
    txt = txt.substring(0, txt.indexOf('@'));
  }

  var splt = null;
  if(txt.indexOf('.') > 0) {
    splt = '.';
  } else if(txt.indexOf('_') > 0) {
    splt = '_';
  } else if(txt.indexOf('-') > 0) {
    splt = '-';
  }

  if(splt != null) {
    // Splitters are here. Spitting them all out as separate names ...
    var tokens = txt.split(splt);
    var result = '';
    var first = true;
    for(var idx = 0; idx < tokens.length; idx++) {
      if(first) {
        first = false;
      } else {
        result += ' ';
      }
      result += tokens[idx].charAt(0).toUpperCase() + tokens[idx].slice(1);
    }
    this.dest.value = result;
  } else {
    var firstName = txt;
    this.dest.value = firstName.charAt(0).toUpperCase() + firstName.slice(1);
  }
}

})();


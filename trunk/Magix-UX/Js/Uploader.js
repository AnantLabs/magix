/*
* Magix - A Managed Ajax Library for ASP.NET
* Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
* Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
*/

(function() {

  MUX.Uploader = MUX.klass();

  MUX.extend(MUX.Uploader.prototype, MUX.Control.prototype);

  MUX.extend(MUX.Uploader.prototype, {
    init: function(el, opt) {
      this.initControl(el, opt);
      var b = MUX.$(document.body);
      b.observe('dragover', this.onDragOver, this);
      b.observe('drop', this.onDrop, this);
    },

    onDragOver: function(evt){
      MUX.$(this.element.id + '_ul').innerHTML = '';
      this.element.setStyle('display', 'block');
      evt.stopPropagation();
      evt.preventDefault();
      return false;
    },

    onDrop: function(evt){
      evt.stopPropagation();
      evt.preventDefault();
      this.postFiles(evt);
      return false;
    },

    postFiles: function(evt){
      var files = evt.dataTransfer.files;
      if(files.length > 0) {
        this._fileCount = files.length;
        var ih = '';
        for (var i = 0, f; f = files[i]; i++) {
          ih += '<li id="' + this.element.id + '_li' + i + '">' + f.name + '</li>';
        }
        MUX.$(this.element.id + '_ul').innerHTML = ih;


        // Loop through the FileList and render image files as thumbnails.
        var idxNo = 0;
        for (var i = 0, f; f = files[i]; i++) {
          var reader = new FileReader();

          // Closure to capture the file information.
          var T = this;
          reader.onload = (function(idxF) {
            return function(e) {
              var img = e.target.result;
              var ul = MUX.$(T.element.id + '_ul');
              MUX.$(ul.children[0]).addClassName('mux-uploader-processing');
              var x = new MUX.Ajax({
                args: '__MUX_CONTROL_CALLBACK=' + T.element.id + 
                  '&__MUX_EVENT=uploaded' + 
                  '&__MUX_TOTAL=' + files.length +
                  '&__MUX_CURRENT=' + (idxNo++) + 
                  '&__FILE=' + encodeURIComponent(img) + 
                  '&__FILENAME=' + encodeURIComponent(idxF.name),
                onSuccess: T.onFinishedUploading,
                callingContext: T
              });
            };
          })(f);
          // Read in the image file as a data URL.
          reader.readAsDataURL(f);
        }
      }
      else {
        this.element.setStyle('display', 'none');
      }
    },

    onFinishedUploading: function(response) {
      this.onFinishedRequest(response);
      this._fileCount -= 1;
      if(this._fileCount == 0) {
        this.element.setStyle('display', 'none');
      } else {
        var ul = MUX.$(this.element.id + '_ul');
        ul.removeChild(ul.children[0]);
        MUX.$(ul.children[0]).addClassName('mux-uploader-processing');
      }
    },

    destroyThis: function() {
      var b = MUX.$(document.body);
      b.stopObserving('dragover', this.onDragOver, this);
      b.stopObserving('drop', this.onDrop, this);
      this._destroyThisControl();
    }
  });
})();

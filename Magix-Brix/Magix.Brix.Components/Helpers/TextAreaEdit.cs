/*
 * Magix - A Web Application Framework for Humans
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.ComponentModel;

using Magix.UX.Widgets;
using Magix.UX.Effects;

namespace Magix.Brix.Components
{
    /**
     * Level4: Basically every single instance you see in Magix which is of type
     * 'InPlaceEditable' is an instance of this component. Alows for viewing textual
     * based information, while at the same time, this tet can be 'clicked' which will
     * change it into a text box, in which you can change its value. Often displayed with
     * a pen next to it
     */
    public class TextAreaEdit : Panel
    {
        private readonly TextArea _text = new TextArea();
        private readonly LinkButton _link = new LinkButton();

        public TextAreaEdit()
        {
            CssClass = "mux-in-place-edit";
        }

        /**
         * Level4: Text was changed by end user
         */
        public event EventHandler TextChanged;

        /**
         * Level4: Raised when TetBox is about to become displayed
         */
        public event EventHandler DisplayTextBox;

        /**
         * Level4: The actual text property of the control
         */
        public string Text
        {
            get { return _text.Text; }
            set { _link.Text = ReduceText(EscapeHTML(value)); _link.Info = value; _text.Text = value; }
        }

        /**
         * Level4: Max length before 'clipping' will occur
         */
        public int TextLength
        {
            get { return ViewState["TextLength"] == null ? 50 : (int)ViewState["TextLength"]; }
            set { ViewState["TextLength"] = value; }
        }

        private string ReduceText(string value)
        {
            if (value == null)
                return null;
            if (value.Length > TextLength)
                return value.Substring(0, TextLength - 3) + "..." + value.Length;
            return value;
        }

        private string EscapeHTML(string value)
        {
            return value == null ? null : value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string UnEscapeHTML(string value)
        {
            return value == null ? null : value.Replace("&lt;", "<").Replace("&gt;", ">");
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            CreateEditControls();
        }

        private void CreateEditControls()
        {
            // Creating LinkButton
            _link.ID = "btn";
            _link.Click += LinkClick;
            Controls.Add(_link);

            // Creating TextBox
            _text.ID = "txt";
            _text.Text = _link.Info;
            _text.Visible = false;
			_text.Blur += TextUpdated;
            _text.EscPressed += TextEscPressed;
            Controls.Add(_text);
        }

        private void LinkClick(object sender, EventArgs e)
        {
            if (_link.Text == "[nothing]")
                _text.Text = "";
            else
                _text.Text = _link.Info;
            _text.Visible = true;
            _text.Style[Styles.display] = "none";
            _link.Visible = false;
            new EffectRollDown(_text, 200)
                .ChainThese(
                    new EffectFocusAndSelect(_text))
                .Render();
            if (DisplayTextBox != null)
                DisplayTextBox(this, new EventArgs());
        }

        void TextEscPressed(object sender, EventArgs e)
        {
            _text.Visible = false;
            _link.Visible = true;
            new EffectFocusAndSelect(_link)
                .Render();
        }

        private void TextUpdated(object sender, EventArgs e)
        {
            _link.Text = EscapeHTML(ReduceText(_text.Text));
            _link.Info = _text.Text;
            _text.Visible = false;
            _link.Visible = true;

            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (string.IsNullOrEmpty(_link.Text))
                _link.Text = "[nothing]";
            base.OnPreRender(e);
        }
    }
}

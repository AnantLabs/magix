/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;
using Magic.UX.Effects;

namespace Magic.UX.Widgets
{
    public class InPlaceTextAreaEdit : Panel
    {
        private LinkButton _link = new LinkButton();
        private TextArea _text = new TextArea();

        public InPlaceTextAreaEdit()
        {
            CssClass = "mux-in-place-edit";
        }

        public event EventHandler TextChanged;

        public string Text
        {
            get { return _text.Text; }
            set { _link.Text = value; _text.Text = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();
            base.OnInit(e);
        }

        protected override void CreateChildControls()
        {
            CreateEditControls();
        }

        private void CreateEditControls()
        {
            // Creating LinkButton
            _link.ID = "btn";
            _link.Click += _link_Click;
            Controls.Add(_link);

            // Creating TextBox
            _text.ID = "txt";
            _text.Text = _link.Text;
            _text.Visible = false;
			_text.Blur += _text_Updated;
            _text.EscPressed += _text_EscPressed;
            Controls.Add(_text);
        }

        private void _link_Click(object sender, EventArgs e)
        {
            if (_link.Text == "[nothing]")
                _text.Text = "";
            else
                _text.Text = _link.Text;
            _text.Visible = true;
            _link.Visible = false;
            new EffectFocusAndSelect(_text)
                .Render();
        }

        void _text_EscPressed(object sender, EventArgs e)
        {
            _text.Visible = false;
            _link.Visible = true;
            new EffectFocusAndSelect(_link)
                .Render();
        }

        private void _text_Updated(object sender, EventArgs e)
        {
            _link.Text = _text.Text;
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

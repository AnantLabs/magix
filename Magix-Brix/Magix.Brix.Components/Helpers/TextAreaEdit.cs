﻿/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * Magix is licensed as GPLv3.
 */

using System;
using System.ComponentModel;

using Magix.UX.Widgets;
using Magix.UX.Effects;

namespace Magix.Brix.Components
{
    public class TextAreaEdit : Panel
    {
        private readonly TextArea _text = new TextArea();
        private readonly LinkButton _link = new LinkButton();

        public TextAreaEdit()
        {
            CssClass = "mux-in-place-edit";
        }

        public event EventHandler TextChanged;

        public string Text
        {
            get { return _text.Text; }
            set { _link.Text = EscapeHTML(value); _text.Text = value; }
        }

        private string EscapeHTML(string value)
        {
            return value.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        private string UnEscapeHTML(string value)
        {
            return value.Replace("&lt;", "<").Replace("&gt;", ">");
        }

        [DefaultValue(-1)]
        public int TextLength
        {
            get
            {
                return ViewState["TextLength"] == null ? -1 : (int)ViewState["TextLength"];
            }
            set { ViewState["TextLength"] = value; }
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
            _text.Text = UnEscapeHTML(_link.Text);
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
                _text.Text = UnEscapeHTML(_link.Text);
            _text.Visible = true;
            _text.Style[Styles.display] = "none";
            _link.Visible = false;
            new EffectRollDown(_text, 200)
                .ChainThese(
                    new EffectFocusAndSelect(_text))
                .Render();
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
            _link.Text = EscapeHTML(_text.Text);
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
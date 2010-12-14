/*
 * MagicUX - A Managed Ajax Library for ASP.NET
 * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
 * MagicUX is licensed as GPLv3.
 */

using System;

namespace Magix.UX.Builder
{
    public class Element : DeterministicExecutor
    {
        private HtmlBuilder _builder;
        private bool _closed;

        public Element(HtmlBuilder builder, string elementName)
        {
            _builder = builder;
            _builder.Writer.Write("<" + elementName);
            End = delegate 
            {
                this.CloseOpeningElement();
                this._builder.Writer.Write("</" + elementName + ">");
            };
        }

        public void AddAttribute(string name, string value)
        {
            if (_closed)
                throw new Exception("Can't add an attribute once the attribute is closed due to accessing the underlaying Writer or something else");
            _builder.WriterUnClosed.Write(" " + name + "=\"" + value + "\"");
        }

        public void Write(string content, params object[] args)
        {
            _builder.Writer.Write(content, args);
        }

        public void Write(string content)
        {
            _builder.Writer.Write(content);
        }

        internal void CloseOpeningElement()
        {
            if (_closed)
                return;
            _closed = true;
            _builder.Writer.Write(">");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _builder.Writer.Flush();
        }
    }
}

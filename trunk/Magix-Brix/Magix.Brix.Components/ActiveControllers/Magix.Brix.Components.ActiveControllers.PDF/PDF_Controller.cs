/*
 * Magix - A Modular-based Framework for building Web Applications 
 * Copyright 2010 - 2011 - Ra-Software, Inc. - thomas.hansen@winergyinc.com
 * Magix is licensed as GPLv3, or Commercially for Proprietary Projects through Ra-Software.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Magix.Brix.Loader;
using Magix.Brix.Types;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using Magix.UX;
using PdfSharp.Drawing;
using System.Xml;
using MigraDoc.DocumentObjectModel.Shapes;

namespace Magix.Brix.Components.ActiveControllers.PDF
{
    /**
     * Level1: Contains logic for creating PDF files and dowloading to Client 
     * or saving locally
     */
    [ActiveController]
    public class PDF_Controller : ActiveController
    {
        [ActiveEvent(Name = "Magix.PDF.CreatePDF")]
        protected void Magix_PDF_CreatePDF(object sender, ActiveEventArgs e)
        {
            // Creating Document
            Document document = new Document();
            document.Info.Title = e.Params["Title"].Get<string>();
            document.Comment = "This document is to be considered as Creative Commons Share-Alike. Copyright 2011 belongs to Ra-Software, Inc.";

            if (e.Params.Contains("FrontPage"))
            {
                Section front = document.AddSection();
                front.PageSetup.TopMargin = new Unit(0);
                front.PageSetup.BottomMargin = new Unit(0);
                front.PageSetup.LeftMargin = new Unit(0);
                front.PageSetup.RightMargin = new Unit(0);
                front.AddImage(Page.MapPath("~/" + e.Params["FrontPage"].Get<string>()));
            }

            // Creating Index section
            Section header = document.AddSection();
            header.PageSetup.StartingNumber = 1;
            header.Footers.Primary.AddParagraph().AddPageField();
            header.PageSetup.TopMargin = new Unit(50);
            header.PageSetup.BottomMargin = new Unit(50);
            header.PageSetup.LeftMargin = new Unit(50);
            header.PageSetup.RightMargin = new Unit(50);

            if (e.Params.Contains("Index"))
            {
                foreach (Node idx in e.Params["Index"])
                {
                    // Header
                    Paragraph paragraph = header.AddParagraph();
                    paragraph.AddLineBreak();

                    paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
                    paragraph.Format.Font.Size = new Unit(8, UnitType.Point);

                    paragraph.AddFormattedText(
                        idx["Header"].Get<string>(),
                        TextFormat.NotBold);

                    // Description
                    paragraph = header.AddParagraph();
                    paragraph.AddLineBreak();

                    paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 50);
                    paragraph.Format.Font.Size = new Unit(6, UnitType.Point);
                    paragraph.Format.Font.Italic = true;

                    paragraph.AddFormattedText(
                        idx["Description"].Get<string>(),
                        TextFormat.NotBold);
                }
            }

            if (e.Params.Contains("Pages"))
            {
                header = document.AddSection();
                header.PageSetup.TopMargin = new Unit(50);
                header.PageSetup.BottomMargin = new Unit(50);
                header.PageSetup.LeftMargin = new Unit(50);
                header.PageSetup.RightMargin = new Unit(50);
                header.AddPageBreak();

                foreach (Node idx in e.Params["Pages"])
                {
                    // Header
                    Paragraph paragraph = header.AddParagraph();
                    paragraph.AddLineBreak();

                    paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
                    paragraph.Format.Font.Size = new Unit(8, UnitType.Point);

                    paragraph.AddFormattedText(
                        idx.Name,
                        TextFormat.Bold);

                    paragraph.AddLineBreak();

                    foreach (Node idxP in idx)
                    {
                        ParseHTML(header, (idxP.Value ?? "").ToString());
                    }
                }
            }

            // Rendering document and redirecting client to document by popping up a
            // JS window ...
            PdfDocumentRenderer pdfRenderer = 
                new PdfDocumentRenderer(true, PdfFontEmbedding.Always);

            pdfRenderer.Document = document;
            pdfRenderer.RenderDocument();

            string fileToSaveTo = e.Params["File"].Get<string>();
            string filename = Page.MapPath("~/" + fileToSaveTo);

            pdfRenderer.PdfDocument.Save(filename);

            AjaxManager.Instance.WriterAtBack.Write("window.open('" + fileToSaveTo + "');");
        }

        private void ParseHTML(Section header, string html)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<doc>" + html.Replace("<em>", "").Replace("</em>", "") + "</doc>");

            foreach (XmlNode idx in doc.DocumentElement.ChildNodes)
            {
                switch (idx.Name)
                {
                    case "h1":
                        {
                            Paragraph paragraph = header.AddParagraph();
                            paragraph.AddLineBreak();

                            paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
                            paragraph.Format.Font.Size = new Unit(9, UnitType.Point);

                            paragraph.AddFormattedText(
                                idx.InnerText,
                                TextFormat.Bold);
                        } break;
                    case "h2":
                        {
                            Paragraph paragraph = header.AddParagraph();
                            paragraph.AddLineBreak();

                            paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
                            paragraph.Format.Font.Size = new Unit(8, UnitType.Point);

                            paragraph.AddFormattedText(
                                idx.InnerText,
                                TextFormat.Bold);
                        } break;
                    case "h3":
                        {
                            Paragraph paragraph = header.AddParagraph();
                            paragraph.AddLineBreak();

                            paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 100);
                            paragraph.Format.Font.Size = new Unit(7, UnitType.Point);

                            paragraph.AddFormattedText(
                                idx.InnerText,
                                TextFormat.Bold);
                        } break;
                    default: // Defaulting to paragraph ...
                        {
                            Paragraph paragraph = header.AddParagraph();
                            paragraph.AddLineBreak();

                            paragraph.Format.Font.Color = Color.FromCmyk(0, 0, 0, 50);
                            paragraph.Format.Font.Size = new Unit(6, UnitType.Point);

                            paragraph.AddFormattedText(
                                idx.InnerText,
                                TextFormat.NotBold);
                        } break;
                }
            }
        }
    }
}

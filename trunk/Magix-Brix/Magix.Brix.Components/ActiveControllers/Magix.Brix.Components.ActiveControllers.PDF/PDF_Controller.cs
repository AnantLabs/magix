/*
 * Magix - A Web Application Framework for Humans
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
     * Level2: Contains logic for creating PDF files and dowloading to Client 
     * or saving locally
     */
    [ActiveController]
    public class PDF_Controller : ActiveController
    {
        /**
         * Level2: Will create a PDF document and redirect the user to that document as named in
         * the 'File' parameter. Takes many different parameters;
         * 'FrontPage' [optional] being an image [600x846 px big] to a cover page you'd like to use.
         * 'Index' [optional] containing all components needed to build the books 'index'.
         * 'Chapters' contains a list of all the pages, with HTML parsing capabilities 
         * [one level only]
         */
        [ActiveEvent(Name = "Magix.PDF.CreatePDF-Book")]
        protected void Magix_PDF_CreatePDF_Book(object sender, ActiveEventArgs e)
        {
            // Creating a Document to hold the stuff
            Document doc = new Document();

            SetDocumentProperties(e.Params, doc);
            SetDocumentStyles(e.Params, doc);
            CreateFrontPage(e.Params, doc);
            CreateIndex(e.Params, doc);
            CreatePages(e.Params, doc);
            PrintDocumentAndRedirectClient(e.Params, doc);
        }

        private void CreatePages(Node node, Document doc)
        {
            int idxNo = 1;
            foreach (Node idx in node["Index"])
            {
                string name = idx.Name;
                if (!node["Pages"].Contains(name))
                {
                    throw new ArgumentException(@"Seems you've got an Index part of your 
PDF which doesn't exist in its Pages hierarchy. The name of the missing index was: " + name);
                }
                CreatePage(node["Pages"][name].Get<string>(), doc, idxNo);
                idxNo += 1;
            }
        }

        private void CreatePage(string html, Document docX, int inNo)
        {
            Section section = docX.AddSection();

            int idxNo = inNo;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<doc>" + html + "</doc>");

            foreach (XmlNode idx in doc.DocumentElement.ChildNodes)
            {
                string value = idx.InnerText;
                if (idxNo != -1)
                {
                    value = idxNo.ToString() + ". " + value;
                    idxNo = -1; // Only first element, regardless of what it is ...
                }
                switch (idx.Name)
                {
                    case "br":
                        {
                            section.AddParagraph("", "NormalNoMargs");
                        } break;
                    case "codenomargs":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Code2");
                        } break;
                    case "codenomargsbold":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Code3");
                        } break;
                    case "big":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Big");
                        } break;
                    case "h1":
                        {
                            section.AddPageBreak();
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Heading1");
                        } break;
                    case "hr":
                        {
                            section.AddPageBreak();
                        } break;
                    case "h2":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Heading2");
                        } break;
                    case "h3":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Heading3");
                        } break;
                    case "h4":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Heading4");
                        } break;
                    case "strong":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Strong");
                        } break;
                    case "em":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Italic");
                        } break;
                    case "p2":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "P2");
                        } break;
                    case "sec":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "SubSection");
                        } break;
                    case "code":
                        {
                            section.AddParagraph(value.Replace("\r\n", "").Replace("\n", ""), "Code");
                        } break;
                    case "codebig":
                        {
                            Paragraph p = section.AddParagraph();
                            p.Style = "Codebig";
                            p.AddText(value.Replace("\r\n", "").Replace("\n", ""));
                        } break;
                    case "img":
                        {
                            Image p = section.AddImage(Page.Server.MapPath("~/" + idx.Attributes["src"].Value.Trim('/')));
                            p.LockAspectRatio = true;
                            p.Height = new Unit(250);
                        } break;
                    case "ul":
                        {
                            foreach (XmlNode idxInner in idx.ChildNodes)
                            {
                                Paragraph p = section.AddParagraph();
                                p.Style = "ListofElements";
                                p.AddText(idxInner.InnerText.Replace("\r\n", "").Replace("\n", ""));
                            }
                        } break;
                    case "codesmall":
                        {
                            Paragraph p = section.AddParagraph();
                            p.Style = "Codesmall";
                            p.AddText(value.Replace("\r\n", "").Replace("\n", ""));
                        } break;
                    case "pre":
                        {
                            Paragraph p = section.AddParagraph();
                            p.Style = "PreFormatted";
                            p.AddFormattedText(value.Replace("   ", "\t"));
                        } break;
                    default: // Defaulting to paragraph ...
                        {
                            Paragraph p = section.AddParagraph();
                            p.Style = "Normal";
                            p.AddText(value);
                        } break;
                }
            }
        }

        /*
         * Helper for above
         */
        private static void SetDocumentProperties(Node node, Document doc)
        {
            string title = "Marvin's latest Graphogenic Epilepsy";
            if (node.Contains("Title"))
                title = node["Title"].Get<string>();
            doc.Info.Title = title;

            string author = "Marvin Siddharta";
            if (node.Contains("Author"))
                author = node["Author"].Get<string>();
            doc.Info.Author = author;
        }

        /*
         * Helper for above
         */
        private void SetDocumentStyles(Node node, Document doc)
        {
            // Settings default style according to input settings
            SetNormalStyle(node, doc);
            SetIndexStyles(node, doc);
            SetHeaderStyles(node, doc);
            SetMainPartStyle(node, doc);
            SetOtherStyles(node, doc);
        }

        private void SetOtherStyles(Node node, Document doc)
        {
            Style normal = doc.Styles["Normal"] as Style;
            normal.ParagraphFormat.Alignment = ParagraphAlignment.Justify;

            Style strong = doc.Styles.AddStyle("Strong", "Normal");
            strong.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            strong.Font.Bold = true;
            strong.Font.Color = new Color(255, 0, 0, 0);
            strong.ParagraphFormat.KeepWithNext = true;

            Style em = doc.Styles.AddStyle("Italic", "Normal");
            em.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            em.Font.Italic = true;
            em.ParagraphFormat.KeepWithNext = true;

            Style code = doc.Styles.AddStyle("Code", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.SpaceBefore = 40;
            code.ParagraphFormat.SpaceAfter = 0;
            code.Font = new Font("Courier", 10);

            code = doc.Styles.AddStyle("Code2", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.SpaceAfter = 0;
            code.ParagraphFormat.SpaceBefore = 0;
            code.ParagraphFormat.KeepTogether = true;
            code.Font = new Font("Courier", 10);

            code = doc.Styles.AddStyle("Code3", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.SpaceAfter = 0;
            code.ParagraphFormat.SpaceBefore = 0;
            code.ParagraphFormat.KeepTogether = true;
            code.Font = new Font("Courier", 10);
            code.Font.Color = new Color(235, 45, 45, 45);
            code.Font.Bold = true;

            code = doc.Styles.AddStyle("NormalNoMargs", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.SpaceAfter = 0;
            code.ParagraphFormat.SpaceBefore = 0;
            code.ParagraphFormat.KeepTogether = true;
            code.Font = new Font("Courier", 10);

            code = doc.Styles.AddStyle("Codebig", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.SpaceBefore = 10;
            code.ParagraphFormat.LeftIndent = 20;
            code.Font = new Font("Courier", 12);
            code.Font.Color = new Color(255, 0, 0, 0);

            code = doc.Styles.AddStyle("ListofElements", "Normal");
            code.ParagraphFormat = new ParagraphFormat();
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.SpaceBefore = 10;
            code.ParagraphFormat.LeftIndent = 45;
            code.ParagraphFormat.RightIndent = 65;
            code.ParagraphFormat.ListInfo.ListType = ListType.BulletList1;
            code.ParagraphFormat.ListInfo.NumberPosition = new Unit(25);
            code.ParagraphFormat.AddTabStop(new Unit(45), TabAlignment.Left);
            code.Font = new Font("Courier", 10);
            code.Font.Color = new Color(255, 0, 0, 0);

            code = doc.Styles.AddStyle("Codesmall", "Normal");
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.KeepTogether = true;
            code.Font = new Font("Courier", 10);

            code = doc.Styles.AddStyle("PreFormatted", "Normal");
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
            code.Font = new Font("Courier", 10);

            code = doc.Styles.AddStyle("SubSection", "Normal");
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.SpaceBefore = 50;
            code.Font.Color = new Color(255, 0, 0, 0);

            code = doc.Styles.AddStyle("P2", "Normal");
            code.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            code.ParagraphFormat.KeepWithNext = true;
            code.ParagraphFormat.KeepTogether = true;
            code.ParagraphFormat.SpaceAfter = 0;
            code.ParagraphFormat.SpaceBefore = 50;
            code.Font.Color = new Color(255, 0, 0, 0);
        }

        private void SetHeaderStyles(Node node, Document doc)
        {
            Style big = doc.Styles.AddStyle("Big", "Normal");
            big.Font.Size = 10;
            big.ParagraphFormat.SpaceBefore = 30;
            big.ParagraphFormat.PageBreakBefore = false;
            big.ParagraphFormat.KeepWithNext = true;
            big.Font.Bold = true;
            big.Font.Color = new Color(255, 0, 0, 0);

            Style h1 = doc.Styles["Heading1"];
            h1.Font.Size = 20;
            h1.ParagraphFormat.KeepWithNext = true;
            h1.ParagraphFormat.PageBreakBefore = false;
            h1.Font.Bold = true;
            h1.Font.Color = new Color(255, 0, 0, 0);

            Style h2 = doc.Styles["Heading2"];
            h2.Font.Size = 17;
            h2.ParagraphFormat.KeepWithNext = true;
            h2.ParagraphFormat.PageBreakBefore = false;
            h2.ParagraphFormat.SpaceBefore = 34;
            h2.Font.Bold = true;
            h2.Font.Color = new Color(255, 0, 0, 0);

            Style h3 = doc.Styles["Heading3"];
            h3.Font.Size = 15;
            h3.ParagraphFormat.KeepWithNext = true;
            h3.ParagraphFormat.PageBreakBefore = false;
            h3.ParagraphFormat.SpaceBefore = 30;
            h3.Font.Bold = true;
            h3.Font.Color = new Color(255, 0, 0, 0);

            Style h4 = doc.Styles["Heading4"];
            h4.Font.Size = 13;
            h4.ParagraphFormat.PageBreakBefore = false;
            h4.ParagraphFormat.KeepWithNext = true;
            h4.ParagraphFormat.SpaceBefore = 26;
            h4.Font.Bold = true;
            h4.Font.Color = new Color(255, 0, 0, 0);

            Style h5 = doc.Styles["Heading5"];
            h5.Font.Size = 11;
            h5.ParagraphFormat.PageBreakBefore = false;
            h5.ParagraphFormat.SpaceBefore = 22;
            h5.ParagraphFormat.KeepWithNext = true;
            h5.Font.Bold = true;
            h5.Font.Color = new Color(255, 0, 0, 0);
        }

        private static void SetNormalStyle(Node node, Document doc)
        {
            Style normal = doc.Styles["Normal"];

            if (node.Contains("FontName"))
                normal.Font.Name = node["FontName"].Get<string>();
            else
                normal.Font = new Font("Times New Roman", 12);

            // TODO: Figure out if this WORKS ...?
            if (node.Contains("Color"))
                normal.Font.Color = Color.Parse(node["Color"].Get<string>());
            else
                normal.Font.Color = new Color(180, 0, 0, 0); // Trying to save _some_ ink at least ...!

            Unit indent = 0;
            if (node.Contains("FirstLineIndent"))
                indent = node["FirstLineIndent"].Get<int>();
            normal.ParagraphFormat.FirstLineIndent = indent;

            Unit spaceAfter = normal.Font.Size;
            if (node.Contains("SpaceAfter"))
                spaceAfter = new Unit(node["SpaceAfter"].Get<int>());

            normal.ParagraphFormat.SpaceAfter = spaceAfter;
            normal.ParagraphFormat.KeepTogether = true;
        }

        private void SetIndexStyles(Node node, Document doc)
        {
            // 'Header'
            Style i = doc.Styles.AddStyle("IndexItem", "Normal");
            i.Font.Size = 13;
            i.Font.Bold = true;
            i.Font.Color = new Color(255, 0, 0, 0);
            i.ParagraphFormat.FirstLineIndent = 0;

            // Description of header
            i = doc.Styles.AddStyle("IndexItemDescription", "Normal");
            i.Font.Size = 12;
            i.Font.Color = new Color(150, 0, 0, 0);
            i.Font.Italic = true;
            i.ParagraphFormat.FirstLineIndent = 0;
        }

        private void SetMainPartStyle(Node node, Document doc)
        {
            Style i = doc.Styles.AddStyle("MainPart", "Normal");
            i.Font.Size = 50;
            i.Font.Bold = true;
            i.Font.Color = new Color(100, 0, 0, 0);
        }

        /*
         * Helper for above
         */
        private void CreateFrontPage(Node node, Document doc)
        {
            if (node.Contains("FrontPage"))
            {
                Section front = doc.AddSection();
                front.PageSetup.TopMargin = new Unit(0);
                front.PageSetup.BottomMargin = new Unit(0);
                front.PageSetup.LeftMargin = new Unit(0);
                front.PageSetup.RightMargin = new Unit(0);
                front.AddImage(Page.MapPath("~/" + node["FrontPage"].Get<string>()));
            }
            else
            {
                Section front = doc.AddSection();
                front.PageSetup.TopMargin = new Unit(0);
                front.PageSetup.BottomMargin = new Unit(0);
                front.PageSetup.LeftMargin = new Unit(0);
                front.PageSetup.RightMargin = new Unit(0);
                front.AddImage(Page.MapPath("~/media/images/default-pdf-cover.png"));
            }
        }

        /*
         * Helper for above
         */
        private void CreateIndex(Node node, Document doc)
        {
            if (node.Contains("Index"))
            {
                Section indexSection = doc.AddSection();

                indexSection.Footers.Primary.AddParagraph().AddPageField();

                indexSection.PageSetup.TopMargin = new Unit(70);
                indexSection.PageSetup.BottomMargin = new Unit(110);
                indexSection.PageSetup.LeftMargin = new Unit(80);
                indexSection.PageSetup.RightMargin = new Unit(80);

                indexSection.AddParagraph("Index", "Heading1");
                indexSection.AddParagraph(@"Find the name of the chapter you'd wish to read up on here, before diving into the 'big pie' ...", "Normal");

                int idxNo = 1;
                foreach (Node idx in node["Index"])
                {
                    string chapterName = idx["Header"].Get<string>();

                    if (string.IsNullOrEmpty(chapterName))
                        throw new ArgumentException("One of your Chapters didn't have a 'Header' for its Index value");

                    Paragraph iName = indexSection.AddParagraph();
                    iName.Style = "IndexItem";
                    iName.AddLineBreak(); // Adding ONE carriage return [space] BEFORE header text
                    iName.AddFormattedText(idxNo.ToString() + ". " + chapterName);
                    iName.Format.KeepWithNext = true;

                    string chapterDescription = "";

                    if (idx.Contains("Description"))
                    {
                        chapterDescription = idx["Description"].Get<string>();
                    }

                    if (!string.IsNullOrEmpty(chapterDescription))
                    {
                        Paragraph txt = indexSection.AddParagraph(chapterDescription, "IndexItemDescription");
                        txt.Format.KeepTogether = true;
                    }
                    idxNo += 1;
                }
            }
        }

        /*
         * Helper for above
         */
        private void PrintDocumentAndRedirectClient(Node node, Document doc)
        {
            // Rendering document and redirecting client to document by popping up a
            // JS window ...
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(
                true, 
                PdfFontEmbedding.Always);

            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();

            if (!node.Contains("File"))
                throw new ArgumentException("You need to specify a 'File' parameter for the PDF renderer to know where to render your PDF document ...");

            string fileToSaveTo = node["File"].Get<string>();
            string filename = Page.MapPath("~/" + fileToSaveTo);

            pdfRenderer.PdfDocument.Save(filename);

            AjaxManager.Instance.WriterAtBack.Write("window.open('" + fileToSaveTo + "');");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using org.pdfclown;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using org.pdfclown.documents.contents.entities;
using org.pdfclown.documents.interchange.metadata;

namespace GreenshotPlugin.Pdf
{
    public class PdfClownExporter : IPdfExporter
    {
        /*
            NOTE: File - is the low-level (syntactic) representation of a PDF file.
            NOTE: Document - is the high-level (semantic) representation of a PDF file.
        */
        public bool Export(Stream stream, System.Drawing.Image imageToSave, PdfInfo info)
        {
            //Step 1: Instantiate a PDF file           
            org.pdfclown.files.File file = new org.pdfclown.files.File();
            //Step 2: Save image to document
            Populate(file.Document, imageToSave);
            //Step 3: Set meta data (info)
            SetInfo(file.Document, info);
            //Step 4: Serialize the PDF file and save to file system
            file.Save(new org.pdfclown.bytes.Stream(stream), SerializationModeEnum.Incremental);
            return true;
        }

        private void SetInfo(Document document, PdfInfo pdfInfo)
        {
            Information info = document.Information;
            info.Clear();
            info.Author = pdfInfo.Author;
            info.Creator = pdfInfo.Creator;
            info.Subject = pdfInfo.Subject;
            info.Title = pdfInfo.Title;
            info.Keywords = pdfInfo.Keywords;
        }

        private void Populate(Document document, System.Drawing.Image image)
        {
            //page and image size
            float realWidth = image.Width / image.HorizontalResolution * 72;
            float realHeight = image.Height / image.VerticalResolution * 72;
            SizeF size = new SizeF(realWidth, realHeight);

            //Step 1: Add the page to the document
            Page page = new Page(document,size); // new page in document context
            document.Pages.Add(page); // add page to document

            //Step 2: Create a content composer to hold the contents of the file
            PrimitiveComposer composer = new PrimitiveComposer(page);

            composer.SetFont(
              new StandardType1Font(document,StandardType1Font.FamilyEnum.Courier,true,false),
              32); //set font
            MemoryStream memStream = new MemoryStream();
            image.Save(memStream, ImageFormat.Jpeg); //save image to stream in jpeg format
            composer.ShowXObject((new JpegImage(memStream)).ToXObject(document),new PointF(0,0),size); 

            //Step 3: finally flush the contents into the page
            composer.Flush();
        }
    }
}

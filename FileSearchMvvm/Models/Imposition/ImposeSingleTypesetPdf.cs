using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// NOTES ON READING FROM STREAM INSTEAD OF SAVING TO FILE:
/// 1. TO READ PDF INTO ITEXT's PDFREADER:
///    var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
/// 2. TO SAVE STREAM TO FILE
///    using (var fs = new System.IO.FileStream(
///        NEWFILENAME, System.IO.FileMode.Create, System.IO.FileAccess.Write))
///        {
///             fs.Write(stream.ToArray(), 0, stream.ToArray().Length);
///        }
/// THIS METHOD
/// </summary>
/// 
namespace FileSearchMvvm.Models.Imposition
{
    public class ImposeSingleTypesetPdf
    {
        #region Properties
        public TypeOfBindEnum BindType { get; set; }
        public bool HasCover { get; private set; }
        public int? LengthOfCover { get; private set; }
        public int OriginalPageCount { get; set; }
        public List<CockleFilePdf> ImposedFilesCreated { get; private set; }
        public System.IO.MemoryStream NewDocMemStream { get; private set; }
        public CockleFilePdf OrigCockleReadyFile { get; private set; }
        #endregion

        #region Constructors
        public ImposeSingleTypesetPdf(System.IO.MemoryStream orig_mem_stream, TypeOfBindEnum bind_type, bool hasCover)
        {
            // initialize properties
            BindType = bind_type;
            HasCover = hasCover;
            if(HasCover)
            {
                LengthOfCover = PdfCropAndNUp.StaticUtils.GetCoverLength_FirstPageTypesetPdf(orig_mem_stream);
            }
            // run imposition
            carryOutImposition(orig_mem_stream);
        }
        public ImposeSingleTypesetPdf(CockleFilePdf imposeReadyCocklePdf, TypeOfBindEnum bind_type, bool hasCover)
        {
            // interrupt if file doesn't exist
            if(!System.IO.File.Exists(imposeReadyCocklePdf.FullName)) return;

            // convert to stream
            System.IO.MemoryStream orig_mem_stream;
            using(var fs = new System.IO.FileStream(imposeReadyCocklePdf.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                orig_mem_stream = new System.IO.MemoryStream();
                fs.CopyTo(orig_mem_stream);
            }
            if(null == orig_mem_stream) return;

            // initialize properties
            OrigCockleReadyFile = imposeReadyCocklePdf;
            BindType = bind_type;
            HasCover = hasCover;
            if(HasCover)
            {
                if(null != OrigCockleReadyFile.CoverLength)
                { LengthOfCover = OrigCockleReadyFile.CoverLength; }
                else { LengthOfCover = PdfCropAndNUp.StaticUtils.GetCoverLength_FirstPageTypesetPdf(OrigCockleReadyFile.FullName); }
            }
            // run imposition
            carryOutImposition(orig_mem_stream);
        }
        #endregion

        #region Primary private method
        private void carryOutImposition(System.IO.MemoryStream orig_ms)
        {
            ImposedFilesCreated = new List<CockleFilePdf>();

            OriginalPageCount = new iTextSharp.text.pdf.PdfReader(orig_ms.ToArray()).NumberOfPages;

            // crop and impose
            switch(BindType)
            {
                case TypeOfBindEnum.SaddleStitch:
                    // with SS, assume that if multiple pages + cover == impose everything
                    // cover + 1 page == cover only
                    // no cover == brief only
                    if(OriginalPageCount == 1 && HasCover)// cover only
                    {
                        var cropped_stream = cropTypesetCover(orig_ms);
                        var imposed_stream = imposeTypesetSaddleStitchCover(cropped_stream);
                        NewDocMemStream = imposed_stream;
                    }
                    else if(OriginalPageCount > 1 && HasCover)
                    {
                        // get cover stream
                        var select_cv_stream = extractPages(orig_ms, 1, 1);
                        var cropped_cv_stream = cropTypesetCover(select_cv_stream);
                        var imposed_cv_stream = imposeTypesetSaddleStitchCover(cropped_cv_stream);
                        // check if user inserted blank ???
                        var imposed_cv_with_blank_icv_stream = addBlankPages(imposed_cv_stream, 1);

                        // account for user option: no blank page after cover or blank page after cover
                        // can only do this with typeset docs: no guarantee text with be identified on camera ready docs
                        var brief_start_page = 2; // check if 2 is blank: user may insert blank here
                        var is_page2_blank = PdfUtilities.IsPdfPageBlank(orig_ms, 2);
                        if(is_page2_blank) brief_start_page = 3;

                        // get brief stream
                        var select_br_stream = extractPages(orig_ms, brief_start_page, OriginalPageCount);
                        var cropped_br_stream = cropTypesetBrief(select_br_stream);

                        var numPagesToAdd = 0;
                        var brief_pages = OriginalPageCount - brief_start_page + 1;
                        if((brief_pages) % 4 != 0) { numPagesToAdd = 4 - ((brief_pages) % 4); }
                        System.IO.MemoryStream imposed_br_stream = null;
                        if(numPagesToAdd != 0)
                        {
                            var added_pages_stream = addBlankPages(cropped_br_stream, numPagesToAdd);
                            var reordered_stream = reorderSaddleStitchBrief(added_pages_stream);
                            imposed_br_stream = imposeTypesetSaddleStitchBrief(reordered_stream);
                        }
                        else
                        {
                            var reordered_stream = reorderSaddleStitchBrief(cropped_br_stream);
                            imposed_br_stream = imposeTypesetSaddleStitchBrief(reordered_stream);
                        }
                        NewDocMemStream = joinTwoStreamsIntoOnePdf(imposed_cv_with_blank_icv_stream, imposed_br_stream);
                    }
                    else if(!HasCover)
                    {
                        var cropped_stream = cropTypesetBrief(orig_ms);

                        // test for % 4 == 0
                        var numPagesToAdd = 0;
                        if(OriginalPageCount % 4 != 0) { numPagesToAdd = 4 - (OriginalPageCount % 4); }
                        if(numPagesToAdd != 0)
                        {
                            var added_pages_stream = addBlankPages(cropped_stream, numPagesToAdd);
                            var reordered_stream = reorderSaddleStitchBrief(added_pages_stream);
                            var imposed_stream = imposeTypesetSaddleStitchBrief(reordered_stream);
                            NewDocMemStream = imposed_stream;
                        }
                        else
                        {
                            var reordered_stream = reorderSaddleStitchBrief(cropped_stream);
                            var imposed_stream = imposeTypesetSaddleStitchBrief(reordered_stream);
                            NewDocMemStream = imposed_stream;
                        }
                    }
                    break;
                case TypeOfBindEnum.PerfectBind:
                    if(HasCover) // in this case, is cover
                    {
                        if(OriginalPageCount != 1)
                        {
                            throw new Exception("You are trying to impose a Perfect Bind Cover."
                                + "Perfect Bind Covers can only have 1 page.");
                        }
                        var cropped_stream = cropTypesetCover(orig_ms);
                        var imposed_stream = imposeTypesetPerfectBindCover(cropped_stream);
                        NewDocMemStream = imposed_stream;
                    }
                    else
                    {
                        var cropped_stream = cropTypesetBrief(orig_ms);
                        var imposed_stream = imposeTypesetPerfectBindBrief(cropped_stream);
                        NewDocMemStream = imposed_stream;
                    }
                    break;
            }
        }
        #endregion

        #region Secondary private methods
        private int getCoverLength(string fullName)
        {
            throw new NotImplementedException();
        }

        private System.IO.MemoryStream extractPages(System.IO.MemoryStream stream,
            int start_page = 1, int end_page = 1)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var doc = new iTextSharp.text.Document();
                var writer = new iTextSharp.text.pdf.PdfCopy(doc, dest_stream);
                doc.Open();

                var reader = new iTextSharp.text.pdf.PdfReader(stream.ToArray());
                for(var i = start_page; i <= end_page; i++)
                {
                    var page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                reader.Close();
                doc.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }

        private System.IO.MemoryStream joinTwoStreamsIntoOnePdf(System.IO.MemoryStream stream_1, System.IO.MemoryStream stream_2)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var doc = new iTextSharp.text.Document();
                var writer = new iTextSharp.text.pdf.PdfCopy(doc, dest_stream);
                doc.Open();

                var reader_1 = new iTextSharp.text.pdf.PdfReader(stream_1.ToArray());
                for(var i = 1; i <= reader_1.NumberOfPages; i++)
                {
                    var page = writer.GetImportedPage(reader_1, i);
                    writer.AddPage(page);
                }
                reader_1.Close();

                var reader_2 = new iTextSharp.text.pdf.PdfReader(stream_2.ToArray());
                for(var i = 1; i <= reader_2.NumberOfPages; i++)
                {
                    var page = writer.GetImportedPage(reader_2, i);
                    writer.AddPage(page);
                }
                reader_2.Close();
                doc.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream addBlankPages(System.IO.MemoryStream orig_stream, int num_to_add = 1)
        {
            if(num_to_add == 0) return orig_stream;

            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();


                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                var stamper = new iTextSharp.text.pdf.PdfStamper(reader, dest_stream);
                for(var i = 1; i <= num_to_add; i++)
                {
                    stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                }
                stamper.Close();
                reader.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream reorderSaddleStitchBrief(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();



                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                var order = new SaddleStitchPageOrder(reader.NumberOfPages);
                reader.SelectPages(order.PageOrder);
                var pdfdoc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));
                var pdfcopy_provider = new iTextSharp.text.pdf.PdfCopy(pdfdoc, dest_stream);
                pdfdoc.Open();
                iTextSharp.text.pdf.PdfImportedPage importedPage;
                for(int i = 1; i <= reader.NumberOfPages; i++)
                {
                    importedPage = pdfcopy_provider.GetImportedPage(reader, i);
                    pdfcopy_provider.AddPage(importedPage);
                }
                pdfdoc.Close();
                reader.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream imposeTypesetSaddleStitchBrief(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, dest_stream);
                docB4.Open();

                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());

                for(int i = 1; i <= reader.NumberOfPages; i += 2)
                {
                    if(i != 1) docB4.NewPage();
                    docB4.Add(new iTextSharp.text.Chunk());

                    iTextSharp.text.pdf.PdfTemplate tLeft = writer.GetImportedPage(reader, i);
                    iTextSharp.text.pdf.PdfTemplate tRight = writer.GetImportedPage(reader, i + 1);

                    iTextSharp.text.pdf.PdfContentByte contentbyte = writer.DirectContent;

                    // MEASUREMENTS GOOD: TESTED 11/16/2015; RETESTED TO MATCH 951s, 11/17/2015
                    // RETESTED AGAIN FOR 951A, where most saddle stitches will print
                    contentbyte.AddTemplate(tLeft, /*113f*/ 108f /*110f*/, -126f); // position for left side of B4 sheet
                    contentbyte.AddTemplate(tRight, /*554.5f*/ /*550.5f*/ 554.5f, -126f); // position for right side of B4 sheet
                }
                docB4.Close();
                reader.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream imposeTypesetSaddleStitchCover(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f));
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, dest_stream);
                docB4.Open();

                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                iTextSharp.text.pdf.PdfContentByte contentbyte;

                docB4.Add(new iTextSharp.text.Chunk());
                iTextSharp.text.pdf.PdfTemplate tCover = writer.GetImportedPage(reader, 1);
                contentbyte = writer.DirectContent;

                // default values
                float x_coordinate = /*550f*/ 554.5f /*17-32 pages*/, y_coordinate = -76.25f /*48 pica*/;

                switch(reader.NumberOfPages - 1 / 16)
                {
                    case 0: //(01 - 16)
                        x_coordinate = 551f; // NOT SURE ABOUT THIS ONE
                        break;
                    default:
                    case 1: //(17 - 32)
                        x_coordinate = 554.5f; // GOOD
                        break;
                    case 2: //(33 - 48)
                        x_coordinate = 558f; // TESTED ON 951A, 11/17/2015
                        break;
                    case 3: //(49 - 64)
                        x_coordinate = /*558f*/ 561.5f; // GOOD
                        break;
                    case 4: //(65 - 80)
                        x_coordinate = /*562f*/ 564f;
                        break;
                }

                switch(LengthOfCover)
                {
                    default:
                    case 48:
                        y_coordinate = -76.25f; // GOOD
                        break;
                    case 49:
                        y_coordinate = -70.5f;
                        break;
                    case 50:
                        y_coordinate = -64.5f;
                        break;
                    case 51:
                        y_coordinate = -58.5f; // GOOD
                        break;
                }

                contentbyte.AddTemplate(tCover, x_coordinate, y_coordinate);

                docB4.Close();
                reader.Close();
                writer.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream imposeTypesetPerfectBindCover(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f));
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, dest_stream);
                docB4.Open();

                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                iTextSharp.text.pdf.PdfContentByte contentbyte;

                docB4.Add(new iTextSharp.text.Chunk());
                iTextSharp.text.pdf.PdfTemplate tCover = writer.GetImportedPage(reader, 1);
                contentbyte = writer.DirectContent;

                // default values: in PB briefs, y_coordinate will not vary
                float x_coordinate = 575.5f, y_coordinate = -76.5f;

                switch(LengthOfCover)
                {
                    default:
                    case 48: // GOOD: TESTED 11/16/15
                        y_coordinate = -88.5f;
                        break;
                    case 49: // STILL AN ESTIMATE
                        y_coordinate = -82.5f;
                        break;
                    case 50: // GOOD: TESTED 11/16/15
                        y_coordinate = -76.5f;
                        break;
                    case 51: // GOOD: TESTED 11/16/15
                        y_coordinate = -70.5f;
                        break;
                }
                contentbyte.AddTemplate(tCover, x_coordinate, y_coordinate);

                docB4.Close();
                reader.Close();
                writer.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream imposeTypesetPerfectBindBrief(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();
                var docB5 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(515.76f, 728.4f));
                var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB5, dest_stream);
                docB5.Open();
                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());

                for(int i = 1; i <= reader.NumberOfPages; i++)
                {
                    if(i != 1) docB5.NewPage();
                    docB5.Add(new iTextSharp.text.Chunk());

                    iTextSharp.text.pdf.PdfTemplate t = writer.GetImportedPage(reader, i);

                    iTextSharp.text.pdf.PdfContentByte contentbyte = writer.DirectContent;

                    // MEASUREMENTS GOOD: TESTED 11/16/2015
                    if(i % 2 == 1)
                    {
                        contentbyte.AddTemplate(t, /*42.5f*/ 35.5f, -141.75f); // position for front side of B5 sheet
                    }
                    else if(i % 2 == 0)
                    {
                        contentbyte.AddTemplate(t, /*110f*/ 110f, -141.75f); // position for back side of B5 sheet
                    }
                }
                docB5.Close();
                reader.Close();
                writer.Close();
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        private System.IO.MemoryStream cropTypesetBrief(System.IO.MemoryStream orig_stream)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();
                var croppedRectangle = new iTextSharp.text.pdf.PdfRectangle(377f, 189f, 0f, 792f);
                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                for(int i = 1; i <= reader.NumberOfPages; i++)
                {
                    iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageN(i);
                    pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, croppedRectangle);
                }
                var stamper = new iTextSharp.text.pdf.PdfStamper(reader, dest_stream);
                stamper.Close();
                reader.Close();
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); }
            return dest_stream;
        }
        private System.IO.MemoryStream cropTypesetCover(System.IO.MemoryStream orig_stream)
        {
            {
                // works for both saddle stitch and perfect bind
                System.IO.MemoryStream dest_stream = null;
                try
                {
                    dest_stream = new System.IO.MemoryStream();
                    iTextSharp.text.pdf.PdfRectangle croppedRectangleCover;
                    switch(LengthOfCover)
                    {
                        case 48:
                        default:
                            croppedRectangleCover = new iTextSharp.text.pdf.PdfRectangle(356f, 130f, 21f, 741f);
                            break;
                        case 49:
                            croppedRectangleCover = new iTextSharp.text.pdf.PdfRectangle(356f, 120f, 21f, 741f);
                            break;
                        case 50:
                            croppedRectangleCover = new iTextSharp.text.pdf.PdfRectangle(356f, 108f, 21f, 741f);
                            break;
                        case 51:
                            croppedRectangleCover = new iTextSharp.text.pdf.PdfRectangle(356f, 92f, 21f, 741f);
                            break;
                    }

                    var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
                    var pageDict = reader.GetPageN(1);
                    pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, croppedRectangleCover);
                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, dest_stream);
                    stamper.Close();
                    reader.Close();

                }
                catch(Exception excpt)
                {
                    System.Diagnostics.Debug.WriteLine(excpt);
                }
                return dest_stream;
            }
        }
        #endregion
    }
}
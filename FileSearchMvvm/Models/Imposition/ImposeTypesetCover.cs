using System;
using FileSearchMvvm.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;

namespace FileSearchMvvm.Models.Imposition
{
    internal class ImposeTypesetCover
    {
        public CockleFilePdf ImposedTypesetCover { get; private set; }

        #region Constructor
        public ImposeTypesetCover(CockleFilePdf src, int cv_len, bool is_typeset, bool is_saddlestitch, int page_count)
        {
            // variables
            CockleFilePdf original_file = src;
            int cover_length = cv_len;

            // new files to create
            string temp_cropped = String.Empty;
            string newSaddleStitchCoverName = String.Empty;
            string newPerfectBindCoverName = String.Empty;

            // first, need a cropped page (camera ready should already be cropped by user)
            if (is_typeset)
            {
                temp_cropped = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(original_file.FullName), "temp cropped cv.pdf");
                cropTypesetCoverAndInsideCover(original_file.FullName, temp_cropped, cover_length);
            }
            else
            {
                // no need to crop if camera ready: rely on user to do that
                temp_cropped = original_file.FullName;
            }


            if (!String.IsNullOrEmpty(temp_cropped))
            {
                // name new file and CockleFilePdf
                if (original_file.TicketNumber == null || original_file.Attorney == null)
                {
                    if (is_saddlestitch)
                    {
                        newSaddleStitchCoverName = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(original_file.FullName), string.Format("Imposed Saddle Stitch Cover.pdf"));
                        ImposedTypesetCover = new CockleFilePdf(newSaddleStitchCoverName, SourceFileTypeEnum.Imposed_Cover);

                        this.layoutCroppedCoverAndInsideCover_SaddleStitch(
                            temp_cropped, newSaddleStitchCoverName, is_typeset, page_count, cover_length);
                    }
                    else
                    {
                        newPerfectBindCoverName = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(original_file.FullName), string.Format("Imposed Perfect Bind Cover.pdf"));
                        ImposedTypesetCover = new CockleFilePdf(newSaddleStitchCoverName, SourceFileTypeEnum.Imposed_Cover);

                        this.layoutCroppedCoverAndInsideCover_PerfectBind(
                            temp_cropped, newPerfectBindCoverName, is_typeset, cover_length);
                    }
                }
                else
                {
                    if (is_saddlestitch)
                    {
                        newSaddleStitchCoverName = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(original_file.FullName),
                            string.Format("{0} {1} cv B4 SS pr.pdf",
                            original_file.TicketNumber,
                            original_file.Attorney));
                        ImposedTypesetCover = new CockleFilePdf(
                            newSaddleStitchCoverName, original_file.Attorney, original_file.TicketNumber, SourceFileTypeEnum.Imposed_Cover, "pdf");
                        layoutCroppedCoverAndInsideCover_SaddleStitch(
                            temp_cropped, newSaddleStitchCoverName, is_typeset, page_count, cover_length);
                    }
                    else
                    {
                        newPerfectBindCoverName = System.IO.Path.Combine(
                            System.IO.Path.GetDirectoryName(original_file.FullName),
                            string.Format("{0} {1} cv B4 PB pr.pdf",
                            original_file.TicketNumber,
                            original_file.Attorney));
                        ImposedTypesetCover = new CockleFilePdf(
                            newSaddleStitchCoverName, original_file.Attorney, original_file.TicketNumber, SourceFileTypeEnum.Imposed_Cover, "pdf");

                        layoutCroppedCoverAndInsideCover_PerfectBind(
                            temp_cropped, newPerfectBindCoverName, is_typeset, cover_length);
                    }
                }
            }
        }
        #endregion

        #region Private utilities
        private bool layoutCroppedCoverAndInsideCover_PerfectBind(
            string src, string dest, bool is_typeset, int? cover_length)
        {
            try
            {
                using (var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    Document docB4 = new Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                    PdfWriter writer = PdfWriter.GetInstance(docB4, stream);
                    docB4.Open();

                    PdfReader reader = new PdfReader(src);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfContentByte contentbyte;
                        if (i == 1)
                        {
                            docB4.Add(new Chunk());
                            PdfTemplate tCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            // basic positioning: need to account for page size in future

                            // default values: in PB briefs, y_coordinate will not vary
                            float x_coordinate = 575.5f, y_coordinate = -76.5f;

                            switch (cover_length)
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
                        }
                        if (i == 2)
                        {
                            // not set up for multiple inside cover pages yet !!!
                            docB4.NewPage();
                            docB4.Add(new Chunk());
                            PdfTemplate tInsideCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            contentbyte.AddTemplate(tInsideCover, 113f, -126f);
                        }

                    }
                    docB4.Close();
                    reader.Close();
                    writer.Close();
                }
                System.IO.File.Delete(src);
                return true;
            }
            catch (Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        private bool layoutCroppedCoverAndInsideCover_SaddleStitch(
            string src, string dest, bool isTypeset, int? page_count, int? cover_length)
        {
            try
            {
                using (var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    Document docB4 = new Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                    PdfWriter writer = PdfWriter.GetInstance(docB4, stream);
                    docB4.Open();

                    PdfReader reader = new PdfReader(src);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfContentByte contentbyte;
                        if (i == 1)
                        {
                            docB4.Add(new Chunk());

                            // typeset positioning
                            if (isTypeset)
                            {
                                PdfTemplate tCover = writer.GetImportedPage(reader, i);
                                contentbyte = writer.DirectContent;

                                // default values for typeset
                                float x_coordinate = 554.5f, y_coordinate = -76.25f;

                                switch (page_count - 1 / 16)
                                {
                                    case 0: //(01 - 16)
                                        x_coordinate = 551f;
                                        break;
                                    default:
                                    case 1: //(17 - 32)
                                        x_coordinate = 554.5f;
                                        break;
                                    case 2: //(33 - 48)
                                        x_coordinate = 558f;
                                        break;
                                    case 3: //(49 - 64)
                                        x_coordinate = 561.5f;
                                        break;
                                    case 4: //(65 - 80)
                                        x_coordinate = 564f;
                                        break;
                                }

                                switch (cover_length)
                                {
                                    default:
                                    case 48:
                                        y_coordinate = -76.25f;
                                        break;
                                    case 49:
                                        y_coordinate = -70.5f;
                                        break;
                                    case 50:
                                        y_coordinate = -64.5f;
                                        break;
                                    case 51:
                                        y_coordinate = -58.5f;
                                        break;
                                }

                                contentbyte.AddTemplate(tCover, x_coordinate, y_coordinate);
                            }
                            // camera ready positioning
                            else
                            {
                                PdfTemplate tCover = writer.GetImportedPage(reader, i);
                                contentbyte = writer.DirectContent;

                                // figure out width: 1031.76f is width of B4 sheet
                                float x_left_margin = 1031.76f / 2;
                                float x_right_margin = (1031.76f / 2) + 441 /*6.125 inches in points*/;

                                // cut off 0.87 inches of paper from 10.12 inch height sheet
                                //float total_paper_cut_from_sheet = 62.64f;
                                float y_top_margin = 728.64f - 31.32f;
                                float y_bottom_margin = 31.32f;

                                // get page size info
                                var page_size = reader.GetPageSizeWithRotation(i);
                                var crop_size = reader.GetCropBox(i);
                                float crop_width = crop_size.Width;
                                float crop_height = crop_size.Height;

                                float crop_llx = crop_size.Left;
                                float crop_lly = crop_size.Bottom;

                                // now, figure out how to center on page
                                float x_difference = (x_right_margin - x_left_margin) - crop_width;
                                float y_difference = (y_top_margin - y_bottom_margin) - crop_height;

                                float x_coordinate = x_left_margin + (x_difference / 2);
                                float y_coordinate = y_bottom_margin + (y_difference / 2);

                                // now, readjust for crop
                                // iow, subtract original llx crop from x_coordinate
                                //      and lly crop from y_coordinate
                                x_coordinate = x_coordinate - crop_llx;
                                y_coordinate = y_coordinate - crop_lly;

                                contentbyte.AddTemplate(tCover, x_coordinate, y_coordinate);
                            }
                        }
                        if (i == 2 && isTypeset)
                        {
                            // not set up for multiple inside cover pages yet !!!
                            docB4.NewPage();
                            docB4.Add(new Chunk());
                            PdfTemplate tInsideCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            contentbyte.AddTemplate(tInsideCover, 113f, -126f);
                        }
                    }
                    docB4.Close();
                    reader.Close();
                    writer.Close();
                }
                if (isTypeset)
                {
                    System.IO.File.Delete(src);
                }
                return true;
            }
            catch (Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }

        private bool cropTypesetCoverAndInsideCover(string src, string dest, int? coverLength)
        {
            // works for both saddle stitch and perfect bind
            try
            {
                using (var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    PdfRectangle croppedRectangleCover;
                    switch (coverLength)
                    {
                        case 48:
                        default:
                            croppedRectangleCover = new PdfRectangle(356f, 130f, 21f, 741f);
                            break;
                        case 49:
                            croppedRectangleCover = new PdfRectangle(356f, 120f, 21f, 741f);
                            break;
                        case 50:
                            croppedRectangleCover = new PdfRectangle(356f, 108f, 21f, 741f);
                            break;
                        case 51:
                            croppedRectangleCover = new PdfRectangle(356f, 92f, 21f, 741f);
                            break;
                    }
                    PdfRectangle croppedRectangleInsideCover = new PdfRectangle(377f, 189f, 0f, 792f);

                    PdfReader reader = new PdfReader(src);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfDictionary pageDict = reader.GetPageN(i);
                        if (i == 1)
                        {
                            pageDict.Put(PdfName.CROPBOX, croppedRectangleCover);
                        }
                        else
                        {
                            pageDict.Put(PdfName.CROPBOX, croppedRectangleInsideCover);
                        }
                    }

                    var stamper = new PdfStamper(reader, stream);
                    stamper.Close();
                    reader.Close();
                }
                return true;
            }
            catch (Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion
    }
}

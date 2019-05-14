using System;
using System.Collections.Generic;
using System.Linq;
using FileSearchMvvm.Models;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;

namespace FileSearchMvvm.Models.Imposition
{
    class PdfUtilities
    {
        public static bool IsPdfPageBlank(System.IO.MemoryStream pdf_stream, int page)
        {
            var text_found = false;
            try
            {
                var reader = new iTextSharp.text.pdf.PdfReader(pdf_stream.ToArray());
                var strat = new Models.Utilities.iTextSharpUtilities.MyLocationTextExtractionStrategy();
                var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, strat);
                var re = new System.Text.RegularExpressions.Regex(@"[a-zA-Z0-9]");
                if(!re.IsMatch(text))
                {
                    text_found = true;
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }

            return text_found;
        }
        public static bool IsPdfPageBlank(string pdf_file, int page)
        {
            var text_found = false;
            try
            {
                if(!System.IO.File.Exists(pdf_file)) throw new Exception("File does not exist.");

                var reader = new iTextSharp.text.pdf.PdfReader(pdf_file);
                var strat = new Models.Utilities.iTextSharpUtilities.MyLocationTextExtractionStrategy();

                var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page, strat);
                var re = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\s,]*$");
                if(!re.IsMatch(text))
                {
                    text_found = true;
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }

            return text_found;
        }

        #region GenerateNewFileName
        public static string GenerateFilenameForNewPdf(string directory, string custom_text, string ticket_atty = "")
        {
            var newfilename = string.Empty;
            var i = 0;
            try
            {
                do
                {
                    if(i == 0)
                    {
                        if(!string.IsNullOrEmpty(ticket_atty))
                        {
                            newfilename = System.IO.Path.Combine(directory, ticket_atty + " " + custom_text + ".pdf");
                        }
                        else
                        {
                            newfilename = System.IO.Path.Combine(directory, custom_text + ".pdf");
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(ticket_atty))
                        {
                            newfilename = System.IO.Path.Combine(directory, ticket_atty + " " + custom_text + "_" + i + ".pdf");
                        }
                        else
                        {
                            newfilename = System.IO.Path.Combine(directory, custom_text + "_" + i + ".pdf");
                        }
                    }
                    i++;
                } while(System.IO.File.Exists(newfilename));

                if(string.IsNullOrEmpty(newfilename)) throw new Exception();
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
            return newfilename;
        }
        #endregion


        #region File Save Utilities
        public static bool SaveToPdfPossibles(string src)
        {
            string dest;
            if(System.IO.Directory.Exists(@"L:\PDF Possible Orders"))
            {
                dest = System.IO.Path.Combine(@"L:\PDF Possible Orders", System.IO.Path.GetFileName(src));
                System.IO.File.Copy(src, dest, true);
                return true;
            }
            else if(System.IO.Directory.Exists(@"\\CLBDC02\Printing\PDF Possible Orders"))
            {
                dest = System.IO.Path.Combine(@"\\CLBDC02\Printing\PDF Possible Orders", System.IO.Path.GetFileName(src));
                System.IO.File.Copy(src, dest, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool SaveToDocsReadyToPrint(string src)
        {
            string dest;
            if(System.IO.Directory.Exists(@"L:\Documents Ready to Print"))
            {
                dest = System.IO.Path.Combine(@"L:\Documents Ready to Print", System.IO.Path.GetFileName(src));
                System.IO.File.Copy(src, dest, true);
                return true;
            }
            else if(System.IO.Directory.Exists(@"\\CLBDC02\Printing\Documents Ready to Print"))
            {
                dest = System.IO.Path.Combine(@"\\CLBDC02\Printing\Documents Ready to Print", System.IO.Path.GetFileName(src));
                System.IO.File.Copy(src, dest, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region General Utilities
        public static System.IO.MemoryStream ConvertPdfFileToStream(string file_name)
        {
            if(!System.IO.File.Exists(file_name)) throw new Exception("File does not exist.");

            System.IO.MemoryStream dest_stream = null;
            try
            {
                using(var fs = new System.IO.FileStream(file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    dest_stream = new System.IO.MemoryStream();
                    fs.CopyTo(dest_stream);
                }
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }
        public static System.IO.MemoryStream ExtractPdfPagesToStream(System.IO.MemoryStream orig_stream,
            int start_page = 1, int end_page = 1)
        {
            System.IO.MemoryStream dest_stream = null;
            try
            {
                dest_stream = new System.IO.MemoryStream();

                var doc = new iTextSharp.text.Document();
                var writer = new iTextSharp.text.pdf.PdfCopy(doc, dest_stream);
                doc.Open();

                var reader = new iTextSharp.text.pdf.PdfReader(orig_stream.ToArray());
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
        public static int GetTicketNumberFromListOfFiles(List<string> Files)
        {
            List<int> ticket_list = new List<int>();

            foreach(var f in Files)
            {
                string short_f = System.IO.Path.GetFileNameWithoutExtension(f);
                string short_f_ticket_only = short_f.Substring(0, short_f.IndexOf(' '));

                int out_ticket;
                if(int.TryParse(short_f_ticket_only, out out_ticket))
                {
                    ticket_list.Add(out_ticket);
                }
            }
            int common = -1;
            for(int i = 0; i < ticket_list.Count; i++)
            {
                if(i == 0)
                {
                    common = ticket_list[i];
                }
                if(common != ticket_list[i])
                {
                    common = -1;
                    break;
                }
            }
            return common;
        }
        public static string GetAttorneyNameFromListOfFiles(List<string> Files)
        {
            // create string array of files, including only (1) attorney & (2) file type
            List<string> ss1 = new List<string>();
            foreach(var f in Files)
            {
                string short_f = System.IO.Path.GetFileNameWithoutExtension(f);
                string short_f_without_ticket = short_f.Substring(short_f.IndexOf(' ') + 1, short_f.Length - short_f.IndexOf(' ') - 1);
                string short_f_without_version = short_f_without_ticket.Substring(0, short_f_without_ticket.LastIndexOf(' '));

                ss1.Add(short_f_without_version);

            }

            // return of an empty string indicates failure
            if(ss1.Count == 0) { return ""; }
            // return simple removal of type if only one file (cannot carry out comparisons if only 1)
            if(ss1.Count == 1)
            {
                return ss1[0].Substring(0, ss1[0].LastIndexOf(' '));
            }

            // initialize prefix length
            int prefixLength = 0;

            // get a test string, taking care that test_string is not a null reference
            string test_string = "";
            for(int k = 0; k < ss1.Count; k++)
            {
                if(string.IsNullOrEmpty(test_string) && !string.IsNullOrEmpty(ss1[k]))
                {
                    test_string = ss1[k];
                    continue;
                }
                else if(!string.IsNullOrEmpty(test_string) &&
                    test_string.Substring(0, test_string.IndexOf(' ')).Equals(ss1[k].Substring(0, ss1[k].IndexOf(' '))))
                {
                    break;
                }
                else if(!string.IsNullOrEmpty(ss1[k]))
                {
                    test_string = ss1[k];
                    continue;
                }
            }
            if(string.IsNullOrEmpty(test_string)) return "";

            // test all file shortened names against test_string
            foreach(char c in test_string)
            {
                foreach(string s in ss1)
                {
                    // skip null or empty strings
                    if(string.IsNullOrEmpty(s)) continue;

                    // final test
                    if(s.Length <= prefixLength || s[prefixLength] != c)
                    {
                        // Nov 12: had problem with 32148 Casto aa 0 files
                        // No brief, index or cover, so Attorney's name became 'Castro a'
                        if(test_string.Substring(prefixLength - 2, 2).Equals(" a") &&
                            ss1.TrueForAll(f =>
                            {
                                string[] a = f.Split(' ');
                                if(a.Length == 2) return true; else return false;
                            }))
                        {
                            return test_string.Substring(0, prefixLength - 2);
                        }

                        // remove trailing space, if present
                        if(test_string[prefixLength - 1].Equals(' '))
                        {
                            return test_string.Substring(0, prefixLength - 1);
                        }
                        else
                        {
                            return test_string.Substring(0, prefixLength);
                        }
                    }
                }
                prefixLength++;
            }

            return test_string; // all strings identical
        }
        public static bool AddBlankPage(string src)
        {
            string dest = (System.IO.Path.GetDirectoryName(src) + @"\temp " + DateTime.Now.ToString("ddMMyyyyhhmmssffff"));
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);
                    iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, stream);
                    stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                    stamper.Close();
                    reader.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion

        #region Combined Utilities
        public static bool CombineBriefPages_AddingBlanks(
            List<CockleFilePdf> srcFiles, string src, TypeOfBindEnum bind)
        {
            // new attempt Dec 28, to account for divider pages (or any page without text)
            // text has to start on odd-numbered page, if followed by divider page

            // first, add 2 pages for each divider page, to account for front and back.
            // then, when everything is together, cycle through doc to add extra dividers...
            // ... so that text always falls on odd-numbered page


            // should work for both Saddle Stitch and Perfect Bind


            // create new list without cover, ordered by rank
            List<CockleFilePdf> files = new List<CockleFilePdf>(
                srcFiles
                .Where(f => f.FileType != SourceFileTypeEnum.Cover)
                .Where(f => f.FileType != SourceFileTypeEnum.InsideCv)
                .Where(f => f.FileType != SourceFileTypeEnum.SidewaysPage)
                .Where(f => f.FileType != SourceFileTypeEnum.Brief_Foldout)
                .Where(f => f.FileType != SourceFileTypeEnum.Brief_ZFold)
                .Where(f => f.FileType != SourceFileTypeEnum.App_Foldout)
                .Where(f => f.FileType != SourceFileTypeEnum.App_ZFold)
                .Where(f => f.FileType != SourceFileTypeEnum.Unrecognized)
                .OrderBy(f => f.Rank));

            if(files.Count < 1) return false;

            // what if files.Count == 1 ??? just return ???

            int pageCount = 0;
            bool hasDividers = false;
            bool firstAppFileFound = false;
            int firstPageOfApp = -1;

            try
            {
                using(var stream = new System.IO.FileStream(src, System.IO.FileMode.Create))
                {
                    // initiate iTextSharp processes
                    iTextSharp.text.Document pdfdoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
                    iTextSharp.text.pdf.PdfCopy pdfcopy = new iTextSharp.text.pdf.PdfCopy(pdfdoc, stream);
                    pdfdoc.Open();

                    // merge pdfs in folder
                    CockleFilePdf f;
                    for(int i = 0; i < files.Count; i++)
                    {
                        f = files[i];
                        // read file
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(f.FullName);
                        int filePageCount = reader.NumberOfPages;

                        // set up pdfstamper
                        iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, stream);

                        // look for divider pages here, add blank if exists
                        List<int> divider_pages = new List<int>();
                        iTextSharp.text.pdf.parser.PdfReaderContentParser parser = new iTextSharp.text.pdf.parser.PdfReaderContentParser(reader);
                        for(int j = 1; j <= reader.NumberOfPages; j++)
                        {
                            iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy extract = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                            var extractedText = parser.ProcessContent(j, extract);
                            string textFromPage = extractedText.GetResultantText();

                            int cnt = textFromPage.ToCharArray().Count();
                            int mch_cnt = System.Text.RegularExpressions.Regex.Matches(textFromPage, @"A(PPENDIX|ppendix)").Count;

                            if(System.Text.RegularExpressions.Regex.Matches(textFromPage, @"\S").Count == 0)
                            {
                                // collect blank pages
                                divider_pages.Add(j);
                            }
                            else if(cnt < 50 && mch_cnt > 0)
                            {
                                // collect other divider pages
                                divider_pages.Add(j);
                            }
                        }
                        if(divider_pages.Count > 0)
                        {
                            hasDividers = true;

                            int k = 0; // adjust for total page number change
                            foreach(int page in divider_pages)
                            {
                                stamper.InsertPage(page + k, reader.GetPageSizeWithRotation(1));
                                filePageCount = reader.NumberOfPages;
                                k++;
                            }
                        }

                        // add blank page if needed to make even number
                        if(files[i].FileType == SourceFileTypeEnum.Index
                            || files[i].FileType == SourceFileTypeEnum.Brief
                            || files[i].FileType == SourceFileTypeEnum.App_Index
                            || files[i].FileType == SourceFileTypeEnum.Motion
                            || files[i].FileType == SourceFileTypeEnum.Divider_Page)
                        {
                            f.AssignNeedsBlankPage(files, reader.NumberOfPages);
                            if(f.NeedsBlankPage)
                            {
                                //PdfStamper stamper2 = new PdfStamper(reader, stream);
                                stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                                filePageCount = reader.NumberOfPages;
                            }
                        }

                        // with last document in 'files', add extra pages to make divisible by 4
                        if(bind == TypeOfBindEnum.SaddleStitch && i == files.Count - 1)
                        {
                            if(bind == TypeOfBindEnum.SaddleStitch
                                && (pageCount + reader.NumberOfPages) % 4 != 0)
                            {
                                //PdfStamper stamper3 = new PdfStamper(reader, stream);
                                while((pageCount + reader.NumberOfPages) % 4 != 0)
                                {
                                    stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                                }
                            }
                        }

                        // get first page of first app file
                        if(!firstAppFileFound && files[i].FileType == SourceFileTypeEnum.App_File)
                        {
                            firstAppFileFound = true;
                            firstPageOfApp = pageCount + 1;
                        }

                        // add document to 'src'
                        pdfcopy.AddDocument(new iTextSharp.text.pdf.PdfReader(reader));
                        pageCount += reader.NumberOfPages;
                    }

                    pdfcopy.Close();
                    pdfdoc.CloseDocument();
                }

                // final cycle, if dividers, to make sure text starts on odd-sided pages
                if(bind == TypeOfBindEnum.PerfectBind && hasDividers)
                {
                    string dest = (System.IO.Path.GetDirectoryName(src) + @"\temp " + DateTime.Now.ToString("ddMMyyyyhhmmssffff"));

                    using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                    {
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);
                        iTextSharp.text.pdf.PdfStamper stamper = new iTextSharp.text.pdf.PdfStamper(reader, stream);

                        // get all blank pages in appendix
                        iTextSharp.text.pdf.parser.PdfReaderContentParser parser = new iTextSharp.text.pdf.parser.PdfReaderContentParser(reader);
                        List<List<int>> groupsOfBlanks = new List<List<int>>();
                        List<int> group_list = new List<int>();
                        int x;
                        for(x = firstPageOfApp; x <= reader.NumberOfPages; x++)
                        {
                            iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy extract = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                            var extractedText = parser.ProcessContent(x, extract);
                            string textFromPage = extractedText.GetResultantText();
                            // find blank pages and cluster into group_list
                            if(System.Text.RegularExpressions.Regex.Matches(textFromPage, @"\S").Count == 0)
                            {
                                // capture blank page cluster (??? but what if only 1 page ???)
                                if(group_list.Count == 0 || group_list.Contains(x - 1))
                                {
                                    group_list.Add(x);
                                }
                            }
                            else
                            {
                                // find first page after cluster
                                if(group_list.Count > 0)
                                {
                                    if(group_list.Last() % 2 == 1)
                                    {
                                        // add blank page
                                        stamper.InsertPage(group_list.Last() + 1, reader.GetPageSizeWithRotation(1));
                                    }
                                }
                                // clear list
                                group_list.Clear();
                            }
                        }
                        stamper.Close();
                        reader.Close();
                    }
                    System.IO.File.Delete(src);
                    System.IO.File.Move(dest, src);
                }
            }
            catch(Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt); return false;
            }
            return true;
        }
        public static bool CropCockleBriefPages(string src)
        {
            // works for both saddle stitch and perfect bind
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "brief cropped.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.pdf.PdfRectangle croppedRectangle = new iTextSharp.text.pdf.PdfRectangle(377f, 189f, 0f, 792f);
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

                    for(int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageN(i);
                        pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, croppedRectangle);
                    }

                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, stream);
                    stamper.Close();
                    reader.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        public static bool CombineCoverAndInsideCover(List<CockleFilePdf> files, string src)
        {
            try
            {
                using(var stream = new System.IO.FileStream(src, System.IO.FileMode.Create))
                {
                    // initiate iTextSharp processes
                    iTextSharp.text.Document pdfdoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.LETTER);
                    iTextSharp.text.pdf.PdfCopy pdfcopy = new iTextSharp.text.pdf.PdfCopy(pdfdoc, stream);
                    pdfdoc.Open();

                    // merge pdfs in folder
                    CockleFilePdf f;
                    for(int i = 0; i < files.Count; i++)
                    {
                        f = files[i];
                        // read file
                        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(f.FullName);
                        int filePageCount = reader.NumberOfPages;
                        pdfcopy.AddDocument(new iTextSharp.text.pdf.PdfReader(reader));
                    }
                    pdfcopy.Close();
                    pdfdoc.CloseDocument();
                }
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        public static bool CropCoverAndInsideCover(string src, int? coverLength)
        {
            // works for both saddle stitch and perfect bind
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "cover cropped.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.pdf.PdfRectangle croppedRectangleCover;
                    switch(coverLength)
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
                    iTextSharp.text.pdf.PdfRectangle croppedRectangleInsideCover = new iTextSharp.text.pdf.PdfRectangle(377f, 189f, 0f, 792f);

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

                    for(int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfDictionary pageDict = reader.GetPageN(i);
                        if(i == 1)
                        {
                            pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, croppedRectangleCover);
                        }
                        else
                        {
                            pageDict.Put(iTextSharp.text.pdf.PdfName.CROPBOX, croppedRectangleInsideCover);
                        }
                    }

                    var stamper = new iTextSharp.text.pdf.PdfStamper(reader, stream);
                    stamper.Close();
                    reader.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion

        #region Perfect Bind Utilities
        public static bool PerfectBind_LayoutCroppedBrief(string src)
        {
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "pb on B5.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document docB5 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(515.76f, 728.4f)); // B5 sized page
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB5, stream);
                    docB5.Open();

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

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
                        else
                        {
                            throw new Exception();
                        }
                    }
                    docB5.Close();
                    reader.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        public static bool PerfectBind_LayoutCroppedCoverAndInsideCover(string src, int? cover_length)
        {
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "pb Cover on B4.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, stream);
                    docB4.Open();

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

                    for(int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfContentByte contentbyte;
                        if(i == 1)
                        {
                            docB4.Add(new iTextSharp.text.Chunk());
                            iTextSharp.text.pdf.PdfTemplate tCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            // basic positioning: need to account for page size in future

                            // default values: in PB briefs, y_coordinate will not vary
                            float x_coordinate = 575.5f, y_coordinate = -76.5f;

                            switch(cover_length)
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
                        if(i == 2)
                        {
                            // not set up for multiple inside cover pages yet !!!
                            docB4.NewPage();
                            docB4.Add(new iTextSharp.text.Chunk());
                            iTextSharp.text.pdf.PdfTemplate tInsideCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            contentbyte.AddTemplate(tInsideCover, 113f, -126f);
                        }
                    }
                    docB4.Close();
                    reader.Close();
                    writer.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion

        #region Saddle Stitch Utilities
        public static bool SaddleStitch_ReorderPagesForLayout(string src)
        {
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "ss brief reordered.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);
                    SaddleStitchPageOrder order = new SaddleStitchPageOrder(reader.NumberOfPages);
                    reader.SelectPages(order.PageOrder);
                    iTextSharp.text.Document pdfdoc = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(1));
                    iTextSharp.text.pdf.PdfCopy pdfcopy_provider = new iTextSharp.text.pdf.PdfCopy(pdfdoc, stream);
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
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        public static bool SaddleStitch_LayoutCroppedBrief(string src)
        {
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "ss on B4.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, stream);
                    docB4.Open();

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

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
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        public static bool SaddleStitch_LayoutCroppedCoverAndInsideCover(string src, int? page_count, int? cover_length)
        {
            string dest = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(src), "ss Cover on B4.pdf");
            try
            {
                using(var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    iTextSharp.text.Document docB4 = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f)); // B4 sized page
                    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(docB4, stream);
                    docB4.Open();

                    iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(src);

                    for(int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfContentByte contentbyte;
                        if(i == 1)
                        {
                            docB4.Add(new iTextSharp.text.Chunk());
                            iTextSharp.text.pdf.PdfTemplate tCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            // basic positioning: need to account for page size in future

                            // default values
                            float x_coordinate = /*550f*/ 554.5f /*17-32 pages*/, y_coordinate = -76.25f /*48 pica*/;

                            switch(page_count - 1 / 16)
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

                            switch(cover_length)
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
                        }
                        if(i == 2)
                        {
                            // not set up for multiple inside cover pages yet !!!
                            docB4.NewPage();
                            docB4.Add(new iTextSharp.text.Chunk());
                            iTextSharp.text.pdf.PdfTemplate tInsideCover = writer.GetImportedPage(reader, i);
                            contentbyte = writer.DirectContent;
                            contentbyte.AddTemplate(tInsideCover, 113f, -126f);
                        }

                    }
                    docB4.Close();
                    reader.Close();
                    writer.Close();
                }
                System.IO.File.Delete(src);
                System.IO.File.Move(dest, src);
                return true;
            }
            catch(Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion
    }
}
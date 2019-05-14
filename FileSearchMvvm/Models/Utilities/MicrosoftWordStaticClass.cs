using FileSearchMvvm.Models.CockleTypes;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;
using PdfMaker = AdobePDFMakerForOffice;
using System;

namespace FileSearchMvvm.Models.Utilities
{
    public class MicrosoftWordStaticClass
    {
        public static bool WordDoc_DeleteFooters(CockleFile _file, Word.Document doc)
        {
            bool deletedFootersSuccessfully = true;
            Word.Range footerRange, headerRange;
            foreach(Word.Section section in doc.Sections)
            {
                footerRange = section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                if(!footerRange.Text.Equals(((char)13).ToString()))
                {
                    // check for dividers
                    if(Regex.Matches(footerRange.Text, @"[Bb][Ll][Uu][Ee]").Count > 0
                        || Regex.Matches(footerRange.Text, @"[Dd][Ii][Vv][Ii][Dd][Ee][Rr]").Count > 0)
                    {
                        //if (this.BindType == TypeOfBindEnum.ProgramDecidesByPageCount)
                        //{
                        //    this.BindType = TypeOfBindEnum.PerfectBind;
                        //}
                    }

                    // experimental: gather word counts
                    int word_limit, word_count_auto, word_count_revised;
                    int out_num;
                    if(doc.FullName.Contains(" br "))
                    {
                        var matches_limit = Regex.Matches(
                            footerRange.Text, @"(Word Limit|WL|wl|word limit)(:)*(\s)*([0-9]{1,2},[0-9]{3}|[0-9]{1,})(\s|$)");
                        if(matches_limit.Count > 0)
                        {
                            if(int.TryParse(matches_limit[0].Groups[4].Value.Replace(",", ""), out out_num))
                            {
                                word_limit = out_num;
                            }
                        }
                        var matches_autocount = Regex.Matches(footerRange.Text, @"(Automatic|Auto) word count: ([0-9]{1,})(\s|$)");
                        if(matches_autocount.Count > 0)
                        {
                            if(int.TryParse(matches_autocount[0].Groups[2].Value, out out_num))
                            {
                                word_count_auto = out_num;
                            }
                        }
                        // Revised word count: 2500 words
                        var matches_revisedcount = Regex.Matches(footerRange.Text, @"Revised word count: ([0-9]{1,})(\s|$)");
                        if(matches_revisedcount.Count > 0)
                        {
                            if(int.TryParse(matches_revisedcount[0].Groups[1].Value, out out_num))
                            {
                                word_count_revised = out_num;
                            }
                        }
                    }
                    // adjust for blank headers in 8.5 x 11 foldouts: the header interferes with the layout
                    if(_file.FileType == SourceFileTypeEnum.App_Foldout
                        || _file.FileType == SourceFileTypeEnum.App_ZFold
                        || _file.FileType == SourceFileTypeEnum.Brief_Foldout
                        || _file.FileType == SourceFileTypeEnum.Brief_ZFold)
                    {
                        headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                        if(headerRange.Text.Equals(((char)13).ToString())) { doc.PageSetup.HeaderDistance = 1; }
                    }
                    //clear text
                    footerRange.Text = "";
                }
                footerRange.Delete();
            }
            headerRange = null;
            footerRange = null;
            return deletedFootersSuccessfully;
        }

        public static int CaptureCoverLength(Word.Document doc)
        {
            int len = (int)doc.PageSetup.BottomMargin;

            switch(len)
            {
                case 150: return 48;
                case 138: return 49;
                case 126: return 50;
                case 114: return 51;
                default: return -1;
            }
        }
        internal static string PrintToFileForProof(Word.Application app, string input_filename)
        {
            // Create pdf filename
            var prnFileName = input_filename;
            // change root directory: try to land the file in scratch directory
            prnFileName = System.IO.Path.Combine(
                @"c:\scratch", System.IO.Path.GetFileNameWithoutExtension(prnFileName) + ".prn");

            // make sure c:\scratch exists
            if(!System.IO.Directory.Exists(@"C:\scratch"))
            {
                // create new if does not
                System.IO.Directory.CreateDirectory(@"C:\scratch");
            }

            // Remove existing files in C:\scratch
            if(System.IO.File.Exists(prnFileName)) { System.IO.File.Delete(prnFileName); }

            // Print to PDF, using method from VBA cockle
            try
            {
                // let Acrobat Distiller determine the filename
                app.PrintOut(
                    FileName: "",
                    Range: Word.WdPrintOutRange.wdPrintAllDocument,
                    Item: Word.WdPrintOutItem.wdPrintDocumentContent,
                    Copies: 1,
                    Pages: "",
                    PageType: Word.WdPrintOutPages.wdPrintAllPages,
                    ManualDuplexPrint: false,
                    Collate: true,
                    Background: true,
                    PrintToFile: true,
                    PrintZoomColumn: 0,
                    PrintZoomRow: 0,
                    PrintZoomPaperWidth: 0,
                    PrintZoomPaperHeight: 0,
                    OutputFileName: prnFileName
                    );

                //Application.PrintOut fileName:= "", Range:= wdPrintAllDocument, Item:= _
                //wdPrintDocumentContent, Copies:= 1, Pages:= "", PageType:= wdPrintAllPages, _
                //ManualDuplexPrint:= False, Collate:= True, Background:= True, PrintToFile:= _
                //False, PrintZoomColumn:= 0, PrintZoomRow:= 0, PrintZoomPaperWidth:= 0, _
                //PrintZoomPaperHeight:= 0 ', OutputFileName:=pdffileName, Append:=False

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"{input_filename} failed to print");
            }
            return prnFileName;
        }
        internal static string PrintToFileWithPdfMaker(Word.Application app, Microsoft.Office.Core.COMAddIn add_in, string filename)
        {
            // Remove existing files in C:\scratch
            string pattern = @"\.docx*";

            // Create pdf filename
            var pdfFileName = filename;
            Regex rx = new Regex(pattern);
            if(rx.IsMatch(pdfFileName)) { pdfFileName = rx.Replace(pdfFileName, @".pdf"); }
            if(System.IO.File.Exists(pdfFileName)) { System.IO.File.Delete(pdfFileName); }

            // Print to PDF, using method from VBA cockle
            try
            {
                // let Acrobat Distiller determine the filename
                PdfMaker.PDFMaker pmkr = null;

                if(!app.ActiveDocument.Saved)
                {
                    // problem: doc needs to be saved first
                }
                foreach(Microsoft.Office.Core.COMAddIn a in app.COMAddIns)
                {
                    if(a.Description.ToLower().Contains("pdfmaker"))
                    {
                        add_in = a;
                        pmkr = a.Object;
                    }
                }

                object stng = null;
                pmkr.GetCurrentConversionSettings(out stng);
                // error handling here !!!
                var stng2 = (AdobePDFMakerForOffice.ISettings)stng;
                stng2.AddBookmarks = true;
                stng2.AddLinks = true;
                stng2.AddTags = false;
                stng2.ConvertAllPages = true;
                stng2.CreateFootnoteLinks = false;
                stng2.CreateXrefLinks = false;
                stng2.OutputPDFFileName = pdfFileName;
                stng2.PromptForPDFFilename = false;
                stng2.ShouldShowProgressDialog = false;
                stng2.ViewPDFFile = false;

                int out_val;
                pmkr.CreatePDFEx(stng2, out out_val);

                while(add_in.Object != null)
                {
                    add_in.Object = null;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"{filename} failed to print");
            }
            return pdfFileName;
        }
        internal static string PrintToFile(Word.Application app, string filename)
        {
            // Remove existing files in C:\scratch
            string pattern = @"\.docx*";

            // Create pdf filename
            var pdfFileName = filename;
            Regex rx = new Regex(pattern);
            if(rx.IsMatch(pdfFileName)) { pdfFileName = rx.Replace(pdfFileName, @".pdf"); }
            if(System.IO.File.Exists(pdfFileName)) { System.IO.File.Delete(pdfFileName); }

            // Print to PDF, using method from VBA cockle
            try
            {
                app.PrintOut(
                    FileName: "",
                    Range: Word.WdPrintOutRange.wdPrintAllDocument,
                    Item: Word.WdPrintOutItem.wdPrintDocumentContent,
                    Copies: 1,
                    Pages: "",
                    PageType: Word.WdPrintOutPages.wdPrintAllPages,
                    ManualDuplexPrint: false,
                    Collate: true,
                    Background: true,
                    PrintToFile: false,
                    PrintZoomColumn: 0,
                    PrintZoomRow: 0,
                    PrintZoomPaperWidth: 0,
                    PrintZoomPaperHeight: 0);

                //Application.PrintOut fileName:= "", Range:= wdPrintAllDocument, Item:= _
                //wdPrintDocumentContent, Copies:= 1, Pages:= "", PageType:= wdPrintAllPages, _
                //ManualDuplexPrint:= False, Collate:= True, Background:= True, PrintToFile:= _
                //False, PrintZoomColumn:= 0, PrintZoomRow:= 0, PrintZoomPaperWidth:= 0, _
                //PrintZoomPaperHeight:= 0 ', OutputFileName:=pdffileName, Append:=False

            }
            catch
            {
                System.Diagnostics.Debug.WriteLine($"{filename} failed to print");
            }
            return pdfFileName;
        }
    }
}
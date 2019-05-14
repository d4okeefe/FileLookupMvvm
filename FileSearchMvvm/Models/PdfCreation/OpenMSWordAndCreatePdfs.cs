using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using FileSearchMvvm.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using Word = Microsoft.Office.Interop.Word;
using System.Linq;

namespace FileSearchMvvm.Models.PdfCreation
{
    class OpenMSWordAndCreatePdfs
    {
        public List<FilePrintedSuccessfully> FilesPrintedSuccessfully { get; internal set; }
        public bool ExceptionThrownInAwait { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IProgress<string> Progress { get; set; }

        public System.IO.FileSystemWatcher FileWatcher { get; set; }
        private string origProgressReport;

        public OpenMSWordAndCreatePdfs(
            List<CockleFile> filesSelectedForConversion,
            string original_progress_report,
            IProgress<string> progress,
            CancellationToken cancellationToken,
            bool exceptionThrownInAwait)
        {
            origProgressReport = original_progress_report;
            Progress = progress;
            CancellationToken = cancellationToken;
            ExceptionThrownInAwait = exceptionThrownInAwait;

            //instantiate Word Application & Document
            Word.Application app = new Word.Application();
            //var app = (Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
            app.Visible = true;
            Word.Document doc = null;

            //System.Runtime.InteropServices.Marshal;

            //get original printer & prepare to create PDF
            var current_printer = app.ActivePrinter;
            var adobe_pdf_printer = "Adobe PDF";
            app.ActivePrinter = adobe_pdf_printer;

            // collection of special class to track files
            FilesPrintedSuccessfully = new List<FilePrintedSuccessfully>();

            try
            {
                // counter to track files printed
                int i = 0;
                var tempFilesSavedToScratch = new List<string>();
                // loop through files
                foreach(CockleFile fileSelected in filesSelectedForConversion)
                {
                    // cancel if requested
                    try { CancellationToken.ThrowIfCancellationRequested(); }
                    catch(OperationCanceledException) { ExceptionThrownInAwait = true; throw new OperationCanceledException(); }

                    // catch pdf files saved to Current
                    if(System.IO.Path.GetExtension(fileSelected.FullName).Equals(".pdf"))
                    {
                        // don't try to open, just save to scratch and add to list
                        var pdfFileInCurrent = System.IO.Path.Combine(@"C:\scratch", System.IO.Path.GetFileName(fileSelected.FullName));
                        System.IO.File.Copy(fileSelected.FullName, pdfFileInCurrent);
                        // here, just a string & no cover length
                        FilesPrintedSuccessfully.Add(
                            new FilePrintedSuccessfully
                            {
                                CockleFile = null,
                                TempWordFile = null,
                                PdfFilename = pdfFileInCurrent,
                                Filetype = SourceFileTypeEnum.Camera_Ready, // may have to adjust type here
                                LengthOfCover = null
                            });
                        continue;
                    }

                    // Open docx files in Word, clean up, and convert
                    try
                    {
                        doc = app.Documents.Open(FileName: fileSelected.FullName, ReadOnly: true);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"{fileSelected.FullName} failed to open");
                    }

                    // save to c:\scratch (overwriting existing files)
                    var tempFileSavedToScratch = System.IO.Path.Combine(@"c:\scratch", System.IO.Path.GetFileName(fileSelected.FullName));
                    if(System.IO.File.Exists(tempFileSavedToScratch)) { System.IO.File.Delete(tempFileSavedToScratch); }
                    doc.SaveAs2(FileName: tempFileSavedToScratch);
                    tempFilesSavedToScratch.Add(tempFileSavedToScratch);

                    // ctrl shift f9 (had problem with links in index: removes links from main story)
                    Word.Range r = doc.StoryRanges[Word.WdStoryType.wdMainTextStory];
                    r.Fields.Unlink();

                    // delete footer
                    MicrosoftWordStaticClass.WordDoc_DeleteFooters(fileSelected, doc);

                    // line to capture length of cover
                    int? lengthOfCover = null;
                    if(fileSelected.FileType == SourceFileTypeEnum.Cover)
                    {
                        lengthOfCover = MicrosoftWordStaticClass.CaptureCoverLength(doc);
                    }

                    // cancel if requested
                    try { CancellationToken.ThrowIfCancellationRequested(); }
                    catch(OperationCanceledException) { ExceptionThrownInAwait = true; throw new OperationCanceledException(); }

                    // print to pdf
                    var newPdfConvertedFromWord = string.Empty;
                    newPdfConvertedFromWord = MicrosoftWordStaticClass.PrintToFile(app, doc.FullName);

                    doc.SaveAs2(FileName: tempFileSavedToScratch);

                    //AdobePDFMakerForOffice.PDFMaker pmkr = null;
                    //Microsoft.Office.Core.COMAddIn add_in = null;
                    //newPdfConvertedFromWord = MicrosoftWordStaticClass.PrintToFileWithPdfMaker(app, add_in, doc.FullName);
                    // report progress to gui
                    var report = original_progress_report +
                        $"\nNow creating:\n{System.IO.Path.GetFileName(newPdfConvertedFromWord)}";
                    Progress.Report(report);

                    // add to files_printed list
                    FilesPrintedSuccessfully.Add(
                        new FilePrintedSuccessfully
                        {
                            CockleFile = fileSelected,
                            TempWordFile = tempFileSavedToScratch,
                            PdfFilename = newPdfConvertedFromWord,
                            Filetype = SourceFileTypeEnum.Unrecognized,
                            LengthOfCover = lengthOfCover
                        });

                    // close document & delete temp file
                    doc.Close(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                    // increment counter
                    i++;
                }// end for loop to convert each files

                // report progress to gui
                var report2 = original_progress_report + "\nPrinting  ...";
                Progress.Report(report2);

                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                // after printing each doc: Leave Word open until printing complete
                while(app.BackgroundPrintingStatus > 0)
                {
                    try
                    {
                        // files not yet printed
                        var tempcount = FilesPrintedSuccessfully.Where(x => System.IO.File.Exists(x.PdfFilename)).Count();
                        // report progress to gui
                        var elapsedtime_toFiveSecs = (sw.ElapsedMilliseconds / 1000) / 5 * 5;
                        var report3 = original_progress_report
                            + $"\nPrinted: {tempcount} / {FilesPrintedSuccessfully.Count} files"
                            + $"\nSeconds elapsed: {elapsedtime_toFiveSecs}";
                        Progress.Report(report3);

                        // this seems to jolt COM into printing the files !!!
                        Thread.Sleep(100);

                        CancellationToken.ThrowIfCancellationRequested();
                    }
                    catch(OperationCanceledException) { ExceptionThrownInAwait = true; throw new OperationCanceledException(); }
                }
                while(app.BackgroundSavingStatus != 0) // this is probably unnecessary !!!
                {
                    try { CancellationToken.ThrowIfCancellationRequested(); }
                    catch(OperationCanceledException) { ExceptionThrownInAwait = true; throw new OperationCanceledException(); }
                }
                // cycle through to make sure all exist and Word is not actively printing:
                while(!FilesPrintedSuccessfully.TrueForAll(x => System.IO.File.Exists(x.PdfFilename)))
                {
                    try { CancellationToken.ThrowIfCancellationRequested(); }
                    catch(OperationCanceledException) { ExceptionThrownInAwait = true; throw new OperationCanceledException(); }
                }

                // delete temp files
                tempFilesSavedToScratch.ForEach(x => System.IO.File.Delete(x));

            }
            catch { } /* skip catch so that I can clean up files left in scratch*/
            finally
            {
                app.ActivePrinter = current_printer;
                app?.Quit();
                if(null != doc) { System.Runtime.InteropServices.Marshal.ReleaseComObject(doc); }
                if(null != app) { System.Runtime.InteropServices.Marshal.ReleaseComObject(app); }
                doc = null;
                app = null;
                GC.Collect();
            }
            if(exceptionThrownInAwait)
            {
                // close app
                app.ActivePrinter = current_printer;
                app?.Quit();
                if(null != doc) { System.Runtime.InteropServices.Marshal.ReleaseComObject(doc); }
                if(null != app) { System.Runtime.InteropServices.Marshal.ReleaseComObject(app); }
                doc = null;
                app = null;
                GC.Collect();
                //System.Runtime.InteropServices.
                // try to clean folder
                FilesPrintedSuccessfully.ForEach(f =>
                {
                    if(System.IO.File.Exists(f.PdfFilename)) { System.IO.File.Delete(f.PdfFilename); }
                    if(System.IO.File.Exists(f.TempWordFile)) { System.IO.File.Delete(f.TempWordFile); }
                });
                FilesPrintedSuccessfully = null;
            }
            // report progress to gui
            Progress.Report(original_progress_report);
        }
    }
}

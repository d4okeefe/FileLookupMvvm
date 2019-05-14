using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using FileSearchMvvm.ViewModels.Utilities;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;

namespace FileSearchMvvm.Models.PdfCreation
{
    // private constructor, public static call to CreatePdf
    public class PdfCreationModel
    {
        #region PRIVATE CONSTRUCTOR
        private PdfCreationModel(
            IProgress<string> _progress,
            System.Threading.CancellationToken _cancellationToken,
            List<CockleFile> _latestFiles,
            List<CockleFile> _selectedFiles,
            bool _isSingleTicket,
            string _destinationFolderConvertedFiles)
        {
            progress = _progress;
            cancellationToken = _cancellationToken;
            latestFiles = _latestFiles;
            selectedFiles = _selectedFiles;
            isSingleTicket = _isSingleTicket;
            destinationFolderConvertedFiles = _destinationFolderConvertedFiles;
        }
        #endregion
        #region FIELDS
        private IProgress<string> progress;
        private System.Threading.CancellationToken cancellationToken;
        private List<CockleFile> latestFiles;
        private List<CockleFile> selectedFiles;
        private bool isSingleTicket;
        private string destinationFolderConvertedFiles;
        #endregion
        #region PROPERTIES

        #endregion
        #region PRIVATE METHODS
        private void setCockleFilePdfRanks(List<CockleFilePdf> _files)
        {
            // special Rank numbers:
            // -1 : foldout or other type
            //    : another rank for files with inaccessible page numbers ???

            var ordered_files =
                from f in _files
                orderby
                    f.FileType,
                    f.PageRange == null ? -1 : f.PageRange.FirstPage,
                    f.FullName
                select f;

            int i = 0;
            foreach (var f in ordered_files)
            {
                if (f.FileType == SourceFileTypeEnum.App_Foldout ||
                   f.FileType == SourceFileTypeEnum.App_ZFold ||
                    f.FileType == SourceFileTypeEnum.Brief_Foldout ||
                    f.FileType == SourceFileTypeEnum.Brief_ZFold ||
                    f.FileType == SourceFileTypeEnum.SidewaysPage ||
                    f.FileType == SourceFileTypeEnum.Unrecognized)
                { f.Rank = -1; continue; }
                else { f.Rank = i; }
                i++;
            }
        }
        #endregion
        #region PUBLIC METHOD: ENTRY TO CLASS
        public static async Task<List<CockleFilePdf>> CreatePdf(
            IProgress<string> _progress,
            System.Threading.CancellationToken _cancellationToken,
            List<CockleFile> _latestFiles,
            List<CockleFile> _selectedFiles,
            bool _isSingleTicket,
            string _destinationFolderConvertedFiles)
        {
            var pdfCreationModel = new PdfCreationModel(
                _progress,
                _cancellationToken,
                _latestFiles,
                _selectedFiles,
                _isSingleTicket,
                _destinationFolderConvertedFiles);

            bool exceptionThrownInAwait = false; // tracks excptn in await

            List<CockleFile> filesSelectedForConversion;
            bool convertAll = false;

            // get files from grid if null
            if (null == pdfCreationModel.latestFiles) { filesSelectedForConversion = pdfCreationModel.selectedFiles; }
            else { convertAll = true; filesSelectedForConversion = pdfCreationModel.latestFiles; }

            if (filesSelectedForConversion.Count < 1) throw new Exception();


            // begin AWAIT
            var filesToReturnFromTask = await System.Threading.Tasks.Task.Run(() =>
            {
                // I think this line can go, as can the OpenMSWordAndCreatePdfs class
                //var files = new OpenMSWordAndCreatePdfs().FilesPrintedSuccessfully;

                //instantiate Word Application & Document
                Word.Application app = new Word.Application();
                app.Visible = true;
                Word.Document doc = null;

                //get original printer & prepare to create PDF
                var current_printer = app.ActivePrinter;
                var adobe_pdf_printer = "Adobe PDF";
                app.ActivePrinter = adobe_pdf_printer;

                // collection of special class to track files
                var filesPrintedSuccessfully = new List<FilePrintedSuccessfully>();
                //the Word file saved to scratch
                try
                {
                    // counter to track files printed
                    int i = 0;

                    // loop through files
                    foreach (CockleFile fileSelected in filesSelectedForConversion)
                    {
                        // cancel if requested
                        try { pdfCreationModel.cancellationToken.ThrowIfCancellationRequested(); }
                        catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }

                        // catch pdf files saved to Current
                        if (System.IO.Path.GetExtension(fileSelected.FullName).Equals(".pdf"))
                        {
                            // don't try to open, just save to scratch and add to list
                            var pdfFileInCurrent = System.IO.Path.Combine(@"C:\scratch", System.IO.Path.GetFileName(fileSelected.FullName));
                            System.IO.File.Copy(fileSelected.FullName, pdfFileInCurrent);
                            // here, just a string & no cover length
                            filesPrintedSuccessfully.Add(
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
                        if (System.IO.File.Exists(tempFileSavedToScratch)) { System.IO.File.Delete(tempFileSavedToScratch); }
                        doc.SaveAs2(FileName: tempFileSavedToScratch);

                        // ctrl shift f9 (had problem with links in index: removes links from main story)
                        Word.Range r = doc.StoryRanges[Word.WdStoryType.wdMainTextStory];
                        r.Fields.Unlink();

                        // delete footer
                        MicrosoftWordStaticClass.WordDoc_DeleteFooters(fileSelected, doc);

                        // line to capture length of cover
                        int? lengthOfCover = null;
                        if (fileSelected.FileType == SourceFileTypeEnum.Cover)
                        {
                            lengthOfCover = MicrosoftWordStaticClass.CaptureCoverLength(doc);
                        }

                        // cancel if requested
                        try { pdfCreationModel.cancellationToken.ThrowIfCancellationRequested(); }
                        catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }

                        // print to pdf, reporting progress
                        var newPdfConvertedFromWord = string.Empty;
                        newPdfConvertedFromWord = MicrosoftWordStaticClass.PrintToFile(app, doc.FullName);
                        // halt process here: wait for COM background status to end
                        while (app.BackgroundPrintingStatus > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"app.BackgroundPrintingStatus is {app.BackgroundPrintingStatus}");
                            // cancel if requested
                            try { pdfCreationModel.cancellationToken.ThrowIfCancellationRequested(); }
                            catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }
                        }
                        while (app.BackgroundSavingStatus > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"app.BackgroundSavingStatus is {app.BackgroundSavingStatus}");
                            // cancel if requested
                            try { pdfCreationModel.cancellationToken.ThrowIfCancellationRequested(); }
                            catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }
                        }

                        // add to files_printed list
                        filesPrintedSuccessfully.Add(
                            new FilePrintedSuccessfully
                            {
                                CockleFile = fileSelected,
                                TempWordFile = tempFileSavedToScratch,
                                PdfFilename = newPdfConvertedFromWord,
                                Filetype = SourceFileTypeEnum.Unrecognized,
                                LengthOfCover = lengthOfCover
                            });

                        // report to ui

                        // make sure file exists before closing
                        while (!System.IO.File.Exists(newPdfConvertedFromWord))
                        {
                            System.Diagnostics.Debug.WriteLine($"Waiting to print to pdf: {newPdfConvertedFromWord}");
                            // cancel if requested
                            try { pdfCreationModel.cancellationToken.ThrowIfCancellationRequested(); }
                            catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }
                        }
                        // close document & delete temp file
                        doc.Close(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                        System.IO.File.Delete(tempFileSavedToScratch);
                        // increment counter
                        i++;
                    }// end for loop to convert each files
                }
                //catch (OperationCanceledException ex) { }
                catch { }
                finally
                {
                    app.ActivePrinter = current_printer;
                    app?.Quit();
                }
                if (exceptionThrownInAwait)
                {
                    // close app
                    app.ActivePrinter = current_printer;
                    app?.Quit();
                    // try to clean folder
                    filesPrintedSuccessfully.ForEach(f =>
                    {
                        if (System.IO.File.Exists(f.PdfFilename)) { System.IO.File.Delete(f.PdfFilename); }
                        if (System.IO.File.Exists(f.TempWordFile)) { System.IO.File.Delete(f.TempWordFile); }
                    });
                    return null;
                }

                // block until all files exist
                while (filesPrintedSuccessfully?.Count != filesSelectedForConversion.Count) ;

                #region POINT OF NO RETURN IN CONVERSION
                // convert files to CockleFilePdf
                var cockleFilePdfsPrintedSuccessfully = new List<CockleFilePdf>();
                foreach (var _f in filesPrintedSuccessfully)
                {
                    if (_f.LengthOfCover == null && _f.CockleFile == null)
                    {
                        cockleFilePdfsPrintedSuccessfully.Add(new CockleFilePdf(_f.PdfFilename, _f.Filetype));
                    }
                    else
                    {
                        cockleFilePdfsPrintedSuccessfully.Add(
                            new CockleFilePdf(_f.CockleFile, _f.PdfFilename, _f.LengthOfCover));
                    }
                }

                // test whether all converted files have same ticket
                bool allConvertedFilesSameTicket = cockleFilePdfsPrintedSuccessfully
                    .TrueForAll(f => f.TicketNumber ==
                    cockleFilePdfsPrintedSuccessfully.First().TicketNumber);

                // move files to unique folder
                if ((allConvertedFilesSameTicket || pdfCreationModel.isSingleTicket)
                    && cockleFilePdfsPrintedSuccessfully.Count() > 0) // PROBLEM HERE !!!
                {
                    var firstFile = cockleFilePdfsPrintedSuccessfully.First();
                    string timeStamp = string.Format("({0} {1}, {2}, {3})", DateTime.Now.ToString("MMM")/*Oct*/,
                        DateTime.Now.ToString("dd")/*09*/, DateTime.Now.ToString("yyy")/*2015*/,
                        DateTime.Now.ToString("T").ToLower()/*10:58:44 AM*/ /*, len_text*/)
                        .Replace(':', ' ');
                    var folderName = $"{firstFile.TicketNumber} {firstFile.Attorney} {timeStamp}";
                    var scratchLocation = @"c:\scratch";

                    var newFolder = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(scratchLocation, folderName));

                    foreach (var f in cockleFilePdfsPrintedSuccessfully)
                    {
                        try
                        {
                            var new_filename = System.IO.Path.Combine(newFolder.FullName, f.Filename);
                            System.IO.File.Move(f.FullName, new_filename);

                            // associate new location with CockleFilePdf
                            f.FullName = new_filename;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                        }
                    }
                }

                // set destination property
                pdfCreationModel.destinationFolderConvertedFiles = System.IO.Path.GetDirectoryName(cockleFilePdfsPrintedSuccessfully.First().FullName);

                // set ranks of pdfs before returning
                pdfCreationModel.setCockleFilePdfRanks(cockleFilePdfsPrintedSuccessfully);

                // combine files into single pdf
                if (convertAll)
                {
                    var createMergedPdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(cockleFilePdfsPrintedSuccessfully);

                    CockleFilePdf mergedCockleFile = null;

                    // add combined file to list of cocklefilepdf files
                    if (System.IO.File.Exists(createMergedPdfAcrobat.CombinedPdfFilename))
                    {

                        mergedCockleFile =
                            new CockleFilePdf(
                                createMergedPdfAcrobat.CombinedPdfFilename,
                                filesSelectedForConversion.First().Attorney,
                                filesSelectedForConversion.First().TicketNumber,
                                SourceFileTypeEnum.Combined_Pdf,
                                "pdf",
                                null);
                        cockleFilePdfsPrintedSuccessfully.Add(mergedCockleFile);
                    }

                    int len_of_cover = -1;
                    if (cockleFilePdfsPrintedSuccessfully.Any(f => f.FileType == SourceFileTypeEnum.Cover))
                    {
                        len_of_cover = cockleFilePdfsPrintedSuccessfully.Where(
                            f => f.FileType == SourceFileTypeEnum.Cover).FirstOrDefault().CoverLength ?? -1;
                    }

                    // remove combined pdf file
                    var cockleFilePdfsPrintedSuccessfullyMinusCombinedFile = cockleFilePdfsPrintedSuccessfully.ToList();
                    cockleFilePdfsPrintedSuccessfullyMinusCombinedFile.Remove(mergedCockleFile);

                    var createImposedPdfiTextSharp =
                        new Models.Imposition.ImposeFullConvertedTicket(
                            pdfCreationModel.destinationFolderConvertedFiles,
                            cockleFilePdfsPrintedSuccessfullyMinusCombinedFile,
                            len_of_cover,
                            TypeOfBindEnum.ProgramDecidesByPageCount);

                    // add imposed files to list of cocklefilepdf files
                    if (createImposedPdfiTextSharp.ImposedFilesCreated.All(f => System.IO.File.Exists(f.FullName)))
                    {
                        createImposedPdfiTextSharp.ImposedFilesCreated.ForEach(f =>
                        {
                            cockleFilePdfsPrintedSuccessfully.Add(
                                new CockleFilePdf(
                                    f.FullName,
                                    filesSelectedForConversion.First().Attorney,
                                    filesSelectedForConversion.First().TicketNumber,
                                    SourceFileTypeEnum.Imposed_Cover_and_Brief,
                                    "pdf",
                                    null));
                        });
                    }
                }
                #endregion


                return cockleFilePdfsPrintedSuccessfully;
            });
            if (exceptionThrownInAwait)
            {
                throw new OperationCanceledException();
            }
            return filesToReturnFromTask;
        }
        #endregion
    }
}
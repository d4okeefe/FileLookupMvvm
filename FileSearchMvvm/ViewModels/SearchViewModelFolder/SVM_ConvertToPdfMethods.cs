using FileSearchMvvm.Models;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.PdfCreation;
using FileSearchMvvm.Models.Utilities;
using FileSearchMvvm.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static FileSearchMvvm.Models.Utilities.MarkLatestFilesStaticClass;
using Word = Microsoft.Office.Interop.Word;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel
    {
        private bool canSearchAndSaveToScratch()
        {
            return IsSingleTicket && !IsExecutingSearch && !IsConvertingToPdf;
        }
        private async void searchAndSaveToScratch()
        {
            #region step 1: search for files
            var error_issue = string.Empty;
            IsExecutingSearch = true;
            Files.Clear();
            FilesConvertedToPdf?.Clear(); // added 12/1/2017
            UpdateLabel = "";
            var searchProgress = new Progress<string>(reportProgress);
            searchCancelTokenSource = new CancellationTokenSource();
            var latestFiles = new List<CockleFile>();
            List<CockleFile> cockleFilesFound = null;
            try
            {
                addItemToRecentSearchTerms();
                cockleFilesFound =
                    await Models.Search.SearchModel.SearchCurrentAndBackupT(
                    SearchText,
                    SearchEverywhere,
                    IsSingleTicket,
                    searchProgress,
                    searchCancelTokenSource.Token,
                    error_issue);
                latestFiles = MarkLatestFiles(cockleFilesFound);
                TicketsFoundInSearch = new HashSet<int>();
                cockleFilesFound?.ForEach(f =>
                {
                    if (null != f.TicketNumber)
                    {
                        TicketsFoundInSearch.Add((int)f.TicketNumber);
                    }
                    Files.Add(f);
                });
                UpdateLabel = $"{TicketsFoundInSearch.Count} ticket{(TicketsFoundInSearch.Count == 1 ? "" : "s")} found in search";
                UpdateLabel += $"\n{cockleFilesFound.Count} file{(cockleFilesFound.Count == 1 ? "" : "s")} found";
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iSearchProgress = searchProgress as IProgress<string>;
                iSearchProgress.Report("Search cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iSearchProgress = searchProgress as IProgress<string>;
                iSearchProgress.Report("Error in search for files");
            }
            finally
            {
                searchCancelTokenSource.Dispose();
                IsExecutingSearch = false;
                CollapseTickets = false;
                ShowLatestFiles = true;
            }
            #endregion
            #region Save files to scratch folder
            if (null != cockleFilesFound)
            {
                var scratchLocation = @"c:\scratch";
                cockleFilesFound
                    .Where(x => x.IsLatestFile)
                    .ToList()
                    .ForEach(x =>
                    {
                        System.IO.File.Copy(x.FullName, System.IO.Path.Combine(scratchLocation, x.Filename));
                    });
                UpdateLabel += '\n' + "Latest files saved to scratch";
            }
            #endregion
        }
        private bool canSearchAndConvertToPdf()
        {
            return IsSingleTicket && !IsExecutingSearch && !IsConvertingToPdf;
        }
        /**
         * Main 'one step' function --
         *   1. searches for ticket
         *   2. saves files to scratch
         *   3. converts saved files
         *   4. places converted files in new folder
         *   5. centers & imposes final pdfs
         *   
         * Goal on 1/25/2023:
         *   Add these steps so that most of this can be done offline   
         */
        private async void searchAndConvertToPdf()
        {
            #region step 1: search for files
            var error_issue = string.Empty;
            IsExecutingSearch = true;
            Files.Clear();
            FilesConvertedToPdf?.Clear(); // added 12/1/2017
            UpdateLabel = "";
            var searchProgress = new Progress<string>(reportProgress);
            searchCancelTokenSource = new CancellationTokenSource();
            var latestFiles = new List<CockleFile>();
            try
            {
                addItemToRecentSearchTerms();
                var cockleFilesFound =
                    await Models.Search.SearchModel.SearchCurrentAndBackupT(
                    SearchText,
                    SearchEverywhere,
                    IsSingleTicket,
                    searchProgress,
                    searchCancelTokenSource.Token,
                    error_issue);
                latestFiles = MarkLatestFiles(cockleFilesFound);
                TicketsFoundInSearch = new HashSet<int>();
                cockleFilesFound?.ForEach(f =>
                {
                    if (null != f.TicketNumber)
                    {
                        TicketsFoundInSearch.Add((int)f.TicketNumber);
                    }
                    Files.Add(f);
                });
                UpdateLabel = $"{TicketsFoundInSearch.Count} ticket{(TicketsFoundInSearch.Count == 1 ? "" : "s")} found in search";
                UpdateLabel += $"\n{cockleFilesFound.Count} file{(cockleFilesFound.Count == 1 ? "" : "s")} found";
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iSearchProgress = searchProgress as IProgress<string>;
                iSearchProgress.Report("Search cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iSearchProgress = searchProgress as IProgress<string>;
                iSearchProgress.Report("Error in search for files");
            }
            finally
            {
                searchCancelTokenSource.Dispose();
                IsExecutingSearch = false;
                CollapseTickets = false;
                ShowLatestFiles = true;
            }
            #endregion
            #region step 2: convert collected files to pdf
            if (null == latestFiles || latestFiles?.Count == 0)
            {
                UpdateLabel = "No files found";
                return;
            }


            // step 4: convert latest files to pdf
            // Files will land in c:\scratch, so capture them
            IsConvertingToPdf = true;
            var convertProgress = new Progress<string>(reportProgress);
            convertCancelTokenSource = new CancellationTokenSource();
            try
            {
                // HERE EXCEPTION !!!
                var _filesConverting =
                    await executePdfConversion(
                        convertProgress,
                        convertCancelTokenSource.Token,
                        latestFiles.ToList());
                FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(_filesConverting);
                UpdateLabel += $"\n{FilesConvertedToPdf.Count} file{(FilesConvertedToPdf.Count == 1 ? "" : "s")} created";
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Conversion cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Error trying to convert files");
            }
            finally
            {
                convertCancelTokenSource.Dispose();
                IsConvertingToPdf = false;
            }
            #endregion
        }
        private bool canConvertToPdfProof()
        {
            var test1 = StaticSystemTests.IsGhostscriptInstalled();

            return (
                SelectedFile != null
                && StaticSystemTests.Is554KonicaPrintToFileDriverInstalled()
                && StaticSystemTests.IsGhostscriptInstalled());
        }
        private async void convertToPdfProof()
        {
            IsConvertingToPdf = true;
            var convertProgress = new Progress<string>(reportProgress);
            convertCancelTokenSource = new CancellationTokenSource();

            try
            {
                var _filesConverting = await executePdfConversionForProof(convertProgress, convertCancelTokenSource.Token, null);
                //var _filesConverting = await Models.PdfCreation.PdfCreationModel.CreatePdfsFromWordFiles(
                //    convertProgress, 
                //    convertCancelTokenSource.Token, 
                //    SelectedFiles, 
                //    convertAll: false);
                FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(_filesConverting);
                UpdateLabel += $"\n{FilesConvertedToPdf.Count} file{(FilesConvertedToPdf.Count == 1 ? "" : "s")} converted";
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Conversion cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Error trying to convert files");
            }
            finally
            {
                convertCancelTokenSource.Dispose();
                IsConvertingToPdf = false;
            }
        }
        private bool canConvertFilesToPdf()
        {
            return SelectedFile != null && !IsExecutingSearch && !IsConvertingToPdf
                && StaticSystemTests.IsAdobePdfPrinterAvailable();
        }
        private async void convertFilesToPdf()
        {
            // Start with simple conversion
            // Files will land in c:\scratch, so capture them
            IsConvertingToPdf = true;
            var convertProgress = new Progress<string>(reportProgress);
            convertCancelTokenSource = new CancellationTokenSource();

            try
            {
                var _filesConverting = await executePdfConversion(convertProgress, convertCancelTokenSource.Token, null);
                //var _filesConverting = await Models.PdfCreation.PdfCreationModel.CreatePdfsFromWordFiles(
                //    convertProgress, 
                //    convertCancelTokenSource.Token, 
                //    SelectedFiles, 
                //    convertAll: false);
                FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(_filesConverting);
                UpdateLabel += $"\n{FilesConvertedToPdf.Count} file{(FilesConvertedToPdf.Count == 1 ? "" : "s")} created";
            }
            catch (OperationCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Conversion cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                var iConvertProgress = convertProgress as IProgress<string>;
                iConvertProgress.Report("Error trying to convert files");
            }
            finally
            {
                convertCancelTokenSource.Dispose();
                IsConvertingToPdf = false;
            }
        }
        private async Task<List<CockleFilePdf>> executePdfConversionForProof(
            IProgress<string> progress, CancellationToken cancellationToken, List<CockleFile> latestFiles)
        {
            bool exceptionThrownInAwait = false; // tracks excptn in await

            List<CockleFile> filesSelectedForConversion;
            bool convertAll = false;

            // get files from grid if null
            if (null == latestFiles) { filesSelectedForConversion = SelectedFiles; }
            else { convertAll = true; filesSelectedForConversion = latestFiles; }

            if (filesSelectedForConversion.Count < 1) throw new Exception();

            // begin AWAIT
            var filesToReturnFromTask = await Task.Run(() =>
            {
                //instantiate Word Application & Document
                Word.Application app = new Word.Application();
                app.Visible = false; //true; // changed 9-7-17
                Word.Document doc = null;

                //get original printer & prepare to create PDF
                var current_printer = app.ActivePrinter;
                var konica_554_prn_printer = "554 Print to File";
                app.ActivePrinter = konica_554_prn_printer;

                // collection of special class to track files
                var filesPrintedToPrnSuccessfully = new List<FilesPrintedToPrnSuccessfully>();
                // the Word file saved to scratch
                try
                {
                    // counter to track files printed
                    int i = 0;
                    // Q. copy all files to scratch?  A. cannot, because need tagline to read server drive



                    // loop through files
                    foreach (CockleFile fileSelected in filesSelectedForConversion)
                    {
                        // cancel if requested
                        try { cancellationToken.ThrowIfCancellationRequested(); }
                        catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }

                        // Open docx files in Word, clean up, and convert
                        try
                        {
                            doc = app.Documents.Open(FileName: fileSelected.LocalFullFilename, ReadOnly: true);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine($"{fileSelected.FullName} failed to open");
                        }

                        // cancel if requested
                        try { cancellationToken.ThrowIfCancellationRequested(); }
                        catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }

                        // print to pdf, reporting progress
                        var newPrnConvertedFromWord = string.Empty;
                        newPrnConvertedFromWord = MicrosoftWordStaticClass.PrintToFileForProof(app, doc.FullName);

                        // halt process here: wait for COM background status to end
                        while (app.BackgroundPrintingStatus > 0 && app.BackgroundSavingStatus > 0)
                        {
                            try { cancellationToken.ThrowIfCancellationRequested(); }
                            catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }
                        }

                        // add to files_printed list
                        filesPrintedToPrnSuccessfully.Add(
                            new FilesPrintedToPrnSuccessfully
                            {
                                CockleFile = fileSelected,
                                PrnFilename = newPrnConvertedFromWord,
                                Filetype = fileSelected.FileType
                            });

                        // make sure file exists before closing
                        while (!System.IO.File.Exists(newPrnConvertedFromWord))
                        {
                            System.Diagnostics.Debug.WriteLine($"Waiting to print to pdf: {newPrnConvertedFromWord}");
                            // cancel if requested
                            try { cancellationToken.ThrowIfCancellationRequested(); }
                            catch (OperationCanceledException) { exceptionThrownInAwait = true; throw new OperationCanceledException(); }
                        }
                        // close document & delete temp file
                        doc.Close(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
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
                    filesPrintedToPrnSuccessfully.ForEach(f =>
                    {
                        if (System.IO.File.Exists(f.PrnFilename)) { System.IO.File.Delete(f.PrnFilename); }
                    });
                    return null;
                }

                // block until all files exist
                while (filesPrintedToPrnSuccessfully?.Count != filesSelectedForConversion.Count) ;

                // NOW, CHANGE NAMES OF FILES TO PS, AND USE GHOSTSCRIPT TO CONVERT TO PDF

                var filesPrintedSuccessfully = new List<FilePrintedSuccessfully>();

                foreach (var f in filesPrintedToPrnSuccessfully)
                {
                    var psFileName = f.PrnFilename.Replace(".prn", ".ps");
                    var pdfFileName = f.PrnFilename.Replace(".prn", ".pdf");

                    System.IO.File.Move(f.PrnFilename, psFileName);
                    // block until successful
                    while (!System.IO.File.Exists(psFileName)) {/*waiting for IO*/}

                    using (var processor = new Ghostscript.NET.Processor.GhostscriptProcessor())
                    {
                        List<string> switches = new List<string>();
                        switches.Add("-empty");
                        switches.Add("-dQUIET");
                        switches.Add("-dSAFER");
                        switches.Add("-dBATCH");
                        switches.Add("-dNOPAUSE");
                        switches.Add("-dNOPROMPT");
                        switches.Add("-sDEVICE=pdfwrite");
                        switches.Add("-o" + pdfFileName);
                        switches.Add("-q");
                        switches.Add("-f");
                        switches.Add(psFileName);

                        processor.StartProcessing(switches.ToArray(), null);
                    }

                    // make sure successful
                    while (!System.IO.File.Exists(pdfFileName)) {/*waiting for IO*/}

                    filesPrintedSuccessfully.Add(
                        new FilePrintedSuccessfully
                        {
                            CockleFile = f.CockleFile,
                            TempWordFile = string.Empty,
                            PdfFilename = pdfFileName,
                            Filetype = SourceFileTypeEnum.Unrecognized, // may have to adjust type here
                            LengthOfCover = null
                        });

                    System.IO.File.Delete(psFileName);
                }

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
                if ((allConvertedFilesSameTicket || IsSingleTicket) && cockleFilePdfsPrintedSuccessfully.Count() > 0) // PROBLEM HERE !!!
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
                DestinationFolderConvertedFiles = System.IO.Path.GetDirectoryName(cockleFilePdfsPrintedSuccessfully.First().FullName);

                // set ranks of pdfs before returning
                setCockleFilePdfRanks(cockleFilePdfsPrintedSuccessfully);


                if (convertAll)
                {
                    // 1. create imposed pdf files
                    Models.Imposition.ImposeFullConvertedTicket createImposedPdfiTextSharp = null;
                    try
                    {
                        var len_of_cover = cockleFilePdfsPrintedSuccessfully
                            .Where(f => f.FileType == SourceFileTypeEnum.Cover).First().CoverLength ?? -1;

                        // prepare files for imposition
                        //var cockleFilePdfsPrintedSuccessfullyToCombine = cockleFilePdfsPrintedSuccessfully.ToList();
                        //cockleFilePdfsPrintedSuccessfullyToCombine.Remove(mergedCockleFile);

                        createImposedPdfiTextSharp =
                            new Models.Imposition.ImposeFullConvertedTicket(
                                DestinationFolderConvertedFiles,
                                cockleFilePdfsPrintedSuccessfully.ToList(),
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
                    catch (Exception ex)
                    {
                        if (null != createImposedPdfiTextSharp.ImposedFilesCreated
                            && createImposedPdfiTextSharp.ImposedFilesCreated.Count > 0)
                        {
                            createImposedPdfiTextSharp.ImposedFilesCreated.ForEach(x => System.IO.File.Delete(x.FullName));

                            foreach (var imposed_file in createImposedPdfiTextSharp.ImposedFilesCreated)
                            {
                                if (cockleFilePdfsPrintedSuccessfully.Contains(imposed_file))
                                {
                                    cockleFilePdfsPrintedSuccessfully.Remove(imposed_file);
                                }
                            }
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    // 2. combine files into single pdf
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedPdfAcrobat = null;
                    CockleFilePdf mergedCockleFile = null;
                    try
                    {
                        createMergedPdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                            cockleFilePdfsPrintedSuccessfully, Models.MergePdf.TypeOfCombinedPdf.All, centerPdf: true);

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
                    }
                    catch (Exception ex)
                    {
                        if (null != createMergedPdfAcrobat.CombinedPdfFilename &&
                            System.IO.File.Exists(createMergedPdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedPdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != mergedCockleFile
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
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
        /// <summary>
        /// This is the core method of the program.
        /// With a single ticket number, it collects Word files from
        /// the Current directory (H:\ locally mapped or CLBDC02\Current\ on server)
        /// and converts the files to PDFs in the user's scratch folder.
        /// 
        /// In the process, it also combines the ticket in several ways:
        /// first, in total, but also as distinct PDF/A brief and appendix.
        /// 
        /// It also imposes the files for printing.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="latestFiles"></param>
        /// <returns></returns>
        
        private async Task<List<CockleFilePdf>> executePdfConversionOffline(
            IProgress<string> progress,
            CancellationToken cancellationToken,
            List<CockleFile> wordFilesToConvert)
        {
            bool exceptionThrownInAwait = false; // tracks excptn in await

            List<CockleFile> filesSelectedForConversion;
            bool convertAll = false;

            // get files from grid if null
            if (null == wordFilesToConvert) { filesSelectedForConversion = SelectedFiles; }
            else { convertAll = true; filesSelectedForConversion = wordFilesToConvert; }

            if (filesSelectedForConversion.Count < 1) throw new Exception();

            // Convert files to Pdf: Open Word and Print to Pdf
            var filesToReturnFromTask = await Task.Run(() =>
            {
                OpenMSWordAndCreatePdfs openMSWordAndCreatePdfs = null;
                List<FilePrintedSuccessfully> filesPrintedSuccessfully = null;
                try
                {
                    openMSWordAndCreatePdfs = new OpenMSWordAndCreatePdfs(
                        filesSelectedForConversion,
                        UpdateLabel,
                        progress,
                        cancellationToken,
                        exceptionThrownInAwait);
                    cancellationToken = openMSWordAndCreatePdfs.CancellationToken;
                    exceptionThrownInAwait = openMSWordAndCreatePdfs.ExceptionThrownInAwait;
                    filesPrintedSuccessfully = openMSWordAndCreatePdfs.FilesPrintedSuccessfully;
                }
                catch (Exception ex)
                {
                    if (null != openMSWordAndCreatePdfs)
                    {
                        cancellationToken = openMSWordAndCreatePdfs.CancellationToken;
                        exceptionThrownInAwait = openMSWordAndCreatePdfs.ExceptionThrownInAwait;
                        filesPrintedSuccessfully = openMSWordAndCreatePdfs.FilesPrintedSuccessfully;
                    }
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null; // should this be here?
                }

                #region POINT OF NO RETURN IN CONVERSION
                // save each new pdf as CockleFilePdf object
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
                if ((allConvertedFilesSameTicket || IsSingleTicket) && cockleFilePdfsPrintedSuccessfully.Count() > 0) // PROBLEM HERE !!!
                {
                    DestinationFolderConvertedFiles = movePdfFilesToNewScratchFolder(cockleFilePdfsPrintedSuccessfully);
                }

                // set ranks of pdfs before returning
                setCockleFilePdfRanks(cockleFilePdfsPrintedSuccessfully);

                if (convertAll)
                {
                    // 1. create imposed pdf files
                    Models.Imposition.ImposeFullConvertedTicket createImposedPdfiTextSharp = null;
                    int? len_of_cover = null;
                    try
                    {
                        len_of_cover = cockleFilePdfsPrintedSuccessfully
                            .Find(x => x.FileType == SourceFileTypeEnum.Cover)
                            ?.CoverLength ?? null;

                        createImposedPdfiTextSharp =
                            new Models.Imposition.ImposeFullConvertedTicket(
                                DestinationFolderConvertedFiles,
                                cockleFilePdfsPrintedSuccessfully.ToList(),
                                len_of_cover != null ? (int)len_of_cover : -1,
                                TypeOfBindEnum.ProgramDecidesByPageCount);

                        // add imposed files to list of cocklefilepdf files
                        if (createImposedPdfiTextSharp.ImposedFilesCreated.All(x => System.IO.File.Exists(x.FullName)))
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
                    catch (Exception ex)
                    {
                        if (null != createImposedPdfiTextSharp
                                && null != createImposedPdfiTextSharp.ImposedFilesCreated
                                && createImposedPdfiTextSharp.ImposedFilesCreated.Count > 0)
                        {
                            createImposedPdfiTextSharp.ImposedFilesCreated.ForEach(x => System.IO.File.Delete(x.FullName));

                            foreach (var imposed_file in createImposedPdfiTextSharp.ImposedFilesCreated)
                            {
                                if (cockleFilePdfsPrintedSuccessfully.Contains(imposed_file))
                                {
                                    cockleFilePdfsPrintedSuccessfully.Remove(imposed_file);
                                }
                            }
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    // 2. combine files into single pdf & create combined files of brief & appendix
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedPdfAcrobat = null;
                    CockleFilePdf mergedCockleFile = null;
                    try
                    {
                        createMergedPdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                            cockleFilePdfsPrintedSuccessfully, typeOfCombinedPdf: Models.MergePdf.TypeOfCombinedPdf.All, centerPdf: true);

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
                    }
                    catch (Exception ex)
                    {
                        if (null != createMergedPdfAcrobat
                                && !string.IsNullOrEmpty(createMergedPdfAcrobat.CombinedPdfFilename)
                                && System.IO.File.Exists(createMergedPdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedPdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != mergedCockleFile
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedBrief_PdfAcrobat = null;
                    CockleFilePdf mergedCockleFile_Brief = null;
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedAppendix_PdfAcrobat = null;
                    CockleFilePdf mergedCockleFile_Appendix = null;
                    try
                    {
                        var app_files = cockleFilePdfsPrintedSuccessfully
                                .Where(x => x.FileType == SourceFileTypeEnum.App_Index
                                || x.FileType == SourceFileTypeEnum.App_File
                                || x.FileType == SourceFileTypeEnum.App_Foldout
                                || x.FileType == SourceFileTypeEnum.App_ZFold)
                                .ToList();


                        // if no appendix files, skip creating separate combined pdfs
                        if (app_files.Count > 0)
                        {
                            createMergedAppendix_PdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                                app_files, Models.MergePdf.TypeOfCombinedPdf.Appendix, centerPdf: true);
                            // add combined file to list of cocklefilepdf files
                            if (System.IO.File.Exists(createMergedAppendix_PdfAcrobat.CombinedPdfFilename))
                            {
                                mergedCockleFile_Appendix =
                                    new CockleFilePdf(
                                        createMergedAppendix_PdfAcrobat.CombinedPdfFilename,
                                        filesSelectedForConversion.First().Attorney,
                                        filesSelectedForConversion.First().TicketNumber,
                                        SourceFileTypeEnum.Combined_Pdf,
                                        "pdf",
                                        null);
                                cockleFilePdfsPrintedSuccessfully.Add(mergedCockleFile_Appendix);
                            }

                            var brief_files = cockleFilePdfsPrintedSuccessfully
                                .Where(x => x.FileType == SourceFileTypeEnum.Cover
                                || x.FileType == SourceFileTypeEnum.InsideCv
                                || x.FileType == SourceFileTypeEnum.Motion
                                || x.FileType == SourceFileTypeEnum.Index
                                || x.FileType == SourceFileTypeEnum.Brief
                                || x.FileType == SourceFileTypeEnum.Brief_Foldout
                                || x.FileType == SourceFileTypeEnum.Brief_ZFold)
                                .ToList();

                            createMergedBrief_PdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                                brief_files, Models.MergePdf.TypeOfCombinedPdf.Brief, centerPdf: true);
                            // add combined file to list of cocklefilepdf files
                            if (System.IO.File.Exists(createMergedBrief_PdfAcrobat.CombinedPdfFilename))
                            {
                                mergedCockleFile_Brief =
                                    new CockleFilePdf(
                                        createMergedBrief_PdfAcrobat.CombinedPdfFilename,
                                        filesSelectedForConversion.First().Attorney,
                                        filesSelectedForConversion.First().TicketNumber,
                                        SourceFileTypeEnum.Combined_Pdf,
                                        "pdf",
                                        null);
                                cockleFilePdfsPrintedSuccessfully.Add(mergedCockleFile_Brief);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (null != createMergedBrief_PdfAcrobat
                            && !string.IsNullOrEmpty(createMergedBrief_PdfAcrobat.CombinedPdfFilename)
                            && System.IO.File.Exists(createMergedBrief_PdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedBrief_PdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != createMergedAppendix_PdfAcrobat
                            && !string.IsNullOrEmpty(createMergedAppendix_PdfAcrobat.CombinedPdfFilename)
                            && System.IO.File.Exists(createMergedAppendix_PdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedAppendix_PdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != mergedCockleFile_Brief
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile_Brief))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile_Brief);
                        }
                        if (null != mergedCockleFile_Appendix
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile_Appendix))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile_Appendix);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
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
        private async Task<List<CockleFilePdf>> executePdfConversion(
            IProgress<string> progress,
            CancellationToken cancellationToken,
            List<CockleFile> latestFiles)
        {
            bool exceptionThrownInAwait = false; // tracks excptn in await

            List<CockleFile> filesSelectedForConversion;
            bool convertAll = false;

            // get files from grid if null
            if (null == latestFiles) { filesSelectedForConversion = SelectedFiles; }
            else { convertAll = true; filesSelectedForConversion = latestFiles; }

            if (filesSelectedForConversion.Count < 1) throw new Exception();

            // Convert files to Pdf: Open Word and Print to Pdf
            var filesToReturnFromTask = await Task.Run(() =>
            {
                OpenMSWordAndCreatePdfs openMSWordAndCreatePdfs = null;
                List<FilePrintedSuccessfully> filesPrintedSuccessfully = null;
                try
                {
                    openMSWordAndCreatePdfs = new OpenMSWordAndCreatePdfs(
                        filesSelectedForConversion,
                        UpdateLabel,
                        progress,
                        cancellationToken,
                        exceptionThrownInAwait);
                    cancellationToken = openMSWordAndCreatePdfs.CancellationToken;
                    exceptionThrownInAwait = openMSWordAndCreatePdfs.ExceptionThrownInAwait;
                    filesPrintedSuccessfully = openMSWordAndCreatePdfs.FilesPrintedSuccessfully;
                }
                catch (Exception ex)
                {
                    if (null != openMSWordAndCreatePdfs)
                    {
                        cancellationToken = openMSWordAndCreatePdfs.CancellationToken;
                        exceptionThrownInAwait = openMSWordAndCreatePdfs.ExceptionThrownInAwait;
                        filesPrintedSuccessfully = openMSWordAndCreatePdfs.FilesPrintedSuccessfully;
                    }
                    System.Diagnostics.Debug.WriteLine(ex);
                    return null; // should this be here?
                }

                #region POINT OF NO RETURN IN CONVERSION
                // save each new pdf as CockleFilePdf object
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
                if ((allConvertedFilesSameTicket || IsSingleTicket) && cockleFilePdfsPrintedSuccessfully.Count() > 0) // PROBLEM HERE !!!
                {
                    DestinationFolderConvertedFiles = movePdfFilesToNewScratchFolder(cockleFilePdfsPrintedSuccessfully);
                }

                // set ranks of pdfs before returning
                setCockleFilePdfRanks(cockleFilePdfsPrintedSuccessfully);

                if (convertAll)
                {
                    // 1. create imposed pdf files
                    Models.Imposition.ImposeFullConvertedTicket createImposedPdfiTextSharp = null;
                    int? len_of_cover = null;
                    try
                    {
                        len_of_cover = cockleFilePdfsPrintedSuccessfully
                            .Find(x => x.FileType == SourceFileTypeEnum.Cover)
                            ?.CoverLength ?? null;

                        createImposedPdfiTextSharp =
                            new Models.Imposition.ImposeFullConvertedTicket(
                                DestinationFolderConvertedFiles,
                                cockleFilePdfsPrintedSuccessfully.ToList(),
                                len_of_cover != null ? (int)len_of_cover : -1,
                                TypeOfBindEnum.ProgramDecidesByPageCount);

                        // add imposed files to list of cocklefilepdf files
                        if (createImposedPdfiTextSharp.ImposedFilesCreated.All(x => System.IO.File.Exists(x.FullName)))
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
                    catch (Exception ex)
                    {
                        if (null != createImposedPdfiTextSharp
                                && null != createImposedPdfiTextSharp.ImposedFilesCreated
                                && createImposedPdfiTextSharp.ImposedFilesCreated.Count > 0)
                        {
                            createImposedPdfiTextSharp.ImposedFilesCreated.ForEach(x => System.IO.File.Delete(x.FullName));

                            foreach (var imposed_file in createImposedPdfiTextSharp.ImposedFilesCreated)
                            {
                                if (cockleFilePdfsPrintedSuccessfully.Contains(imposed_file))
                                {
                                    cockleFilePdfsPrintedSuccessfully.Remove(imposed_file);
                                }
                            }
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    // 2. combine files into single pdf & create combined files of brief & appendix
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedPdfAcrobat = null;
                    CockleFilePdf mergedCockleFile = null;
                    try
                    {
                        createMergedPdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                            cockleFilePdfsPrintedSuccessfully, typeOfCombinedPdf: Models.MergePdf.TypeOfCombinedPdf.All, centerPdf: true);

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
                    }
                    catch (Exception ex)
                    {
                        if (null != createMergedPdfAcrobat
                                && !string.IsNullOrEmpty(createMergedPdfAcrobat.CombinedPdfFilename)
                                && System.IO.File.Exists(createMergedPdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedPdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != mergedCockleFile
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedBrief_PdfAcrobat = null;
                    CockleFilePdf mergedCockleFile_Brief = null;
                    Models.MergePdf.CreateMergedPDFAcrobat createMergedAppendix_PdfAcrobat = null;
                    CockleFilePdf mergedCockleFile_Appendix = null;
                    try
                    {
                        var app_files = cockleFilePdfsPrintedSuccessfully
                                .Where(x => x.FileType == SourceFileTypeEnum.App_Index
                                || x.FileType == SourceFileTypeEnum.App_File
                                || x.FileType == SourceFileTypeEnum.App_Foldout
                                || x.FileType == SourceFileTypeEnum.App_ZFold)
                                .ToList();


                        // if no appendix files, skip creating separate combined pdfs
                        if (app_files.Count > 0)
                        {
                            createMergedAppendix_PdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                                app_files, Models.MergePdf.TypeOfCombinedPdf.Appendix, centerPdf: true);
                            // add combined file to list of cocklefilepdf files
                            if (System.IO.File.Exists(createMergedAppendix_PdfAcrobat.CombinedPdfFilename))
                            {
                                mergedCockleFile_Appendix =
                                    new CockleFilePdf(
                                        createMergedAppendix_PdfAcrobat.CombinedPdfFilename,
                                        filesSelectedForConversion.First().Attorney,
                                        filesSelectedForConversion.First().TicketNumber,
                                        SourceFileTypeEnum.Combined_Pdf,
                                        "pdf",
                                        null);
                                cockleFilePdfsPrintedSuccessfully.Add(mergedCockleFile_Appendix);
                            }

                            var brief_files = cockleFilePdfsPrintedSuccessfully
                                .Where(x => x.FileType == SourceFileTypeEnum.Cover
                                || x.FileType == SourceFileTypeEnum.InsideCv
                                || x.FileType == SourceFileTypeEnum.Motion
                                || x.FileType == SourceFileTypeEnum.Index
                                || x.FileType == SourceFileTypeEnum.Brief
                                || x.FileType == SourceFileTypeEnum.Brief_Foldout
                                || x.FileType == SourceFileTypeEnum.Brief_ZFold)
                                .ToList();

                            createMergedBrief_PdfAcrobat = new Models.MergePdf.CreateMergedPDFAcrobat(
                                brief_files, Models.MergePdf.TypeOfCombinedPdf.Brief, centerPdf: true);
                            // add combined file to list of cocklefilepdf files
                            if (System.IO.File.Exists(createMergedBrief_PdfAcrobat.CombinedPdfFilename))
                            {
                                mergedCockleFile_Brief =
                                    new CockleFilePdf(
                                        createMergedBrief_PdfAcrobat.CombinedPdfFilename,
                                        filesSelectedForConversion.First().Attorney,
                                        filesSelectedForConversion.First().TicketNumber,
                                        SourceFileTypeEnum.Combined_Pdf,
                                        "pdf",
                                        null);
                                cockleFilePdfsPrintedSuccessfully.Add(mergedCockleFile_Brief);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (null != createMergedBrief_PdfAcrobat
                            && !string.IsNullOrEmpty(createMergedBrief_PdfAcrobat.CombinedPdfFilename)
                            && System.IO.File.Exists(createMergedBrief_PdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedBrief_PdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != createMergedAppendix_PdfAcrobat
                            && !string.IsNullOrEmpty(createMergedAppendix_PdfAcrobat.CombinedPdfFilename)
                            && System.IO.File.Exists(createMergedAppendix_PdfAcrobat.CombinedPdfFilename))
                        {
                            System.IO.File.Delete(createMergedAppendix_PdfAcrobat.CombinedPdfFilename);
                        }
                        if (null != mergedCockleFile_Brief
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile_Brief))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile_Brief);
                        }
                        if (null != mergedCockleFile_Appendix
                            && cockleFilePdfsPrintedSuccessfully.Contains(mergedCockleFile_Appendix))
                        {
                            cockleFilePdfsPrintedSuccessfully.Remove(mergedCockleFile_Appendix);
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
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

        private string movePdfFilesToNewScratchFolder(List<CockleFilePdf> cockleFilePdfsPrintedSuccessfully)
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
            return System.IO.Path.GetDirectoryName(cockleFilePdfsPrintedSuccessfully.FirstOrDefault().FullName);
        }
    }
}

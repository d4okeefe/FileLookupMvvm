using Acrobat;
using FileSearchMvvm.Models;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Imposition;
using FileSearchMvvm.Models.PdfCreation;
using FileSearchMvvm.Models.Utilities;
using FileSearchMvvm.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    /* MANIPULATE PDFs, NOTES
     * 
     * All methods of this partial class manipulate PDF
     * files from the user's c:\scratch folder. Documents
     * are presented to use in a datagrid, and the user 
     * can:
     * 
     *   1) Combine several documents to create a new one
     *   2) Center the text from a PDF on the page
     *   3) Impose the text from  a PDF for printing
     * 
     * As a rule, the files that center text on the page
     * use Acrobat Javascript to make changes to the the PDF.
     * 
     * In contrast, files that are imposed for printing use 
     * iTextSharp. iText is generally faster, as it doesn't
     * have to rely on opening another program (interop).
     * However, we don't want to circulate any documents
     * to the public that use iText, as it has a
     * prohibitive user agreement.
     * 
     * When a new file is created, it should always land 
     * in the DestinationFolderConvertedFiles. This makes
     * it easy to keep track of any changes. Also, there 
     * should be an alert for the user when a new file 
     * is added. Without the alert, it can difficult to
     * find the file in the datagrid the user sees.
     * 
     * */

    public partial class SearchViewModel
    {
        #region IMPOSE PDF: 1) Show user options, 2) Impose
        private bool canImposePdfOptions()
        {
            return null != SelectedPdfFile;
        }
        private void imposePdfOptions(object o)
        {
            if (o.ToString() == "SupremeCourt")
            {
                SelectImposeCoverOptions = true;
                SelectImpositionDetailsIsVisible = true;
                ModalOverlayIsVisible = true;
            }
            else if (o.ToString() == "SpecialImpositions")
            {
                SpecialImpositionsOverlayIsVisible = true;
                ModalOverlayIsVisible = true;
            }
        }
        private bool canImposePdf()
        {
            // make sure that all three options are checked:
            return (IsTypesetPdf && !IsCameraReadyCenteredPdf && !IsCameraReadyOffsetPdf
                || !IsTypesetPdf && IsCameraReadyCenteredPdf && !IsCameraReadyOffsetPdf
                || !IsTypesetPdf && !IsCameraReadyCenteredPdf && IsCameraReadyOffsetPdf)
                && (IsSaddleStitchPdf ^ IsPerfectBindPdf)
                && (HasCoverPdf ^ NoCoverBriefOnlyPdf)
                && (BlankPagesAdded ^ BlankPagesNotAdded)
                && BlankPagesAdded;
        }
        /// <summary>
        /// imposePdf() function:
        /// The only source is the modal SelectImpositionDetails.xaml,
        /// and the RunImposePdf Command.
        /// The imposition process is controlled by a group of
        /// boolean properties:
        /// 1) IsTypeset: Upper Left Corner Document Only
        ///               All Centered Briefs are considered Camera Ready
        /// 2) IsSaddleStitchPdf: True or False (if false: IsPerfectBindPdf)
        /// 3) HasCover: self explanatory
        /// 4) BlankPagesAdded: Works as a Gatekeeper: User cannot run function
        ///                     without clicking Yes
        ///                     
        /// Function uses PDFCropAndNUp:
        ///     How many functions in the SVM others here use this?
        /// 
        /// Problem: When creating imposed files -- only CR? --
        ///          user may click NoCover, but program creates a cover anyway
        ///          (one that won't open)
        ///          Solved: Solved this issue by simply doing a null test on the result.
        /// 
        /// 
        /// 
        /// </summary>
        private void imposePdf()
        {
            ModalOverlayIsVisible = false;
            SelectImpositionDetailsIsVisible = false;
            UpdateLabelPdf = "";
            var new_files_created = new List<CockleFilePdf>();
            try
            {
                if (IsTypesetPdf)
                {
                    string[] new_file_name = null;
                    var num_pages = new iTextSharp.text.pdf.PdfReader(SelectedPdfFile.FullName).NumberOfPages;
                    new_file_name = generateImposedTypesetPdfFilename(num_pages);

                    ImposeSingleTypesetPdf imposed_doc = null;
                    if (HasCoverPdf && IsPerfectBindPdf && num_pages > 1)
                    {
                        // two docs
                        // cover first: extract page 1

                        using (var file_stream = PdfUtilities.ConvertPdfFileToStream(SelectedPdfFile.FullName))
                        {
                            // problem here: cv_stream is dying !!!
                            using (var cv_stream = extractPagesFromPerfectBindStream(file_stream, 1, 1))
                            {
                                imposed_doc = new ImposeSingleTypesetPdf(cv_stream,
                                    IsSaddleStitchPdf ? TypeOfBindEnum.SaddleStitch : TypeOfBindEnum.PerfectBind, hasCover: true);
                                using (var fs = new System.IO.FileStream(new_file_name[0], System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    fs.Write(imposed_doc.NewDocMemStream.ToArray(), 0, imposed_doc.NewDocMemStream.ToArray().Length);
                                }
                            }
                            using (var br_stream = PdfUtilities.ExtractPdfPagesToStream(file_stream, 2, num_pages))
                            {
                                imposed_doc = new ImposeSingleTypesetPdf(br_stream,
                                    IsSaddleStitchPdf ? TypeOfBindEnum.SaddleStitch : TypeOfBindEnum.PerfectBind, hasCover: false);
                                using (var fs = new System.IO.FileStream(new_file_name[1], System.IO.FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    fs.Write(imposed_doc.NewDocMemStream.ToArray(), 0, imposed_doc.NewDocMemStream.ToArray().Length);
                                }
                            }
                        }
                        //new_files_created.Add(new CockleFilePdf( new_file_name[0]);
                    }
                    else
                    {
                        imposed_doc = new ImposeSingleTypesetPdf(SelectedPdfFile,
                            IsSaddleStitchPdf ? TypeOfBindEnum.SaddleStitch : TypeOfBindEnum.PerfectBind, HasCoverPdf);
                        using (var fs = new System.IO.FileStream(new_file_name[0], System.IO.FileMode.Create, System.IO.FileAccess.Write))
                        {
                            fs.Write(imposed_doc.NewDocMemStream.ToArray(), 0, imposed_doc.NewDocMemStream.ToArray().Length);
                        }
                    }
                }
                else if (IsCameraReadyCenteredPdf)
                {
                    var new_files = PdfCropAndNUp.PdfCookbook.CreatePrintReadyFile(
                        SelectedPdfFile.FullName,
                        HasCoverPdf,
                        IsSaddleStitchPdf
                            ? PdfCropAndNUp.PdfBindTypeEnum.SaddleStitch
                            : PdfCropAndNUp.PdfBindTypeEnum.PerfectBind,
                        null,
                        IsCameraReadyCenteredPdf,
                        IsCameraReadyOffsetPdf
                        );
                    if (new_files.All(x => System.IO.File.Exists(x.FullName)))
                    {
                        UpdateLabelPdf = "Created:";
                        foreach (var x in new_files)
                        {
                            if (!FilesConvertedToPdf.Any(y => x.FullName == y.FullName))
                            {
                                FilesConvertedToPdf.Add(new CockleFilePdf(x.FullName, SourceFileTypeEnum.Imposed_Pdf));
                            }
                            UpdateLabelPdf += " " + System.IO.Path.GetFileNameWithoutExtension(x.FullName);
                        }
                    }
                }
                else if (IsCameraReadyOffsetPdf)
                {
                    var new_files = PdfCropAndNUp.PdfCookbook.CreatePrintReadyFile(
                        SelectedPdfFile.FullName,
                        HasCoverPdf,
                        IsSaddleStitchPdf
                            ? PdfCropAndNUp.PdfBindTypeEnum.SaddleStitch
                            : PdfCropAndNUp.PdfBindTypeEnum.PerfectBind,
                        null,
                        IsCameraReadyCenteredPdf,
                        IsCameraReadyOffsetPdf
                        );
                    if (new_files.All(x => System.IO.File.Exists(x.FullName)))
                    {
                        UpdateLabelPdf = "Created:";
                        new_files.ForEach(x =>
                        {
                            FilesConvertedToPdf.Add(new CockleFilePdf(x.FullName, SourceFileTypeEnum.Imposed_Pdf));
                            UpdateLabelPdf += " " + System.IO.Path.GetFileNameWithoutExtension(x.FullName);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        private string[] generateImposedTypesetPdfFilename(int num_pages)
        {
            var new_file_name = new string[] { string.Empty, string.Empty };

            // get custom name
            var custom_name_a = string.Empty;
            var custom_name_b = string.Empty;
            if (IsSaddleStitchPdf)
            {
                if (HasCoverPdf && num_pages == 1)
                {
                    custom_name_a = "Cover_SaddleStitch";
                }
                else if (HasCoverPdf && num_pages > 1)
                {
                    custom_name_a = "Cover_&_Brief_SaddleStitch";
                }
                else if (!HasCoverPdf)
                {
                    custom_name_a = "Brief_SaddleStitch";
                }
            }
            else // Perfect Bind
            {
                if (HasCoverPdf && num_pages == 1)
                {
                    custom_name_a = "Cover_PerfectBind";
                }
                else if (HasCoverPdf && num_pages > 1)
                {
                    // need two names here
                    custom_name_a = "Cover_PerfectBind";
                    custom_name_b = "Brief_PerfectBind";
                }
                else if (!HasCoverPdf)
                {
                    custom_name_a = "Brief_PerfectBind";
                }
            }

            // get ticket + atty
            var ticket_atty = FilesConvertedToPdf.Where(x => x.TicketNumber != null && !(string.IsNullOrEmpty(x.Attorney))).Select(x => x.TicketPlusAttorney).FirstOrDefault().Trim();
            if (string.IsNullOrEmpty(ticket_atty))
            { ticket_atty = FilesConvertedToPdf.Where(x => x.TicketNumber != null).Select(x => x.TicketNumber).FirstOrDefault().ToString().Trim(); }
            custom_name_a = ticket_atty + " " + custom_name_a;
            if (!string.IsNullOrEmpty(custom_name_b)) custom_name_b = (ticket_atty + " " + custom_name_b).Trim();

            new_file_name[0] = System.IO.Path.Combine(DestinationFolderConvertedFiles, custom_name_a + ".pdf");
            var i = 0;
            while (System.IO.File.Exists(new_file_name[0]))
            {
                new_file_name[0] = new_file_name[0].Substring(0, new_file_name[0].Length - 4);
                new_file_name[0] = System.IO.Path.Combine(DestinationFolderConvertedFiles, new_file_name[0] + "_" + i + ".pdf");
                i++;
            }
            if (!string.IsNullOrEmpty(custom_name_b))
            {
                new_file_name[1] = System.IO.Path.Combine(DestinationFolderConvertedFiles, custom_name_b + ".pdf");
                i = 0;
                while (System.IO.File.Exists(new_file_name[1]))
                {
                    new_file_name[1] = new_file_name[1].Substring(0, new_file_name[1].Length - 4);
                    new_file_name[1] = System.IO.Path.Combine(DestinationFolderConvertedFiles, new_file_name[1] + "_" + i + ".pdf");
                    i++;
                }
            }
            return new_file_name;
        }

        private System.IO.MemoryStream extractPagesFromPerfectBindStream(System.IO.MemoryStream orig_stream,
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
                for (var i = start_page; i <= end_page; i++)
                {
                    var page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                reader.Close();
                doc.Close();
            }
            catch (Exception excpt)
            {
                System.Diagnostics.Debug.WriteLine(excpt);
            }
            return dest_stream;
        }

        #endregion

        #region CENTER PDF: 1) Show user options, 2) Execute
        private bool canCenterPdfOptions()
        {
            if (null != SelectedPdfFile)
            {
                return true;
            }
            return false;
        }
        private void centerPdfOptions()
        {
            try
            {
                // preset booleans
                // preset cover length
                var cv_len = FilesConvertedToPdf.Where(x => x.CoverLength != null).Select(x => x.CoverLength).FirstOrDefault();
                switch (cv_len)
                {
                    case 48:
                        Cover48PicaPdf = true;
                        break;
                    case 49:
                        Cover49PicaPdf = true;
                        break;
                    case 50:
                        Cover50PicaPdf = true;
                        break;
                    case 51:
                        Cover51PicaPdf = true;
                        break;
                    default:
                        Cover48PicaPdf = false;
                        Cover49PicaPdf = false;
                        Cover50PicaPdf = false;
                        Cover51PicaPdf = false;
                        break;
                }

                // preset "has cover"
                var file_type = SelectedPdfFile.FileType;
                switch (file_type)
                {
                    case SourceFileTypeEnum.Cover:
                        NoCover_CenterPdf = false;
                        HasCover_CenterPdf = true;
                        break;
                    default:
                        NoCover_CenterPdf = true;
                        HasCover_CenterPdf = false;
                        break;
                }

                // preset page size
                LetterSelected_CenterPdf = true;
                BookletSelected_CenterPdf = false;

                // preset position of original
                UpperLeftPosition_CenterPdf = false;
                CenterPosition_CenterPdf = true;
                NotCentered_CenterPdf = false;

                // call modal overlay
                ModalOverlayIsVisible = true;
                CenterPdfSelectorIsVisible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private bool canExecuteCenterPdf()
        {
            var can_ex = false;

            if (null != SelectedPdfFile)
            {
                can_ex = true;
            }

            if (UpperLeftPosition_CenterPdf)
            {
                if (Cover48PicaPdf || Cover49PicaPdf || Cover50PicaPdf || Cover51PicaPdf)
                {
                    can_ex = true;
                }
            }
            return can_ex;
        }
        private async void executeCenterPdf()
        {
            ModalOverlayIsVisible = false;
            CenterPdfSelectorIsVisible = false;
            UpdateLabelPdf = "";
            try
            {
                // get new file name
                var cv_type = CenteredCoverType.NotSet;
                var custom_filename_text = string.Empty;
                if (BookletSelected_CenterPdf)
                {
                    cv_type = CenteredCoverType.Booklet;
                    custom_filename_text = "CenteredOnBookletSizedPage";
                }
                else if (LetterSelected_CenterPdf)
                {
                    cv_type = CenteredCoverType.Letter;
                    custom_filename_text = "CenteredOnLetterSizedPage";
                }
                var ticket_atty = FilesConvertedToPdf.Where(x => !string.IsNullOrEmpty(x.TicketPlusAttorney)).Select(x => x.TicketPlusAttorney).FirstOrDefault();
                var new_file_name = PdfUtilities.GenerateFilenameForNewPdf(DestinationFolderConvertedFiles, custom_filename_text, ticket_atty);

                // typeset -- upper left
                if (UpperLeftPosition_CenterPdf)
                {
                    // this is a simple typeset centering, done with the one step process
                    int? cv_len = null;
                    if (HasCover_CenterPdf)
                    {
                        // two ways to get cover length: 1) check other files in directory, 2) iText method
                        if (FilesConvertedToPdf.Any(x => null != x.CoverLength))
                        {
                            cv_len = FilesConvertedToPdf.Where(x => null != x.CoverLength).FirstOrDefault().CoverLength;
                        }
                        else
                        {
                            cv_len = PdfCropAndNUp.StaticUtils.GetCoverLength_FirstPageTypesetPdf(SelectedPdfFile.FullName);
                        }
                    }
                    var c = await System.Threading.Tasks.Task.Run(() =>
                        new Models.CenterPdfText.CenterTypesetPdf(SelectedPdfFile, cv_len, new_file_name, cv_type));
                    if (null != c && null != c.NewFileCreated
                        && System.IO.File.Exists(c.NewFileCreated.FullName))
                    {
                        if (!FilesConvertedToPdf.Any(x => x.FullName == c.NewFileCreated.FullName))
                        {
                            FilesConvertedToPdf.Add(c.NewFileCreated);
                        }
                        UpdateLabelPdf = "Created: " + System.IO.Path.GetFileNameWithoutExtension(c.NewFileCreated.FullName);
                    }
                }
                // already centered
                else if (CenterPosition_CenterPdf)
                {
                    // if already centered, no need to separate out cover
                    Models.CenterPdfText.CenterCameraReadyBrief ctr = null;
                    if (BookletSelected_CenterPdf)
                    {
                        ctr = await System.Threading.Tasks.Task.Run(() =>
                            new Models.CenterPdfText.CenterCameraReadyBrief(SelectedPdfFile, new_file_name, CenteredCoverType.Booklet));
                    }
                    else if (LetterSelected_CenterPdf)
                    {
                        ctr = await System.Threading.Tasks.Task.Run(() =>
                            new Models.CenterPdfText.CenterCameraReadyBrief(SelectedPdfFile, new_file_name, CenteredCoverType.Letter));
                    }
                    if (null != ctr && null != ctr.NewFileCreated
                        && System.IO.File.Exists(ctr.NewFileCreated.FullName))
                    {
                        if (!FilesConvertedToPdf.Any(x => x.FullName == ctr.NewFileCreated.FullName))
                        {
                            FilesConvertedToPdf.Add(ctr.NewFileCreated);
                            UpdateLabelPdf = "Created: " + System.IO.Path.GetFileNameWithoutExtension(ctr.NewFileCreated.FullName);
                        }
                    }
                }
                // need to develop this: method to crop (find text boundaries) and center
                else if (NotCentered_CenterPdf)
                {

                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                UpdateLabelPdf = ex.Message;
            }
        }
        #endregion

        #region COLLECT FILES FROM 1) Scratch,  2) Camera Ready
        private bool canOpenSelectedCameraReadyFile()
        {
            return null != FilesFoundInCameraReady
                && FilesFoundInCameraReady.Any(x => x.IsSelected);
        }
        private void openSelectedCameraReadyFile()
        {
            FilesFoundInCameraReady.Where(x => x.IsSelected)
                .ToList()
                .ForEach(x =>
                {
                    System.Diagnostics.Process.Start(x.FileName);
                });

            //System.Diagnostics.Process.Start(SelectedFileInCameraReady.FileName);
        }
        private bool canShowOnlyCameraReadySearchTextTickets(object o)
        {
            return !string.IsNullOrEmpty(CameraReadySearchText);
        }
        private bool canSaveSelectedCameraReadyFilesToScratch(object o)
        {
            return null != SelectedFileInCameraReady;
        }
        private void saveSelectedCameraReadyFilesToScratch(object o)
        {
            try
            {
                if (null == FilesConvertedToPdf) { FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(); }
                else { FilesConvertedToPdf.Clear(); DestinationFolderConvertedFiles = string.Empty; }

                // also clear Files & UpdateLabel
                Files?.Clear();
                UpdateLabel = string.Empty;
                UpdateLabelPdf = string.Empty;

                var selections = FilesFoundInCameraReady.Where(x => x.IsSelected).ToList();
                if (selections.Count() == 0)
                {
                    throw new Exception("No files selected");
                }
                var time_stamp = string.Format("({0} {1}, {2}, {3})", DateTime.Now.ToString("MMM")/*Oct*/,
                    DateTime.Now.ToString("dd")/*09*/, DateTime.Now.ToString("yyy")/*2015*/,
                    DateTime.Now.ToString("T").ToLower()/*10:58:44 AM*/ /*, len_text*/)
                    .Replace(':', ' ');
                var ticket_no = selections.Where(x => !string.IsNullOrEmpty(x.Ticket)).FirstOrDefault().Ticket;
                var atty = selections.Where(x => !string.IsNullOrEmpty(x.Attorney)).FirstOrDefault().Attorney;
                var new_folder = $"c:\\scratch\\{ticket_no} {atty} {time_stamp}";
                System.IO.Directory.CreateDirectory(new_folder);

                DestinationFolderConvertedFiles = new_folder;
                // move to scratch and add to FilesConvertedToPdf collection
                selections.ForEach(x =>
                {
                    var new_full_name = System.IO.Path.Combine(new_folder, x.ShortFileName);
                    System.IO.File.Copy(x.FileName, new_full_name, true);
                    if (System.IO.File.Exists(new_full_name))
                    {
                        FilesConvertedToPdf.Add(new CockleFilePdf(new_full_name, SourceFileTypeEnum.Camera_Ready));
                    }
                });
            }
            catch (Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
            finally
            {
                ModalOverlayIsVisible = false;
                CameraReadyFileSelectorIsVisible = false;
            }
        }
        private bool canSelectFolderInScratch(object o)
        {
            return true;
            //throw new NotImplementedException();
        }
        private void selectFolderInScratch(object o)
        {
            // PROBLEM HERE: CRASHES IF SelectedFolderInScratch is null;

            // close overlay
            ModalOverlayIsVisible = false;
            ScratchFolderSelectorIsVisible = false;

            UpdateLabelPdf = "";

            // clear out existing files
            Files?.Clear(); UpdateLabel = string.Empty;
            if (null == FilesConvertedToPdf) { FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(); }
            else { FilesConvertedToPdf.Clear(); DestinationFolderConvertedFiles = string.Empty; }
            if (null == WordFilesInScratch) { WordFilesInScratch = new ObservableCollection<CockleFile>(); }
            else { WordFilesInScratch.Clear(); DestinationFolderConvertedFiles = string.Empty; }

            if (null == SelectedFolderInScratch) { return; }

            var files = System.IO.Directory.EnumerateFiles(SelectedFolderInScratch.FolderName).ToList();

            files.ForEach(x =>
            {
                FilesConvertedToPdf.Add(new CockleFilePdf(x, SourceFileTypeEnum.Unrecognized));

                if (x.EndsWith(".docx")) 
                {
                    CockleFile wordFileInScratch = new CockleFile(x);
                    wordFileInScratch.IsLatestFile = true;
                    WordFilesInScratch.Add(wordFileInScratch); 
                }
            });

            DestinationFolderConvertedFiles =
                System.IO.Path.GetDirectoryName(FilesConvertedToPdf.FirstOrDefault().FullName);
        }
        private bool canGetFilesFromScratchFolder()
        {
            return true;
        }
        private void getFilesFromScratchFolder()
        {
            try
            {
                // clear old results
                if (null == FoldersFoundInScratch) { FoldersFoundInScratch = new ObservableCollection<CockleFolderInScratch>(); }
                else { FoldersFoundInScratch?.Clear(); }
                // populate listbox
                var foldersInScratch = System.IO.Directory.EnumerateDirectories(@"c:\scratch").ToList();
                foldersInScratch.ForEach(x => FoldersFoundInScratch.Add(new CockleFolderInScratch(x)));
                ModalOverlayIsVisible = true;
                ScratchFolderSelectorIsVisible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region COMBINE FILES
        private void combineFilesConvertedToPdf_Ordered()
        {
            ModalOverlayIsVisible = false;
            SetFileOrderModalIsVisible = false;
            UpdateLabelPdf = "";

            try
            {
                var combined_file_names = new List<string>();
                var combine_files = FilesConvertedToPdf_Ordered.ToList();

                var ticket_atty = FilesConvertedToPdf.Where(x => !string.IsNullOrEmpty(x.TicketPlusAttorney)).Select(x => x.TicketPlusAttorney).FirstOrDefault();
                var new_file_name = PdfUtilities.GenerateFilenameForNewPdf(DestinationFolderConvertedFiles, "CombinedPdf", ticket_atty);

                if (StaticSystemTests.IsAdobePdfPrinterAvailable())
                {
                    // create combined pdf
                    Models.MergePdf.SimpleMergedPdf simpleMergedPdf;
                    if (!string.IsNullOrEmpty(new_file_name))
                    {
                        simpleMergedPdf = new Models.MergePdf.SimpleMergedPdf(combine_files, "Acrobat", true, new_file_name);
                    }
                    else
                    {
                        simpleMergedPdf = new Models.MergePdf.SimpleMergedPdf(combine_files, "Acrobat", true);
                    }

                    // add new file to FilesConvertedToPdf collection
                    if (System.IO.File.Exists(simpleMergedPdf?.NewCombinedFile))
                    {
                        if (!FilesConvertedToPdf.Any(x => x.FullName == simpleMergedPdf.NewCombinedFile))
                        {
                            FilesConvertedToPdf.Add(new CockleFilePdf(simpleMergedPdf.NewCombinedFile, SourceFileTypeEnum.Combined_Pdf));
                            UpdateLabelPdf = "Created: " + simpleMergedPdf.NewCombinedFile;
                        }
                    }
                }
                else // using PdfSharp or iTextSharp ???
                {
                    //combined_file_name = new SimpleMergedPdf(SelectedPdfFiles, "PdfSharp").NewCombinedFile;
                    //var combined_file = new SimpleMergedPdf(SelectedPdfFiles, "iTextSharp").NewFileName;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                UpdateLabelPdf = ex.Message;
            }
        }
        private void combineSelectedPdfFilesOptions()
        {
            // clear old results
            if (null == FilesConvertedToPdf_Ordered)
            { FilesConvertedToPdf_Ordered = new ObservableCollection<CockleFilePdf>(); }
            else { FilesConvertedToPdf_Ordered.Clear(); }
            // load list
            var ranked_files = SelectedPdfFiles.OrderBy(x => x.Rank).ToList();
            ranked_files.ForEach(x => FilesConvertedToPdf_Ordered.Add(x));

            // open overlay
            ModalOverlayIsVisible = true;
            SetFileOrderModalIsVisible = true;
        }
        private bool canCombineSelectedPdfFilesOptions()
        {
            return null != SelectedPdfFiles
                && SelectedPdfFiles.Count > 1
                && SelectedPdfFiles.TrueForAll(x => x.TicketNumber != null && x.TicketNumber == SelectedPdfFiles.FirstOrDefault().TicketNumber);
        }
        #endregion

        #region CONVERT TO PDF/A FILES
        private bool canRedistillPdfA(object o)
        {
            return null != SelectedPdfFile;
        }
        private async void redistillPdfA(object o)
        {

            UpdateLabelPdf = "";

            try
            {
                var new_cockleFilePdf = await Task.Run(() =>
                {
                    var conversion = new ConvertToPdfa(SelectedPdfFile);
                    return conversion.PdfaCockleFilePdf;
                });

                FilesConvertedToPdf.Add(new_cockleFilePdf);
                FilesConvertedToPdf.Remove(SelectedPdfFile);
                SelectedPdfFile = new_cockleFilePdf;

                UpdateLabelPdf = "PDF/A file successfully converted.";
            }
            catch (Exception ex)
            {
                UpdateLabelPdf = ex.Message;
                //UpdateLabelPdf = $"File from '{FilesConvertedToPdf.FirstOrDefault().TicketPlusAttorney}' failed PDF/A conversion.";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        private bool canConvertToPdfA(object o)
        {
            return null != SelectedPdfFile;
        }
        private async void convertToPdfA(object o)
        {
            UpdateLabelPdf = "";

            try
            {
                var new_cockleFilePdf = await Task.Run(() =>
                {
                    var conversion = new ConvertToPdfa(SelectedPdfFile);
                    return conversion.PdfaCockleFilePdf;
                });

                FilesConvertedToPdf.Add(new_cockleFilePdf);
                FilesConvertedToPdf.Remove(SelectedPdfFile);
                SelectedPdfFile = new_cockleFilePdf;

                UpdateLabelPdf = "PDF/A file successfully converted.";
            }
            catch (Exception ex)
            {
                UpdateLabelPdf = ex.Message;
                //UpdateLabelPdf = $"File from '{FilesConvertedToPdf.FirstOrDefault().TicketPlusAttorney}' failed PDF/A conversion.";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}

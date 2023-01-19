using FileSearchMvvm.Commands;
using FileSearchMvvm.Models.CockleTypes;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel : ViewModelBase
    {
        #region CONSTRUCTOR
        public SearchViewModel()
        {
            // initialize major collections
            Files = new ObservableCollection<CockleFile>();
            FilesListCollectionView = new System.Windows.Data.ListCollectionView(Files);
            FilesListCollectionView.GroupDescriptions.Add(
                new System.Windows.Data.PropertyGroupDescription("TicketPlusAttorney"));
            FilesFoundInCameraReady = new ObservableCollection<CockleFileInCameraReady>();
            FoldersFoundInScratch = new ObservableCollection<CockleFolderInScratch>();
            FilesConvertedToPdf_Ordered = new ObservableCollection<CockleFilePdf>();

            // initialize gui controls
            IsExecutingSearch = false;
            ShowLatestFiles = true;
            closeModalContentOverlay(); // set to all modals to false
            CenteredCoverType = CenteredCoverType.NotSet;

            // test for Javascript, and replace if necessary
            var acro_js_is_working = Models.Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace();

            #region populate recent search terms
            UserSearchTerms = new ObservableCollection<string>();

            if(null == Properties.Settings.Default.RecentSearchTerms)
            {
                for(var i = 0; i < 6; i++) { UserSearchTerms.Add(""); }
            }
            else if(Properties.Settings.Default.RecentSearchTerms.Equals(""))
            {
                for(var i = 0; i < 6; i++) { UserSearchTerms.Add(""); }
            }
            else
            {
                var orig = Properties.Settings.Default.RecentSearchTerms.Split(',');
                var origCount = 0;
                foreach(var o in orig)
                {
                    if(origCount == 6) { break; }
                    UserSearchTerms.Add(o);
                    origCount++;
                }
                while(UserSearchTerms.Count() < 6) { UserSearchTerms.Add(""); }
            }
            #endregion

            #region Command Registry: instantiate command properties (new RelayCommand)
            // Search command logic
            RunSearch = new RelayCommand(o => search(), o => canSearch());
            RunCancelSearch = new RelayCommand(o => cancelSearchOrConvert(), o => canCancelSearchOrConvert());
            RunOpenSourceFolder = new RelayCommand(o => openSourceFolder(), o => canOpenSourceFolder());
            RunOpenSelectedFile = new RelayCommand(o => openSelectedFiles(), o => canOpenSelectedFile());
            RunConvertToPdf = new RelayCommand(o => convertFilesToPdf(), o => canConvertFilesToPdf());
            RunConvertToPdfProof = new RelayCommand(o => convertToPdfProof(), o => canConvertToPdfProof());
            RunSearchAndConvert = new RelayCommand(o => searchAndConvertToPdf(), o => canSearchAndConvertToPdf());
            RunSearchAndSaveToScratch = new RelayCommand(o => searchAndSaveToScratch(), o => canSearchAndSaveToScratch());

            // Modal overlay logic commands
            RunSelectFolderInScratch = new RelayCommand(o => selectFolderInScratch(o), o => canSelectFolderInScratch(o));
            RunImposePdfOptions = new RelayCommand(o => imposePdfOptions(o/*param for circuit or supreme court*/), o => canImposePdfOptions());
            RunConvertWordFilesOffline = new RelayCommand(o => convertWordFilesOffline(o), o => canConvertWordFilesOffline(o));
            RunShowOnlyCameraReadySearchTextTickets = new RelayCommand(o => showOnlyCameraReadySearchTextTickets(o), o => canShowOnlyCameraReadySearchTextTickets(o));
            RunShowAllCameraReadyFilesAndClearSearchText = new RelayCommand(o => showAllCameraReadyFilesAndClearSearchText(), o => canShowAllCameraReadyFilesAndClearSearchText());
            RunCloseModalContentOverlay = new RelayCommand(o => closeModalContentOverlay());

            // Simple PDF logic commands
            RunOpenSelectedCameraReadyFile = new RelayCommand(o => openSelectedCameraReadyFile(), o => canOpenSelectedCameraReadyFile());
            RunGetFilesFromScratchFolder = new RelayCommand(o => getFilesFromScratchFolder(), o => canGetFilesFromScratchFolder());
            RunSaveSelectedCameraReadyFilesToScratch = new RelayCommand(o => saveSelectedCameraReadyFilesToScratch(o), o => canSaveSelectedCameraReadyFilesToScratch(o));

            // Manipulate PDF logic commands
            RunConvertToPdfA = new RelayCommand(o => convertToPdfA(o), o => canConvertToPdfA(o));
            RunRedistillPdfA = new RelayCommand(o => redistillPdfA(o), o => canRedistillPdfA(o));
            RunCombineFilesConvertedToPdf_Ordered = new RelayCommand(o => combineFilesConvertedToPdf_Ordered());
            RunCombineSelectedPdfFilesOptions = new RelayCommand(o => combineSelectedPdfFilesOptions(), o => canCombineSelectedPdfFilesOptions());
            RunImposedPdf = new RelayCommand(o => imposePdf(), o => canImposePdf());
            RunImposeCircuitCourtCover_11x19 = new RelayCommand(o => imposeCircuitCourtCover_11x19());
            RunImposeCircuitCourtCover_8pt5x23 = new RelayCommand(o => imposeCircuitCourtCover_8pt5x23());
            RunImposeCircuitCourtBrief = new RelayCommand(o => imposeCircuitCourtBrief(), o => canImposeCircuitCourtBrief());
            RunCenterPdfOptions = new RelayCommand(o => centerPdfOptions(), o => canCenterPdfOptions());
            RunExecuteCenterPdf = new RelayCommand(o => executeCenterPdf(), o => canExecuteCenterPdf());
            RunExtractPagesFromPdfDocument = new RelayCommand(o => extractPagesFromPdfDocument(), o => canExtractPagesFromPdfDocument());

            // Logic commands associated with UI (SelectionChanged, Refresh list, Clear list, Open directories/files)
            RunSelectionChanged = new RelayCommand(o => searchGridSelectionChanged(o));
            RunPdfSelectionChanged = new RelayCommand(o => pdfGridSelectionChanged(o));
            RunPdfSelectionChanged_Ordered = new RelayCommand(o => pdfSelectionChanged_Ordered(o));
            RunRefreshLoadedPdfFolder = new RelayCommand(o => refreshLoadedPdfFolder(), o => canRefreshLoadedPdfFolder());
            RunClearPdfFiles = new RelayCommand(o => clearPdfFiles(), o => canClearPdfFiles());
            RunOpenSelectedPdfFile = new RelayCommand(o => openSelectedPdfFiles(), o => canOpenSelectedPdfFiles());
            RunOpenPdfSourceFolder = new RelayCommand(o => openPdfSourceFolder(), o => canOpenPdfSourceFolder());
            RunGetFilesFromCameraReadyFolder = new RelayCommand(o => getFilesFromCameraReadyFolder(), o => canGetFilesFromCameraReadyFolder());
            #endregion
        }

        private void convertWordFilesOffline(object o)
        {
            throw new NotImplementedException();
        }

        private bool canConvertWordFilesOffline(object o)
        {
            return WordFilesInScratch != null && WordFilesInScratch.Count() > 0;
        }

        private bool canExtractPagesFromPdfDocument()
        {
            return null != FilesConvertedToPdf
                && FilesConvertedToPdf.Count() > 0
                && SelectedPdfFile != null;
        }

        private async void extractPagesFromPdfDocument()
        {
            UpdateLabelPdf = string.Empty;
            SpecialImpositionsOverlayIsVisible = false;
            ModalOverlayIsVisible = false;

            try
            {
                var low_pg = 1;
                var high_pg = 1;
                if(!string.IsNullOrEmpty(PdfPageToExtract_First))
                {
                    bool l, h;
                    int r;
                    l = int.TryParse(PdfPageToExtract_First, out r);
                    if(l)
                    {
                        low_pg = r;
                    }
                    if(!string.IsNullOrEmpty(PdfPageToExtract_Last))
                    {
                        h = int.TryParse(PdfPageToExtract_Last, out r);
                        if(h && r > low_pg)
                        {
                            high_pg = r;
                        }
                    }
                    else
                    {
                        high_pg = low_pg;
                    }
                }

                // test nums in pdf
                var page_count = new iTextSharp.text.pdf.PdfReader(SelectedPdfFile.FullName).NumberOfPages;
                if(low_pg > page_count || high_pg > page_count)
                {
                    throw new Exception("Page numbers incorrect");
                }

                // cannot save directly to subdirectory
                // directories have commas, and commas not allowed in JS filenames
                var ticket_directory = System.IO.Path.GetDirectoryName(SelectedPdfFile.FullName);
                var atty_dir_file_name = Models.Imposition.PdfUtilities.GenerateFilenameForNewPdf(
                    System.IO.Path.GetDirectoryName(SelectedPdfFile.FullName),
                    "extracted_pages", SelectedPdfFile.TicketPlusAttorney);
                var scratch_file_name = System.IO.Path.Combine("c:\\scratch",
                    System.IO.Path.GetFileName(atty_dir_file_name));

                // convert doc_name to js style
                var js_file_name = scratch_file_name.Replace("C:\\", "/C/");
                js_file_name = js_file_name.Replace("c:\\", "/c/");
                js_file_name = js_file_name.Replace('\\', '/');

                await System.Threading.Tasks.Task.Run(() =>
                {
                    carryOutPdfExtractPages(js_file_name, low_pg, high_pg);
                });

                // try to move file to correct folder
                if(System.IO.File.Exists(scratch_file_name))
                {
                    System.IO.File.Move(scratch_file_name, atty_dir_file_name);
                }
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
            finally
            {
                PdfPageToExtract_First = string.Empty;
                PdfPageToExtract_Last = string.Empty;
            }
        }

        private void carryOutPdfExtractPages(string new_file_name, int lo, int hi)
        {
            Acrobat.CAcroApp app = new Acrobat.AcroApp();           // Acrobat
            Acrobat.CAcroPDDoc doc = new Acrobat.AcroPDDoc();       // First document

            // use reflection to center the pdf
            try
            {
                var opened = doc.Open(selectedPdfFile.FullName);

                if(opened)
                {
                    object js_object = doc.GetJSObject();
                    Type js_type = js_object.GetType();
                    //object[] js_param = { 0, 0, "/c/scratch/blah.pdf" };
                    object[] js_param = { lo - 1, hi - 1, new_file_name }; // subtract 1 to make base 0
                    string script_name = Models.Utilities.AcrobatJS.Javascripts
                                    [Models.Utilities.LocalJavascripts.extractPagesFromDocument];
                    js_type.InvokeMember(script_name,
                        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                        null, js_object, js_param);
                }
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                app = null;

                GC.Collect();
            }
        }
        #endregion

        #region UNDEVELOPED METHODS
        private bool canImposeCircuitCourtBrief()
        {
            return false;
        }

        private void imposeCircuitCourtBrief()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
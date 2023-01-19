using System.Windows.Input;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel
    {
        #region ICOMMAND PROPERTIES
        public ICommand RunExtractPagesFromPdfDocument { get; private set; }
        public ICommand RunCancelSearch { get; private set; }
        public ICommand RunCenterPdfOptions { get; private set; }
        public ICommand RunClearPdfFiles { get; private set; }
        public ICommand RunCloseModalContentOverlay { get; private set; }
        public ICommand RunCombineFilesConvertedToPdf_Ordered { get; private set; }
        public ICommand RunCombineSelectedPdfFilesInOrderOptions { get; private set; }
        public ICommand RunCombineSelectedPdfFilesOptions { get; private set; }
        public ICommand RunConvertToPdf { get; private set; }
        public ICommand RunConvertToPdfA { get; private set; }
        public ICommand RunConvertToPdfProof { get; private set; }
        public ICommand RunExecuteCenterPdf { get; private set; }
        public ICommand RunGetFilesFromCameraReadyFolder { get; private set; }
        public ICommand RunGetFilesFromScratchFolder { get; private set; }
        public ICommand RunImposeCircuitCourtBrief { get; private set; }
        public ICommand RunImposeCircuitCourtCover_11x19 { get; private set; }
        public ICommand RunImposeCircuitCourtCover_8pt5x23 { get; private set; }
        public ICommand RunImposedPdf { get; private set; }
        public ICommand RunImposePdfOptions { get; private set; }
        public ICommand RunConvertWordFilesOffline { get; private set; }
        public ICommand RunOpenPdfSourceFolder { get; private set; }
        public ICommand RunOpenSelectedCameraReadyFile { get; private set; }
        public ICommand RunOpenSelectedFile { get; private set; }
        public ICommand RunOpenSelectedPdfFile { get; private set; }
        public ICommand RunOpenSourceFolder { get; private set; }
        public ICommand RunPdfSelectionChanged { get; private set; }
        public ICommand RunPdfSelectionChanged_Ordered { get; private set; }
        public ICommand RunRedistillPdfA { get; private set; }
        public ICommand RunRefreshLoadedPdfFolder { get; private set; }
        public ICommand RunSaveSelectedCameraReadyFilesToScratch { get; private set; }
        public ICommand RunSearch { get; private set; }
        public ICommand RunSearchAndConvert { get; private set; }
        public ICommand RunSearchAndSaveToScratch { get; private set; }
        public ICommand RunSelectFolderInScratch { get; private set; }
        public ICommand RunSelectionChanged { get; private set; }
        public ICommand RunShowAllCameraReadyFilesAndClearSearchText { get; private set; }
        public ICommand RunShowOnlyCameraReadySearchTextTickets { get; private set; }
        #endregion
    }
}

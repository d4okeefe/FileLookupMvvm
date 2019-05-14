namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel
    {
        private string pdfPageToExtract_First;
        private string pdfPageToExtract_Last;
        private bool blankPagesAdded;
        private bool blankPagesNotAdded;
        private bool bookletSelected_CenterPdf;
        private bool cameraReadyFileSelectorIsVisible;
        private bool centerPosition_CenterPdf;
        private bool collapseTickets;
        private bool cover48PicaPdf;
        private bool cover49PicaPdf;
        private bool cover50PicaPdf;
        private bool cover51PicaPdf;
        private bool coverLengthBookletSelectorIsVisible;
        private bool coverLengthSelectorIsVisible;
        private bool hasCover_CenterPdf;
        private bool hasCoverPdf;
        private bool isCameraReadyCenteredPdf;
        private bool isCameraReadyOffsetPdf;
        private bool isCameraReadyPdf;
        private bool isConvertingToPdf;
        private bool isExecutingSearch;
        private bool isPerfectBindPdf;
        private bool isSaddleStitchPdf;
        private bool isTypesetPdf;
        private bool letterSelected_CenterPdf;
        private bool modalOverlayIsVisible;
        private bool noCover_CenterPdf;
        private bool noCoverBriefOnlyPdf;
        private bool notCentered_CenterPdf;
        private bool pages00to16Pdf;
        private bool pages17to32Pdf;
        private bool pages33to48Pdf;
        private bool pages49to64Pdf;
        private bool pages64to80Pdf;
        private bool pdfFilesExist;
        private bool scratchFolderSelectorIsVisible;
        private bool searchEverywhere;
        private bool searchTextHasValue;
        private bool selectImposeBriefOptions;
        private bool selectImposeCoverOptions;
        private bool specialImpositionsOverlayIsVisible;
        private bool selectImpositionDetailsIsVisible;
        private bool setFileOrderModalIsVisible;
        private bool showLatestFiles;
        private bool upperLeftPosition_CenterPdf;
        private CenteredCoverType centeredCoverType;
        private int coverLengthInput;
        private Models.CockleTypes.CockleFileInCameraReady selectedFileInCameraReady;
        private Models.CockleTypes.CockleFilePdf selectedPdfFile;
        private Models.CockleTypes.CockleFilePdf selectedPdfFile_Ordered;
        private string cameraReadySearchText;
        private string cameraReadyTicketNumber;
        private string destinationFolderConvertedFiles;
        private string searchText;
        private string updateLabel;
        private string updateLabelPdf;
        private System.Collections.Generic.List<Models.CockleTypes.CockleFilePdf> selectedPdfFiles;
        private System.Collections.ObjectModel.ObservableCollection<Models.CockleTypes.CockleFilePdf> filesConvertedToPdf;
        private System.Collections.ObjectModel.ObservableCollection<string> userSearchTerms;
        private System.IO.FileSystemWatcher watcherPdfFolder;
        private System.Linq.IOrderedEnumerable<Models.CockleTypes.CockleFilePdf> pdfFileSelected_Ordered;
        private System.Threading.CancellationTokenSource convertCancelTokenSource;
        private System.Threading.CancellationTokenSource searchCancelTokenSource;

    }
}

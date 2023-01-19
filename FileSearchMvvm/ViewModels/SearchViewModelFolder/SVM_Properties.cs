using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.ViewModels.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using DocumentFormat.OpenXml.Drawing;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public enum CenteredCoverType { Letter, Booklet, NotSet }
    public partial class SearchViewModel
    {
        public string PdfPageToExtract_Last
        {
            get { return pdfPageToExtract_Last; }
            set
            {
                var orig_val = pdfPageToExtract_Last;
                if(canExtractPagesFromPdfDocument())
                {
                    pdfPageToExtract_Last = "";
                }

                int result;
                if(!int.TryParse(value, out result)) pdfPageToExtract_Last = "";
                else pdfPageToExtract_Last = value;

                if(orig_val != pdfPageToExtract_Last) RaisePropertyChanged();
            }
        }
        public string PdfPageToExtract_First
        {
            get { return pdfPageToExtract_First; }
            set
            {
                var orig_val = pdfPageToExtract_First;
                if(canExtractPagesFromPdfDocument())
                {
                    pdfPageToExtract_First = "";
                }

                int result;
                if(!int.TryParse(value, out result)) pdfPageToExtract_First = "";
                else pdfPageToExtract_First = value;

                if(orig_val != pdfPageToExtract_First) RaisePropertyChanged();
            }
        }
        #region new properties related to centering pdf
        public bool BookletSelected_CenterPdf
        {
            get { return bookletSelected_CenterPdf; }
            set
            {
                bookletSelected_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool LetterSelected_CenterPdf
        {
            get { return letterSelected_CenterPdf; }
            set
            {
                letterSelected_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool HasCover_CenterPdf
        {
            get { return hasCover_CenterPdf; }
            set
            {
                hasCover_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool NoCover_CenterPdf
        {
            get { return noCover_CenterPdf; }
            set
            {
                noCover_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool UpperLeftPosition_CenterPdf
        {
            get { return upperLeftPosition_CenterPdf; }
            set
            {
                upperLeftPosition_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool CenterPosition_CenterPdf
        {
            get { return centerPosition_CenterPdf; }
            set
            {
                centerPosition_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool NotCentered_CenterPdf
        {
            get { return notCentered_CenterPdf; }
            set
            {
                notCentered_CenterPdf = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        public bool Pages00to16Pdf
        {
            get { return pages00to16Pdf; }
            set
            {
                pages00to16Pdf = value;
                if(pages00to16Pdf)
                {
                    Pages17to32Pdf = false;
                    Pages33to48Pdf = false;
                    Pages49to64Pdf = false;
                    Pages65to80Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Pages17to32Pdf
        {
            get { return pages17to32Pdf; }
            set
            {
                pages17to32Pdf = value;
                if(pages17to32Pdf)
                {
                    Pages00to16Pdf = false;
                    Pages33to48Pdf = false;
                    Pages49to64Pdf = false;
                    Pages65to80Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Pages33to48Pdf
        {
            get { return pages33to48Pdf; }
            set
            {
                pages33to48Pdf = value;
                if(pages33to48Pdf)
                {
                    Pages00to16Pdf = false;
                    Pages17to32Pdf = false;
                    Pages49to64Pdf = false;
                    Pages65to80Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Pages49to64Pdf
        {
            get { return pages49to64Pdf; }
            set
            {
                pages49to64Pdf = value;
                if(pages49to64Pdf)
                {
                    Pages00to16Pdf = false;
                    Pages17to32Pdf = false;
                    Pages33to48Pdf = false;
                    Pages65to80Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Pages65to80Pdf
        {
            get { return pages64to80Pdf; }
            set
            {
                pages64to80Pdf = value;
                if(pages64to80Pdf)
                {
                    Pages00to16Pdf = false;
                    Pages17to32Pdf = false;
                    Pages33to48Pdf = false;
                    Pages49to64Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public CenteredCoverType CenterCameraReadyBriefType { get; private set; }
        public bool IsCameraReadyPdf
        {
            get { return isCameraReadyPdf; }
            set
            {
                isCameraReadyPdf = value;
                if(isCameraReadyPdf)
                {
                    IsTypesetPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool IsCameraReadyCenteredPdf
        {
            get { return isCameraReadyCenteredPdf; }
            set
            {
                isCameraReadyCenteredPdf = value;
                if(isCameraReadyCenteredPdf)
                {
                    IsTypesetPdf = false;
                    IsCameraReadyOffsetPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool IsCameraReadyOffsetPdf
        {
            get { return isCameraReadyOffsetPdf; }
            set
            {
                isCameraReadyOffsetPdf = value;
                if(isCameraReadyOffsetPdf)
                {
                    IsTypesetPdf = false;
                    IsCameraReadyCenteredPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool IsTypesetPdf
        {
            get { return isTypesetPdf; }
            set
            {
                isTypesetPdf = value;
                if(isTypesetPdf)
                {
                    IsCameraReadyOffsetPdf = false;
                    IsCameraReadyCenteredPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Cover48PicaPdf
        {
            get { return cover48PicaPdf; }
            set
            {
                cover48PicaPdf = value;
                if(cover48PicaPdf)
                {
                    Cover49PicaPdf = false;
                    Cover50PicaPdf = false;
                    Cover51PicaPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Cover49PicaPdf
        {
            get { return cover49PicaPdf; }
            set
            {
                cover49PicaPdf = value;
                if(cover49PicaPdf)
                {
                    Cover48PicaPdf = false;
                    Cover50PicaPdf = false;
                    Cover51PicaPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Cover50PicaPdf
        {
            get { return cover50PicaPdf; }
            set
            {
                cover50PicaPdf = value;
                if(cover50PicaPdf)
                {
                    Cover48PicaPdf = false;
                    Cover49PicaPdf = false;
                    Cover51PicaPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool Cover51PicaPdf
        {
            get { return cover51PicaPdf; }
            set
            {
                cover51PicaPdf = value;
                if(cover51PicaPdf)
                {
                    Cover48PicaPdf = false;
                    Cover49PicaPdf = false;
                    Cover50PicaPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool HasCoverPdf
        {
            get { return hasCoverPdf; }
            set
            {
                hasCoverPdf = value;
                if(hasCoverPdf)
                {
                    NoCoverBriefOnlyPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool NoCoverBriefOnlyPdf
        {
            get { return noCoverBriefOnlyPdf; }
            set
            {
                noCoverBriefOnlyPdf = value;
                if(noCoverBriefOnlyPdf)
                {
                    HasCoverPdf = false;
                    Cover48PicaPdf = false;
                    Cover49PicaPdf = false;
                    Cover50PicaPdf = false;
                    Cover51PicaPdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool BlankPagesNotAdded
        {
            get
            {
                return blankPagesNotAdded;
            }
            set
            {
                blankPagesNotAdded = value;
                RaisePropertyChanged();
            }
        }
        public bool BlankPagesAdded
        {
            get
            {
                return blankPagesAdded;
            }
            set
            {
                blankPagesAdded = value;
                RaisePropertyChanged();
            }
        }
        public bool IsSaddleStitchPdf
        {
            get { return isSaddleStitchPdf; }
            set
            {
                isSaddleStitchPdf = value;
                if(isSaddleStitchPdf)
                {
                    IsPerfectBindPdf = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool IsPerfectBindPdf
        {
            get { return isPerfectBindPdf; }
            set
            {
                isPerfectBindPdf = value;
                if(isPerfectBindPdf)
                {
                    IsSaddleStitchPdf = false;
                    Pages00to16Pdf = false;
                    Pages17to32Pdf = false;
                    Pages33to48Pdf = false;
                    Pages49to64Pdf = false;
                    Pages65to80Pdf = false;
                }
                RaisePropertyChanged();
            }
        }
        public bool SelectImposeBriefOptions
        {
            get { return selectImposeBriefOptions; }
            set
            {
                selectImposeBriefOptions = value;
                RaisePropertyChanged();
            }
        }
        public bool SelectImposeCoverOptions
        {
            get { return selectImposeCoverOptions; }
            set
            {
                selectImposeCoverOptions = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<CockleFilePdf> FilesConvertedToPdf_Ordered { get; set; }

        public CockleFileInCameraReady SelectedFileInCameraReady
        {
            get { return selectedFileInCameraReady; }
            set
            {
                selectedFileInCameraReady = value;
                RaisePropertyChanged();
            }
        }
        public CenteredCoverType CenteredCoverType
        {
            get { return centeredCoverType; }
            set
            {
                centeredCoverType = value;
                RaisePropertyChanged();
            }
        }
        public string CameraReadySearchText
        {
            get { return cameraReadySearchText; }
            set
            {
                cameraReadySearchText = value;
                RaisePropertyChanged();
            }
        }
        public CockleFolderInScratch SelectedFolderInScratch { get; set; }
        public ObservableCollection<CockleFolderInScratch> FoldersFoundInScratch { get; set; }
        public ObservableCollection<CockleFileInCameraReady> FilesFoundInCameraReady { get; set; }
        public int CoverLengthInput
        {
            get { return coverLengthInput; }
            set
            {
                if(value == 0)
                {
                    coverLengthInput = 48;
                }
                else
                {
                    coverLengthInput = value;
                }
                RaisePropertyChanged();
            }
        }
        public bool SelectImpositionDetailsIsVisible
        {
            get { return selectImpositionDetailsIsVisible; }
            set
            {
                selectImpositionDetailsIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool SpecialImpositionsOverlayIsVisible
        {
            get { return specialImpositionsOverlayIsVisible; }
            set
            {
                specialImpositionsOverlayIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool CameraReadyFileSelectorIsVisible
        {
            get { return cameraReadyFileSelectorIsVisible; }
            set
            {
                cameraReadyFileSelectorIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool SetFileOrderModalIsVisible
        {
            get { return setFileOrderModalIsVisible; }
            set
            {
                setFileOrderModalIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool ScratchFolderSelectorIsVisible
        {
            get { return scratchFolderSelectorIsVisible; }
            set
            {
                scratchFolderSelectorIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool CoverLengthBookletSelectorIsVisible
        {
            get { return coverLengthBookletSelectorIsVisible; }
            set
            {
                coverLengthBookletSelectorIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool CoverLengthSelectorIsVisible
        {
            get { return coverLengthSelectorIsVisible; }
            set
            {
                coverLengthSelectorIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public bool ModalOverlayIsVisible
        {
            get { return modalOverlayIsVisible; }
            set
            {
                modalOverlayIsVisible = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<string> UserSearchTerms
        {
            get { return userSearchTerms; }
            set
            {
                userSearchTerms = value;
                RaisePropertyChanged();
            }
        }
        public bool ShowHideConvertToPdfWithAcrobat
        {
            get { return StaticSystemTests.IsWordInstalled() && StaticSystemTests.IsAdobePdfPrinterAvailable(); }
        }
        public bool ShowHideConvertToPdfForProof
        {
            get { return StaticSystemTests.IsWordInstalled() && StaticSystemTests.IsGhostscriptInstalled(); }
        }
        public bool IsExecutingSearch
        {
            get { return isExecutingSearch; }
            set
            {
                isExecutingSearch = value;
                RaisePropertyChanged();
            }
        }
        public bool IsConvertingToPdf
        {
            get { return isConvertingToPdf; }
            set
            {
                isConvertingToPdf = value;
                RaisePropertyChanged();
            }
        }
        public bool ShowLatestFiles
        {
            get { return showLatestFiles; }
            set
            {
                showLatestFiles = value;
                RaisePropertyChanged();
            }
        }
        public bool SearchEverywhere
        {
            get { return searchEverywhere; }
            set
            {
                searchEverywhere = value;
                RaisePropertyChanged();
            }
        }
        public bool CollapseTickets
        {
            get { return collapseTickets; }
            set
            {
                collapseTickets = value;
                RaisePropertyChanged();
            }
        }
        public bool IsSingleTicket
        {
            get
            {
                if(string.IsNullOrEmpty(SearchText))
                {
                    return false;
                }

                int outNum;
                if(int.TryParse(SearchText, out outNum))
                {
                    if(outNum > 10000 && outNum < 99999)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public string UpdateLabel
        {
            get { return updateLabel; }
            set
            {
                updateLabel = value;
                RaisePropertyChanged();
            }
        }
        public string UpdateLabelPdf
        {
            get { return updateLabelPdf; }
            set
            {
                updateLabelPdf = value;
                RaisePropertyChanged();
            }
        }
        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                RaisePropertyChanged();
            }
        }
        public bool SearchTextHasValue
        {
            get
            {
                return searchTextHasValue;
            }
            set
            {
                searchTextHasValue = !string.IsNullOrEmpty(SearchText);
                RaisePropertyChanged();
            }
        }
        public HashSet<int> TicketsFoundInSearch { get; set; }
        public ObservableCollection<CockleFilePdf> FilesConvertedToPdf
        {
            get { return filesConvertedToPdf; }
            set
            {
                filesConvertedToPdf = value;
                PdfFilesExist = FilesConvertedToPdf != null && FilesConvertedToPdf.Count > 0;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<CockleFile> WordFilesInScratch
        {
            get { return wordFilesInScratch; }
            set
            {
                wordFilesInScratch = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<CockleFile> Files { get; set; }
        public CockleFile SelectedFile { get; set; }
        public List<CockleFile> SelectedFiles { get; set; }
        public ListCollectionView FilesListCollectionView { get; set; }
        public string DestinationFolderConvertedFiles
        {
            get { return destinationFolderConvertedFiles; }
            set
            {
                destinationFolderConvertedFiles = value;
                if(null == watcherPdfFolder) watcherPdfFolder = new System.IO.FileSystemWatcher();
                if(!string.IsNullOrEmpty(DestinationFolderConvertedFiles)
                    && watcherPdfFolder.Path != DestinationFolderConvertedFiles)
                {
                    watcherPdfFolder.Path = DestinationFolderConvertedFiles;
                    watcherPdfFolder.Filter = "*.pdf";
                    watcherPdfFolder.NotifyFilter =
                        System.IO.NotifyFilters.LastAccess
                        | System.IO.NotifyFilters.LastWrite
                        | System.IO.NotifyFilters.FileName
                        | System.IO.NotifyFilters.DirectoryName;
                    watcherPdfFolder.Changed += new System.IO.FileSystemEventHandler(watcherPdfFolderChanged);
                    watcherPdfFolder.Created += new System.IO.FileSystemEventHandler(watcherPdfFolderChanged);
                    watcherPdfFolder.Deleted += new System.IO.FileSystemEventHandler(watcherPdfFolderChanged);
                    watcherPdfFolder.EnableRaisingEvents = true;
                }
                RaisePropertyChanged();
            }
        }

        public bool PdfFilesExist
        {
            get { return pdfFilesExist; }
            set
            {
                pdfFilesExist = value;
                RaisePropertyChanged();
            }
        }
        public CockleFilePdf SelectedPdfFile_Ordered
        {
            get
            {
                return selectedPdfFile_Ordered;
            }
            set
            {
                selectedPdfFile_Ordered = value;
                RaisePropertyChanged();
            }
        }
        public CockleFilePdf SelectedPdfFile
        {
            get
            {
                return selectedPdfFile;
            }
            set
            {
                selectedPdfFile = value;
                RaisePropertyChanged();
            }
        }
        public List<CockleFilePdf> SelectedPdfFiles
        {
            get { return selectedPdfFiles; }
            set
            {
                selectedPdfFiles = value;
                RaisePropertyChanged();
            }
        }
        public List<CockleFilePdf> SelectedPdfFiles_Ordered { get; set; }
        public System.Linq.IOrderedEnumerable<CockleFilePdf> PdfFileSelected_Ordered
        {
            get { return pdfFileSelected_Ordered; }
            set
            {
                pdfFileSelected_Ordered = value;
                RaisePropertyChanged();
            }
        }
        public string SourceTextForFilesInGrid { get; set; }
        public string CameraReadyTicketNumber
        {
            get
            { return cameraReadyTicketNumber; }
            set
            {
                cameraReadyTicketNumber = value;
                RaisePropertyChanged();
            }
        }
        //public bool PdfDataGridIsSelected
        //{
        //    get
        //    {
        //        return pdfDataGridIsSelected;
        //    }
        //    set
        //    {
        //        pdfDataGridIsSelected = value;
        //        RaisePropertyChanged();
        //    }
        //}
    }
}

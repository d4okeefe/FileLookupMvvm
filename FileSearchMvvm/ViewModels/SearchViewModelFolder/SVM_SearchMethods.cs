using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using static FileSearchMvvm.Models.Utilities.MarkLatestFilesStaticClass;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel
    {
        #region search methods
        private bool canSearch()
        {
            return !string.IsNullOrEmpty(SearchText) && !IsExecutingSearch && !IsConvertingToPdf;
        }
        private bool canCancelSearchOrConvert()
        {
            if (IsExecutingSearch)
            {
                return true;
            }
            if (Files?.Count() > 0)
            {
                return true;
            }
            return false;
        }
        private void reportProgress(string value)
        {
            UpdateLabel = value;
        }
        private void cancelSearchOrConvert()
        {
            try
            {
                searchCancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            try
            {
                convertCancelTokenSource?.Cancel();
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            try
            {
                UpdateLabel = UpdateLabel.Equals("Operation cancelled") ? "" : "Operation cancelled";
                if (null == Files) { Files = new ObservableCollection<CockleFile>(); }
                else { Files.Clear(); }
                if (null == FilesConvertedToPdf) { FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>(); }
                else { FilesConvertedToPdf.Clear(); }
                DestinationFolderConvertedFiles = string.Empty;
                SearchText = string.Empty;
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        private async void search()
        {
            var error_issue = string.Empty;
            IsExecutingSearch = true;
            Files?.Clear();
            var searchProgress = new Progress<string>(reportProgress);
            searchCancelTokenSource = new CancellationTokenSource();
            try
            {
                addItemToRecentSearchTerms();
                var cockleFilesFound = await Models.Search.SearchModel.SearchCurrentAndBackupT(
                    SearchText,
                    SearchEverywhere,
                    IsSingleTicket,
                    searchProgress,
                    searchCancelTokenSource.Token,
                    error_issue);
                MarkLatestFiles(cockleFilesFound);
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
                iSearchProgress.Report("Error in search for files" + '\n' + ex.Message);
            }
            finally
            {
                searchCancelTokenSource.Dispose();
                IsExecutingSearch = false;
                CollapseTickets = false;
                ShowLatestFiles = true;
            }
        }
        private bool canGetFilesFromCameraReadyFolder()
        {
            return true;
        }
        private void getFilesFromCameraReadyFolder()
        {
            // clear out existing files
            Files?.Clear();
            FilesConvertedToPdf?.Clear();
            FilesFoundInCameraReady?.Clear();
            CameraReadyFileSelectorIsVisible = true;
            ModalOverlayIsVisible = true;

            if (null == FilesFoundInCameraReady)
            {
                FilesFoundInCameraReady = new ObservableCollection<CockleFileInCameraReady>();
            }

            string CAMERA_READY_FOLDER = System.IO.Directory.Exists(@"L:\Camera Ready")
                ? @"L:\Camera Ready" : @"\\CLBDC02\Printing\Camera Ready";

            var filesFoundInCameraReady = System.IO.Directory.EnumerateFiles(CAMERA_READY_FOLDER);

            filesFoundInCameraReady.ToList().ForEach(x =>
            {
                FilesFoundInCameraReady.Add(new CockleFileInCameraReady(x));
            });
        }
        #endregion
    }
}

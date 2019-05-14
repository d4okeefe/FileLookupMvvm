using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileSearchMvvm.ViewModels.SearchViewModelFolder
{
    public partial class SearchViewModel
    {
        private async void imposeCircuitCourtCover_8pt5x23()
        {
            ModalOverlayIsVisible = false;
            SpecialImpositionsOverlayIsVisible = false;
            UpdateLabelPdf = "";
            try
            {
                // get ticket, atty:
                var ticket_atty = FilesConvertedToPdf.Where(x => !string.IsNullOrEmpty(x.TicketPlusAttorney)).Select(x => x.TicketPlusAttorney).FirstOrDefault();

                Models.Imposition.NUpCircuitCourtCoverOn8pt5x23 nupfile = null;
                await System.Threading.Tasks.Task.Run(() =>
                {
                    nupfile = new Models.Imposition.NUpCircuitCourtCoverOn8pt5x23(SelectedPdfFile);
                });
                if(null != nupfile && System.IO.File.Exists(nupfile.NewFileCreated.FullName))
                {
                    if(!FilesConvertedToPdf.Any(x => x.FullName == nupfile.NewFileCreated.FullName))
                    {
                        FilesConvertedToPdf.Add(nupfile.NewFileCreated);
                        UpdateLabelPdf = "Created: " + System.IO.Path.GetFileNameWithoutExtension(nupfile.NewFileCreated.FullName);
                    }
                }
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        private async void imposeCircuitCourtCover_11x19()
        {
            ModalOverlayIsVisible = false;
            SpecialImpositionsOverlayIsVisible = false;
            UpdateLabelPdf = "";
            try
            {
                // get ticket, atty:
                var ticket_atty = FilesConvertedToPdf.Where(x => !string.IsNullOrEmpty(x.TicketPlusAttorney)).Select(x => x.TicketPlusAttorney).FirstOrDefault();


                Models.Imposition.NUpCircuitCourtCoverOn11x19 nupfile = null;
                await System.Threading.Tasks.Task.Run(() =>
                {
                    nupfile = new Models.Imposition.NUpCircuitCourtCoverOn11x19(SelectedPdfFile);
                });
                if(null != nupfile && System.IO.File.Exists(nupfile.NewFileCreated.FullName))
                {
                    if(!FilesConvertedToPdf.Any(x => x.FullName == nupfile.NewFileCreated.FullName))
                    {
                        FilesConvertedToPdf.Add(nupfile.NewFileCreated);
                        UpdateLabelPdf = "Created: " + System.IO.Path.GetFileNameWithoutExtension(nupfile.NewFileCreated.FullName);
                    }
                }
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        private void refreshLoadedPdfFolder()
        {
            try
            {
                // Command:RunRefreshLoadedPdfFolder
                if(!System.IO.Directory.Exists(DestinationFolderConvertedFiles))
                {
                    FilesConvertedToPdf.Clear();
                    DestinationFolderConvertedFiles = string.Empty;
                    return;
                }
                else
                {
                    var files = System.IO.Directory.EnumerateFiles(DestinationFolderConvertedFiles).ToList();

                    var files_to_add = files.Where(x => !FilesConvertedToPdf.Any(y => x == y.FullName));
                    if(files_to_add.Count() > 0)
                    {
                        files_to_add.ToList().ForEach(x =>
                        {
                            FilesConvertedToPdf.Add(new CockleFilePdf(x, Models.Utilities.SourceFileTypeEnum.Unrecognized));
                        });
                    }

                    var files_to_remove = FilesConvertedToPdf.Where(x => !System.IO.File.Exists(x.FullName));
                    if(files_to_remove?.Count() > 0)
                    {
                        foreach(var x in files_to_remove) { FilesConvertedToPdf.Remove(x); }
                    }

                    var files_dup = FilesConvertedToPdf.GroupBy(x => x.FullName)
                        .Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                    foreach(var y in files_dup)
                    {
                        var options_to_remove = FilesConvertedToPdf.Where(x => x.FullName == y);
                        var unrec_option = options_to_remove.Where(x => x.FileType == SourceFileTypeEnum.Unrecognized || x.FileType == SourceFileTypeEnum.UnrecognizedCentered);
                        if(unrec_option.Count() == 1)
                        {
                            FilesConvertedToPdf.Remove(unrec_option.FirstOrDefault());
                        }
                        else
                        {
                            var other_option = FilesConvertedToPdf.Where(x => x.FullName == y);
                            FilesConvertedToPdf.Remove(other_option.FirstOrDefault());
                        }
                    }

                    foreach(var x in FilesConvertedToPdf)
                    {
                        foreach(var y in files_dup)
                        {
                            if(x.FullName == y)
                            {
                                if(x.FileType == SourceFileTypeEnum.Unrecognized) { }

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
        private bool canRefreshLoadedPdfFolder()
        {
            // Command:RunRefreshLoadedPdfFolder
            return !string.IsNullOrEmpty(DestinationFolderConvertedFiles);
        }


        private void watcherPdfFolderChanged(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                if(!string.IsNullOrEmpty(DestinationFolderConvertedFiles)
                    && System.IO.Directory.Exists(DestinationFolderConvertedFiles)
                    && null != FilesConvertedToPdf)
                {
                    if(e.ChangeType == System.IO.WatcherChangeTypes.Deleted)
                    {
                        var file_to_remove = FilesConvertedToPdf.Where(x => x.FullName == e.FullPath).FirstOrDefault();
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate
                        {
                            FilesConvertedToPdf.Remove(file_to_remove);
                        });
                    }
                    else if(e.ChangeType == System.IO.WatcherChangeTypes.Created)
                    {
                        var temp_count = FilesConvertedToPdf.Count();
                        if(!FilesConvertedToPdf.Any(x => x.FullName == e.FullPath))
                        {
                            var file_to_add = new CockleFilePdf(e.FullPath, SourceFileTypeEnum.Unrecognized);

                            System.Windows.Application.Current.Dispatcher.Invoke(delegate
                            {
                                if(temp_count == FilesConvertedToPdf.Count())
                                {
                                    FilesConvertedToPdf.Add(file_to_add);
                                }
                            });
                        }
                    }
                    else if(e.ChangeType == System.IO.WatcherChangeTypes.Renamed)
                    {
                        var file_to_remove = FilesConvertedToPdf.Where(x => x.FullName == e.FullPath).FirstOrDefault();
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate
                        {
                            FilesConvertedToPdf.Remove(file_to_remove);
                        });
                        var file_to_add = new CockleFilePdf(e.FullPath, SourceFileTypeEnum.Unrecognized);
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate
                        {
                            FilesConvertedToPdf.Add(file_to_add);
                        });
                    }
                }
                else
                {
                    //Files?.Clear();
                    //FilesConvertedToPdf?.Clear();
                    //FilesConvertedToPdf_Ordered?.Clear();
                    //DestinationFolderConvertedFiles = string.Empty;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void addItemToRecentSearchTerms()
        {
            if(!string.IsNullOrEmpty(SearchText)
                && UserSearchTerms.Contains(SearchText))
            {
                if(UserSearchTerms.Count() != UserSearchTerms.Distinct().Count())
                {
                    // problem
                }
                else
                {
                    // can only contain one search term
                    var txt = SearchText;
                    UserSearchTerms.Remove(UserSearchTerms.Where(t => txt.Equals(t)).Single());
                    SearchText = txt;
                    UserSearchTerms.Insert(0, txt);
                }
            }
            else
            {
                UserSearchTerms.Insert(0, SearchText);
                UserSearchTerms.RemoveAt(6);
            }

            var csvSearchTerms = string.Empty;
            foreach(var itm in UserSearchTerms)
            {
                if(string.IsNullOrEmpty(csvSearchTerms))
                {
                    csvSearchTerms = itm;
                }
                else
                {
                    csvSearchTerms = csvSearchTerms + "," + itm;
                }

            }
            Properties.Settings.Default.RecentSearchTerms = csvSearchTerms;
            Properties.Settings.Default.Save();
        }
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
            foreach(var f in ordered_files)
            {
                if(f.FileType == SourceFileTypeEnum.App_Foldout ||
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

        #region Pdfview Modal Overlays
        private void closeModalContentOverlay()
        {
            CameraReadyFileSelectorIsVisible = false;
            CoverLengthSelectorIsVisible = false;
            ScratchFolderSelectorIsVisible = false;
            SetFileOrderModalIsVisible = false;
            CoverLengthSelectorIsVisible = false;
            SelectImpositionDetailsIsVisible = false;
            SpecialImpositionsOverlayIsVisible = false;
            ModalOverlayIsVisible = false;
        }
        private bool canShowAllCameraReadyFilesAndClearSearchText()
        {
            return true;
        }
        private void showAllCameraReadyFilesAndClearSearchText()
        {
            getFilesFromCameraReadyFolder();
            CameraReadySearchText = string.Empty;
        }
        private void showOnlyCameraReadySearchTextTickets(object o)
        {
            // limit camera ready files shown in list

            var limited_group = FilesFoundInCameraReady.Where(x => x.FileName.Contains(CameraReadySearchText)).ToList();
            FilesFoundInCameraReady.Clear();
            limited_group.ToList().ForEach(
                x => FilesFoundInCameraReady.Add(new CockleFileInCameraReady(x.FileName)));
        }
        #endregion

        #region Clear Datagrid & Collection Objects
        private bool canClearPdfFiles()
        {
            return FilesConvertedToPdf != null && FilesConvertedToPdf.Count > 0;
        }
        private void clearPdfFiles()
        {
            try
            {
                if(null == FilesConvertedToPdf) FilesConvertedToPdf = new ObservableCollection<CockleFilePdf>();
                else FilesConvertedToPdf.Clear();

                if(null == FilesConvertedToPdf_Ordered) FilesConvertedToPdf_Ordered = new ObservableCollection<CockleFilePdf>();
                else FilesConvertedToPdf_Ordered.Clear();

                if(null == Files) Files = new ObservableCollection<CockleFile>();
                else Files.Clear();

                DestinationFolderConvertedFiles = string.Empty;
                UpdateLabelPdf = "";
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        #endregion

        #region Open File/Folder Events
        private bool canOpenSourceFolder()
        {
            return SelectedFile != null && !IsExecutingSearch;
        }
        private void openSourceFolder()
        {
            bool ueser_cancelled = false;
            try
            {
                var foldersToOpen = new HashSet<string>();
                SelectedFiles.ForEach(f => foldersToOpen.Add(f.SourceFolderLocal));

                if(foldersToOpen.Count >= 2)
                {
                    var result = System.Windows.MessageBox.Show(
                        $"Do you want to open {foldersToOpen.Count} folders?",
                        "Confirmation",
                        System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question);
                    if(result == System.Windows.MessageBoxResult.No)
                    {
                        ueser_cancelled = true;
                        throw new Exception();
                    }
                }

                foldersToOpen.ToList().ForEach(f => System.Diagnostics.Process.Start(f));
            }
            catch(Exception ex)
            {
                if(ueser_cancelled) UpdateLabel = "";
                else UpdateLabel = ex.Message;
            }
        }
        private bool canOpenSelectedFile()
        {
            return SelectedFile != null && !IsExecutingSearch;
        }
        private void openSelectedFiles()
        {
            if(null != SelectedFile || SelectedFiles.Count > 0)
            {
                try
                {
                    if(SelectedFiles.Count >= 2)
                    {
                        var result = System.Windows.MessageBox.Show(
                            $"Do you want to open {SelectedFiles.Count} files?",
                            "Confirmation",
                            System.Windows.MessageBoxButton.YesNo,
                            System.Windows.MessageBoxImage.Question);
                        if(result == System.Windows.MessageBoxResult.No)
                        {
                            throw new Exception("User cancelled");
                        }
                    }

                    // IF MULTIPLE: INSTEAD OF PROCESS.START(), USE INTEROP ???
                    foreach(var file in SelectedFiles)
                    {
                        // check stream to make sure file is not already open
                        System.IO.FileStream stream = null;
                        bool isOpen = false;
                        try
                        {
                            stream = System.IO.File.Open(
                                file.FullName,
                                System.IO.FileMode.Open,
                                System.IO.FileAccess.ReadWrite,
                                System.IO.FileShare.None);
                        }
                        catch(System.IO.IOException ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);
                            isOpen = true;
                        }
                        finally
                        {
                            if(stream != null)
                            {
                                stream.Close();
                            }
                        }
                        if(!isOpen)
                        {
                            var start_info = new System.Diagnostics.ProcessStartInfo
                            {
                                WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized,
                                FileName = file.FullName
                            };
                            System.Diagnostics.Process.Start(start_info);
                        }
                    }
                }
                catch(Exception ex)
                {
                    UpdateLabel = ex.Message;
                }
            }
        }
        private bool canOpenSelectedPdfFiles()
        {
            return SelectedPdfFile == null ? false : true;
        }
        private void openSelectedPdfFiles()
        {
            try
            {
                System.Diagnostics.Process.Start(SelectedPdfFile.FullName);
            }
            catch
            {
                UpdateLabel = "An error occurred trying to opening files";
            }
        }
        private bool canOpenPdfSourceFolder()
        {
            return System.IO.Directory.Exists(DestinationFolderConvertedFiles);
        }
        private void openPdfSourceFolder()
        {
            System.Diagnostics.Process.Start(DestinationFolderConvertedFiles);
        }
        #endregion

        #region GongSolutions Drag Events
        public void DragOver(GongSolutions.Wpf.DragDrop.IDropInfo dropInfo) { }
        public void Drop(GongSolutions.Wpf.DragDrop.IDropInfo dropInfo) { }
        #endregion

        #region Datagrid Selection Changed Events
        /// <summary>
        /// Selection Changed Event Command:
        /// Viemmodel interaction of search datagrid ui
        /// </summary>
        /// <param name="param"></param>
        private void searchGridSelectionChanged(object param)
        {
            UpdateLabel = string.Empty;
            try
            {
                // catch normal changes to user selection
                var selectedRows = param as IList;
                if(null == selectedRows) throw new Exception();
                if(ShowLatestFiles)
                {
                    SelectedFiles = selectedRows
                        .Cast<CockleFile>()
                        .Where(x => x.IsLatestFile)
                        .Select(x => x as CockleFile)
                        .ToList();
                }
                else
                {
                    SelectedFiles = selectedRows
                        .Cast<CockleFile>()
                        .Select(x => x as CockleFile)
                        .ToList();
                }
            }
            catch(Exception ex)
            {
                UpdateLabel = ex.Message;
            }
        }
        private void pdfSelectionChanged_Ordered(object param)
        {
            try
            {
                // catch normal changes to user selection
                var selectedRows = param as IList;
                if(null == selectedRows) throw new Exception();


                SelectedPdfFiles_Ordered = selectedRows
                        .Cast<CockleFilePdf>()
                        .Select(x => x as CockleFilePdf)
                        .ToList();
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        private void pdfGridSelectionChanged(object param)
        {
            try
            {
                // catch normal changes to user selection
                var selectedRows = param as IList;
                if(null == selectedRows) throw new Exception();


                SelectedPdfFiles = selectedRows
                        .Cast<CockleFilePdf>()
                        .Select(x => x as CockleFilePdf)
                        .ToList();
            }
            catch(Exception ex)
            {
                UpdateLabelPdf = ex.Message;
            }
        }
        #endregion

    }
}

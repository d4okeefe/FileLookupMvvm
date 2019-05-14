using System;
using FileSearchMvvm.Commands;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using FileSearchMvvm.Models.CockleTypes;
using System.Collections.Generic;

namespace FileSearchMvvm.ViewModels
{
    public class PdfViewModel: ViewModelBase
    {
        #region FIELDS
        private string folderFromScratch;
        private string cameraReadyTicketNumber;
        private string sourceTextForFilesInGrid;
        //private List<ICockleFile> FilesInGrid;
        private List<CockleFile> filesConvertedFromScratch;
        #endregion
        #region PROPERTIES
        public List<CockleFile> FilesConvertedFromScratch
        {
            get { return filesConvertedFromScratch; }
            set
            {
                filesConvertedFromScratch = value;
                RaisePropertyChanged();
            }
        }
        public string FolderFromScratch
        {
            get { return folderFromScratch; }
            set
            {
                folderFromScratch = value;
                RaisePropertyChanged();
            }
        }
        public string CameraReadyTicketNumber
        {
            get { return cameraReadyTicketNumber; }
            set
            {
                cameraReadyTicketNumber = value;
                RaisePropertyChanged();
            }
        }
        public string SourceTextForFilesInGrid
        {
            get
            {
                return sourceTextForFilesInGrid;
            }
            set
            {
                // CONDITIONS
                // if datagrid is empty: mark this
                // if datagrid has files, let user know the source
                sourceTextForFilesInGrid = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #region CONSTRUCTOR
        public PdfViewModel()
        {
            RunGetFilesFromScratchFolder = new RelayCommand(o => getFilesFromScratchFolder(), o => canGetFilesFromScratchFolder());
        }

        public PdfViewModel(List<CockleFile> filesConvertedFromScratch)
        {
            FilesConvertedFromScratch = filesConvertedFromScratch;
        }
        #endregion
        #region ICOMMAND PROPERTIES
        public ICommand RunGetFilesFromScratchFolder { get; private set; }
        #endregion
        #region PRIVATE METHODS
        private bool canGetFilesFromScratchFolder()
        {
            return true;
        }

        private void getFilesFromScratchFolder()
        {
            try
            {
                var dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                dialog.InitialDirectory = @"c:\scratch";
                var result = dialog.ShowDialog();
                var folderName = dialog.FileName;
                FolderFromScratch = folderName;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        #endregion
        #region PUBLIC METHODS
        #endregion
    }
}
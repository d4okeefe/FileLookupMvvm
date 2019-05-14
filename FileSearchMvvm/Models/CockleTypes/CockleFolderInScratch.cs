using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearchMvvm.Models.CockleTypes
{
    public class CockleFolderInScratch
    {
        public CockleFolderInScratch(string folderName)
        {
            FolderName = folderName;
        }
        public string FolderName { get; set; }
        public string ShortFolderName
        {
            get { return System.IO.Path.GetFileName(FolderName); }
        }
        public int FilesInFolder
        {
            get { return System.IO.Directory.EnumerateFiles(FolderName).Count(); }
        }

    }
}

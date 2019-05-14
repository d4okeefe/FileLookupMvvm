using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;

namespace FileSearchMvvm.ViewModels.Utilities
{
    class FilesPrintedToPrnSuccessfully
    {
        public CockleFile CockleFile { get; set; }
        public string PrnFilename { get; set; }
        public SourceFileTypeEnum Filetype { get; set; }
    }
}

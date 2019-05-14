using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;

namespace FileSearchMvvm.ViewModels.Utilities
{
    class FilePrintedSuccessfully
    {
        public CockleFile CockleFile { get; set; }
        public string TempWordFile { get; set; }
        public string PdfFilename { get; set; }
        public int? LengthOfCover { get; set; }
        public SourceFileTypeEnum Filetype { get; set; }
    }
}

using FileSearchMvvm.Models.Utilities;

namespace FileSearchMvvm.Models.CockleTypes
{
    public interface ICockleFile
    {
        #region Properties
        string FullName { get; set; }
        string Filename { get; set; }
        FileLocationEnum Location { get; }
        int? TicketNumber { get; set; }
        string Attorney { get; set; }
        SourceFileTypeEnum FileType { get; set; }
        string FileTypeString { get; }
        string TicketPlusAttorney { get; }
        int? Version { get; set; }
        #endregion
    }
}

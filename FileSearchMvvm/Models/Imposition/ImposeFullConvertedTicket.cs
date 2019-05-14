using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FileSearchMvvm.Models.Imposition
{
    class ImposeFullConvertedTicket
    {
        #region Properties
        public string FolderPath { get; private set; }
        public string Attorney { get; private set; }
        public int? Ticket { get; private set; }
        public bool SaddleStitch { get; private set; }
        public TypeOfBindEnum BindType { get; set; }
        public int LengthOfCover { get; private set; }
        public int TotalPageCount { get; set; }
        public List<CockleFilePdf> ConvertedFiles { get; private set; }
        public List<CockleFilePdf> ImposedFilesCreated { get; private set; }
        #endregion

        #region Constructor
        public ImposeFullConvertedTicket(
            string folder, 
            List<CockleFilePdf> 
            converted_files, 
            int lenOfCover, 
            TypeOfBindEnum bind_type)
        {
            // interrupt if no folder, or inconsistent file information
            if (string.IsNullOrEmpty(folder) || converted_files.Count == 0) { return; }

            // set properties
            Attorney = converted_files[0].Attorney;
            Ticket = converted_files[0].TicketNumber;

            // make sure same Attorney & Ticket for all files
            if (!converted_files.TrueForAll(f => f.Attorney.Equals(Attorney) && f.TicketNumber == Ticket)) { return; }

            // initialize properties
            FolderPath = folder;
            LengthOfCover = lenOfCover;
            ConvertedFiles = converted_files;
            ImposedFilesCreated = new List<CockleFilePdf>();

            // get page ranges for app files
            ConvertedFiles.ForEach(f =>
            {
                if (f.FileType == SourceFileTypeEnum.App_File && f.PageRange == null)
                {
                    f.GetPageRangeForFile();
                }
            });

            // get non foldout files
            var nonFoldouts = ConvertedFiles
                .Where(f =>
                f.FileType != SourceFileTypeEnum.App_Foldout ||
                f.FileType != SourceFileTypeEnum.App_ZFold ||
                f.FileType != SourceFileTypeEnum.Brief_Foldout ||
                f.FileType != SourceFileTypeEnum.Brief_ZFold ||
                f.FileType != SourceFileTypeEnum.Certificate_of_Service ||
                f.FileType != SourceFileTypeEnum.SidewaysPage ||
                f.FileType != SourceFileTypeEnum.Unrecognized)
                .OrderBy(f => f.FileType);

            // get foldout files
            var foldouts = ConvertedFiles.Where(f =>
                f.FileType == SourceFileTypeEnum.App_Foldout ||
                f.FileType == SourceFileTypeEnum.App_ZFold ||
                f.FileType == SourceFileTypeEnum.Brief_Foldout ||
                f.FileType == SourceFileTypeEnum.Brief_ZFold);

            // get page numbers for foldout pages
            List<int> foldout_page_nums = new List<int>();
            if (foldouts.Count() > 0)
            {
                foldouts.ToList().ForEach(f =>
                {
                    f.GetPageRangeForFile();
                });
            }

            // get total page count
            int page_count = 0;
            ConvertedFiles.ForEach(f =>
            {
                using (iTextSharp.text.pdf.PdfReader r = new iTextSharp.text.pdf.PdfReader(f.FullName))
                { page_count += r.NumberOfPages; }
            });
            TotalPageCount = page_count;

            // set bind type
            switch (bind_type)
            {
                case TypeOfBindEnum.ProgramDecidesByPageCount:
                    if (foldouts.Count() == 0 && page_count <= 65)
                    { SaddleStitch = true; BindType = TypeOfBindEnum.SaddleStitch; }
                    else
                    { SaddleStitch = false; BindType = TypeOfBindEnum.PerfectBind; }
                    break;
                case TypeOfBindEnum.SaddleStitch:
                    SaddleStitch = true; BindType = TypeOfBindEnum.SaddleStitch;
                    break;
                case TypeOfBindEnum.PerfectBind:
                    SaddleStitch = false; BindType = TypeOfBindEnum.PerfectBind;
                    break;
            }

            // impose files
            // saddle stitch: enforce that document has a cover: without cover, could get unreliable results
            if (BindType == TypeOfBindEnum.SaddleStitch && (nonFoldouts?.ToList()).Any(x=>x.FileType == SourceFileTypeEnum.Cover))
            {
                var impose = new ImposeSaddleStitch(
                    FolderPath, nonFoldouts, LengthOfCover, BindType, Ticket, Attorney);

                // keep track of files created
                foreach (var f in impose.NewFilesCreated) { ImposedFilesCreated.Add(f); }
            }
            else if (BindType == TypeOfBindEnum.PerfectBind)
            {
                var impose = new ImposePerfectBind(
                    FolderPath,
                    nonFoldouts, foldouts,
                    LengthOfCover, BindType, Ticket, Attorney);

                // keep track of files created
                foreach (var f in impose.NewFilesCreated) { ImposedFilesCreated.Add(f); }
            }
        }
        #endregion
    }
}

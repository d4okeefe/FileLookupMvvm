using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearchMvvm.Models.Imposition
{
    class ImposePerfectBind
    {
        #region properties
        public string SourceFolder { get; set; }
        public List<CockleFilePdf> ImposedFiles { get; private set; }
        public TypeOfBindEnum TypeOfBind { get; private set; }
        public int? CoverLength { get; private set; }
        public int? PageCountOfBody { get; private set; }

        public System.IO.DirectoryInfo ProcessedFolder { get; private set; }
        public Dictionary<string, string> NewFileNames { get; private set; }
        public List<CockleFilePdf> NewFilesCreated { get; private set; }
        #endregion

        #region constructor (which carries out the imposition)
        public ImposePerfectBind(
            string src, IOrderedEnumerable<CockleFilePdf> list, IEnumerable<CockleFilePdf> listFoldouts,
            int cvLen, TypeOfBindEnum bind, int? ticket, string atty)
        {
            this.SourceFolder = src;
            this.ImposedFiles = new List<CockleFilePdf>(list);
            this.TypeOfBind = bind;
            this.CoverLength = cvLen;
            this.PageCountOfBody = 0;
            this.NewFilesCreated = new List<CockleFilePdf>();

            // create folder for processed files
            this.ProcessedFolder = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(this.SourceFolder, "processed"));

            // create new file names
            NewFileNames = new Dictionary<string, string>
            {
                {"Cover", System.IO.Path.Combine(ProcessedFolder.FullName, "Cover.pdf")},
                {"Brief", System.IO.Path.Combine(ProcessedFolder.FullName, "Brief.pdf")},
                {"FinalCover", System.IO.Path.Combine(SourceFolder, string.Format("{0} {1} cv B4 PB pr.pdf", ticket, atty))},
                {"FinalBrief", System.IO.Path.Combine(SourceFolder, string.Format("{0} {1} br app B5 pr.pdf", ticket, atty))}
            };

            bool hasCover = this.ImposedFiles.Any(f => f.FileType == SourceFileTypeEnum.Cover);
            if (hasCover) { ImposeCover(this.ImposedFiles, this.NewFileNames["Cover"]); }

            bool hasOnlyCover = true;
            this.ImposedFiles.ForEach(f => { if (f.FileType != SourceFileTypeEnum.Cover) { hasOnlyCover = false; } });
            if (!hasOnlyCover) { ImposeBrief(this.ImposedFiles, this.NewFileNames["Brief"], this.TypeOfBind); }

            // move files to main folder
            try
            {
                if (hasCover)
                {
                    if (System.IO.File.Exists(this.NewFileNames["FinalCover"])) { System.IO.File.Delete(this.NewFileNames["FinalCover"]); }
                    System.IO.File.Copy(this.NewFileNames["Cover"], this.NewFileNames["FinalCover"], true);
                    this.NewFilesCreated.Add(new CockleFilePdf(
                        this.NewFileNames["FinalCover"], atty, ticket, SourceFileTypeEnum.Imposed_Cover, ""));
                }
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }
            try
            {
                if (!hasOnlyCover)
                {
                    if (System.IO.File.Exists(this.NewFileNames["FinalBrief"])) { System.IO.File.Delete(this.NewFileNames["FinalBrief"]); }
                    System.IO.File.Copy(this.NewFileNames["Brief"], this.NewFileNames["FinalBrief"], true);
                    this.NewFilesCreated.Add(new CockleFilePdf(
                        this.NewFileNames["FinalBrief"], atty, ticket, SourceFileTypeEnum.Imposed_Brief, ""));
                }
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }

            // attempt to delete processed folder
            try { ProcessedFolder.Delete(true); }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }
        }
        #endregion
        private void ImposeBrief(List<CockleFilePdf> imposedFiles, string imposedBody, TypeOfBindEnum bind)
        {
            // remove cover, combine body, and add blank pages where needed
            if (!PdfUtilities.CombineBriefPages_AddingBlanks(imposedFiles, imposedBody, bind))
            {
                // problem if returns false
            }

            // crop pages
            if (!PdfUtilities.CropCockleBriefPages(imposedBody))
            {
                // problem if returns false
            }
            // layout on B5 pages
            if (!PdfUtilities.PerfectBind_LayoutCroppedBrief(imposedBody))
            {
                // problem if returns false
            }
        }
        private void ImposeCover(List<CockleFilePdf> imposedFiles, string imposedCover)
        {
            var iterCoverFiles = imposedFiles
                .Where(f => f.FileType == SourceFileTypeEnum.Cover
                    || f.FileType == SourceFileTypeEnum.InsideCv);
            List<CockleFilePdf> coverFiles = new List<CockleFilePdf>(iterCoverFiles);

            if (!PdfUtilities.CombineCoverAndInsideCover(coverFiles, imposedCover))
            { }

            if (!PdfUtilities.CropCoverAndInsideCover(imposedCover, this.CoverLength))
            { }

            if (!PdfUtilities.PerfectBind_LayoutCroppedCoverAndInsideCover(imposedCover, this.CoverLength))
            { }
        }
    }
}

using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Acrobat;
using FileSearchMvvm;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FileSearchMvvm.Models.Imposition
{
    class ImposeSaddleStitch
    {
        #region Properties
        public string SourceFolder { get; set; }
        public List<CockleFilePdf> ImposedFiles { get; private set; }
        public TypeOfBindEnum TypeOfBind { get; private set; }
        public int? CoverLength { get; private set; }
        public int? PageCountOfBody { get; private set; }
        public System.IO.DirectoryInfo ProcessedFolder { get; private set; }
        public Dictionary<string, string> NewFileNames { get; private set; }
        public List<CockleFilePdf> NewFilesCreated { get; private set; }
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor invoked on auto creation of files.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="list"></param>
        /// <param name="cvLen"></param>
        /// <param name="bind"></param>
        /// <param name="ticket"></param>
        /// <param name="atty"></param>
        public ImposeSaddleStitch(
            string src, 
            IOrderedEnumerable<CockleFilePdf> list,
            int cvLen, 
            TypeOfBindEnum bind, 
            int? ticket, 
            string atty)
        {
            SourceFolder = src;
            ImposedFiles = new List<CockleFilePdf>(list);
            TypeOfBind = bind;
            CoverLength = cvLen;
            PageCountOfBody = 0;
            NewFilesCreated = new List<CockleFilePdf>();

            // get total page count
            ImposedFiles.ForEach(f =>
            {
                using (PdfReader r = new PdfReader(f.FullName))
                {
                    PageCountOfBody += r.NumberOfPages;
                }
            });
            if (PageCountOfBody == 0) PageCountOfBody = null;

            // create folder for processed files
            ProcessedFolder = System.IO.Directory.CreateDirectory(System.IO.Path.Combine(SourceFolder, "processed"));

            // create new file names
            NewFileNames = new Dictionary<string, string>
            {
                {"Cover", System.IO.Path.Combine(ProcessedFolder.FullName, "Cover.pdf")},
                {"Brief", System.IO.Path.Combine(ProcessedFolder.FullName, "Brief.pdf")},
                {"Combined", System.IO.Path.Combine(ProcessedFolder.FullName, "Combined.pdf")},
                {"FinalVersion", System.IO.Path.Combine(SourceFolder, string.Format(
                    "{0} {1} cv{2}{3}br{4}{5}B4 pr.pdf", ticket, atty,
                    ImposedFiles.Any(x=>x.FileType == SourceFileTypeEnum.InsideCv) ? " icv " : " ",
                    ImposedFiles.Any(x=>x.FileType == SourceFileTypeEnum.Motion) ? " brmo " : " ",
                    ImposedFiles.Any(x=>x.FileType == SourceFileTypeEnum.App_Index) ? " ain " : " ",
                    ImposedFiles.Any(x=>x.FileType == SourceFileTypeEnum.App_File) ? " app " : " ")
                    .Replace("  ", " "))},
                {"FinalVersionWithOnlyCover", System.IO.Path.Combine(SourceFolder, string.Format("{0} {1} cv B4 pr.pdf", ticket, atty))}
            };

            bool hasCover = ImposedFiles.Any(f => f.FileType == SourceFileTypeEnum.Cover);
            if (hasCover) { ImposeCover(ImposedFiles, NewFileNames["Cover"]); }

            bool hasOnlyCover = true;
            ImposedFiles.ForEach(f => { if (f.FileType != SourceFileTypeEnum.Cover) { hasOnlyCover = false; } });
            if (!hasOnlyCover) { ImposeBrief(ImposedFiles, NewFileNames["Brief"], TypeOfBind); }

            // COMBINE COVER WITH BRIEF
            try
            {
                if (hasCover && !hasOnlyCover)
                {
                    if (!combinedImposedCoverAndBrief(NewFileNames["Brief"], NewFileNames["Cover"], NewFileNames["Combined"]))
                    {
                        System.Diagnostics.Debug.WriteLine("Failure here!");
                    }
                }
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }

            // copy file to main folder
            try
            {
                if (System.IO.File.Exists(NewFileNames["FinalVersion"])) { System.IO.File.Delete(NewFileNames["FinalVersion"]); }
                //File.Copy(NewFileNames["Combined"], NewFileNames["FinalVersion"], true);

                // open, save, close with acrobat
                AcroApp _myAdobe = new Acrobat.AcroApp();
                AcroAVDoc _acroDoc = new AcroAVDoc();

                CAcroPDDoc _pdDoc = null;
                if (hasOnlyCover)
                {
                    _acroDoc.Open(NewFileNames["Cover"], null);
                    _pdDoc = (Acrobat.AcroPDDoc)(_acroDoc.GetPDDoc());
                    _pdDoc.Save(1, NewFileNames["FinalVersionWithOnlyCover"]);

                    // keep track of files created
                    NewFilesCreated.Add(new CockleFilePdf(
                        NewFileNames["FinalVersionWithOnlyCover"], atty, ticket, SourceFileTypeEnum.Imposed_Cover, ""));
                }
                else
                {
                    _acroDoc.Open(NewFileNames["Combined"], null);
                    _pdDoc = (Acrobat.AcroPDDoc)(_acroDoc.GetPDDoc());
                    _pdDoc.Save(1, NewFileNames["FinalVersion"]);

                    // keep track of files created
                    NewFilesCreated.Add(new CockleFilePdf(
                        NewFileNames["FinalVersion"], atty, ticket, SourceFileTypeEnum.Imposed_Cover_and_Brief, ""));
                }
                _pdDoc.Close();
                _acroDoc.Close(0);
                _myAdobe.Exit();
            }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }

            // attempt to delete processed folder
            try { ProcessedFolder.Delete(true); }
            catch (Exception e) { System.Diagnostics.Debug.WriteLine(e); }
        }
        #endregion
        #region Private methods
        #region Process brief
        private bool ImposeBrief(List<CockleFilePdf> imposedFiles, string imposedBody, TypeOfBindEnum bind)
        {
            // remove cover, combine body, and add blank pages where needed
            PdfUtilities.CombineBriefPages_AddingBlanks(imposedFiles, imposedBody, bind);

            if (// reorder saddlestitch pages
                !PdfUtilities.SaddleStitch_ReorderPagesForLayout(imposedBody) ||
                // crop pages
                !PdfUtilities.CropCockleBriefPages(imposedBody) ||
                // layout on B4 pages
                !PdfUtilities.SaddleStitch_LayoutCroppedBrief(imposedBody))
            { return false; }
            else
            { return true; }
        }
        #endregion

        #region Process cover
        private bool ImposeCover(List<CockleFilePdf> imposedFiles, string imposedCover)
        {
            var iterCoverFiles = imposedFiles
                .Where(f => f.FileType == SourceFileTypeEnum.Cover
                    || f.FileType == SourceFileTypeEnum.InsideCv);
            List<CockleFilePdf> coverFiles = new List<CockleFilePdf>(iterCoverFiles);

            if (
                !PdfUtilities.CombineCoverAndInsideCover(coverFiles, imposedCover) ||
                !PdfUtilities.CropCoverAndInsideCover(imposedCover, CoverLength) ||
                !PdfUtilities.SaddleStitch_LayoutCroppedCoverAndInsideCover(imposedCover, PageCountOfBody, CoverLength))
            { return false; }
            else
            { return true; }
        }
        #endregion

        #region Combine cover & brief
        private bool combinedImposedCoverAndBrief(string srcBrief, string srcCover, string dest)
        {
            try
            {
                using (var stream = new System.IO.FileStream(dest, System.IO.FileMode.Create))
                {
                    Document pdfdoc = new Document(new iTextSharp.text.Rectangle(1031.76f, 728.64f));

                    PdfCopy pdfcopy = new PdfCopy(pdfdoc, stream);

                    pdfdoc.Open();
                    string[] files = { srcCover, srcBrief };

                    // merge pdfs in folder
                    string f;
                    for (int i = 0; i < files.Length; i++)
                    {
                        f = files[i];
                        // read file
                        PdfReader reader = new PdfReader(f);
                        // add blank page if needed
                        if (i == 0 && reader.NumberOfPages == 1) // if NumPages == 2, then brief has inside cover
                        {
                            PdfStamper stamper = new PdfStamper(reader, stream);
                            //stamper.Writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_4);
                            stamper.InsertPage(reader.NumberOfPages + 1, reader.GetPageSizeWithRotation(1));
                        }
                        pdfcopy.AddDocument(reader);
                        reader.Close();
                    }
                    pdfcopy.Close();
                    pdfdoc.Close();
                }
                return true;
            }
            catch (Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); return false; }
        }
        #endregion
        #endregion
    }
}

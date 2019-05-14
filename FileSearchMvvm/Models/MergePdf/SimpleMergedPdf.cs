using FileSearchMvvm.Models.CockleTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSearchMvvm.Models.MergePdf
{
    public class SimpleMergedPdf
    {
        private List<string> selectedPdfFileNames;
        private List<CockleFilePdf> selectedPdfFileCockleObjects;
        private string pdf_merge_program;
        private string directory;
        public string NewCombinedFile { get; set; }
        public string NewCombinedBrief { get; set; }
        public string NewCombinedAppendix { get; set; }
        private int i;
        const string preset_filename = "combined pdf.pdf";
        private string new_filename;

        public SimpleMergedPdf(
            List<CockleFilePdf> _selectedPdfFiles,
            string _pdf_merge_program,
            bool alreadyOrdered = false,
            string _new_filename = preset_filename)
        {
            pdf_merge_program = _pdf_merge_program;
            selectedPdfFileNames = _selectedPdfFiles.Select(x => x.FullName).ToList();
            selectedPdfFileCockleObjects = _selectedPdfFiles.ToList();
            if (_new_filename == preset_filename) { new_filename = preset_filename; }
            else { new_filename = _new_filename; }

            if (alreadyOrdered)
            {
                NewCombinedFile = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(selectedPdfFileNames.First()),
                    new_filename);
                if (pdf_merge_program == "Acrobat")
                {
                    createCombinedPdf_Acrobat(_selectedPdfFiles, NewCombinedFile);
                }
                return;
            }

            // as of 12/27/2017, not using any of this !!!
            selectedPdfFileCockleObjects = selectedPdfFileCockleObjects.OrderBy(x => x.FileType).ToList();
            var brief_files = selectedPdfFileCockleObjects.Where(x =>
                x.FileType == Utilities.SourceFileTypeEnum.Cover
                || x.FileType == Utilities.SourceFileTypeEnum.InsideCv
                || x.FileType == Utilities.SourceFileTypeEnum.Index
                || x.FileType == Utilities.SourceFileTypeEnum.Brief
                || x.FileType == Utilities.SourceFileTypeEnum.Brief_Foldout)
                .Select(x => x).OrderBy(x => x.FileType).ToList();
            var appendix_files = selectedPdfFileCockleObjects.Where(x =>
                x.FileType == Utilities.SourceFileTypeEnum.App_Index
                || x.FileType == Utilities.SourceFileTypeEnum.App_File
                || x.FileType == Utilities.SourceFileTypeEnum.App_Foldout
                || x.FileType == Utilities.SourceFileTypeEnum.App_ZFold)
                .Select(x => x).OrderBy(x => x.FileType).ToList();

            // determine which files to produce
            directory = System.IO.Path.GetDirectoryName(selectedPdfFileNames.First());

            var ticket_atty = selectedPdfFileCockleObjects.FirstOrDefault().TicketPlusAttorney;
            NewCombinedFile = System.IO.Path.Combine(directory, ticket_atty + " combined pdf.pdf");
            NewCombinedBrief = System.IO.Path.Combine(directory, ticket_atty + " combined br.pdf");
            NewCombinedAppendix = System.IO.Path.Combine(directory, ticket_atty + " combined app.pdf");

            // add number to filename if already exists
            i = 0;
            while (System.IO.File.Exists(NewCombinedFile))
            {
                NewCombinedFile = System.IO.Path.Combine(directory, ticket_atty + " combined pdf" + "_" + i.ToString() + ".pdf");
                i += 1;
            }
            i = 0;
            while (System.IO.File.Exists(NewCombinedBrief))
            {
                NewCombinedBrief = System.IO.Path.Combine(directory, ticket_atty + " combined br" + "_" + i.ToString() + ".pdf");
                i += 1;
            }
            i = 0;
            while (System.IO.File.Exists(NewCombinedAppendix))
            {
                NewCombinedAppendix = System.IO.Path.Combine(directory, ticket_atty + " combined app" + "_" + i.ToString() + ".pdf");
                i += 1;
            }

            // combine files
            if (pdf_merge_program == "Acrobat")
            {
                if (brief_files.Count == 0 && appendix_files.Count > 0)
                {
                    createCombinedPdf_Acrobat(appendix_files, NewCombinedAppendix);
                }
                if (appendix_files.Count == 0 && brief_files.Count > 0)
                {
                    createCombinedPdf_Acrobat(brief_files, NewCombinedBrief);
                }
                if (brief_files.Count > 0 && appendix_files.Count > 0)
                {
                    createCombinedPdf_Acrobat(selectedPdfFileCockleObjects, NewCombinedFile);
                    createCombinedPdf_Acrobat(brief_files, NewCombinedBrief);
                    createCombinedPdf_Acrobat(appendix_files, NewCombinedAppendix);
                }
            }
            else if (pdf_merge_program == "iTextSharp")
            {
                //createCombinedPdf_iTextSharp(selectedPdfFileNames, NewCombinedFile);
                //createCombinedPdf_iTextSharp(selectedPdfFileNames, NewCombinedBrief);
                //createCombinedPdf_iTextSharp(selectedPdfFileNames, NewCombinedAppendix);
            }
            else if (pdf_merge_program == "PdfSharp")
            {
                //createCombinedPdf_PdfSharp(selectedPdfFileNames, NewCombinedFile);
                //createCombinedPdf_iTextSharp(selectedPdfFileNames, NewCombinedBrief);
                //createCombinedPdf_iTextSharp(selectedPdfFileNames, NewCombinedAppendix);
            }
        }

        private void createCombinedPdf_PdfSharp(List<string> selectedPdfFiles, string newFileName)
        {
            var outputPDFDocument = new PdfSharp.Pdf.PdfDocument();
            foreach (var pdfFile in selectedPdfFiles)
            {
                var inputPDFDocument = PdfSharp.Pdf.IO.PdfReader.Open(pdfFile, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                outputPDFDocument.Version = inputPDFDocument.Version;
                foreach (PdfSharp.Pdf.PdfPage page in inputPDFDocument.Pages)
                {
                    outputPDFDocument.AddPage(page);
                }
            }
            outputPDFDocument.Info.Author = "Cockle Legal Briefs";
            outputPDFDocument.Info.Creator = "Cockle Legal Briefs";
            outputPDFDocument.Save(newFileName);
        }
        private void createCombinedPdf_iTextSharp(List<string> InFiles, string OutFile)
        {
            try
            {
                using (var stream = new System.IO.FileStream(OutFile, System.IO.FileMode.Create))
                using (var doc = new iTextSharp.text.Document())
                using (var pdf = new iTextSharp.text.pdf.PdfCopy(doc, stream))
                {
                    doc.Open();

                    iTextSharp.text.pdf.PdfReader reader = null;
                    iTextSharp.text.pdf.PdfImportedPage page = null;

                    //fixed typo
                    InFiles.ForEach(file =>
                    {
                        reader = new iTextSharp.text.pdf.PdfReader(file);

                        for (int i = 0; i < reader.NumberOfPages; i++)
                        {
                            page = pdf.GetImportedPage(reader, i + 1);
                            pdf.AddPage(page);
                        }

                        pdf.FreeReader(reader);
                        reader.Close();
                        System.IO.File.Delete(file);
                    });
                }
            }
            catch (Exception excpt) { System.Diagnostics.Debug.WriteLine(excpt); }
        }
        private void createCombinedPdf_Acrobat(List<CockleFilePdf> selectedPdfFiles, string new_file_name)
        {
            Acrobat.CAcroApp app = new Acrobat.AcroApp();           // Acrobat
            Acrobat.CAcroPDDoc doc = new Acrobat.AcroPDDoc();       // First document
            Acrobat.CAcroPDDoc docToAdd = new Acrobat.AcroPDDoc();  // Next documents

            try
            {
                int numPages = 0, numPagesToAdd = 0;
                foreach (var _f in selectedPdfFiles)
                {
                    var f = _f.FullName;
                    if (_f == selectedPdfFiles.First()) // both 0
                    {
                        doc.Open(f);
                        numPages = doc.GetNumPages();
                    }
                    else
                    {
                        if (!docToAdd.Open(f)) { break; }
                        numPagesToAdd = docToAdd.GetNumPages();
                        if (!doc.InsertPages(numPages - 1, docToAdd, 0, numPagesToAdd, 0)) { break; }
                        if (!docToAdd.Close()) { break; }
                        numPages = doc.GetNumPages();
                    }
                }

                doc.Save(1, new_file_name);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                docToAdd = null;
                app = null;

                GC.Collect();
            }
        }
    }
}

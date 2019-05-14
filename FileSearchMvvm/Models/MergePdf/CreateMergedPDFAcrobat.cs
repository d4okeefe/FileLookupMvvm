using Acrobat;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileSearchMvvm.Models.MergePdf
{
    enum TypeOfCombinedPdf { All, Brief, Appendix };
    class CreateMergedPDFAcrobat
    {
        public string CombinedPdfFilename { get; set; }
        public CockleFilePdf CockleFilePdf_CombinedPdfFilename { get; set; }
        public string CombinedPdfFilenameWithFoldouts { get; set; }
        public CockleFilePdf CockleFilePdf_CombinedPdfFilenameWithFoldouts { get; set; }

        public CreateMergedPDFAcrobat(
            List<CockleFilePdf> filesToCombine,
            TypeOfCombinedPdf typeOfCombinedPdf = TypeOfCombinedPdf.All,
            bool centerPdf = false)
        {
            if (filesToCombine.Count() < 1) { return; }

            var first_filename = filesToCombine.First();
            var destination_folder = System.IO.Path.GetDirectoryName(first_filename.FullName);

            // collect non-foldout files
            var files_no_foldouts = filesToCombine.Where(f => f.Rank >= 0).OrderBy(f => f.Rank);

            // collect foldout files
            var files_foldouts = filesToCombine
                .Where(f => f.FileType == SourceFileTypeEnum.Brief_Foldout || f.FileType == SourceFileTypeEnum.Brief_ZFold
                || f.FileType == SourceFileTypeEnum.App_Foldout || f.FileType == SourceFileTypeEnum.App_ZFold)
                .OrderBy(f => f.PageRange.FirstPage);

            // create new filename
            if (typeOfCombinedPdf == TypeOfCombinedPdf.All)
            {
                CombinedPdfFilename = System.IO.Path.Combine(destination_folder,
                    string.Format($"{first_filename.TicketNumber} pdf {first_filename.Attorney}.pdf"));
            }
            else
            {
                if (typeOfCombinedPdf == TypeOfCombinedPdf.Brief)
                {
                    CombinedPdfFilename = System.IO.Path.Combine(destination_folder,
                        string.Format($"{first_filename.TicketNumber} pdf {first_filename.Attorney} br.pdf"));
                }
                if (typeOfCombinedPdf == TypeOfCombinedPdf.Appendix)
                {
                    CombinedPdfFilename = System.IO.Path.Combine(destination_folder,
                        string.Format($"{first_filename.TicketNumber} pdf {first_filename.Attorney} app.pdf"));
                }
            }

            // adjust new filenames if foldouts
            if (files_foldouts.Count() > 0)
            {
                var temp_filename = CombinedPdfFilename;
                CombinedPdfFilename =
                    temp_filename.Replace(".pdf", @" (without foldouts).pdf");
                CombinedPdfFilenameWithFoldouts =
                    temp_filename.Replace(".pdf", @" (with foldouts).pdf");

                // combine files
                createCombinedPdf_noFoldouts(files_no_foldouts, typeOfCombinedPdf, centerPdf);
                createCombinedPdf_withFoldouts(files_no_foldouts, files_foldouts);
            }
            else
            {
                createCombinedPdf_noFoldouts(files_no_foldouts, typeOfCombinedPdf, centerPdf);
            }
        }
        private void createCombinedPdf_noFoldouts(
            IOrderedEnumerable<CockleFilePdf> files_no_foldouts,
            TypeOfCombinedPdf typeOfCombinedPdf,
            bool centerPdf = false)
        {
            CAcroApp app = new AcroApp();           // Acrobat
            CAcroPDDoc doc = new AcroPDDoc();       // First document
            CAcroPDDoc docToAdd = new AcroPDDoc();  // Next documents

            try
            {
                int numPages = 0, numPagesToAdd = 0;
                foreach (var f in files_no_foldouts)
                {
                    if (f.Rank == 0) // both 0
                    {
                        doc.Open(f.FullName);
                        numPages = doc.GetNumPages();
                    }
                    else if (f.Rank != 0 && numPages == 0)
                    {
                        doc.Open(f.FullName);
                        numPages = doc.GetNumPages();
                    }
                    else
                    {
                        if (!docToAdd.Open(f.FullName)) { break; }
                        numPagesToAdd = docToAdd.GetNumPages();
                        if (!doc.InsertPages(numPages - 1, docToAdd, 0, numPagesToAdd, 0)) { break; }
                        if (!docToAdd.Close()) { break; }
                        numPages = doc.GetNumPages();
                    }
                }
                doc.Save(1, CombinedPdfFilename);
                doc.Close();
                // use reflection to center the pdf
                if (centerPdf)
                {
                    var opened = doc.Open(CombinedPdfFilename);
                    if (opened)
                    {
                        object js_object = doc.GetJSObject();
                        Type js_type = js_object.GetType();
                        object[] js_param = { };
                        string script_name = string.Empty;
                        int? cover_length = null;
                        if (typeOfCombinedPdf == TypeOfCombinedPdf.Appendix
                            && files_no_foldouts.All(x => x.CoverLength == null))
                        {
                            script_name = Models.Utilities.AcrobatJS.Javascripts
                                [Models.Utilities.LocalJavascripts.center_letter_no_cover];
                        }
                        else if (files_no_foldouts.All(x => x.CoverLength == null))
                        {
                            script_name = Models.Utilities.AcrobatJS.Javascripts
                                [Models.Utilities.LocalJavascripts.center_letter_no_cover];
                        }
                        else
                        {
                            cover_length = files_no_foldouts.Where(x => x.FileType == SourceFileTypeEnum.Cover).FirstOrDefault().CoverLength;
                            switch (cover_length)
                            {
                                case 48:
                                default:
                                    script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.center_letter_48pica];
                                    break;
                                case 49:
                                    script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.center_letter_49pica];
                                    break;
                                case 50:
                                    script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.center_letter_50pica];
                                    break;
                                case 51:
                                    script_name = Models.Utilities.AcrobatJS.Javascripts
                                        [Models.Utilities.LocalJavascripts.center_letter_51pica];
                                    break;
                            }
                        }

                        js_type.InvokeMember(script_name,
                            System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                            null, js_object, js_param);
                        // test that the file exists
                        doc.Save(1, CombinedPdfFilename);
                    }
                    else
                    {
                        throw new Exception("Could not center pdf.");
                    }
                }
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
        private void createCombinedPdf_withFoldouts(
            IOrderedEnumerable<CockleFilePdf> files_no_foldouts,
            IOrderedEnumerable<CockleFilePdf> files_foldouts)
        {
            // cycle through foldouts, matching each foldout with a parent
            var parent_dictionary = new Dictionary<CockleFilePdf, List<CockleFilePdf>>();
            foreach (var fo in files_foldouts)
            {
                var matches = Regex.Matches(fo.FileTypeCode, @"([azy][a-z]|br)");

                if (matches.Count > 0)
                {
                    var parent_to_foldout = matches[0].Groups[1].ToString();
                    var parent_file_items = files_no_foldouts.Where(f => f.FileTypeCode.Equals(parent_to_foldout));

                    if (parent_file_items.Count() == 1)
                    {
                        var new_parent = parent_file_items.First();

                        // create dictionary entries for parent-foldout matches
                        if (parent_dictionary.ContainsKey(new_parent))
                        {
                            // 2 or more
                            parent_dictionary[new_parent].Add(fo);
                        }
                        else
                        {
                            // first items
                            var temp_list = new List<CockleFilePdf>();
                            temp_list.Add(fo);
                            parent_dictionary.Add(parent_file_items.First(), temp_list);
                        }
                    }
                }
            }

            // prepare list of files to merge
            var files_to_merge = new List<CockleFilePdf>(files_no_foldouts);

            // keep track of temp files
            var files_temp = new List<CockleFilePdf>();

            // cycle through dictionary, creating temp files to hold foldouts for merge
            foreach (var parent_pdf in parent_dictionary)
            {
                // gather info for new temp file (PROBLEM WITH PORT TO MVVM VERSION)
                var temp_parent = new CockleFilePdf(
                    parent_pdf.Key.FullName.Replace(".pdf", " temp.pdf"),
                    parent_pdf.Key.Attorney,
                    (int)parent_pdf.Key.TicketNumber,
                    parent_pdf.Key.FileType,
                    parent_pdf.Key.FileTypeCode);
                temp_parent.Rank = parent_pdf.Key.Rank;
                temp_parent.PageRange = parent_pdf.Key.PageRange;
                // create new temp file
                System.IO.File.Copy(parent_pdf.Key.FullName, temp_parent.FullName, true);
                // add foldouts to temp file
                foreach (var child_foldout_pdf in parent_pdf.Value)
                {
                    if (!insertFoldoutsIntoOriginals(temp_parent, child_foldout_pdf)) { break; }
                }
                // correct merge list
                files_to_merge.Remove(parent_pdf.Key);
                files_to_merge.Add(temp_parent);
                files_temp.Add(temp_parent);
            }
            // sort merge list
            files_to_merge.Sort((f, g) => f.Rank - g.Rank);

            // merge files into single pdf (WEIRD MOVE HERE!!!)
            //ConversionSuccessful = new CreateMergedPDFAcrobat(files_to_merge, target).ConversionSuccessful;

            // delete temp files
            foreach (var f in files_temp)
            {
                System.IO.File.Delete(f.FullName);
            }
        }

        private bool insertFoldoutsIntoOriginals(CockleFilePdf parent, CockleFilePdf foldout)
        {
            bool insertSuccessful = true;
            if (parent.PageRange == null || foldout.PageRange == null) { return false; }

            CAcroApp app = new AcroApp();           // Acrobat
            CAcroPDDoc doc = new AcroPDDoc();       // First document
            CAcroPDDoc docToAdd = new AcroPDDoc();  // Next documents

            try
            {

                doc.Open(parent.FullName);
                docToAdd.Open(foldout.FullName);

                int numPagesParent = doc.GetNumPages();
                int numPagesFoldout = docToAdd.GetNumPages();

                // ROTATE IF RECOMMENDED
                //if (foldout.PageRange.Rotation == PageRangePdf.ROTATE_ENUM.CLOCKWISE)
                //{
                //    // reflection to access acrobat javascript
                //    object jso = docToAdd.GetJSObject();
                //    Type type = jso.GetType();
                //    object[] getRotatePageParams = { 0, numPagesFoldout - 1, 90 };
                //    object rotatePage = type.InvokeMember("setPageRotations",
                //        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                //        null, jso, getRotatePageParams);
                //}
                //else if (foldout.PageRange.Rotation == PageRangePdf.ROTATE_ENUM.COUNTERCLOCKWISE)
                //{
                //    // reflection to access acrobat javascript
                //    object jso = docToAdd.GetJSObject();
                //    Type type = jso.GetType();
                //    object[] getRotatePageParams = { 0, numPagesFoldout - 1, 270 };
                //    object rotatePage = type.InvokeMember("setPageRotations",
                //        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                //        null, jso, getRotatePageParams);
                //}
                //else
                //{
                //    // do nothing
                //}

                if (parent.PageRange.TotalPages == foldout.PageRange.TotalPages)
                {
                    // check 1: phantom file was never updated    
                    if (parent.PageRange.FirstPage == 1 && foldout.PageRange.FirstPage != 1)
                    {
                        // if true, simply swap, page for page
                        doc.ReplacePages(0, docToAdd, 0, parent.PageRange.TotalPages, 0);
                    }
                    // check 2: all page numbers match, just do full replacement
                    else if (parent.PageRange.FirstPage == foldout.PageRange.FirstPage &&
                        parent.PageRange.LastPage == foldout.PageRange.LastPage)
                    {
                        doc.ReplacePages(0, docToAdd, 0, parent.PageRange.TotalPages, 0);
                    }
                    // not sure what else to check for here
                }
                else
                {
                    int InsertionPoint = (int)foldout.PageRange.FirstPage - (int)parent.PageRange.FirstPage;

                    if (!doc.DeletePages(InsertionPoint, InsertionPoint + numPagesFoldout - 1))
                    {
                        insertSuccessful = false;
                    }
                    if (!doc.InsertPages(InsertionPoint - 1, docToAdd, 0, numPagesFoldout, 0))
                    {
                        insertSuccessful = false;
                    }
                }
                docToAdd.Close();
                doc.Save(1, parent.FullName);
                //System.IO.Path.Combine(System.IO.Path.GetDirectoryName(parent.FullName), "testfile.pdf"));

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
            return insertSuccessful;
        }
    }
}
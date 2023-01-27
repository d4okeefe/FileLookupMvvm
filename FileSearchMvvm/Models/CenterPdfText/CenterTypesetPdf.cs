using FileSearchMvvm.Models.CockleTypes;
using System;
using Acrobat;
using System.Reflection;

namespace FileSearchMvvm.Models.CenterPdfText
{
    class CenterTypesetPdf
    {
        private CockleFilePdf selectedPdfFile;
        private bool hasCover;
        private int? coverLen;
        private ViewModels.SearchViewModelFolder.CenteredCoverType courtBriefSizeDoc;
        private string newFileName;

        public CockleFilePdf NewFileCreated { get; set; }

        public CenterTypesetPdf(CockleFilePdf _selectedFile,
            int? _cv_len, string _new_filename,
            ViewModels.SearchViewModelFolder.CenteredCoverType _courtBriefSizeDoc =
            ViewModels.SearchViewModelFolder.CenteredCoverType.Letter)
        {
            //if(!Models.Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace()) { throw new Exception(); }

            selectedPdfFile = _selectedFile;
            hasCover = null == _cv_len ? false : true;
            coverLen = _cv_len;
            courtBriefSizeDoc = _courtBriefSizeDoc;
            newFileName = _new_filename;

            // get type
            var orig_type = _selectedFile.FileType;
            var new_type = Utilities.SourceFileTypeEnum.UnrecognizedCentered;
            switch(orig_type)
            {
                case Utilities.SourceFileTypeEnum.Cover:
                    new_type = Utilities.SourceFileTypeEnum.CoverCentered;
                    break;
                case Utilities.SourceFileTypeEnum.InsideCv:
                    new_type = Utilities.SourceFileTypeEnum.InsideCvCentered;
                    break;
                case Utilities.SourceFileTypeEnum.Brief:
                    new_type = Utilities.SourceFileTypeEnum.BriefCentered;
                    break;
                case Utilities.SourceFileTypeEnum.App_File:
                    new_type = Utilities.SourceFileTypeEnum.App_FileCentered;
                    break;
                case Utilities.SourceFileTypeEnum.App_Index:
                    new_type = Utilities.SourceFileTypeEnum.App_IndexCentered;
                    break;
                case Utilities.SourceFileTypeEnum.Motion:
                    new_type = Utilities.SourceFileTypeEnum.MotionCentered;
                    break;
                case Utilities.SourceFileTypeEnum.Combined_Pdf:
                case Utilities.SourceFileTypeEnum.Combined_Pdf_No_FOs:
                case Utilities.SourceFileTypeEnum.Combined_Pdf_with_FOs:
                    new_type = Utilities.SourceFileTypeEnum.PdfCombinedCentered;
                    break;
            }

            NewFileCreated = new CockleFilePdf(newFileName, selectedPdfFile, new_type);

            if(courtBriefSizeDoc == ViewModels.SearchViewModelFolder.CenteredCoverType.Letter)
            {
                centerOnLetterPaper();
            }
            else
            {
                centerOnBriefPaper();
            }
        }

        private void centerOnBriefPaper()
        {
            CAcroApp app = new AcroApp();
            CAcroPDDoc doc = new AcroPDDoc();
            try
            {
                var opened = doc.Open(selectedPdfFile.FullName);
                if(!opened) { throw new Exception("Unable to open file."); }

                object js_object = doc.GetJSObject();


                Type js_type = js_object.GetType();
                object[] js_param = { };
                string script_name = string.Empty;
                if(hasCover)
                {
                    switch(coverLen)
                    {
                        case 48:
                        default:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_booklet_48pica];
                            break;
                        case 49:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_booklet_49pica];
                            break;
                        case 50:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_booklet_50pica];
                            break;
                        case 51:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_booklet_51pica];
                            break;
                    }
                }
                else
                {
                    script_name = Utilities.AcrobatJS.Javascripts
                        [Utilities.LocalJavascripts.center_booklet_no_cover];
                }

                js_type.InvokeMember(script_name,
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null, js_object, js_param);


                // save document
                var test = doc.Save(1, NewFileCreated.FullName); // doc.Save(1, NewFileCreated.FullName); would overwrite original
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                NewFileCreated = null;
                throw new Exception("Error in attempt to center Pdf file.");
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                app = null;

                GC.Collect();
            }
        }


        private void centerOnLetterPaper()
        {
            CAcroApp app = new AcroApp();           // Acrobat
            CAcroPDDoc doc = new AcroPDDoc();       // First document
            try
            {
                // use reflection to center the pdf
                var opened = doc.Open(selectedPdfFile.FullName);
                if(opened)
                {
                    object js_object = doc.GetJSObject();
                    Type js_type = js_object.GetType();
                    object[] js_param = { };
                    string script_name = string.Empty;
                    int? cover_length = null;
                    if(hasCover)
                    {
                        cover_length = coverLen;
                    }
                    switch(cover_length)
                    {
                        case 48:
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
                        default:
                            script_name = Models.Utilities.AcrobatJS.Javascripts
                                [Models.Utilities.LocalJavascripts.center_letter_no_cover];
                            break;
                    }

                    js_type.InvokeMember(script_name,
                        System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                        null, js_object, js_param);
                    // test that the file exists
                    doc.Save(1, newFileName);
                }
                else
                {
                    throw new Exception("Could not center pdf.");
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                doc.Close();
                app.CloseAllDocs();
                app.Exit();

                doc = null;
                app = null;

                GC.Collect();
            }
        }
    }
}
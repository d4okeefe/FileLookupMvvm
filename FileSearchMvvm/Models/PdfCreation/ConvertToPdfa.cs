using Acrobat;
using FileSearchMvvm.Models.CockleTypes;
using FileSearchMvvm.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using org.apache.pdfbox.pdfparser;
//using static org.bouncycastle.mail.smime.validator.SignedMailValidator;

namespace FileSearchMvvm.Models.PdfCreation
{
    class ConvertToPdfa
    {
        public CockleFilePdf OriginalCockleFilePdf { get; set; }
        public CockleFilePdf PdfaCockleFilePdf { get; set; }

        public ConvertToPdfa(CockleFilePdf cockleFilePdf, bool center)
        {
            if (null == cockleFilePdf) { throw new Exception("Original file is null."); }
            if (!Models.Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace()) { throw new Exception(); }

            OriginalCockleFilePdf = cockleFilePdf;


            CAcroApp app = new AcroApp();
            CAcroPDDoc doc = new AcroPDDoc();
            try
            {
                doc.Open(cockleFilePdf.FullName);

                // first center
                object js_object = doc.GetJSObject();
                Type js_type = js_object.GetType();
                object[] js_param = { };
                string script_name = string.Empty;
                if (cockleFilePdf.CoverLength != null)
                {
                    switch (cockleFilePdf.CoverLength)
                    {
                        case 48:
                        default:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_letter_48pica];
                            break;
                        case 49:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_letter_49pica];
                            break;
                        case 50:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_letter_50pica];
                            break;
                        case 51:
                            script_name = Utilities.AcrobatJS.Javascripts
                                [Utilities.LocalJavascripts.center_letter_51pica];
                            break;
                    }
                }
                else
                {
                    script_name = Utilities.AcrobatJS.Javascripts
                        [Utilities.LocalJavascripts.center_letter_no_cover];
                }

                object createCenteredPdf = js_type.InvokeMember(
                    script_name, /* name of js function */
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null, js_object, js_param);

                // then create pdfa
                object createNewPdfADoc = js_type.InvokeMember(
                    /*"convertToPdfA", name of js function */
                    AcrobatJS.Javascripts[LocalJavascripts.convert_pdfa],
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null, js_object, js_param);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception("Error in attempt to convert original to PDF/A file.");
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

            try
            {
                var new_file_name = OriginalCockleFilePdf.FullName.Replace(".pdf", "_A1b.pdf");
                while (!System.IO.File.Exists(new_file_name)) {/*block*/}
                PdfaCockleFilePdf = cockleFilePdf;
                PdfaCockleFilePdf.FullName = new_file_name.Replace("_A1b.pdf", " pdfa.pdf");

                System.IO.File.Move(new_file_name, PdfaCockleFilePdf.FullName);
                while (!System.IO.File.Exists(PdfaCockleFilePdf.FullName)) {/*block*/}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception("Error in attempt to rename PDF/A file.");
            }
        }
        public ConvertToPdfa(CockleFilePdf cockleFilePdf)
        {
            if (null == cockleFilePdf) { throw new Exception("Original file is null."); }
            if (!Models.Utilities.AcrobatJS.AreAcrobatJavascriptsInPlace()) { throw new Exception(); }

            OriginalCockleFilePdf = cockleFilePdf;


            CAcroApp app = new AcroApp();
            CAcroPDDoc doc = new AcroPDDoc();
            try
            {
                doc.Open(cockleFilePdf.FullName);

                object js_object = doc.GetJSObject();
                Type js_type = js_object.GetType();
                object[] js_param = { };
                object createNewPdfADoc = js_type.InvokeMember(
                    /*"convertToPdfA", name of js function */
                    AcrobatJS.Javascripts[LocalJavascripts.convert_pdfa],
                    BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null, js_object, js_param);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception("Error in attempt to convert original to PDF/A file.");
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

            try
            {
                var new_file_name = OriginalCockleFilePdf.FullName.Replace(".pdf", "_A1b.pdf");
                while (!System.IO.File.Exists(new_file_name)) {/*block*/}
                PdfaCockleFilePdf = cockleFilePdf;
                PdfaCockleFilePdf.FullName = new_file_name.Replace("_A1b.pdf", " pdfa.pdf");

                System.IO.File.Move(new_file_name, PdfaCockleFilePdf.FullName);
                while (!System.IO.File.Exists(PdfaCockleFilePdf.FullName)) {/*block*/}
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw new Exception("Error in attempt to rename PDF/A file.");
            }
        }
    }
}

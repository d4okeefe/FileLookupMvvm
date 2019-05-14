using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ACRODISTXLib;
using Acrobat;
using System.Reflection;

namespace FileSearchMvvm.Models.PdfCreation
{
    //internal class RedistillPdf
    //{
    //    public string DistilledPdf { get; set; }


    //    // simple open and print to Adobe PDF
    //    public RedistillPdf(string pdf)
    //    {
    //        //var successful = await Task<bool>()=>{ };
    //        CAcroApp app = new AcroApp();
    //        CAcroPDDoc doc = new AcroPDDoc();
    //        try
    //        {
    //            doc.Open(pdf);

    //            object js_object = doc.GetJSObject();
    //            Type js_type = js_object.GetType();
    //            object[] js_param = { };
    //            object createNewPdfADoc = js_type.InvokeMember(
    //                "convertToPdfA", /* name of js function */
    //                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
    //                null, js_object, js_param);
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            System.Diagnostics.Debug.WriteLine(ex.Message);
    //            return false;
    //        }
    //        finally
    //        {
    //            doc.Close();
    //            app.CloseAllDocs();
    //            app.Exit();

    //            doc = null;
    //            app = null;
    //        }


            //    using (var processor = new Ghostscript.NET.Processor.GhostscriptProcessor())
            //    {
            //        List<string> switches = new List<string>();
            //        switches.Add("-dPDFA=1");
            //        switches.Add("-dNOOUTERSAVE");
            //        switches.Add("-sProcessColorModel=DeviceRGB");
            //        switches.Add("-sDEVICE=pdfwrite");
            //        switches.Add("-dPDFACompatibilityPolicy=1");
            //        switches.Add(@"-o c:\scratch\output_file.pdf");
            //        switches.Add(@"c:\scratch\test.pdf.pdf");

            //        processor.StartProcessing(switches.ToArray(), null);
            //    }


            //    //gswin32 -dPDFA=1 -dNOOUTERSAVE -sProcessColorModel=DeviceRGB -sDEVICE=pdfwrite -o c:\scratch\output_file.pdf C:\Program Files (x86)\gs\gs9.15\lib\PDFA_def.ps -dPDFACompatibilityPolicy=1 c:\scratch\test.pdf.pdf
            //}


            // not working to redistill pdf
            //public RedistillPdf(string pdf)
            //{
            //    try
            //    {
            //        var distiller = new PdfDistiller();

            //        var directory = System.IO.Path.GetDirectoryName(pdf);
            //        var filename = System.IO.Path.GetFileNameWithoutExtension(pdf);
            //        var new_pdf = System.IO.Path.Combine(directory, filename + "_redistilled.pdf");
            //        var user = Environment.UserName;
            //        //var job_options = @"C:\Users\" + user + @"\AppData\Roaming\Adobe\Adobe PDF\Settings\";
            //        var job_options = @"C:\Program Files (x86)\Adobe\Acrobat 10.0\Acrobat\Settings\PDFA1b 2005 RGB.joboptions";
            //        distiller.FileToPDF(pdf, new_pdf, job_options);
            //        DistilledPdf = new_pdf;
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //    }
            //}
        //}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearchMvvm.Models.Utilities.iTextSharpUtilities
{
    public class MyLocationTextExtractionStrategy : iTextSharp.text.pdf.parser.LocationTextExtractionStrategy
    {
        //Hold each coordinate
        public List<RectAndText> myPoints = new List<RectAndText>();

        //Automatically called for each chunk of text in the PDF
        public override void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo)
        {
            base.RenderText(renderInfo);

            //Get the bounding box for the chunk of text
            var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
            var topRight = renderInfo.GetAscentLine().GetEndPoint();

            //Create a rectangle from it
            var rect = new iTextSharp.text.Rectangle(
                bottomLeft[iTextSharp.text.pdf.parser.Vector.I1], bottomLeft[iTextSharp.text.pdf.parser.Vector.I2],
                topRight[iTextSharp.text.pdf.parser.Vector.I1], topRight[iTextSharp.text.pdf.parser.Vector.I2]);

            //Add this to our main collection
            this.myPoints.Add(new RectAndText(rect, renderInfo.GetText()));
        }
    }
}

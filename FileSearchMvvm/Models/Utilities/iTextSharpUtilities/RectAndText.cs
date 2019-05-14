namespace FileSearchMvvm.Models.Utilities.iTextSharpUtilities
{
    public class RectAndText
    {
        public iTextSharp.text.Rectangle Rect;
        public System.String Text;
        public RectAndText(iTextSharp.text.Rectangle rect, System.String text)
        {
            this.Rect = rect;
            this.Text = text;
        }
    }
}
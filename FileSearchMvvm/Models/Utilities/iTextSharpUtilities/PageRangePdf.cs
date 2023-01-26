using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileSearchMvvm.Models.Utilities.iTextSharpUtilities
{
    public class PageRangePdf
    {
        #region Properties
        public int? FirstPage { get; set; }
        public int? LastPage { get; set; }
        public int TotalPages { get; set; }
        public int[] Pages; // indexing is BASE1
        public enum ROTATE_ENUM { CLOCKWISE, COUNTERCLOCKWISE, NONE };
        public ROTATE_ENUM Rotation { get; private set; }
        #endregion

        #region Constructor
        public PageRangePdf(string src, SourceFileTypeEnum type)
        {
            this.FirstPage = -1;
            this.LastPage = -1;
            this.Rotation = ROTATE_ENUM.NONE;

            if (type == SourceFileTypeEnum.Cover || type == SourceFileTypeEnum.InsideCv)
            {
                using (PdfReader reader = new PdfReader(src))
                {
                    this.TotalPages = reader.NumberOfPages;
                    if (this.TotalPages == 1)
                    {
                        this.FirstPage = 1;
                        this.LastPage = 1;
                    }
                    else if (this.TotalPages > 1)
                    {
                        this.FirstPage = 1;
                        this.LastPage = this.TotalPages;
                    }
                    else
                    {
                        this.TotalPages = -1;
                    }
                }
            }
            else if (type == SourceFileTypeEnum.Combined_Pdf || type == SourceFileTypeEnum.Combined_Pdf_No_FOs)
            {
                using (PdfReader reader = new PdfReader(src))
                {
                    TotalPages = reader.NumberOfPages;
                    FirstPage = null;
                    LastPage = null;
                }
            }
            else
            {
                using (PdfReader reader = new PdfReader(src))
                {
                    this.TotalPages = reader.NumberOfPages;
                    this.Pages = new int[this.TotalPages + 1];

                    PdfReaderContentParser parser = new PdfReaderContentParser(reader);
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        SimpleTextExtractionStrategy extract = new SimpleTextExtractionStrategy();
                        var extractedText = parser.ProcessContent(i, extract);
                        string textFromPage = extractedText.GetResultantText();

                        // here, check for blank page: means it's a divider page
                        if (System.Text.RegularExpressions.Regex.Matches(textFromPage, @"\S").Count == 0)
                        {
                            this.Pages[i] = -2; // -2 indicates blank page
                        }
                        else
                        {
                            int posNewLine = textFromPage.IndexOf('\n');

                            string strPageNum = "";
                            string firstLine = "";
                            int j = 0;
                            while (strPageNum.Equals("") && Pages[i] == 0)
                            {
                                // test for classic page number
                                if (j == 0)
                                {
                                    firstLine = textFromPage.Substring(0, posNewLine);
                                    strPageNum = new String(firstLine.Where(Char.IsDigit).ToArray());
                                }
                                // test for roman numeral
                                else if (j == 1)
                                {
                                    firstLine = textFromPage.Substring(0, posNewLine);
                                    char[] removeNewlineAndSpace = firstLine.Replace(" ", "").Replace("\n", "").ToArray();

                                    // January 2023 -- odd case of paging AIN ia, iia, iiia, iva
                                    // remove 'a' from end if starts with 'i'
                                    if (removeNewlineAndSpace[0] == 'i' && removeNewlineAndSpace[removeNewlineAndSpace.Length - 1] == 'a')
                                    {
                                        removeNewlineAndSpace = removeNewlineAndSpace.Take(removeNewlineAndSpace.Length - 1).ToArray();
                                    }
                                    int n = Roman_Parse(removeNewlineAndSpace);
                                    if (n != 0)
                                    {
                                        Pages[i] = n;
                                    }
                                }
                                // search for App. on page
                                else if (j == 2)
                                {
                                    var matches = System.Text.RegularExpressions.Regex.Matches(textFromPage, @"(App.?)( *)(\d+)");
                                    if (matches.Count > 0)
                                    {
                                        strPageNum = matches[0].Groups[3].Value;
                                    }
                                }
                                // test alternative foldout numbering style
                                else if (j == 3)
                                {
                                    var matches = System.Text.RegularExpressions.Regex.Matches(textFromPage, @"(\d+)([Aa])");
                                    if (matches.Count > 0)
                                    {
                                        strPageNum = matches[0].Groups[1].Value;
                                    }
                                }
                                else if (j == 4)
                                {
                                    if (type == SourceFileTypeEnum.App_Foldout || type == SourceFileTypeEnum.App_ZFold
                                        || type == SourceFileTypeEnum.Brief_Foldout || type == SourceFileTypeEnum.Brief_ZFold)
                                    {
                                        while (!textFromPage.Equals("") && !Char.IsDigit(textFromPage[0]))
                                        {
                                            textFromPage = textFromPage.Substring(1, textFromPage.Length - 1);
                                        }
                                        string digits = String.Empty; int k = 0;
                                        while (!textFromPage.Equals("") && Char.IsDigit(textFromPage[k]))
                                        {
                                            digits = digits + textFromPage[k++];
                                        }
                                        strPageNum = new String(digits.Where(Char.IsDigit).ToArray());
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                j++;
                            } // end while

                            if (Pages[i] == 0)
                            {
                                int intPageNum;
                                if (int.TryParse(strPageNum, out intPageNum))
                                {
                                    Pages[i] = intPageNum;
                                }
                            } // end parse number

                            // GET LOCATION OF FIRST LINE FOR FOLDOUTS
                            // THIS WILL GIVE US ROTATION THAT IS NEEDED TO GET PAGE NUMBER ON TOP
                            if (type == SourceFileTypeEnum.App_Foldout || type == SourceFileTypeEnum.App_ZFold
                                || type == SourceFileTypeEnum.Brief_Foldout || type == SourceFileTypeEnum.Brief_ZFold)
                            {
                                // attempt to get location, if foldout, of number found
                                MyLocationTextExtractionStrategy extract_loc = new MyLocationTextExtractionStrategy();
                                var extractedText_loc = parser.ProcessContent(i, extract_loc);
                                string textFromPage_loc = extractedText_loc.GetResultantText();

                                var ex = PdfTextExtractor.GetTextFromPage(reader, 1, extract_loc);

                                float llx = float.NaN;
                                float urx = float.NaN;
                                float ury = float.NaN;
                                float lly = float.NaN;

                                foreach (var p in extract_loc.myPoints)
                                {
                                    var a = p.Text;
                                    if (this.Pages[i] > 0 && a.Contains(this.Pages[i].ToString()))
                                    {
                                        llx = p.Rect.Left;
                                        lly = p.Rect.Bottom;
                                        ury = p.Rect.Top;
                                        urx = p.Rect.Right;
                                    }
                                }

                                // get page dimensions
                                var page_size = reader.GetPageSize(i);
                                var page_width = page_size.Width;
                                var page_height = page_size.Height;

                                // find which side
                                if (page_height > page_width)
                                {
                                    float mid_point = page_width / 2;
                                    if (llx < mid_point && lly < mid_point && urx < mid_point && ury < mid_point)
                                    {
                                        this.Rotation = ROTATE_ENUM.CLOCKWISE;
                                    }
                                    else if (llx > mid_point && lly > mid_point && urx > mid_point && ury > mid_point)
                                    {
                                        this.Rotation = ROTATE_ENUM.COUNTERCLOCKWISE;
                                    }
                                    else
                                    {
                                        // do nothing
                                        this.Rotation = ROTATE_ENUM.NONE;
                                    }
                                }
                            }


                        } // end else
                    } // end for loop for reader

                    // CAPTURE FIRST AND LAST PAGE NUMBER
                    // capture first page number (base 1)
                    this.FirstPage = Pages[1];

                    // capture first page for files with first page number blank
                    if (this.Pages[1] == 0)
                    {
                        // check second page
                        if (this.TotalPages > 1 && this.Pages[2] > 0)
                        {
                            this.Pages[1] = this.Pages[2] - 1;
                        }
                    }
                    // skip actual first page, if a divider page
                    if (this.Pages[1] == -2 && this.TotalPages > 1 && this.Pages[2] > 0)
                    {
                        this.FirstPage = this.Pages[2];
                    }
                    else
                    {
                        this.FirstPage = this.Pages[1];
                    }
                    this.LastPage = Pages[TotalPages];
                } // end using statement
            }
        }
        #endregion

        #region Public Methods
        public int Roman_Parse(char[] roman)
        {
            Dictionary<char, short> lookup = new Dictionary<char, short>
            {
                {'M', 1000},
                {'C', 100},
                {'L', 50},
                {'X', 10},
                {'V', 5},
                {'I', 1},
                {'m', 1000},
                {'c', 100},
                {'l', 50},
                {'x', 10},
                {'v', 5},
                {'i', 1}
            };

            int arabic = 0;

            for (int i = 0; i < roman.Count(); i++)
            {
                // return 0 if not valid roman numeral
                if (!lookup.ContainsKey(roman[i]))
                    return 0;

                if (i == roman.Count() - 1)
                {
                    arabic += lookup[roman[i]];
                }
                else
                {
                    if (lookup[roman[i + 1]] > lookup[roman[i]]) arabic -= lookup[roman[i]];
                    else arabic += lookup[roman[i]];
                }
            }
            return arabic;
        }
        #endregion
    }
}

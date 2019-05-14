using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSearchMvvm.Models.Imposition
{
    class SaddleStitchPageOrder
    {
        public int PageCount { get; private set; }
        public List<int> PageOrder { get; private set; }

        public SaddleStitchPageOrder(int pageCount)
        {
            if (pageCount % 4 != 0)
                throw new ArgumentException("Parameter must be divisible by 4.");

            this.PageCount = pageCount;
            populateList();
        }

        private void populateList()
        {
            PageOrder = new List<int>();

            int a = 1;
            int b = PageCount;
            int j;
            for (int i = 0; i < PageCount; ++i)
            {
                j = i + 1;
                if (j % 4 == 2 || j % 4 == 3)
                {
                    PageOrder.Add(a);
                    a++;
                }
                if (j % 4 == 1 || j % 4 == 0)
                {
                    PageOrder.Add(b);
                    b--;
                }
            }
        }
    }
}

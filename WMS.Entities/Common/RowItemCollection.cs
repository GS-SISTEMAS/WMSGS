using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMS.Entities.Common
{
    public class RowItemCollection
    {
        public int nroColumns { get; set; }
        public List<Column> columns { get; set; }
        public List<RowItem> rows { get; set; }
        public Transaction transaction { get; set; }
        public int rowsCount { get; set; }

        public RowItemCollection()
        {
            rows = new List<RowItem>();
            transaction = Common.InitTransaction();
            rowsCount = 0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Page
    {
        public int Id { get; set; }

        public int? Number { get; set; }

        public DateTime? DateRecord { get; set; }

        public int WordBookId { get; set; }

        public IEnumerable<Line> Lines { get; set; }

        public int? RowId { get; set; }
        public ObjectMin Row { get; set; }
    }
}

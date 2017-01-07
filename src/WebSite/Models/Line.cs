using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Line
    {
        public int Id { get; set; }

        public bool Up { get; set; }

        public int Number { get; set; }

        public int? PageId { get; set; }
    }
}

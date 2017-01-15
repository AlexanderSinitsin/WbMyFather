using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.ViewModels.Words
{
    public class SelectedWordBook
    {
        public int SelectedBookId { get; set; }

        public string Book { get; set; }

        public int? Number { get; set; }

        public DateTime? DateRecord { get; set; }

        public int? SelectedRowId { get; set; }

        public bool Up { get; set; }

        public int? LineNumber { get; set; }
    }
}

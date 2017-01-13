using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models;

namespace WebSite.ViewModels.Words
{
    public class WordBookViewModel
    {
        public int Id { get; set; }

        public int WordId { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public IEnumerable<Page> Pages { get; set; }

        public IEnumerable<string> Directions { get; set; }
    }
}

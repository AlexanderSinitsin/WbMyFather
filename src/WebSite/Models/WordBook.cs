using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class WordBook
    {
        public int Id { get; set; }

        public int WordId { get; set; }

        public int BookId { get; set; }

        public IEnumerable<Book> Books { get; set; }

        public IEnumerable<Page> Pages { get; set; }
    }
}

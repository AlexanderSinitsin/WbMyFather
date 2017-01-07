using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CityOfPublication { get; set; }

        public DateTime? DateOfPublication { get; set; }

        public string Publication { get; set; }

        public string Reference { get; set; }
    }
}

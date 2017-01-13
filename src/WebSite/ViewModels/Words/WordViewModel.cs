using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebSite.Models;

namespace WebSite.ViewModels.Words
{
    public class WordViewModel
    {
        public int? Id { get; set; }

        public bool IsNew => !Id.HasValue || Id.Value == 0;

        [Display(Name = "Дата создания")]
        public DateTime? DateCreate { get; set; }

        [Display(Name = "Слово")]
        public string Name { get; set; }

        [Display(Name = "Книги")]
        public IEnumerable<WordBookViewModel> WordBooks { get; set; }

        public IEnumerable<Book> BookList { get; set; }
    }
}

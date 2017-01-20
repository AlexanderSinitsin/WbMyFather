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

        [Required(ErrorMessage ="Поле {0} не может быть пустым.")]
        [Display(Name = "Слово")]
        public string Name { get; set; }

        [Display(Name = "Книги")]
        public IEnumerable<WordBookViewModel> WordBooks { get; set; }

        public IEnumerable<ObjectMin> BookList { get; set; }

        public IEnumerable<ObjectMin> RowList { get; set; }

        public SelectedWordBook SelectedWordBook { get; set; }
    }
}

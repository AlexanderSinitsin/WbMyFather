using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.ViewModels.Book
{
    public class BookViewModel
    {
        public int? Id { get; set; }

        public bool IsNew => !Id.HasValue || Id.Value == 0;

        [Display(Name = "Создана")]
        public DateTime? DateCreate { get; set; }

        [Display(Name = "Книга")]
        public string Name { get; set; }

        [Display(Name = "Издание")]
        public string Publication { get; set; }

        [Display(Name = "Город")]
        public string CityOfPublication { get; set; }

        [Display(Name = "Дата издания")]
        public DateTime? DateOfPublication { get; set; }

        [Display(Name = "Ссылка на книгу")]
        public string Reference { get; set; }
    }
}

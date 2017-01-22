using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.ViewModels.Books
{
    public class BookViewModel
    {
        public int? Id { get; set; }

        public bool IsNew => !Id.HasValue || Id.Value == 0;

        [Display(Name = "Дата создания")]
        public DateTime? DateCreate { get; set; }

        [Required(ErrorMessage = "Поле {0} не может быть пустым.")]
        [Display(Name = "Книга")]
        public string Name { get; set; }

        [Display(Name = "Издание")]
        public string Publication { get; set; }

        [Display(Name = "Город")]
        public string CityOfPublication { get; set; }

        [Display(Name = "Дата издания")]
        public DateTime? DateOfPublication { get; set; }

        public string YearOfPublication { get; set; }

        [Display(Name = "Ссылка на книгу")]
        public string Reference { get; set; }
    }
}

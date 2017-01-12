using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebSite.Models;
using WebSite.Models.Shared.Tables.Attributes;
using WebSite.Models.Shared.Tables.Attributes.Filters;

namespace WebSite.ViewModels.Books
{
    public class BookListItemViewModel
    {
        [Sortable]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [StringContainsFilter("Name")]
        [Sortable]
        [Display(Name = "Книга")]
        public string Name { get; set; }

        [StringContainsFilter("Publication")]
        [Sortable]
        [Display(Name = "Издание")]
        public string Publication { get; set; }

        [StringContainsFilter("CityOfPublication")]
        [Sortable]
        [Display(Name = "Город")]
        public string CityOfPublication { get; set; }

        [StringContainsFilter("DateOfPublication")]
        [Sortable]
        [Display(Name = "Дата издания")]
        public DateTime? DateOfPublication { get; set; }

        public string Reference { get; set; }

        public IEnumerable<WordBook> WordBooks { get; set; }

        [StringContainsFilter("WordBooks", RelatedEntityPropertyNames = "Word.Name")]
        [Display(Name = "Слова")]
        public string Words { get; set; }
    }
}

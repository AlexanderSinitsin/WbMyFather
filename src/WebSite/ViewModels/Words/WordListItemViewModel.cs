using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebSite.Models;
using WebSite.Models.Shared.Tables.Attributes;
using WebSite.Models.Shared.Tables.Attributes.Filters;

namespace WebSite.ViewModels.Words
{
    public class WordListItemViewModel
    {
        [Sortable]
        [Display(Name = "№")]
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        [StringContainsFilter("Name")]
        [Sortable]
        [Display(Name = "Слово")]
        public string Name { get; set; }

        public IEnumerable<WordBook> WordBooks { get; set; }

        [StringContainsFilter("WordBooks", RelatedEntityPropertyNames = "Book.Name")]
        [Display(Name = "Книги")]
        public string Books { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL.Entities
{
    public class Book : IBaseTable<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<WordBook> WordBooks { get; set; }

        public string CityOfPublication { get; set; }

        public DateTime? DateOfPublication { get; set; }

        public string Publication { get; set; }

        public string Reference { get; set; }
    }
}

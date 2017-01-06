using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL.Entities
{
    public class WordBook : IBaseTable<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int WordId { get; set; }
        public virtual Word Word { get; set; }

        public int BookId { get; set; }
        public virtual Book Book { get; set; }

        public ICollection<Page> Pages { get; set; }
    }
}

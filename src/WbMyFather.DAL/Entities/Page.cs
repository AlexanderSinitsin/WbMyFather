using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL.Entities
{
    public class Page : IBaseTable<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Number { get; set; }

        public int WordBookId { get; set; }
        public virtual WordBook WordBook { get; set; }

        public virtual ICollection<Line> Lines { get; set; }
    }
}

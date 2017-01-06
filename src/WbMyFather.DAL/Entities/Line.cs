using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WbMyFather.DAL.Model.Base;

namespace WbMyFather.DAL.Entities
{
    public class Line : IBaseTable<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool Up { get; set; }

        public int Number { get; set; }

        public int? PageId { get; set; }
        public virtual Page Page { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbMyFather.DTO.Models
{
    public class WordBookDto
    {
        public int Id { get; set; }

        public int WordId { get; set; }
        public WordDto Word { get; set; }

        public int BookId { get; set; }
        public BookDto Book { get; set; }

        public IEnumerable<PageDto> Pages { get; set; }
    }
}

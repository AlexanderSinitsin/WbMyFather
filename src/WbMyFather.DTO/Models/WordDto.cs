using System;
using System.Collections.Generic;
using WbMyFather.DTO.Models.Base;

namespace WbMyFather.DTO.Models
{
    public class WordDto : IKeyableDto<int>
    {
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        public string Name { get; set; }

        public IEnumerable<BookDto> Books { get; set; }

        public DateTime? Deleted { get; set; }
    }
}

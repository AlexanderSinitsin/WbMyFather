using System;
using System.Collections.Generic;
using WbMyFather.DTO.Models.Base;

namespace WbMyFather.DTO.Models
{
    public class PageDto : IKeyableDto<int>
    {
        public int Id { get; set; }

        public int? Number { get; set; }

        public int? RowId { get; set; }
        public ObjectMinDto Row { get; set; }

        public DateTime? DateRecord { get; set; }

        public int WordBookId { get; set; }

        public IEnumerable<LineDto> Lines { get; set; }
    }
}

using System;
using System.Collections.Generic;
using WbMyFather.DTO.Models.Base;

namespace WbMyFather.DTO.Models
{
    public class LineDto : IKeyableDto<int>
    {
        public int Id { get; set; }

        public bool Up { get; set; }

        public int Number { get; set; }

        public int? PageId { get; set; }
        public PageDto Page { get; set; }
    }
}

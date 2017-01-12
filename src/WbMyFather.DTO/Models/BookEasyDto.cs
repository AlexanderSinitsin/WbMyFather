using System;
using System.Collections.Generic;
using WbMyFather.DTO.Models.Base;

namespace WbMyFather.DTO.Models
{
    public class BookEasyDto : IKeyableDto<int>
    {
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        public string Name { get; set; }

        public string CityOfPublication { get; set; }

        public DateTime? DateOfPublication { get; set; }

        public string Publication { get; set; }

        public string Reference { get; set; }

        public DateTime? Deleted { get; set; }
    }
}

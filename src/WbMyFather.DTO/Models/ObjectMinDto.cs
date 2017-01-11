using System;
using System.Collections.Generic;
using WbMyFather.DTO.Models.Base;

namespace WbMyFather.DTO.Models
{
    public class ObjectMinDto : IKeyableDto<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}

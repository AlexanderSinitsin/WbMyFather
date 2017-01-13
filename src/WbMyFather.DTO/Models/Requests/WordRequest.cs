using System;
using System.Collections.Generic;

namespace WbMyFather.DTO.Models.Requests
{
    public class WordRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<WordBookDto> WordBooks { get; set; }
    }
}

using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbMyFather.DAL.Entities;
using WbMyFather.DAL.Model;
using WbMyFather.DTO;
using WbMyFather.DTO.Models;

namespace WbMyFather.BLL
{
    public class BllMapperConfig : Profile
    {
        public BllMapperConfig()
        {
            CreateMap(typeof(PagedList<>), typeof(PagedListDto<>));

            //Мапирование обектов доменной модели на объекты DTO или обекты BLL
            CreateMap<Row, ObjectMinDto>();
            CreateMap<Line, LineDto>();
            CreateMap<Page, PageDto>();
            CreateMap<Word, WordDto>();
            CreateMap<Book, BookDto>();
            CreateMap<Book, BookEasyDto>();
            CreateMap<WordBook, WordBookDto>();
            CreateMap<Word, ObjectMinDto>();
            CreateMap<Book, ObjectMinDto>();
        }
    }
}

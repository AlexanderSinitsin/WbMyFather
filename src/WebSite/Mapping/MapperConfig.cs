using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WbMyFather.DTO;
using WbMyFather.DTO.Models;
using WebSite.Models;
using WebSite.Models.Shared;
using WebSite.ViewModels.Word;

namespace WebSite.Mapping
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap(typeof(PagedListDto<>), typeof(PagedList<>));

            CreateMap<BookDto, Book>();
            CreateMap<LineDto, Line>();
            CreateMap<PageDto, Page>();
            CreateMap<WordBookDto, WordBook>()
                .ForMember(x => x.Books, d => d.MapFrom(p => new List<Book> {
                    new Book {
                        Id =p.Book.Id,
                        Name=p.Book.Name,
                        CityOfPublication=p.Book.CityOfPublication,
                        DateOfPublication=p.Book.DateOfPublication,
                        Publication=p.Book.Publication,
                        Reference=p.Book.Reference
                    }
                }));
            CreateMap<WordDto, WordListItemViewModel>()
                .ForMember(x => x.Books, d => d.MapFrom(p =>
                    string.Join(", ", p.WordBooks.Select(wb => wb.Book.Name))
                ))
                .ForMember(x => x.Pages, d => d.MapFrom(p =>
                    string.Join(", ", p.WordBooks.Select(wb =>
                        string.Join("; ", wb.Pages.Select(pg =>
                            pg.Number + " " + string.Join(": ", pg.Lines.Select(l =>
                                l.Up ? "^" + l.Number : "_" + l.Number))
                            ))
                         ))
                     ));
        }
    }
}

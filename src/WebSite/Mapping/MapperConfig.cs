using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WbMyFather.DTO;
using WbMyFather.DTO.Models;
using WebSite.Models;
using WebSite.Models.Shared;
using WebSite.ViewModels.Books;
using WebSite.ViewModels.Words;

namespace WebSite.Mapping
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap(typeof(PagedListDto<>), typeof(PagedList<>));

            CreateMap<ObjectMinDto, ObjectMin>();
            CreateMap<BookDto, Book>();
            CreateMap<LineDto, Line>();
            CreateMap<PageDto, Page>();
            CreateMap<WordBookDto, WordBook>();
            CreateMap<WordDto, WordListItemViewModel>()
                .ForMember(x => x.Books, d => d.MapFrom(p =>
                    //Названия книг
                    string.Join("; ", p.WordBooks.Select(wb => wb.Book.Name + " " +
                        //Страницы
                        string.Join(", ", wb.Pages.Select(pg =>
                            //Строки
                            pg.DateRecord.HasValue ? pg.DateRecord.Value.ToString("D") :
                            pg.RowId.HasValue ?
                            pg.Number + pg.Row.Name + " " + string.Join(" ", pg.Lines.Select(l =>
                                l.Up ? "&uarr;" + l.Number : "&darr;" + l.Number
                            )) :
                            pg.Number + " " + string.Join(" ", pg.Lines.Select(l =>
                                l.Up ? "&uarr;" + l.Number : "&darr;" + l.Number
                            ))
                        ))
                    ))
                 ));
            CreateMap<BookDto, BookListItemViewModel>()
                .ForMember(x => x.Words, d => d.MapFrom(p =>
                    //Названия книг
                    string.Join("; ", p.WordBooks.Select(wb => wb.Word.Name + " " +
                        //Страницы
                        string.Join(", ", wb.Pages.Select(pg =>
                            //Строки
                            pg.DateRecord.HasValue ? pg.DateRecord.Value.ToString("D") :
                            pg.RowId.HasValue ? 
                            pg.Number + pg.Row.Name + " " + string.Join(" ", pg.Lines.Select(l =>
                                l.Up ? "&uarr;" + l.Number : "&darr;" + l.Number
                            )) :
                            pg.Number + " " + string.Join(" ", pg.Lines.Select(l =>
                                l.Up ? "&uarr;" + l.Number : "&darr;" + l.Number
                            ))
                        ))
                    ))
                 ));
            CreateMap<BookDto, BookViewModel>();
        }
    }
}

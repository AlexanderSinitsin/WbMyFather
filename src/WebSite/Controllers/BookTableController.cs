using AutoMapper;
using Common.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DTO.Models;
using WbMyFather.DTO.Models.Requests;
using WebSite.Controllers.Base;
using WebSite.Models.Shared;
using WebSite.Models.Shared.Tables;
using WebSite.Models.Shared.Tables.Requests;
using WebSite.ViewModels.Book;

namespace WebSite.Controllers
{
    public class BookTableController : TableControllerBase<BookListItemViewModel>
    {
        private readonly IMapper _mapper;
        private readonly IBooksService _booksService;
        public BookTableController(IBooksService booksService, IMapper mapper, ILog log, ILog someService) : base(log, someService)
        {
            _booksService = booksService;
            _mapper = mapper;
        }

        protected override async Task<PagedList<BookListItemViewModel>> GetModels(GetSortedFilteredPaging pagingRequest, IEnumerable<Column> columns)
        {
            var data = await _booksService.GetAllPaged<BookDto>(pagingRequest);
            return _mapper.Map<PagedList<BookListItemViewModel>>(data);
        }

        protected override string OnClickFunction => "show";

        protected override IList<TableButton> TableButtons => new List<TableButton>
        {
            new TableButton("<a class=\"btn btn-sm btn-default m-r-md\"id=\"add-book\"><i class=\"fa fa-plus\"></i> Добавить</a>", "add-book", "addBook"),
            new TableButton("<a class=\"btn btn-sm btn-default m-r-md\" id=\"delete-selected-books\"><i class=\"fa fa-trash\"></i></a>", "delete-selected-books", "deleteSelectedBooks")
        };
    }
}

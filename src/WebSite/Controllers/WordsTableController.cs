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
using WebSite.ViewModels.Words;

namespace WebSite.Controllers
{
    public class WordsTableController : TableControllerBase<WordListItemViewModel>
    {
        private readonly IMapper _mapper;
        private readonly IWordsService _wordsService;
        public WordsTableController(IWordsService wordsService, IMapper mapper, ILog log, ILog someService) : base(log, someService)
        {
            _wordsService = wordsService;
            _mapper = mapper;
        }

        protected override async Task<PagedList<WordListItemViewModel>> GetModels(GetSortedFilteredPaging pagingRequest, IEnumerable<Column> columns)
        {
            var data = await _wordsService.GetAllPaged<WordDto>(pagingRequest);
            return _mapper.Map<PagedList<WordListItemViewModel>>(data);
        }

        protected override string OnClickFunction => "show";

        protected override IList<TableButton> TableButtons => new List<TableButton>
        {
            new TableButton("<a class=\"btn btn-sm btn-default m-r-md\"id=\"add-word\"><i class=\"fa fa-plus\"></i> Добавить</a>", "add-word", "addWord"),
            new TableButton("<a class=\"btn btn-sm btn-default m-r-md\" id=\"delete-selected-words\"><i class=\"fa fa-trash\"></i></a>", "delete-selected-words", "deleteSelectedWords")
        };
    }
}

using AutoMapper;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DTO.Models;
using WbMyFather.DTO.Models.Requests;
using WebSite.Controllers.Base;
using WebSite.Models;
using WebSite.ViewModels.Words;

namespace WebSite.Controllers
{
    [AllowAnonymous]
    public class WordsController : BaseController
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IWordsService _wordsService;
        private readonly IBooksService _booksService;
        private readonly IRowsService _rowsService;

        public WordsController(IWordsService wordsService,
           IBooksService booksService,
           IRowsService rowsService,
           IMapper mapper,
           ILog log,
           ILog someService) : base(someService)
        {
            _wordsService = wordsService;
            _booksService = booksService;
            _rowsService = rowsService;
            _mapper = mapper;
            _log = log;
        }

        public ActionResult Index()
        {
            Session["WordBooks"] = null;
            return View();
        }

        [Route("api/words/{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            Session["WordBooks"] = null;
            var wordDto = await _wordsService.GetById<WordDto>(id);
            var model = _mapper.Map<WordViewModel>(wordDto);

            return PartialView("_Word", model);
        }

        [HttpPost]
        [Route("api/words/del")]
        public async Task<ActionResult> Delete(int[] ids)
        {
            try
            {
                await _wordsService.Delete(ids.ToList());

                return Json(new { result = true });
            }
            catch (Exception)
            {
                return Json(new { result = false });
            }
        }

        [Route("api/words/add")]
        [Route("api/words/{id:int}/edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            var model = new WordViewModel();
            if (id.HasValue)
            {
                var wordDto = await _wordsService.GetById<WordDto>(id.Value);
                model = _mapper.Map<WordViewModel>(wordDto);
            }
            Session["WordBooks"] = model.WordBooks;
            await getLists(model);
            return PartialView("_WordEdit", model);
        }

        [HttpPost]
        [Route("api/words")]
        public async Task<ActionResult> Save(WordViewModel model)
        {
            model.WordBooks = Session["WordBooks"] != null ?
                (List<WordBookViewModel>)Session["WordBooks"] :
                new List<WordBookViewModel>();
            await getLists(model);
            if (!ModelState.IsValid)
            {
                return PartialView("_WordEdit", model);
            }

            var request = new WordRequest
            {
                Name = model.Name,
                WordBooks = model.WordBooks.Select(wb => new WordBookDto
                {
                    WordId = wb.WordId,
                    BookId = wb.BookId,
                    Pages = wb.Pages.Select(p => new PageDto
                    {
                        Id = p.Id,
                        Number = p.Number,
                        DateRecord = p.DateRecord,
                        RowId = p.RowId,
                        Lines = p.Number.HasValue ?
                            p.Lines.Select(l => new LineDto
                            {
                                Id = l.Id,
                                Number = l.Number,
                                Up = l.Up
                            }) : null
                    })
                })
            };

            try
            {
                if (model.IsNew)
                {
                    model.Id = await _wordsService.Create(request);
                }
                else
                {
                    request.Id = model.Id.Value;
                    await _wordsService.Update(request);
                }
            }
            catch (Exception exception)
            {
                _log.Error($"Ошибка при сохранении слова {JsonConvert.SerializeObject(request)}", exception);
                ModelState.AddModelError("", "Ошибка при сохранении в базу данных");

                return PartialView("_WordEdit", model);
            }

            return Json(new { id = model.Id });
        }

        [HttpPost]
        [Route("api/words/edit/book/add")]
        public ActionResult AddWordBook(SelectedWordBook wordBook)
        {
            var wordBooks = Session["WordBooks"] != null ?
                (List<WordBookViewModel>)Session["WordBooks"] :
                new List<WordBookViewModel>();

            wordBook.SelectedRowId = wordBook.SelectedRowId == 0 ? null : wordBook.SelectedRowId;

            if (wordBook.SelectedBookId <= 0 && string.IsNullOrEmpty(wordBook.Book))
            {
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Поле книга не может быть пустым." } });
            }

            if (!wordBook.Number.HasValue && !wordBook.DateRecord.HasValue)
            {
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Необходимо указать номер страницы или дату записи." } });
            }

            if (wordBook.Number.HasValue && !wordBook.LineNumber.HasValue)
            {
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Номер строки не может быть пустым." } });
            }

            var selectedWordBook = wordBooks.FirstOrDefault(wb => wb.BookId == wordBook.SelectedBookId
                 && wb.Pages.Any(p => (p.Number == wordBook.Number && p.RowId== wordBook.SelectedRowId) ||
                 p.DateRecord== wordBook.DateRecord)) ?? 
                 wordBooks.FirstOrDefault(wb => wb.BookId == wordBook.SelectedBookId) ?? new WordBookViewModel {
                     BookId = wordBook.SelectedBookId,
                     Pages = new List<Page>()
                 };
            wordBooks.Remove(selectedWordBook);
            var pages = selectedWordBook.Pages.ToList();
            var page = pages.FirstOrDefault(p => (p.Number == wordBook.Number && p.RowId == wordBook.SelectedRowId) ||
                 (wordBook.DateRecord.HasValue && p.DateRecord == wordBook.DateRecord)) ?? new Page
                 {
                     DateRecord = wordBook.DateRecord,
                     Number = wordBook.Number,
                     RowId = wordBook.SelectedRowId,
                     Lines = new List<Line>()
                 };
            pages.Remove(page);
            var lines = page.Lines.ToList();
            if(!lines.Any(l => l.Number == wordBook.LineNumber && l.Up == wordBook.Up))
            {
                lines.Add(new Line
                {
                    Number = wordBook.LineNumber.HasValue ? wordBook.LineNumber.Value : 0,
                    Up = wordBook.Up
                });
            }
            page.Lines = lines;
            pages.Add(page);
            selectedWordBook.Pages = pages;
            wordBooks.Add(selectedWordBook);

            Session["WordBooks"] = wordBooks;

            return Json(new { result = wordBooks });
        }

        [HttpPost]
        [Route("api/words/edit/book/delete")]
        public ActionResult DeleteWordBook(SelectedWordBook wordBook, string row)
        {
            var wordBooks = Session["WordBooks"] != null ?
                (List<WordBookViewModel>)Session["WordBooks"] :
                new List<WordBookViewModel>();

            if (wordBook.DateRecord.HasValue)
            {
                var selectedWordBook = wordBooks.FirstOrDefault(wb => wb.BookId == wordBook.SelectedBookId &&
                    wb.Pages.Any(p => p.DateRecord == wordBook.DateRecord));
                var pages = selectedWordBook.Pages.ToList();
                pages.Remove(pages.FirstOrDefault(p => p.DateRecord == wordBook.DateRecord));

                foreach (var wb in wordBooks)
                {
                    if (wb.BookId == selectedWordBook.BookId && wordBooks.Count <= 1)
                    {
                        wordBooks.Remove(selectedWordBook);
                    }
                    else if (wb.BookId == selectedWordBook.BookId)
                    {
                        wb.Pages = pages;
                    }
                }
            }
            else if (wordBook.Number.HasValue)
            {
                var selectedWordBook = wordBooks.FirstOrDefault(wb => wb.BookId == wordBook.SelectedBookId &&
                    wb.Pages.Any(p => p.Number == wordBook.Number));
                var pageOnRemove = new Page();
                var pages = selectedWordBook.Pages.ToList();
                foreach (var page in selectedWordBook.Pages)
                {
                    if (page.Number == wordBook.Number)
                    {
                        var lines = page.Lines.ToList();
                        lines.Remove(lines.FirstOrDefault(l => l.Number == wordBook.LineNumber));
                        page.Lines = lines;
                        pageOnRemove = page;
                    }
                }
                if (pages.Count <= 1)
                {
                    pages.Remove(pageOnRemove);
                }
                foreach (var wb in wordBooks)
                {
                    if (wb.BookId == selectedWordBook.BookId && wordBooks.Count <= 1)
                    {
                        wordBooks.Remove(selectedWordBook);
                    }
                    else if (wb.BookId == selectedWordBook.BookId)
                    {
                        wb.Pages = selectedWordBook.Pages;
                    }
                }
            }


            Session["WordBooks"] = wordBooks;

            return Json(new { result = row });
        }

        private async Task getLists(WordViewModel model)
        {
            try
            {
                var books = _mapper.Map<IEnumerable<ObjectMin>>(await _booksService.GetAll<ObjectMinDto>()).ToList();
                var rows = _mapper.Map<IEnumerable<ObjectMin>>(await _rowsService.GetAll<ObjectMinDto>()).ToList();
                books.Add(new ObjectMin());
                rows.Add(new ObjectMin());
                model.BookList = books.OrderBy(b => b.Name);
                model.RowList = rows.OrderBy(b => b.Name);
            }
            catch (Exception ex)
            {
                _log.Error($"Ошибка при получении списков", ex);
                ModelState.AddModelError("", "Ошибка при сохранении в базу данных");

                model.BookList = new List<ObjectMin>();
                model.RowList = new List<ObjectMin>();
            }
        }
    }
}

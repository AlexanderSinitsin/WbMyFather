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
            if (!model.WordBooks.Any())
            {
                ModelState.AddModelError("SelectedWordBook.SelectedBookId", "Необходимо добавить хотя бы одну запись.");
            }
            if (!ModelState.IsValid)
            {
                return PartialView("_WordEdit", model);
            }

            var request = new WordRequest
            {
                Name = model.Name,
                WordBooks = model.WordBooks.Select(wb => new WordBookDto
                {
                    Id = wb.Id,
                    WordId = wb.WordId,
                    BookId = wb.BookId,
                    Book = new BookDto
                    {
                        Name = wb.BookId == 0 ? wb.Book?.Name : null
                    },
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
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Поле Книга не может быть пустым." } });
            }

            if (!wordBook.Number.HasValue && !wordBook.DateRecord.HasValue)
            {
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Необходимо указать Номер страницы или дату записи." } });
            }

            if (wordBook.Number.HasValue && !wordBook.LineNumber.HasValue)
            {
                return Json(new { result = false, error = new { field = "SelectedWordBook.SelectedBookId", text = "Номер строки не может быть пустым." } });
            }

            // Приводим введенные данные кобщему представлению
            var selectedWordBook = wordBooks.FirstOrDefault(wb => (wb.BookId == wordBook.SelectedBookId || (!string.IsNullOrEmpty(wordBook.Book) && wordBook.Book == wb.Book?.Name))
                 && wb.Pages.Any(p => (wordBook.Number.HasValue && p.Number == wordBook.Number && p.RowId == wordBook.SelectedRowId) ||
                 (wordBook.DateRecord.HasValue && p.DateRecord == wordBook.DateRecord))) ??
                 wordBooks.FirstOrDefault(wb => wb.BookId == wordBook.SelectedBookId || (!string.IsNullOrEmpty(wordBook.Book) && wordBook.Book == wb.Book?.Name)) ?? new WordBookViewModel
                 {
                     Pages = new List<Page>()
                 };
            // Фиксируем данные о выбранной книге
            if (!string.IsNullOrEmpty(wordBook.Book))
            {
                selectedWordBook.Book = new Book
                {
                    Name = wordBook.Book
                };
            }
            else
            {
                selectedWordBook.BookId = wordBook.SelectedBookId;
            }
            // Создаем список страниц с линиями
            if (!selectedWordBook.Pages.Any() || !selectedWordBook.Pages.Any(p => (wordBook.Number.HasValue && p.Number == wordBook.Number && p.RowId == wordBook.SelectedRowId) ||
                  (wordBook.DateRecord.HasValue && p.DateRecord == wordBook.DateRecord)))
            {
                selectedWordBook.Pages = selectedWordBook.Pages.Concat(new List<Page> {
                    new Page
                    {
                        DateRecord=wordBook.DateRecord,
                        Number=wordBook.Number,
                        RowId=wordBook.SelectedRowId,
                        Lines = new List<Line>
                        {
                          new Line{
                              Number=wordBook.LineNumber.HasValue ? wordBook.LineNumber.Value : 0,
                              Up=wordBook.Up
                            }
                        }
                    }
                });
            }
            else
            {
                var pages = selectedWordBook.Pages.Any(p => (wordBook.Number.HasValue && p.Number == wordBook.Number && p.RowId == wordBook.SelectedRowId) ||
                  (wordBook.DateRecord.HasValue && p.DateRecord == wordBook.DateRecord));
                foreach (var page in selectedWordBook.Pages)
                {
                    if ((wordBook.Number.HasValue && page.Number == wordBook.Number && page.RowId == wordBook.SelectedRowId) ||
                        (wordBook.DateRecord.HasValue && page.DateRecord == wordBook.DateRecord))
                    {
                        page.Lines = page.Lines.Concat(new List<Line> {
                            new Line {
                                Number=wordBook.LineNumber.HasValue ? wordBook.LineNumber.Value : 0,
                                Up=wordBook.Up
                            }
                        });
                        break;
                    }
                }
            }

            // Ищем в общем списке выбранную книгу
            if (!wordBooks.Any(wb => (wb.BookId == wordBook.SelectedBookId || (!string.IsNullOrEmpty(wordBook.Book) && wordBook.Book == wb.Book?.Name))))
            {
                wordBooks.Add(selectedWordBook);
            }

            Session["WordBooks"] = wordBooks;
            return Json(new { result = wordBooks });
        }

        [HttpPost]
        [Route("api/words/edit/book/delete")]
        public ActionResult DeleteWordBook(SelectedWordBook wordBook, int? wbId, int? pageId, int? lineId)
        {
            var wordBooks = Session["WordBooks"] != null ?
                (List<WordBookViewModel>)Session["WordBooks"] :
                new List<WordBookViewModel>();

            var wordBookSelected = wordBooks.FirstOrDefault(wb => wb.Id == wbId || wb.BookId == wordBook.SelectedBookId || wb.Book?.Name == wordBook.Book);
            if (wordBookSelected == null)
            {
                return Json(new { error = true });
            }
            foreach (var wb in wordBooks)
            {
                if (wb.Id == wbId || wb.BookId == wordBook.SelectedBookId || (!string.IsNullOrEmpty(wordBook.Book) && wordBook.Book == wb.Book?.Name))
                {
                    if (wb.Pages.Count() <= 1 && wb.Pages.Any(p => p.Lines.Count() <= 1))
                    {
                        wordBooks.Remove(wb);
                    }
                    else
                    {
                        var pages = wb.Pages.ToList();
                        foreach (var p in pages)
                        {
                            if (p.Id == pageId ||
                                (p.Number == wordBook.Number && p.RowId == wordBook.SelectedRowId) ||
                                (wordBook.DateRecord.HasValue && p.DateRecord == wordBook.DateRecord))
                            {
                                var lines = p.Lines.ToList();
                                if (lines.Count > 1)
                                {
                                    lines.Remove(lines.FirstOrDefault(l => l.Id == lineId || (l.Number == wordBook.LineNumber && l.Up == wordBook.Up)));
                                    p.Lines = lines;
                                }
                                else
                                {
                                    pages.Remove(p);
                                }
                                break;
                            }
                        }
                        wb.Pages = pages;
                    }
                    break;
                }
            }

            Session["WordBooks"] = wordBooks;

            return Json(true);
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

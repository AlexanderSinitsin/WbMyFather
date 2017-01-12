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
using WebSite.ViewModels.Books;

namespace WebSite.Controllers
{
    public class BooksController : BaseController
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService,
           IMapper mapper,
           ILog log,
           ILog someService) : base(someService)
        {
            _booksService = booksService;
            _mapper = mapper;
            _log = log;
        }

        [Route("books")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("api/books/{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            var bookDto = await _booksService.GetById<BookEasyDto>(id);
            var model = _mapper.Map<BookViewModel>(bookDto);

            return PartialView("_Book", model);
        }

        [HttpDelete]
        [Route("api/books")]
        public async Task<ActionResult> Delete(int[] ids)
        {
            try
            {
                await _booksService.Delete(ids.ToList());

                return Json(new { result = true });
            }
            catch (Exception)
            {
                return Json(new { result = false });
            }
        }

        [Route("api/books/add")]
        [Route("api/books/{id:int}/edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            var model = new BookViewModel();
            if (id.HasValue)
            {
                var bookDto = await _booksService.GetById<BookEasyDto>(id.Value);
                model = _mapper.Map<BookViewModel>(bookDto);
            }

            return PartialView("_BookEdit", model);
        }

        [HttpPost]
        [Route("api/books")]
        public async Task<ActionResult> Save(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_BookEdit", model);
            }

            var request = new BookRequest
            {
                Name = model.Name,
                CityOfPublication = model.CityOfPublication,
                DateOfPublication = model.DateOfPublication,
                Publication = model.Publication,
                Reference = model.Reference
            };

            try
            {
                if (model.IsNew)
                {
                    model.Id = await _booksService.Create(request);
                }
                else
                {
                    request.Id = model.Id.Value;
                    await _booksService.Update(request);
                }
            }
            catch (Exception exception)
            {
                _log.Error($"Ошибка при сохранении контракта {JsonConvert.SerializeObject(request)}", exception);
                ModelState.AddModelError("", "Ошибка при сохранении в базу данных");

                return PartialView("_BookEdit", model);
            }

            return JavaScript($"$('.modal').modal('hide');Popups.showPopup(Url.action('api/books/{model.Id}'), null, $('#object-show-content'), $('#object-show'));$('#BookListItemViewModelTable').DataTable().ajax.reload(null, false);");
        }
    }
}

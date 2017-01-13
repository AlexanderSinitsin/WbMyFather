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
using WebSite.ViewModels.Words;

namespace WebSite.Controllers
{
    [AllowAnonymous]
    public class WordsController : BaseController
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IWordsService _wordsService;

        public WordsController(IWordsService wordsService,
           IMapper mapper,
           ILog log,
           ILog someService) : base(someService)
        {
            _wordsService = wordsService;
            _mapper = mapper;
            _log = log;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Route("api/words/{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
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

            return PartialView("_WordEdit", model);
        }

        [HttpPost]
        [Route("api/words")]
        public async Task<ActionResult> Save(WordViewModel model)
        {
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
                        Number = p.Number,
                        DateRecord = p.DateRecord,
                        RowId = p.RowId,
                        Lines = p.Number.HasValue ?
                            p.Lines.Select(l => new LineDto
                            {
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
    }
}

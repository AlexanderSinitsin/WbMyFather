using AutoMapper;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WbMyFather.BLL.Services.Interfaces;
using WebSite.Controllers.Base;

namespace WebSite.Controllers
{
    public class BookController : BaseController
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IWordsService _wordsService;

        public BookController(IWordsService wordsService,
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
    }
}

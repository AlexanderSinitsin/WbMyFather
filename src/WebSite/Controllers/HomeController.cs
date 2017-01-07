using AutoMapper;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WbMyFather.BLL.Services.Interfaces;

namespace WebSite.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly ILog _log;
        private readonly IMapper _mapper;
        private readonly IWordsService _wordsService;

        public HomeController(IWordsService wordsService,
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

using AutoMapper;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WbMyFather.BLL.Services.Interfaces;
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

        public ActionResult Index()
        {
            return View();
        }
    }
}

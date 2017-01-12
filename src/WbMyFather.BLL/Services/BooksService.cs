using AutoMapper;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using WbMyFather.BLL.Services.Base;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DAL;
using WbMyFather.DAL.Entities;
using WbMyFather.DTO;
using WbMyFather.DTO.Models;
using WbMyFather.DTO.Models.Requests;

namespace WbMyFather.BLL.Services
{
    public class BooksService : ServiceBase<Book>, IBooksService
    {
        private readonly IRepository<WordBook> _wordBookRepository;

        public BooksService(IUnitOfWork uoW, IRepository<Book> repository, IMapper mapper, ILog logger, IRepository<WordBook> wordBookRepository)
            : base(uoW, repository, mapper, logger)
        {
            _wordBookRepository = wordBookRepository;
        }

        public async Task<IEnumerable<TDto>> GetAll<TDto>()
        {
            return await Get<TDto>(null, sortBy: "Name");
        }

        public async Task<PagedListDto<TDto>> GetAllPaged<TDto>(GetSortedFilteredPaging param)
        {
            return await GetPaged<TDto>(param, null);
        }

        public async Task<TDto> GetById<TDto>(int id)
        {
            return await GetById<TDto>(id);
        }

        public async Task Delete(int id)
        {
            await Delete(id);
        }

        public async Task Delete(IEnumerable<int> ids)
        {
            await Delete(ids);
        }

        public async Task<int> Create(BookRequest request)
        {
            var book = new Book
            {
                DateCreate = DateTime.UtcNow,
                Name = request.Name,
                CityOfPublication = request.CityOfPublication,
                DateOfPublication = request.DateOfPublication,
                Publication = request.Publication,
                Reference = request.Reference,
            };

            return await Create(book);
        }

        public async Task Update(BookRequest request)
        {
            try
            {
                var word = await Repository.Where(l => l.Id == request.Id).SingleOrDefaultAsync();
                if (word.Name != request.Name) { word.Name = request.Name; }
                if (word.CityOfPublication != request.CityOfPublication)
                {
                    word.CityOfPublication = request.CityOfPublication;
                }
                if (word.DateOfPublication != request.DateOfPublication)
                {
                    word.DateOfPublication = request.DateOfPublication;
                }
                if (word.Publication != request.Publication) { word.Publication = request.Publication; }
                if (word.Reference != request.Reference) { word.Reference = request.Reference; }

                await Uow.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger.Error($"Ошибка обновления данных подрядчика. Id: {request.Id}, entity: {JsonConvert.SerializeObject(request)}", ex);
                throw;
            }
        }
    }
}

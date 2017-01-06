using AutoMapper;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbMyFather.BLL.Exceptions;
using WbMyFather.BLL.Services.Base;
using WbMyFather.BLL.Services.Interfaces;
using WbMyFather.DAL;
using WbMyFather.DAL.Entities;

namespace WbMyFather.BLL.Services
{
    public class WordsService : ServiceBase<Word>, IWordsService
    {
        public WordsService(IUnitOfWork uoW, IRepository<Word> repository, IMapper mapper, ILog logger) 
            : base(uoW, repository, mapper, logger) { }

        public async Task<IEnumerable<TDto>> GetAll<TDto>()
        {
            return await Get<TDto>(null, sortBy: "Name");
        }

        /*public async Task<PagedListDto<TDto>> GetAllPaged<TDto>(GetSortedFilteredPaging param, int? userId)
        {
            return await GetPaged<TDto>(param, null);
        }*/

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

        /*public async Task<int> Create(WordRequest request)
        {
            var obj = new Word
            {
                DateCreate = DateTime.UtcNow,
                Name = request.Name
            };

            return await Create(obj);
        }

        public async Task Update(WordRequest request)
        {
            try
            {
                var query = Repository.Where(e => e.Id == request.Id);

                var ent = await query.SingleOrDefaultAsync();
                if (ent == null) throw new EntityNotFoundException();

                if (ent.Name != request.Name) { ent.Name = request.Name; }

                Repository.Update(ent);
                await Uow.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Logger.Error($"Ошибка обновления данных подрядчика. Id: {request.Id}, entity: {JsonConvert.SerializeObject(request)}", ex);
                throw;
            }
        }*/
    }
}

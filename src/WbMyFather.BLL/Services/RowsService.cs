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
using WbMyFather.DTO.Models.Requests;

namespace WbMyFather.BLL.Services
{
    public class RowsService : ServiceBase<Row>, IRowsService
    {
        public RowsService(IUnitOfWork uoW, IRepository<Row> repository, IMapper mapper, ILog logger)
            : base(uoW, repository, mapper, logger)
        { }

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
            return await base.GetById<TDto>(id);
        }
    }
}

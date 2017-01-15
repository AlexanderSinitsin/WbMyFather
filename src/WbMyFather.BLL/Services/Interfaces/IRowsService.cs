using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WbMyFather.DTO;
using WbMyFather.DTO.Models.Requests;

namespace WbMyFather.BLL.Services.Interfaces
{
    public interface IRowsService
    {
        Task<IEnumerable<TDto>> GetAll<TDto>();
        Task<PagedListDto<TDto>> GetAllPaged<TDto>(GetSortedFilteredPaging param);
        Task<TDto> GetById<TDto>(int id);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace WbMyFather.BLL.Services.Interfaces
{
    public interface IWordsService
    {
        Task<IEnumerable<TDto>> GetAll<TDto>();
        //Task<PagedListDto<TDto>> GetAllPaged<TDto>(GetSortedFilteredPaging param);
        Task<TDto> GetById<TDto>(int id);
        Task Delete(int id);
        Task Delete(IEnumerable<int> ids);
        //Task<int> Create(WordRequest request, int? userId);
        //Task Update(WordRequest request, int? userId);
    }
}

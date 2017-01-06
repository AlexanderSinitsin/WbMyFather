namespace WbMyFather.DAL.Model
{
    public interface IPagedList
    {
        int TotalCount { get; }
        int PageCount { get; }
        int PageIndex { get; }
        int PageSize { get; }
    }
}

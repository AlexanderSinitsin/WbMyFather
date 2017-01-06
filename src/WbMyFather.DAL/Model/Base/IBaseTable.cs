namespace WbMyFather.DAL.Model.Base
{
    public interface IBaseTable<T>
    {
        T Id { get; set; }
    }
}

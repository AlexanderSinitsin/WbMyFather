namespace WbMyFather.DTO.Models.Base
{
    public interface IKeyableDto<T>
    {
        T Id { get; set; }
    }
}

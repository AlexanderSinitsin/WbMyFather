using WbMyFather.DAL;

namespace WbMyFather.BLL
{
    public interface IUnitOfWorkFactory
    {
        IWbMyFatherUnitOfWork CreateSmikUnitOfWork();
    }
}

using WbMyFather.DAL;

namespace WbMyFather.BLL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IWbMyFatherUnitOfWork CreateSmikUnitOfWork()
        {
            return new WbMyFatherUnitOfWork();
        }
    }
}

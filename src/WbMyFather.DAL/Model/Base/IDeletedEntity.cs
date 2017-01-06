using System;

namespace WbMyFather.DAL.Model.Base
{
    /// <summary> С признаком удаления </summary>
    public interface IDeletedEntity
    {
        DateTime? Deleted { get; set; }
    }
}

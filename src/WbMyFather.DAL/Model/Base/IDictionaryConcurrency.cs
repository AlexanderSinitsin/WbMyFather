using System.ComponentModel.DataAnnotations;

namespace WbMyFather.DAL.Model.Base
{
    public interface IDictionaryConcurrency
    {
        /// <summary> Optimistic Concurrency </summary>
        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}

using System.Collections.Generic;

namespace Arma3BE.Client.Infrastructure.Data
{
    public interface IBasicRepository<T>
    {
        IEnumerable<T> Get();
        void Set(T[] data);
    }
}
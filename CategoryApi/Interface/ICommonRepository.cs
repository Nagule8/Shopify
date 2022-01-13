using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopApi.Interface
{
    public interface ICommonRepository<T>
    {
        Task<IEnumerable<T>> Get();
        Task<T> GetSpecific(int id);
        Task<T> Add(T input);
        Task<T> Update(T input);
        Task Delete(int id);
        bool Exists(int id);

    }
}

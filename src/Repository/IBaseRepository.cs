using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroHeart.InfluxDB.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        void Insert(T t);

        Task InsertAsync(T t);

        void InsertMany(List<T> entities);

        Task InsertManyAsync(List<T> entities);

        Task DeleteAsync(DeletePredicateRequest request);

        Task<List<T>> QueryAsync(string query);

        Task<List<T>> QueryAsync(Query query);

        Task QueryAsync(string query, Action<T> onNext, Action<Exception> onError = null, Action onComplete = null);

        Task QueryAsync(Query query, Action<T> onNext, Action<Exception> onError = null, Action onComplete = null);

        InfluxDBQueryable<T> Query();
    }
}

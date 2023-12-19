using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroHeart.InfluxDB.Repository
{
    public partial class BaseRepository<T> where T : class
    {
        public async Task InsertAsync(T t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            await ExistOrCreateOrgBucketAsync();

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurement(t, WritePrecision, Bucket, Org);
            }
        }

        public async Task InsertManyAsync(List<T> entities)
        {
            if ((entities?.Count ?? 0) == 0)
                throw new ArgumentNullException(nameof(entities));

            await ExistOrCreateOrgBucketAsync();
            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurements<T>(entities, WritePrecision, Bucket, Org);
            }
        }

        public async Task DeleteAsync(DeletePredicateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await ExistOrCreateOrgBucketAsync();

            var deleteApi = client.GetDeleteApi();
            await deleteApi.Delete(request, Bucket, Org);
        }

        public async Task<List<T>> QueryAsync(string query)
        {
            await ExistOrCreateOrgBucketAsync();

            var queryApi = client.GetQueryApi();
            return await queryApi.QueryAsync<T>(query, Org);
        }

        public async Task<List<T>> QueryAsync(Query query)
        {
            await ExistOrCreateOrgBucketAsync();

            var queryApi = client.GetQueryApi();
            return await queryApi.QueryAsync<T>(query, Org);
        }

        public async Task QueryAsync(string query, Action<T> onNext, Action<Exception> onError = null, Action onComplete = null)
        {
            await ExistOrCreateOrgBucketAsync();

            var queryApi = client.GetQueryApi();
            await queryApi.QueryAsync<T>(query, onNext, onError, onComplete, Org);
        }

        public async Task QueryAsync(Query query, Action<T> onNext, Action<Exception> onError = null, Action onComplete = null)
        {
            await ExistOrCreateOrgBucketAsync();

            var queryApi = client.GetQueryApi();
            await queryApi.QueryAsync<T>(query, onNext, onError, onComplete, Org);
        }


        #region 私有方法

        private async Task ExistOrCreateOrgBucketAsync()
        {
            if (organization == null)
                await ExistOrCreateOrgAsync();
            if (bucket == null)
                await ExistOrCreateBucketAsync();
        }

        private async Task ExistOrCreateOrgAsync()
        {
            var orgApi = client.GetOrganizationsApi();
            var orgs = await orgApi.FindOrganizationsAsync();
            var org = orgs.Where(x => x.Name == Org).FirstOrDefault();
            if (org != null)
            {
                organization = org;
                return;
            }

            organization = await orgApi.CreateOrganizationAsync(Org);
        }

        private async Task ExistOrCreateBucketAsync()
        {

            var bucketApi = client.GetBucketsApi();
            var bucket = await bucketApi.FindBucketByNameAsync(Bucket);
            if (bucket != null)
            {
                this.bucket = bucket;
                return;
            }

            this.bucket = await bucketApi.CreateBucketAsync(Bucket, organization);
        } 
        #endregion
    }
}

using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Linq;
using MicroHeart.InfluxDB.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroHeart.InfluxDB.Repository
{
    public partial class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private Bucket bucket;

        private Organization organization;

        private readonly InfluxDBClient client;

        private readonly InfluxDBOptions options;

        public virtual WritePrecision WritePrecision { get; set; } = WritePrecision.Ns;

        public virtual string Bucket { get; set; }

        public virtual string Org { get; set; }

        public QueryableOptimizerSettings queryableOptimizerSettings { get; set; }

        public BaseRepository(InfluxDBClient influxDBClient, IOptions<InfluxDBOptions> options)
        {
            if (influxDBClient == null)
                throw new ArgumentNullException(nameof(influxDBClient));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            this.client = influxDBClient;
            this.options = options.Value;

            this.Bucket = this.options.Bucket;
            this.Org = this.options.Org;

        }


        public void Insert(T t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));

            ExistOrCreateOrgBucket();

            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurement(t, WritePrecision, Bucket, Org);
            }
        }

        public void InsertMany(List<T> entities)
        {
            if ((entities?.Count ?? 0) == 0)
                throw new ArgumentNullException(nameof(entities));

            ExistOrCreateOrgBucket();
            using (var writeApi = client.GetWriteApi())
            {
                writeApi.WriteMeasurements<T>(entities, WritePrecision, Bucket, Org);
            }
        }

        public InfluxDBQueryable<T> Query()
        {
            ExistOrCreateOrgBucket();

            var queryApi =  client.GetQueryApiSync();
            return InfluxDBQueryable<T>.Queryable(Bucket, Org, queryApi, queryableOptimizerSettings);
        }

        #region 私有方法
        private void ExistOrCreateOrgBucket()
        {
            if (organization == null)
                ExistOrCreateOrg();
            if (bucket == null)
                ExistOrCreateBucket();
        }

        private void ExistOrCreateOrg()
        {
            lock (Org)
            {
                var orgApi = client.GetOrganizationsApi();
                var orgs = orgApi.FindOrganizationsAsync().Result;
                var org = orgs.Where(x => x.Name == Org).FirstOrDefault();
                if (org != null)
                {
                    organization = org;
                    return;
                }

                organization = orgApi.CreateOrganizationAsync(Org).Result;
            }

        }

        private void ExistOrCreateBucket()
        {
            lock (Org)
            {
                var bucketApi = client.GetBucketsApi();
                var bucket = bucketApi.FindBucketByNameAsync(Bucket).Result;
                if (bucket != null)
                {
                    this.bucket = bucket;
                    return;
                }


                this.bucket = bucketApi.CreateBucketAsync(Bucket, organization).Result;
            }
        }
        #endregion
    }
}

using InfluxDB.Client.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using static InfluxDB.Client.InfluxDBClientOptions;

namespace MicroHeart.InfluxDB.Options
{
    public class InfluxDBOptions
    {
        public string Url { get; set; }

        public TimeSpan Timeout { get; set; }

        public LogLevel LogLevel { get; set; }

        public string Token { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Org { get; set; }

        public string Bucket { get; set; }
    }
}

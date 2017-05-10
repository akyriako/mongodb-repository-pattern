using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    public class MongoConnectionOptions
    {
        public MongoConnectionOptions()
        {

        }

        public MongoConnectionOptions(string key, string database, string connectionString)
        {
            Key = key;
            Database = database;
            ConnectionString = connectionString;
        }

        public string Key { get; set; }
        public string Database { get; set; }
        public string ConnectionString { get; set; }
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    public class MongoEntity : IEntity
    {
        [BsonId]
        public string Id { get; set; }

        [DisplayName("Modified")]
        public DateTime LastModified { get; set; }

        [DisplayName("Created")]
        public DateTime Created { get; set; }

    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoDb.Repository
{
    public interface IEntity
    {
        [BsonId]
        string Id { get; set; }
        DateTime LastModified { get; set; }
        DateTime Created { get; set; }
    }
}

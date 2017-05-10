using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    public class MongoEntityNotFoundException : Exception
    {
        

        public MongoEntityNotFoundException(string message, Exception innerException) :  base(message, innerException)
        {
           
        }

        public MongoEntityNotFoundException(string message) : this(message, null)
        {

        }
    }

    public class MongoEntityUniqueIndexException : Exception
    {


        public MongoEntityUniqueIndexException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public MongoEntityUniqueIndexException(string message) : this(message, null)
        {

        }
    }
}

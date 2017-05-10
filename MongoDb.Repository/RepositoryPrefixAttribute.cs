using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class RepositoryAttribute : Attribute
    {
        readonly string m_Prefix;

        public RepositoryAttribute(string prefix)
        {
            this.m_Prefix = prefix;
        }

        public string Prefix
        {
            get { return m_Prefix; }
        }

        //// This is a named argument
        //public int NamedInt { get; set; }
    }
}

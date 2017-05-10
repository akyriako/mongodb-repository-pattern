using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDb.Repository
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MongoEntityDisplayNameAttribute : Attribute
    {
        readonly string m_DisplayName;

        public MongoEntityDisplayNameAttribute(string displayName)
        {
            this.m_DisplayName = displayName;
        }

        public string Value
        {
            get { return m_DisplayName; }
        } 
    }  
}

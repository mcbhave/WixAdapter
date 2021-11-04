using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WixAdapter.Db
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CasesCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string MessagesCollectionName { get; set; }
      
    }

    public interface IDatabaseSettings
    {
        public string CasesCollectionName { get; set; }
        string MessagesCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }    
    }
}

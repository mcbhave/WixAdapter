using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WixAdapter.Db;
using WixAdapter.Models;

namespace WixAdapter.Services
{
    public class WixServices
    {
        IDatabaseSettings _settings;
        private MongoClient _dBClient;
        private IMongoCollection<WixCase> _casecollection;
        private IMongoDatabase _masterdb;
        public WixServices(IDatabaseSettings settings)
        {
            try
            {
                _settings = settings;
                _dBClient = new MongoClient(settings.ConnectionString);
                _masterdb = _dBClient.GetDatabase(settings.DatabaseName);
                _casecollection = _masterdb.GetCollection<WixCase>(_settings.CasesCollectionName);
            }
            catch { throw; }
        }
        public List<WixCaseType> Searchcasetypes(string sfilter, bool bLocal)
        {
            WixCaseType colC = new WixCaseType();
            colC.Casetype = "car";
            colC.Casestypedesc = "cars";
            colC.Fields = new List<WixCasetypefield>();
            WixCasetypefield ofld = new WixCasetypefield();
            ofld.Fieldid = "1";
            ofld.Fieldname = "Name";
            ofld.Type = "text";
            colC.Fields.Add(ofld);
            //keep adding more fields
            List<WixCaseType> oretcase = new List<WixCaseType>();
            oretcase.Add(colC);

            return oretcase;

        }
        public WixCase Get(string id)
        {
            WixCase ocase = _casecollection.Find<WixCase>(c => c._id == id).FirstOrDefault();
            return ocase;
        }
        public string Create(WixCase ocase)
        {
            _casecollection.InsertOne(ocase);
            return ocase._id;
        }
        public void Update(string id, WixCase CaseIn)
        {
                _casecollection.ReplaceOne(ocase => ocase._id == id, CaseIn); ;
        }
        public void Remove(string id)
        {
           _casecollection.DeleteOne(book => book._id == id);
        }
        public Message SetMessage(Message oms)
        {

            MessageService omesssrv = new MessageService(_settings, _masterdb);
            oms = omesssrv.Create(oms);
            return oms;
        }
        public List<WixCase> Searchcasesbytype(string casetype)
        {
           List<WixCase> ocases = _casecollection.Find<WixCase>(c => c.casetype == casetype).ToList();

            return ocases;
        }
    }
}

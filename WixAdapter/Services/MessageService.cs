using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WixAdapter.Db;
using WixAdapter.Models;

namespace WixAdapter.Services
{
    public class MessageService
    {
        private readonly IMongoCollection<Message> _message;
        private readonly IMongoCollection<Message> _messagemaster;
        public IMongoDatabase _database;

        public MessageService(IDatabaseSettings settings ,IMongoDatabase _Database)
        {
            if (_Database == null) { throw new Exception("Unable to connect to the tenant database."); }
            //var client = new MongoClient(settings.ConnectionString);
            _database = _Database;
            try
            {
                _message = _Database.GetCollection<Message>("Messages");
                _messagemaster = _Database.GetCollection<Message>("Logs");
            }
            catch
            {
                throw new Exception("Unable to connect to the database.");
            }

        }
        public Message Create(Message omess)
        {
            try
            {
             
                _message.InsertOneAsync(omess);
                return omess;
            }
            catch
            {
                throw;
            }

        }
    }

}

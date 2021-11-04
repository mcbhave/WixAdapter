using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WixAdapter.Models
{
    public class Message
    {
        public Message()
        {
            Messagedate = DateTime.UtcNow.ToString();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string Callertype { get; set; }
        public string Callerid { get; set; }
        public string Messageype { get; set; }
        public string Messagecode { get; set; }
        public string Callerrequest { get; set; }
        public string Callresponse { get; set; }
        public string Headerrequest { get; set; }
        public string Callerrequesttype { get; set; }
        public string MessageDesc { get; set; }
        public string Userid { get; set; }
        public string Messagedate { get; set; }
    }
    public static class ICallerType
    {
        public const string CASE = "WIX";
    }

}

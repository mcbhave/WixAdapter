using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WixAdapter.Models
{
    public class Case0
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }

   
        public int Casenumber { get; set; }
        public string Casetitle { get; set; }
        public string Casetype { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixAdapter.Models;

namespace WixAdapter.Authentication
{
    public class BasicAuthWix : Attribute, IAuthorizationFilter
    {
        private readonly string _realm = string.Empty;
        public BasicAuthWix(string realm)
        {
            _realm = realm;
            if (string.IsNullOrWhiteSpace(_realm))
            {
                throw new ArgumentNullException(nameof(realm), @"Please provide a non-empty realm value.");
            }
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool allpass = true;
            Message omess = new Message();
            omess.Callertype = "Auth";
            omess.Messagecode = "wix";
            IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            string dbconn = configuration.GetSection("DatabaseSettings").GetSection("ConnectionString").Value;
            if (dbconn == null || dbconn == "") { allpass = false; omess.Messageype = "Unauthorized"; omess.Messagecode = "00000"; omess.Callertype = "DB"; }

            string sDatabaseName = configuration.GetSection("DatabaseSettings").GetSection("DatabaseName").Value;
            if (sDatabaseName == null || sDatabaseName == "") { allpass = false; omess.Messageype = "Unauthorized"; omess.Messagecode = "00001"; omess.Callertype = "DB"; }

            string sSecretKey = configuration.GetSection("AppConfig").GetSection("SecretKey").Value;
            string sSecretKeyValue = configuration.GetSection("AppConfig").GetSection("SecretKeyValue").Value;

            //USE THIS IF YOU ARE PLANNING TO CALL IT VIA JAVA SCRIPT, OR WHEN WIX SUPPORTS HEADERS FROM EXTERNAL DATABASE
            //string sActualKeyvalue = context.HttpContext.Request.Headers[sSecretKey];
            //if (sActualKeyvalue != sSecretKeyValue)
            //{
            //    allpass = false; omess.Messageype = "Unauthorized"; omess.Messagecode = "00002"; omess.Callertype = "Headers";
            //}

            MongoClient _client;
            IMongoDatabase MBADDatabase;
            IMongoCollection<Message> _messages;
         
            _client = new MongoClient(dbconn);
            MBADDatabase = _client.GetDatabase(sDatabaseName);
            _messages = MBADDatabase.GetCollection<Message>("WIXlogins");

            try
            {
               

                //this is to check what else are you getting from wix
                string sheaders = Newtonsoft.Json.JsonConvert.SerializeObject(context.HttpContext.Request.Headers);
                omess.Headerrequest = sheaders;
                _messages.InsertOneAsync(omess);

                if (allpass)
                {
                    //reject any other host, update appsettings AllowedHosts list with ; seprated. Works with .net core 3.1
                    //additionally you can also write code here. Unfortunatly wix external database wont let you pass headers but
                    //if you use java script this will come in handy
                   //e.g string xhost = context.HttpContext.Request.Headers["Host"];

                }

                if (allpass) { return; }

                ReturnUnauthorizedResult(context,omess);
            }
            catch (FormatException e)
            {
                omess.MessageDesc = "Unabel to validate user" + e.ToString();
                _messages.InsertOneAsync(omess);
                ReturnUnauthorizedResult(context, omess);
            }
        }
        private void ReturnUnauthorizedResult(AuthorizationFilterContext context)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.Result = new UnauthorizedResult();
        }
        private void ReturnUnauthorizedResult(AuthorizationFilterContext context, Message message)
        {
            // Return 401 and a basic authentication challenge (causes browser to show login dialog)
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
            context.HttpContext.Response.Headers["Content-type"] = "application/json";
            Errorresp errmess = new Errorresp();
            errmess.type = message.Callertype;
            errmess.title = message.Messageype;
            errmess.status = message.Messagecode;
            errmess.traceId = message._id;

            var bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(errmess));

            context.HttpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            context.Result = new UnauthorizedResult();

        }
        //public class Settings
        //{
        //    public string Yauthtenantname { get; set; }
        //}

        //public class RequestContext
        //{
        //    public Settings settings { get; set; }
        //    public string instanceId { get; set; }
        //    public string installationId { get; set; }
        //    public string memberId { get; set; }
        //    public string role { get; set; }
        //}

        //public class wixreqC
        //{
        //    public RequestContext requestContext { get; set; }
        //}
    }
}

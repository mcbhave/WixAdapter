using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using WixAdapter.Authentication;
using WixAdapter.Models;
using WixAdapter.Services;
using static WixAdapter.Models.WixDB;

namespace WixAdapter.Controllers
{
    [Route("data")]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/data")]
    [BasicAuthWix("wix")]
    public class WixController : ControllerBase
    {
        private readonly WixServices _wixServices;
        public WixController(WixServices wixServices)
        {
            _wixServices = wixServices;
        }
        [Route("insert")]
        [Route("insert/{id?}")]
        [HttpPost]
        public IActionResult data(object oid)
        {
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
                srequest = oid.ToString();
                WixDB.data id = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(oid.ToString());

                WixCase ocase = new WixCase();
                ocase.casetype = id.collectionName;
             
                _wixServices.Create(ocase);
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(ocase);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocase);

            }
            catch (Exception ex)
            {
                _wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString()});
                throw;
            }
            
        }

        [Route("get")]
        [Route("get/{id?}")]
        [HttpPost]
        public IActionResult getitem(object sid)
        {
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
 
                srequest = sid.ToString();
                WixDB.data id = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(sid.ToString());

                DataItem<WixCase> oi = new DataItem<WixCase>();
                string js = Newtonsoft.Json.JsonConvert.SerializeObject(id.item);
                WixCase oc = Newtonsoft.Json.JsonConvert.DeserializeObject<WixCase>(js);


                WixCase owix = _wixServices.Get(oc._id);
                oi.item = owix;

                //LOG OR NOT LOG
                //sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oi);
                //_wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "INFO", MessageDesc = smessage + " " + ex.ToString() });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, oi);

            }
            catch (Exception ex)
            {
                _wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString() });

                throw;
            }
            


        }
        [Route("find")]
        [Route("find/{id?}")]
        [HttpPost]
        public IActionResult finditem(WixDB.data id)
        {
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
              
                string js = Newtonsoft.Json.JsonConvert.SerializeObject(id.item);
                var oitm = Newtonsoft.Json.JsonConvert.DeserializeObject<WixCase>(js);
                DataItem<WixCase> oi = new DataItem<WixCase>();
                List<JObject> o = new List<JObject>();
                List<WixCase> ocases=_wixServices.Searchcasesbytype(id.collectionName);
                if(ocases!=null && ocases.Count > 0)
                {
                    string oitem = Newtonsoft.Json.JsonConvert.SerializeObject(_wixServices.Searchcasesbytype(id.collectionName)[0]);
                    JObject job = JObject.Parse(oitem);
                    o.Add(job);
                }
              
                FindItems<JObject> olistdata = new FindItems<JObject>();
                 olistdata.items = o;
                olistdata.totalCount = o.Count;
                //below two lines are for the log
                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(olistdata);
                var retj = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(sresponse);
                //log or not log
                //_wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "INFO", MessageDesc = smessage + " " + ex.ToString() });

                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, olistdata);

            }
            catch (Exception ex)
            {
                _wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString() });
                throw;
            }
        }

        [Route("update")]
        [Route("update/{id?}")]
        [HttpPost]
        public IActionResult updateitem(object id)
        {
          
            string srequest = "";
            string smessage = "";
            string scasetypes = "";
            string sresponse = "";
            try
            {
              
                srequest = Newtonsoft.Json.JsonConvert.SerializeObject(id);
                WixDB.data sid = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(srequest);

                string js = Newtonsoft.Json.JsonConvert.SerializeObject(sid.item);
                WixCase oc = Newtonsoft.Json.JsonConvert.DeserializeObject<WixCase>(js);

                _wixServices.Update(oc._id, oc);

                sresponse = Newtonsoft.Json.JsonConvert.SerializeObject(oc);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, oc);

            }
            catch (Exception ex)
            {
                _wixServices.SetMessage(new Message() { Callerid = "Wix", Callerrequest = srequest, Callresponse = sresponse, Callerrequesttype = scasetypes, Callertype = "Wix data", Messageype = "ERROR", MessageDesc = smessage + " " + ex.ToString() });

                throw;
            }
            
        }

        [Route("remove")]
        [Route("remove/{id?}")]
        [HttpPost]
        public IActionResult removeitem(object id)
        {
            string srequest = "";
            srequest = Newtonsoft.Json.JsonConvert.SerializeObject(id);
            WixDB.data sid = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(srequest);

          
            _wixServices.Remove(sid.itemId);

            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, "");
        }

        [Route("count")]
        [Route("count/{id?}")]
        [HttpPost]
        public IActionResult countitem(object id)
        {
            string srequest = "";
            srequest = Newtonsoft.Json.JsonConvert.SerializeObject(id);
            WixDB.data sid = Newtonsoft.Json.JsonConvert.DeserializeObject<WixDB.data>(srequest);
 

            DataCount ocount = new DataCount();
            ocount.totalCount = _wixServices.Searchcasesbytype(sid.collectionName).Count;


            return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status200OK, ocount);
        }

    }
}
    
 
 

using System;
using System.Collections.Generic;
using System.Linq;
using ProvisioningLibrary;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ScampApi.Controllers.Controllers
{
    [Route("api/user/resources/{resourceId}/")]
    public class UserResourcesController : Controller
    {

        IWebJobController _webJobController;

        public UserResourcesController(IConfiguration config, IWebJobController webJobController)
        {
            _webJobController = webJobController;
        }


        // POST /api/user/resources/{resourceId}/{actionname}
        [HttpPost("{actionname}")]
        public void Post(string resourceId, string actionname)
        {
            _webJobController.SubmitActionInQueue(resourceId, actionname);
        }

        // GET /api/user/resources/{resourceId}/connect
        [HttpGet("connect")]
        public string Get(int resourceId)
        {
            //return the RDP connection string
            return "rdpconnection 1";
        }


    }
}

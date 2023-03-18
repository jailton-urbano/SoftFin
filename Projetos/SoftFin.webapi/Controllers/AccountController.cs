using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SoftFin.webapi.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        /// <summary>
        /// Register a new user on application
        /// </summary>
        /// <param name="user">New user to register</param>
        /// <remarks>Adds new user to application and grant access</remarks>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal Server Error</response>
        [AllowAnonymous]
        [Route("Register2")]
        [ResponseType(typeof(RegisterBindingModel))]
        [HttpPost]
        public async Task<IHttpActionResult> Register2(RegisterBindingModel user)
        {
            //IMPLEMENTATION OF CODE GOES HERE!!
            throw new NotImplementedException();
        }
    }
   
}

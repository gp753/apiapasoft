using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using api2.Models;

namespace api2.Controllers
{
    public class wPusersController : ApiController
    {
        private apaEntities db = new apaEntities();

      

     
        /// <summary>
        /// Devuelve el mail de los usuarios wp
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/wpusers/{name}")]
        public IHttpActionResult Get(string name)
        {

            var data = from gk1hyugy_users in db.gk1hyugy_users
                       where gk1hyugy_users.user_login == name
                       select gk1hyugy_users.user_email;

            if (data.ToList().Count() == 0)
            {

                return NotFound();
            }

            return Ok(data);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool gk1hyugy_usersExists(decimal id)
        {
            return db.gk1hyugy_users.Count(e => e.ID == id) > 0;
        }
    }
}
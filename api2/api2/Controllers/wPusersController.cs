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
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;

namespace api2.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    public class wPusersController : ApiController
    {
        private apaEntities db = new apaEntities();
        private apausrEntities3 db2 = new apausrEntities3(); //bd de los usuarios



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

        [Authorize]
        [Route("api/wuserid")]
        public IHttpActionResult Get()
        {
            string id_usr = User.Identity.GetUserId();

            var user__mail = from Users in db2.Users
                           where Users.Id == id_usr
                           select Users.Email;
            string aux = user__mail.FirstOrDefault();

            var data = from gk1hyugy_users in db.gk1hyugy_users
                       where gk1hyugy_users.user_email == aux
                       select gk1hyugy_users.ID;

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
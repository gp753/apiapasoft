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
    public class SOCIOsController : ApiController
    {
        private apiapaEntities db = new apiapaEntities();
        private apausrEntities3 db2 = new apausrEntities3();
        // GET: api/SOCIOs/5
        /// <summary>
        /// Devuelve los datos del socio con su id
        /// </summary>
        /// <param name="id"></param>Se le pasa el id de socio
        /// <returns></returns>
        /// 
        [Authorize]
        [ResponseType(typeof(SOCIO))]
        public IHttpActionResult GetSOCIO()
        {
            string id_usr = User.Identity.GetUserId();
            var cedula = from Users in db2.Users
                         where Users.Id == id_usr
                         select Users.Cedula;
            string cedula2 = cedula.FirstOrDefault();

            var id_var = from SOCIO in db.SOCIO
                     where SOCIO.CEDULA == cedula2
                     select SOCIO.ID_SOCIO;
            string id = id_var.FirstOrDefault();

            SOCIO sOCIO = db.SOCIO.Find(id);
            if (sOCIO == null)
            {
               return NotFound();
            }

            return Ok(new { sOCIO.CEDULA, sOCIO.NOMBRES, sOCIO.APELLIDOS });
            
        }

        /// <summary>
        /// Devuelve todos los datos del socio a partir de la cedula
        /// </summary>
        /// <param name="cedula"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("api/socios/ci/{cedula}")]
        public IHttpActionResult Get(string cedula)
        {
            var data = from SOCIO in db.SOCIO
                       where SOCIO.CEDULA == cedula
                       select SOCIO;
            if (data.ToList().Count() == 0)
            {

                return NotFound();
            }

            return Ok(data);
        }

        /*
        // PUT: api/SOCIOs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSOCIO(string id, SOCIO sOCIO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sOCIO.ID_SOCIO)
            {
                return BadRequest();
            }

            db.Entry(sOCIO).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SOCIOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SOCIOs
        [ResponseType(typeof(SOCIO))]
        public IHttpActionResult PostSOCIO(SOCIO sOCIO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SOCIO.Add(sOCIO);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (SOCIOExists(sOCIO.ID_SOCIO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = sOCIO.ID_SOCIO }, sOCIO);
        }

        // DELETE: api/SOCIOs/5
        [ResponseType(typeof(SOCIO))]
        public IHttpActionResult DeleteSOCIO(string id)
        {
            SOCIO sOCIO = db.SOCIO.Find(id);
            if (sOCIO == null)
            {
                return NotFound();
            }

            db.SOCIO.Remove(sOCIO);
            db.SaveChanges();

            return Ok(sOCIO);
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SOCIOExists(string id)
        {
            return db.SOCIO.Count(e => e.ID_SOCIO == id) > 0;
        }
    }
}
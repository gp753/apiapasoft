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
    public class SOCIOs2Controller : ApiController
    {
        private apiapaEntities5 db = new apiapaEntities5();
        private apausrEntities3 db2 = new apausrEntities3();

       
        // GET: api/SOCIOs2/5
        [Authorize]
        [ResponseType(typeof(SOCIO))]
        public IHttpActionResult GetSOCIO()
        {
            string id_usr = User.Identity.GetUserId();
            var id_socio = from Users in db2.Users
                         where Users.Id == id_usr
                         select Users.id_socio;
            string id_socio2 = id_socio.FirstOrDefault();

            SOCIO sOCIO = db.SOCIO.Find(id_socio2);
            if (sOCIO == null)
            {
                return NotFound();
            }

            return Ok(new { sOCIO.CEDULA, sOCIO.NOMBRES, sOCIO.APELLIDOS,sOCIO.DIRECCION, sOCIO.TELEFONO_FIJO,sOCIO.TELEFONO_CELULAR,sOCIO.NRO_SOCIO,sOCIO.FECHA_NACIMIENTO,sOCIO.FECHA_ALTA });
        }
        /*
        // PUT: api/SOCIOs2/5
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

        // POST: api/SOCIOs2
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

        // DELETE: api/SOCIOs2/5
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
        }
        */
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
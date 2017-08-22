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
using Microsoft.AspNet.Identity;

namespace api2.Controllers
{
    public class BENEFICIARIOsController : ApiController
    {
        private BeneficiariosEntity db = new BeneficiariosEntity();
        private apausrEntities3 db2 = new apausrEntities3();
        private apiapaEntities5 db3 = new apiapaEntities5();

        // GET: api/BENEFICIARIOs
        public IQueryable<BENEFICIARIO> GetBENEFICIARIO()
        {
            return db.BENEFICIARIO;
        }

        [Authorize]
        [ResponseType(typeof(BENEFICIARIO))]
        public IHttpActionResult GetBENEFICIARIO(string id)
        {
            string id_usr = User.Identity.GetUserId();
            var id_socio = from Users in db2.Users
                           where Users.Id == id_usr
                           select Users.id_socio;
            string id_socio2 = id_socio.FirstOrDefault();

            SOCIO sOCIO = db3.SOCIO.Find(id_socio2);
            if (sOCIO == null)
            {
                return NotFound();
            }

            return Ok(new { sOCIO.CEDULA, sOCIO.NOMBRES, sOCIO.APELLIDOS, sOCIO.DIRECCION, sOCIO.TELEFONO_FIJO, sOCIO.TELEFONO_CELULAR, sOCIO.NRO_SOCIO, sOCIO.FECHA_NACIMIENTO, sOCIO.FECHA_ALTA });
        }
        // GET: api/BENEFICIARIOs/5
       /* [ResponseType(typeof(BENEFICIARIO))]
        public IHttpActionResult GetBENEFICIARIO(string id)
        {
            BENEFICIARIO bENEFICIARIO = db.BENEFICIARIO.Find(id);
            if (bENEFICIARIO == null)
            {
                return NotFound();
            }

            return Ok(bENEFICIARIO);
        }
        */
        // PUT: api/BENEFICIARIOs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBENEFICIARIO(string id, BENEFICIARIO bENEFICIARIO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bENEFICIARIO.ID_BENEFICIARIO)
            {
                return BadRequest();
            }

            db.Entry(bENEFICIARIO).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BENEFICIARIOExists(id))
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

        // POST: api/BENEFICIARIOs
        [ResponseType(typeof(BENEFICIARIO))]
        public IHttpActionResult PostBENEFICIARIO(BENEFICIARIO bENEFICIARIO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BENEFICIARIO.Add(bENEFICIARIO);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (BENEFICIARIOExists(bENEFICIARIO.ID_BENEFICIARIO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = bENEFICIARIO.ID_BENEFICIARIO }, bENEFICIARIO);
        }

        // DELETE: api/BENEFICIARIOs/5
        [ResponseType(typeof(BENEFICIARIO))]
        public IHttpActionResult DeleteBENEFICIARIO(string id)
        {
            BENEFICIARIO bENEFICIARIO = db.BENEFICIARIO.Find(id);
            if (bENEFICIARIO == null)
            {
                return NotFound();
            }

            db.BENEFICIARIO.Remove(bENEFICIARIO);
            db.SaveChanges();

            return Ok(bENEFICIARIO);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BENEFICIARIOExists(string id)
        {
            return db.BENEFICIARIO.Count(e => e.ID_BENEFICIARIO == id) > 0;
        }
    }
}
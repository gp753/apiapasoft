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
    public class SOCIOsController : ApiController
    {
        private apiapaEntities db = new apiapaEntities();

        // GET: api/SOCIOs
        public IQueryable<SOCIO> GetSOCIO()
        {
            return db.SOCIO;
        }

        // GET: api/SOCIOs/5
        [ResponseType(typeof(SOCIO))]
        public IHttpActionResult GetSOCIO(string id)
        {
            SOCIO sOCIO = db.SOCIO.Find(id);
            if (sOCIO == null)
            {
                return NotFound();
            }

            return Ok(sOCIO);
        }

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
        }

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
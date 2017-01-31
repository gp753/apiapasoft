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
    public class RUBROsController : ApiController
    {
        private RubroEntities db = new RubroEntities();

        // GET: api/RUBROs
        public IQueryable<RUBRO> GetRUBRO()
        {
            return db.RUBRO;
        }

        // GET: api/RUBROs/5
        [ResponseType(typeof(CUENTA))]
        public IHttpActionResult GetCUENTA(string id)
        {

            var datos = from CUENTA in db.CUENTA
                        join RUBRO in db.RUBRO on
                        CUENTA.ID_RUBRO equals
                        RUBRO.ID_RUBRO
                        where CUENTA.ID_SOCIO == id && CUENTA.LIQUIDADO == 1
                        select new { CUENTA.ID_RUBRO, RUBRO.DESCRIPCION };
            //select CUENTA.ID_RUBRO;



            if (datos == null)
            {

                return NotFound();
            }

            var distinto = datos.Distinct();
            // var datos = mysql_query();


            //            var datos = select CUENTA.ID_RUBRO from CUENTA;

            return Ok(distinto);



            //return Ok(cUENTA);




            /*
            CUENTA cUENTA = db.CUENTA.Find(id);
            if (cUENTA == null)
            {
                return NotFound();
            }

            return Ok(cUENTA);
            */
        }

        // PUT: api/RUBROs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRUBRO(string id, RUBRO rUBRO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rUBRO.ID_RUBRO)
            {
                return BadRequest();
            }

            db.Entry(rUBRO).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RUBROExists(id))
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

        // POST: api/RUBROs
        [ResponseType(typeof(RUBRO))]
        public IHttpActionResult PostRUBRO(RUBRO rUBRO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RUBRO.Add(rUBRO);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RUBROExists(rUBRO.ID_RUBRO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = rUBRO.ID_RUBRO }, rUBRO);
        }

        // DELETE: api/RUBROs/5
        [ResponseType(typeof(RUBRO))]
        public IHttpActionResult DeleteRUBRO(string id)
        {
            RUBRO rUBRO = db.RUBRO.Find(id);
            if (rUBRO == null)
            {
                return NotFound();
            }

            db.RUBRO.Remove(rUBRO);
            db.SaveChanges();

            return Ok(rUBRO);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RUBROExists(string id)
        {
            return db.RUBRO.Count(e => e.ID_RUBRO == id) > 0;
        }
    }
}
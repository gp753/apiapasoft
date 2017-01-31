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
    public class CUENTAsController : ApiController
    {
        private apiapaEntities3 db = new apiapaEntities3();

        // GET: api/CUENTAs
        public IQueryable<CUENTA> GetCUENTA()
        {
            return db.CUENTA;
        }

        // GET: api/CUENTAs/5
        /*  [ResponseType(typeof(CUENTA))]
          public IHttpActionResult GetCUENTA(string id,string id2,string id3)
          {
              CUENTA cUENTA = db.CUENTA.Find(id);
              if (cUENTA == null)
              {
                  return NotFound();
              }

              return Ok(cUENTA);
          }*/


        [Route("api/cuentas/{id_s}/{cod_periodo}/{id_rubro}")]
        public IHttpActionResult Get(string id_s, string cod_periodo,string id_rubro)
        {
            var data = from CUENTA in db.CUENTA
                       where (CUENTA.ID_SOCIO == id_s && CUENTA.CODIGO_PERIODO == cod_periodo) && (CUENTA.ID_RUBRO == id_rubro && CUENTA.LIQUIDADO ==1)
                       select new { CUENTA.FECHA, CUENTA.IMPORTE };

            return Ok(data);
        }

        // PUT: api/CUENTAs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCUENTA(string id, CUENTA cUENTA)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cUENTA.ID_MOVIMIENTO)
            {
                return BadRequest();
            }

            db.Entry(cUENTA).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CUENTAExists(id))
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

        // POST: api/CUENTAs
        [ResponseType(typeof(CUENTA))]
        public IHttpActionResult PostCUENTA(CUENTA cUENTA)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CUENTA.Add(cUENTA);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CUENTAExists(cUENTA.ID_MOVIMIENTO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = cUENTA.ID_MOVIMIENTO }, cUENTA);
        }

        // DELETE: api/CUENTAs/5
        [ResponseType(typeof(CUENTA))]
        public IHttpActionResult DeleteCUENTA(string id)
        {
            CUENTA cUENTA = db.CUENTA.Find(id);
            if (cUENTA == null)
            {
                return NotFound();
            }

            db.CUENTA.Remove(cUENTA);
            db.SaveChanges();

            return Ok(cUENTA);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CUENTAExists(string id)
        {
            return db.CUENTA.Count(e => e.ID_MOVIMIENTO == id) > 0;
        }
    }
}
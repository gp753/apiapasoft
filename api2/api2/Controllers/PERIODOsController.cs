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
    public class PERIODOsController : ApiController
    {
        private apiapaEntities2 db = new apiapaEntities2();

        // GET: api/PERIODOs
        public IQueryable<PERIODO> GetPERIODO()
        {
            return db.PERIODO;
        }

        // GET: api/PERIODOs/5
        [ResponseType(typeof(PERIODO))]
        public IHttpActionResult GetPERIODO(string id)
        {
            //CUENTA cUENTA = db.CUENTA.Find(id);

            // var datos = from CUENTA in db.CUENTA where CUENTA.ID_SOCIO == id
            //             select new { fecha = CUENTA.FECHA, importe = CUENTA.IMPORTE} ;
            var datos = from PERIODO in db.PERIODO
                        join CUENTA in db.CUENTA on PERIODO.CODIGO_PERIODO equals CUENTA.CODIGO_PERIODO
                        where CUENTA.ID_SOCIO == id
                        select  new { PERIODO };
            

 
            
            if (datos == null)
            {
 
              return NotFound();
            }
            var distintos = datos.Distinct();
            return Ok(distintos);
        }

        // PUT: api/PERIODOs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPERIODO(string id, PERIODO pERIODO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pERIODO.CODIGO_PERIODO)
            {
                return BadRequest();
            }

            db.Entry(pERIODO).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PERIODOExists(id))
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

        // POST: api/PERIODOs
        [ResponseType(typeof(PERIODO))]
        public IHttpActionResult PostPERIODO(PERIODO pERIODO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PERIODO.Add(pERIODO);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PERIODOExists(pERIODO.CODIGO_PERIODO))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = pERIODO.CODIGO_PERIODO }, pERIODO);
        }

        // DELETE: api/PERIODOs/5
        [ResponseType(typeof(PERIODO))]
        public IHttpActionResult DeletePERIODO(string id)
        {
            PERIODO pERIODO = db.PERIODO.Find(id);
            if (pERIODO == null)
            {
                return NotFound();
            }

            db.PERIODO.Remove(pERIODO);
            db.SaveChanges();

            return Ok(pERIODO);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PERIODOExists(string id)
        {
            return db.PERIODO.Count(e => e.CODIGO_PERIODO == id) > 0;
        }
    }
}
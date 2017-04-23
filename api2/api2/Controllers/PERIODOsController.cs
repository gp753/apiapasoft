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
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PERIODOsController : ApiController
    {
        private apiapaEntities2 db = new apiapaEntities2();
        private apiapaEntities db3 = new apiapaEntities();
        private apausrEntities3 db2 = new apausrEntities3();

        // GET: api/PERIODOs/5
        /// <summary>
        /// Devuelve todos los periodos que tiene el socio a partir de su id para la lista desplegable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [ResponseType(typeof(PERIODO))]
        public IHttpActionResult GetPERIODO()
        {
            //CUENTA cUENTA = db.CUENTA.Find(id);

            // var datos = from CUENTA in db.CUENTA where CUENTA.ID_SOCIO == id
            //             select new { fecha = CUENTA.FECHA, importe = CUENTA.IMPORTE} ;

            string id_usr = User.Identity.GetUserId();
            var cedula = from Users in db2.Users
                         where Users.Id == id_usr
                         select Users.Cedula;
            string cedula2 = cedula.FirstOrDefault();

            var id_var = from SOCIO in db3.SOCIO
                         where SOCIO.CEDULA == cedula2
                         select SOCIO.ID_SOCIO;
            string id = id_var.FirstOrDefault();
            var datos = from PERIODO in db.PERIODO
                        join CUENTA in db.CUENTA on PERIODO.CODIGO_PERIODO equals CUENTA.CODIGO_PERIODO
                        where CUENTA.ID_SOCIO == id && CUENTA.LIQUIDADO == 1
                        select  new { PERIODO };

            if (datos.ToList().Count() == 0)
            {
 
              return NotFound();
            }
            var distintos = datos.Distinct();
            return Ok(distintos);
        }
/*
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
        */
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
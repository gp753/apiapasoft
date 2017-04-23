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
    public class RUBROsController : ApiController
    {
        private RubroEntities db = new RubroEntities();
        private apausrEntities3 db2 = new apausrEntities3();
        private apiapaEntities db3 = new apiapaEntities();

        // GET: api/RUBROs/5
        /// <summary>
        /// Devuelve el id_rubro y la descripcion del rubro para hacer la lista desplegable de los rubros que tiene un socio y recibe el id de socio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [Authorize]
        [ResponseType(typeof(CUENTA))]
        public IHttpActionResult GetCUENTA()
        {
            string id_usr = User.Identity.GetUserId();
            var cedula = from Users in db2.Users
                         where Users.Id == id_usr
                         select Users.Cedula;
            string cedula2 = cedula.FirstOrDefault();

            var id_var = from SOCIO in db3.SOCIO
                         where SOCIO.CEDULA == cedula2
                         select SOCIO.ID_SOCIO;
            string id = id_var.FirstOrDefault();

            var datos = from CUENTA in db.CUENTA
                        join RUBRO in db.RUBRO on
                        CUENTA.ID_RUBRO equals
                        RUBRO.ID_RUBRO
                        where CUENTA.ID_SOCIO == id && CUENTA.LIQUIDADO == 1
                        select new { CUENTA.ID_RUBRO, RUBRO.DESCRIPCION };
            //select CUENTA.ID_RUBRO;



            if (datos.ToList().Count() == 0)
            {

                return NotFound();
            }

            var distinto = datos.Distinct();
            // var datos = mysql_query();


            //            var datos = select CUENTA.ID_RUBRO from CUENTA;

            return Ok(distinto);



          
        }
        /*
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
        */
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
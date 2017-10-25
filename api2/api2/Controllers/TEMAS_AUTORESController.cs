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
    public class TEMAS_AUTORESController : ApiController
    {
        private temas_beneficiarios_Entities db = new temas_beneficiarios_Entities();
        private apausrEntities3 db2 = new apausrEntities3();

        // GET: api/TEMAS_AUTORES
        public IQueryable<TEMAS_AUTORES> GetTEMAS_AUTORES()
        {
            return db.TEMAS_AUTORES;
        }

        [Authorize]
        [Route("api/temas_autor")]
        public IHttpActionResult Get()
        {
            string id_usr = User.Identity.GetUserId();

            var id_socio = from Users in db2.Users
                             where Users.Id == id_usr
                             select Users.id_socio;
            //string id_socio = "2";
            string id = id_socio.FirstOrDefault();
            var data = from SOCIO in db.SOCIO
                       join BENEFICIARIO in db.BENEFICIARIO on SOCIO.IP equals BENEFICIARIO.CODIGOCAE
                       join TEMAS_AUTORES in db.TEMAS_AUTORES on BENEFICIARIO.ID_BENEFICIARIO equals TEMAS_AUTORES.ID_AUTOR
                       join TEMAS in db.TEMAS on TEMAS_AUTORES.ID_TEMA equals TEMAS.ID_TEMA
                       join ROL in db.ROL on TEMAS_AUTORES.ID_ROL equals ROL.ID_ROL
                       where SOCIO.ID_SOCIO == id
                       select new { TEMAS.ID_TEMA, TEMAS.NOMBRE, ROL.ROL1 , TEMAS_AUTORES.PORCEFONO, TEMAS_AUTORES.PORCEJECU};
            
            if (data.ToList().Count() == 0)
            {

                return NotFound();
            }
            
            return Ok(data);
        }

        [Route("api/autores_tema/{id_tema}")]
        public IHttpActionResult Get(string id_tema)
        {
            
            var data = from BENEFICIARIO in db.BENEFICIARIO
                       join TEMAS_AUTORES in db.TEMAS_AUTORES on BENEFICIARIO.ID_BENEFICIARIO equals TEMAS_AUTORES.ID_AUTOR
                       join TEMAS in db.TEMAS on TEMAS_AUTORES.ID_TEMA equals TEMAS.ID_TEMA
                       join ROL in db.ROL on TEMAS_AUTORES.ID_ROL equals ROL.ID_ROL
                       where TEMAS.ID_TEMA == id_tema
                       select new { BENEFICIARIO.NOMBRES, BENEFICIARIO.APELLIDOS, TEMAS_AUTORES.PORCEFONO, TEMAS_AUTORES.PORCEJECU };

        /*    if (data.ToList().Count() == 0)
            {

                return NotFound();
            }
            */
            return Ok(data);
        }


        // GET: api/TEMAS_AUTORES/5
        [ResponseType(typeof(TEMAS_AUTORES))]
        public IHttpActionResult GetTEMAS_AUTORES(string id)
        {
            TEMAS_AUTORES tEMAS_AUTORES = db.TEMAS_AUTORES.Find(id);
            if (tEMAS_AUTORES == null)
            {
                return NotFound();
            }

            return Ok(tEMAS_AUTORES);
        }

        // PUT: api/TEMAS_AUTORES/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTEMAS_AUTORES(string id, TEMAS_AUTORES tEMAS_AUTORES)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tEMAS_AUTORES.ID_TEMA)
            {
                return BadRequest();
            }

            db.Entry(tEMAS_AUTORES).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TEMAS_AUTORESExists(id))
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

        // POST: api/TEMAS_AUTORES
        [ResponseType(typeof(TEMAS_AUTORES))]
        public IHttpActionResult PostTEMAS_AUTORES(TEMAS_AUTORES tEMAS_AUTORES)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TEMAS_AUTORES.Add(tEMAS_AUTORES);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (TEMAS_AUTORESExists(tEMAS_AUTORES.ID_TEMA))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tEMAS_AUTORES.ID_TEMA }, tEMAS_AUTORES);
        }

        // DELETE: api/TEMAS_AUTORES/5
        [ResponseType(typeof(TEMAS_AUTORES))]
        public IHttpActionResult DeleteTEMAS_AUTORES(string id)
        {
            TEMAS_AUTORES tEMAS_AUTORES = db.TEMAS_AUTORES.Find(id);
            if (tEMAS_AUTORES == null)
            {
                return NotFound();
            }

            db.TEMAS_AUTORES.Remove(tEMAS_AUTORES);
            db.SaveChanges();

            return Ok(tEMAS_AUTORES);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TEMAS_AUTORESExists(string id)
        {
            return db.TEMAS_AUTORES.Count(e => e.ID_TEMA == id) > 0;
        }
    }
}
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

namespace api2.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MOVIMIENTOsController : ApiController
    {
        
        private apiapaEntities4 db = new apiapaEntities4();
        /// <summary>
        /// Devuelve el Periodo, descripcion, fecha, importe y la suma total de los importes recibiendo el socio, el periodo el tipo de rubro y el id_rubro, si en id_rubro mandas todos te devuelve todos
        /// </summary>
        /// <param name="id_s"></param>
        /// <param name="cod_periodo"></param>
        /// <param name="tipo"></param>
        /// <param name="id_rubro"></param>
        /// <returns></returns>
        [Route("api/movimientos/{id_s}/{cod_periodo}/{tipo}/{id_rubro}")]
        public IHttpActionResult Get(string id_s, string cod_periodo,  string tipo,string id_rubro)
        {
            
            if (id_rubro == "todos")
            {
                var data = from CUENTA in db.CUENTA
                           join RUBRO in db.RUBRO on
                           CUENTA.ID_RUBRO equals RUBRO.ID_RUBRO
                           where ((CUENTA.ID_SOCIO == id_s && CUENTA.CODIGO_PERIODO == cod_periodo) && (RUBRO.TIPO == tipo && CUENTA.LIQUIDADO == 1))
                           select new { CUENTA.CODIGO_PERIODO, RUBRO.DESCRIPCION, CUENTA.FECHA, CUENTA.IMPORTE };

                if (data.ToList().Count() == 0)
                {

                    return NotFound();
                }

                var total = data.Sum(a => a.IMPORTE);
                return Ok(new { data, total });
            }
            else
            {
                var data = from CUENTA in db.CUENTA
                           join RUBRO in db.RUBRO on
                           CUENTA.ID_RUBRO equals RUBRO.ID_RUBRO
                           where ((CUENTA.ID_SOCIO == id_s && CUENTA.CODIGO_PERIODO == cod_periodo) && (CUENTA.ID_RUBRO == id_rubro && CUENTA.LIQUIDADO == 1)) && RUBRO.TIPO == tipo
                           select new { CUENTA.CODIGO_PERIODO, RUBRO.DESCRIPCION, CUENTA.FECHA, CUENTA.IMPORTE };

                if (data.ToList().Count() == 0)
                {

                    return NotFound();
                }

                var total = data.Sum(a => a.IMPORTE);
                return Ok(new { data, total });
            }
            
        }
        /// <summary>
        /// Hace lo mismo que el anterior pero es por si no se elige el id_rubro, seria para mostrar sin filtrar por id_rubro
        /// </summary>
        /// <param name="id_s"></param>
        /// <param name="cod_periodo"></param>
        /// <param name="tipo"></param>
        /// <returns></returns>
        [Route("api/movimientos/{id_s}/{cod_periodo}/{tipo}")]
        public IHttpActionResult Get(string id_s, string cod_periodo, string tipo)
        {

            var data = from CUENTA in db.CUENTA
                       join RUBRO in db.RUBRO on
                       CUENTA.ID_RUBRO equals RUBRO.ID_RUBRO
                       where ((CUENTA.ID_SOCIO == id_s && CUENTA.CODIGO_PERIODO == cod_periodo) && (CUENTA.LIQUIDADO == 1)) && RUBRO.TIPO == tipo
                       select new { CUENTA.CODIGO_PERIODO, RUBRO.DESCRIPCION, CUENTA.FECHA, CUENTA.IMPORTE };
            if (data.ToList().Count() == 0)
            {

                return NotFound();
            }
            var total = data.Sum(a => a.IMPORTE);
            return Ok(new { data, total});
        }


        

      /*
        // PUT: api/MOVIMIENTOs/5
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

        // POST: api/MOVIMIENTOs
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

        // DELETE: api/MOVIMIENTOs/5
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
        }*/

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
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace api2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TEMAS_AUTORES
    {
        public string ID_TEMA { get; set; }
        public string ID_AUTOR { get; set; }
        public string ID_ROL { get; set; }
        public long PORCEJECU { get; set; }
        public long PORCEFONO { get; set; }
    
        public virtual ROL ROL { get; set; }
        public virtual TEMAS TEMAS { get; set; }
    }
}

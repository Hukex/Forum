using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ProyectoAspNet.Models
{
    [Table("Proyectos")]
    public class Proyecto
    {
        public int id { get; set; }
        public string nombre { get; set; }
        [AllowHtml]
        public string texto { get; set; }

        public int userId { get; set; }

        public int votosAFavor { get; set; }

        public int votosEnContra { get; set; }

        public Proyecto()
        {
        }
        public Proyecto(string name, string text, int userID)
        {
            nombre = name;
            texto = text;
            userId = userID;
        }
        public String ImagePath
        {
            get
            {
                return "~/dislike.svg";
            }

        }

        public String ImagePath2
        {
            get
            {
                return "~/like.svg";
            }

        }

    }

}
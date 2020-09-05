using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAspNet.Models
{
    [Table("Comentarios")]
    public class Comentario
    {
        public int id { get; set; }
        public string texto { get; set; }
        public string proyId { get; set; }

        public string userId { get; set; }

        public Comentario()
        {
        }
        public Comentario(string text, string proyID, string userID)
        {
            proyId = proyID;
            texto = text;
            userId = userID;
        }

    }
}
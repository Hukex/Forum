using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAspNet.Models
{
    [Table("Votos")]
    public class Voto

    {
        public int id { get; set; }
        public int userId { get; set; }
        public int proyId { get; set; }
        public bool voto { get; set; }

        public Voto(int userID, int proyID, bool v)
        {
            userId = userID;
            proyId = proyID;
            voto = v;
        }
        public Voto()
        {

        }
    }
}
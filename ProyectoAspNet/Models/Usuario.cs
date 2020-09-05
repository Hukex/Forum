using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ProyectoAspNet.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        public int id { get; set; }
        public string usuario { get; set; }
        public string contraseña { get; set; }

        public Usuario()
        {
        }
        public Usuario(string name, string pass)
        {
            usuario = name;
            contraseña = pass;
        }

    }

    public class MyDb : DbContext
    {
        public MyDb()
        {

        }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Proyecto> Proyecto { get; set; }
        public DbSet<Comentario> Comentario { get; set; }
        public DbSet<Voto> Voto { get; set; }
    }
}

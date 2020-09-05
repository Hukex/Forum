using ProyectoAspNet.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;

namespace ProyectoAspNet.Controllers
{
    public class HomeController : Controller
    {
        private MyDb db = new MyDb();

        // INICIO //
        public ActionResult Index()
        {
                //crearMuchosUsuariosEnDbParaPruebas();

            if (Session["login"] != null)
            {

                return RedirectToAction("InternoDB");
            }
            else
            {
                return View();
            }
        }




        [HttpPost]
        public ActionResult Interno(Usuario user)
        {
            var query = (from e in db.Usuario
                         where e.usuario == user.usuario && e.contraseña == user.contraseña
                         select e).FirstOrDefault<Usuario>();

            if (query != null && query.contraseña.Equals(user.contraseña))
            {
                Session["login"] = query.id;
                Session["usuario"] = devolverUsuarioLogueado(); // Para la vista general _Layoutel nombre en boton CS
                return RedirectToAction("InternoDB");
            }
            else
            {
                ViewBag.error = "El usuario o la contraseña no son validos";
                return View("Index");
            }
        }

        // CARGAR TABLA CON PROYECTOS //
        public ActionResult InternoDB()
        {
            if (Session["login"] != null)
            {
                dynamic model = new ExpandoObject();
                var query = (from p in db.Proyecto
                             join u in db.Usuario on p.userId equals u.id
                             orderby p.id descending
                             select new { p, u.usuario });

                List<Proyecto> lista = new List<Proyecto>();
                List<string> usuarios = new List<string>();
                foreach (var j in query)
                {
                    lista.Add(j.p);
                    usuarios.Add(j.usuario);
                }
                model.Proyectos = lista;
                model.Usuarios = usuarios;
                return View(model);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult CrearUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearUsuarioDB(Usuario user)
        {

            var query = (from e in db.Usuario
                         where e.usuario == user.usuario
                         select e).FirstOrDefault<Usuario>();
            if (query != null)
            {
                ViewBag.error = "El usuario ya existe";
                ViewBag.color = "0";
            }
            else
            {
                db.Usuario.Add(user);
                db.SaveChanges();
                ViewBag.error = "Usuario creado con exito";
                ViewBag.color = "1";
                ModelState.Clear();
            }
            return View("CrearUsuario");
        }

        public ActionResult AdministrarUsuarios()
        {

            if (Session["login"] != null && (int)Session["login"] == 1)
            {
                var Usuarios = (from e in db.Usuario
                                orderby e.id
                                select e);
                return View(Usuarios);
            }
            else
            {
                @ViewBag.error = "No tienes permisos necesarios";
                return View("Index");
            }
        }

        public ActionResult Edit(Usuario user)
        {
            if (Session["login"] != null && (int)Session["login"] == 1)
            {
                var Usuario = (from e in db.Usuario
                               where e.id == user.id
                               select e).FirstOrDefault<Usuario>();
                if (Usuario != null)
                {
                    return View(Usuario);
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult Editar(Usuario user)
        {
            if (Session["login"] != null && (int)Session["login"] == 1)
            {
                var query =
                     (from e in db.Usuario
                      where e.id == user.id
                      select e).FirstOrDefault<Usuario>();
                query.usuario = user.usuario;
                query.contraseña = user.contraseña;
                db.SaveChanges();
                ViewBag.change = "Los datos se han actualizado";
                return View("Edit", query);
            }
            else
            {
                return View("Error");
            }
        }
        public ActionResult Delete(Usuario user)
        {
            if (Session["login"] != null && (int)Session["login"] == 1)
            {
                var query =
                         (from e in db.Usuario
                          where e.id == user.id
                          select e).FirstOrDefault<Usuario>();
                if (query != null)
                {
                    var query2 = from p in db.Proyecto
                                 where p.userId == user.id
                                 select p;
                    if (query2 != null)
                    {
                        foreach (var a in query2)
                        {
                            var query3 = from c in db.Comentario
                                         where c.proyId.Equals(a.nombre)
                                         select c;

                            foreach (var x in query3)
                            {
                                db.Comentario.Remove(x);
                            }


                            var query4 = from v in db.Voto
                                         where v.proyId == a.id
                                         select v;

                            foreach (var x in query4)
                            {
                                db.Voto.Remove(x);
                            }
                              db.Proyecto.Remove(a);

                        }
                        if (query.id != 1)
                        {
                            ViewBag.change = query.usuario + " - Ha sido borrado correctamente";
                            db.Usuario.Remove(query);
                            db.SaveChanges();
                        }
                        else
                        {
                            ViewBag.change = query.usuario + " - No puede ser eliminado";
                        }
                    }
                }
              
                AdministrarUsuarios();
                return View("AdministrarUsuarios");
            }
            else
            {
                return View("Index");
            }
        }

        // PROYECTOS //
        [HttpPost]
        public ActionResult CrearProyecto()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearProyectoDB(Proyecto proyect)
        {
            if (Session["login"] != null)
            {
                var query = (from e in db.Proyecto
                             where e.nombre.Equals(proyect.nombre)
                             select e).FirstOrDefault<Proyecto>();
                if (query == null)
                {
                    proyect.userId = (int)Session["login"];
                    db.Proyecto.Add(proyect);
                    db.SaveChanges();
                    ModelState.Clear();
                    @ViewBag.edit = "Proyecto creado con exito";
                    Proyectos(proyect);
                    return View("Proyectos");
                }
                else
                {
                    @ViewBag.mensaje = "Ya existe un proyecto con esas caracteristicas";
                    return View("CrearProyecto");
                }

            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult Proyectos(Proyecto proyect)
        {

            if (Session["login"] != null)
            {

                var query = (from a in db.Proyecto
                             where a.id == proyect.id
                             select a).FirstOrDefault<Proyecto>();
                if (query != null)
                {
                    var query2 = (from p in db.Proyecto
                                  join u in db.Usuario on p.userId equals u.id
                                  where query.userId == u.id
                                  select u).FirstOrDefault<Usuario>();
                    ViewBag.usuario = query2.usuario;
                    ViewBag.id = query2.id;
                    return View(query);
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }


        public ActionResult EditarProyecto(Proyecto proyect)
        {
            if (proyect != null && Session["login"] != null)
            {

                var proy = (from p in db.Proyecto
                            where p.id == proyect.id
                            select p).FirstOrDefault<Proyecto>();

                if ((int)Session["login"] == proy.userId || (int)Session["login"] == 1)
                {
                    return View(proy);
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                return View("Error");
            }

        }


        [HttpPost]
        public ActionResult EditarProyectoDB(Proyecto proyect)
        {

            if (Session["login"] != null)
            {
                var query = db.Proyecto.FirstOrDefault(p => p.id == proyect.id);
                if (query != null && (int)Session["login"] == query.userId || (int)Session["login"] == 1)
                {

                    query.nombre = proyect.nombre;
                    query.texto = proyect.texto;
                    db.SaveChanges();
                    @ViewBag.edit = "Proyecto editado con exito";
                }
                else
                {
                    @ViewBag.edit = "No existe ese proyecto";
                }
                Proyectos(proyect);
                return View("Proyectos");
            }
            else
            {
                return View("Error");
            }
        }


        public ActionResult DeleteProyecto(Proyecto proyect)
        {
            if (Session["Login"] != null)
            {
                var query = db.Proyecto.FirstOrDefault(p => p.id == proyect.id);
                if (query != null)
                {
                    var query2 = from c in db.Comentario
                                 where c.proyId.Equals(query.nombre)
                                 select c;
                    foreach (var x in query2)
                    {
                        db.Comentario.Remove(x);
                    }
                    var query3 = from v in db.Voto
                                 where v.proyId == query.id
                                 select v;
                    foreach (var x in query3)
                    {
                        db.Voto.Remove(x);
                    }
                    db.Proyecto.Remove(query);
                    db.SaveChanges();
                }
                return RedirectToAction("InternoDB");
            }
            else
            {
                return View("Error");
            }

        }

        // OTRAS FUNCIONALIDADES //

        [HttpPost]
        public ActionResult CerrarSession()
        {
            Session["login"] = null;
            ViewBag.error = "Se ha cerrado la sesion";
            return View("Index");
        }


        [HttpPost]
        public ActionResult likeOrDislike(Proyecto proyect, string id2)
        {
            if (Session["login"] != null)
            {
                if (id2 != null && (id2.Equals("0") || id2.Equals("1")))
                {

                    var query = (from a in db.Proyecto
                                 where a.id == proyect.id
                                 select a).FirstOrDefault<Proyecto>();
                    if (query != null)
                    {
                        int idU = (int)Session["login"];
                        var query2 = (from v in db.Voto
                                      where v.userId == idU && v.proyId == proyect.id
                                      select v).FirstOrDefault<Voto>();

                        bool votado = true;
                        if (id2.Equals("0"))
                            votado = false;
                        if (query2 == null)
                        {
                            if (votado)
                            {
                                query.votosAFavor++;
                            }
                            else
                            {
                                query.votosEnContra++;
                            }
                            db.Voto.Add(new Voto((int)Session["login"], proyect.id, votado));
                            db.SaveChanges();
                        }
                        else if (query2.voto != votado)
                        {
                            if (votado)
                            {
                                query.votosEnContra--;
                                query.votosAFavor++;
                            }
                            else
                            {
                                query.votosEnContra++;
                                query.votosAFavor--;
                            }
                            query2.voto = votado;
                            db.SaveChanges();
                        }
                        return PartialView("LikeODislike", query);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Index");
            }



        }

        [HttpPost]
        public ActionResult mostrarComentarios(Proyecto proyect)
        {
            if (Session["login"] != null)
            {

                var query = (from a in db.Proyecto
                             where a.id == proyect.id
                             select a).FirstOrDefault<Proyecto>();
                if (query != null)
                {
                    var query2 = (from c in db.Comentario
                                  where c.proyId.Equals(query.nombre)
                                  select c);
                    int uID = (int)Session["login"];

                    var query3 = db.Usuario.FirstOrDefault(u => u.id == uID);
                    Session["U"] = query3.usuario;
                    System.Diagnostics.Debug.WriteLine(Session["U"]);
                    return PartialView(query2);
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost]
        public ActionResult CrearComentario(Comentario comment, string id)
        {
            if (Session["login"] != null)
            {
                int idU = (int)Session["login"];
                var query = db.Usuario.FirstOrDefault(u => u.id == idU);
                var query2 = db.Proyecto.FirstOrDefault(p => p.nombre == id);
                if (query2 != null && query != null)
                {
                    comment.userId = query.usuario;
                    comment.proyId = id;
                    db.Comentario.Add(comment);
                    db.SaveChanges();
                    ViewBag.edit = "Comentario añadido con exito";
                    Proyectos(query2);
                    return View("Proyectos");
                }
                return View("Index");
            }
            else
            {

            }
            return View("Index");
        }


        [HttpPost]
        public ActionResult DeleteComment(Comentario comment)
        {
            if (Session["login"] != null)
            {
                var query = db.Comentario.FirstOrDefault(c => c.id == comment.id);
                if (query != null)
                {
                    var query2 = db.Proyecto.FirstOrDefault(p => p.nombre.Equals(query.proyId));
                    db.Comentario.Remove(query);
                    db.SaveChanges();
                    return PartialView();
                }
                else
                {
                    return View("Index");
                }
            }
            else
            {
                return View("Index");
            }

        }


        public void crearMuchosUsuariosEnDbParaPruebas()
        {
            for (int i = 0; i < 100; i++)
            {
                db.Usuario.Add(new Usuario("Usuario" + i, "Pass" + i));
                db.Proyecto.Add(new Proyecto("Proyecto" + i, "nada" + i, i));
            }
            db.SaveChanges();
        }

        public string devolverUsuarioLogueado()
        {

            int num = (int)Session["login"];
            var query = (from e in db.Usuario
                         where e.id == num
                         select e).FirstOrDefault<Usuario>();
            return query.usuario;

        }

    }
}
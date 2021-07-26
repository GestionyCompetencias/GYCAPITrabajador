using System;
using System.Linq;
using System.Threading.Tasks;
using GYCEmpresa.App_Start;
using GYCEmpresa.Models;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Http;
//using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GYCEmpresa.Controllers
{
    [Route("api/cambioclave")]

    //[ApiController]
    public class cambioclaveController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public JsonResult Get()
        {
            respuesta respuesta = new respuesta();
            respuesta.mensaje = "Cambio de clave";
            return Json(new
            {
                respuesta
            }, JsonRequestBehavior.AllowGet);
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solcambio data)
        {
            int clave = APITrabajador.CambioClave(data);
            respuesta respuesta = new respuesta();
            respuesta.mensaje = "Contraseña modificada exitosamente";
            if(clave==0)respuesta.mensaje = "No se pudo modicar contraseña";
            return Json(new
            {
               respuesta
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solcambio
    {
        public string rut { get; set; }
        public string antigua { get; set; }
        public string nueva { get; set; }
    }
}

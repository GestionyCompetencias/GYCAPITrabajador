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
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace GYCEmpresa.Controllers
{
    [Route("api/solicitudpermiso")]

    //[ApiController]
    public class permisoController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public JsonResult Get()
        {
            respuesta respuesta = new respuesta();
            respuesta.mensaje = "Solicitud de permiso";
            return Json(new
            {
               respuesta
            }, JsonRequestBehavior.AllowGet);

        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solpermiso data)
        {
            Reply resultado = APITrabajador.SolicitudPermiso(data.token,data.rut, data.fecha1, data.fecha2,data.hora1,data.hora2);
            return Json(new
            {
                resultado
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solpermiso
    {
        public string token { get; set; }
        public string rut { get; set; }
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }
        public string hora1 { get; set; }
        public string hora2 { get; set; }
    }
}

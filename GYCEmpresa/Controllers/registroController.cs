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

namespace GYCEmpresa.Controllers
{
    [Route("api/registro")]

    //[ApiController]
    public class registroController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public JsonResult Get()
        {
            respuesta respuesta = new respuesta();
            respuesta.mensaje = "Registro de asistencia";
            return Json(new
            {
                respuesta
            }, JsonRequestBehavior.AllowGet);
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solregistro data)
        {
            DateTime fec1 = Convert.ToDateTime(data.fecha1);
            DateTime fec2 = Convert.ToDateTime(data.fecha2);
            JsonResult cabezera = APITrabajador.Cabezera(data.rut, fec1, fec2);
            JsonResult asistencia = APITrabajador.Registro(data.rut, fec1,fec2);
            return Json(new
            {   cabezera,
                asistencia
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solregistro
    {
        public string rut { get; set; }
        public string fecha1 { get; set; }
        public string fecha2 { get; set; }
    }
}

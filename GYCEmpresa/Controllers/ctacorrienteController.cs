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
    [Route("api/ctacorriente")]

    //[ApiController]
    public class ctacorrienteController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public JsonResult Get()
        {
            respuesta respuesta = new respuesta();
            respuesta.mensaje = "Cuenta corriente vacaciones";
            return Json(new
            {
                respuesta
            }, JsonRequestBehavior.AllowGet);
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solctacte data)
        {
            Reply respuesta = APITrabajador.CtaCte(data.token,data.rut);
            return Json(new
            {
                respuesta
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solctacte
    {
        public string token{ get; set; }
        public string rut { get; set; }
    }
}

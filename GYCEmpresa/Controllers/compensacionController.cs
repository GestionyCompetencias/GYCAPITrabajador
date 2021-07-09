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
    [Route("api/compensacion")]

    //[ApiController]
    public class compensacionController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public string[] Get()
        {
            return new string[] { "Hola ", "Solicitud de compensacion" };
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solcompensacion data)
        {
            JsonResult compensacion = APITrabajador.SolicitudCompensacion(data.rut, data.ndias);
            return Json(new
            {
                compensacion
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solcompensacion
    {
        public string rut { get; set; }
        public string ndias { get; set; }
    }
}

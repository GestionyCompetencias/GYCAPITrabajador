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
    [Route("api/personal")]

    //[ApiController]
    public class personalController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public string[] Get()
        {
            return new string[] { "Hola ", "Informacion personal" };
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult Post([Microsoft.AspNetCore.Mvc.FromBody] solpersonal data)
        {
            JsonResult trabajador = APITrabajador.ExistePersonaDetalle(data.rut);
            return Json(new
            {
                trabajador
            }, JsonRequestBehavior.AllowGet);

        }
    }

}
namespace GYCEmpresa.Models
{

    public class solpersonal
    {
        public string rut { get; set; }
    }
}

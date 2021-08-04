//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
using System;
//using System.Collections.Generic;
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
    [Route("api/Values")] 

    //[ApiController]
    public class ValuesController : Controller
    {
        // Get API
        private APITrabajadorController APITrabajador = new APITrabajadorController();
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public string[] Get()
        {
            return new string[] { "Hola ", "Trabajador" };
        }
        // Get Api Id
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public JsonResult  Trabajador([Microsoft.AspNetCore.Mvc.FromBody] coneccion data)
        {
            Reply trabajador = APITrabajador.ExistePersonaDetalle(data.usuario,data.clave);
            return Json(new
            {
                trabajador
            }, JsonRequestBehavior.AllowGet);

        }
    }


}
namespace GYCEmpresa.Models
{

    public class coneccion
    {
        public string usuario { get; set; }
        public string clave { get; set; }
        public string dispositivo { get; set; }
    }
}

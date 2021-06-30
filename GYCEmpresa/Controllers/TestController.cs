using GYCEmpresa.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;

namespace GYCEmpresa.Controllers
{
    public class TestController : Controller
    {

        public async System.Threading.Tasks.Task<ActionResult> VerificaApiAsync(string Texto)
        {
            using (HttpClient client = new HttpClient())
            {
                string urlApi = "https://localhost:44348/api/";

                string uRL = urlApi + "FirmaElectronica/Firmar";

                client.BaseAddress = new Uri(uRL);

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                FirmarViewModel trabajador = new FirmarViewModel();
                trabajador.trabajador = "158531682";

                HttpResponseMessage response = await client.PostAsJsonAsync(uRL, trabajador);

                response.EnsureSuccessStatusCode();

                ViewBag.Respuesta = response.Headers.Location;


            }
            return View();
        }
    }
}
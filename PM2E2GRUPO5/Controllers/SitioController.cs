using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PM2E2GRUPO5.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PM2E2GRUPO5.Controllers
{
    public class SitioController
    {
    private static readonly string URL_SITIOS = "http://192.168.1.10/examenmovilgrupo5/";
    private static HttpClient client = new HttpClient();

        /*public static async Task<List<SitiosFirma>> GetAllSite()
        {
            List<SitiosFirma> listBooks = new List<SitiosFirma>();
            try
            {
                var uri = new Uri(URL_SITIOS + "GetSitios.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    listBooks = JsonConvert.DeserializeObject<List<SitiosFirma>>(content);
                    return listBooks;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error 1: " + ex.Message);
            }

            return listBooks;
        }*/

        public static async Task<List<SitiosFirma>> GetAllSite()
        {
            List<SitiosFirma> listBooks = new List<SitiosFirma>();
            try
            {
                var uri = new Uri(URL_SITIOS + "GetSitios.php");
                var response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var jobject = JObject.Parse(content);
                    var sitioArray = JArray.Parse(jobject["sitio"].ToString());
                    foreach (var sitioObject in sitioArray)
                    {
                        var sitio = new SitiosFirma();
                        sitio.Id = Convert.ToInt32(sitioObject["id"]);
                        sitio.Descripcion = sitioObject["descripcion"].ToString();
                        sitio.Latitud = sitioObject["latitud"].ToString();
                        sitio.Longitud = sitioObject["longitud"].ToString();
                        listBooks.Add(sitio);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetSitios: " + ex.Message);
            }
            return listBooks;
        }
    public async static Task<bool> DeleteSite(string id)
    {
        try
        {
            var uri = new Uri(URL_SITIOS + "eliminarsitio.php?id=" + id);
            var result = await client.GetAsync(uri);
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    public async static Task<bool> CreateSite(SitiosFirma sitio)
    {
        try
        {
            Uri requestUri = new Uri(URL_SITIOS + "Create.php");
            var jsonObject = JsonConvert.SerializeObject(sitio);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Create" + ex.Message);
        }

        return false;
    }


 public async static Task<bool> UpdateSitio(SitiosFirma sitio)
    {
        try
        {
            Uri requestUri = new Uri(URL_SITIOS + "actualizarsitio.php");
            var jsonObject = JsonConvert.SerializeObject(sitio);
            var content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            //var response = await client.PutAsync(requestUri, content);
            var response = await client.PostAsync(requestUri, content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

}
}

using System.Reflection;
using System.Text;
using System.Text.Json;
using wsConsultas.Neg;
using wsTokenJwt.Model;

namespace wsTokenJwt.Neg.Utils
{

    public class ServiceHttp<T>
    {
        public async Task<T> GetRestServiceDataAsync(string serviceAddress)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(serviceAddress);
                var response = await client.GetAsync(client.BaseAddress);
                response.EnsureSuccessStatusCode();
                var jsonResult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(jsonResult)!;
                return result!;
            }
            catch
            {
                Console.WriteLine("Error");
                throw;
            }
        }

        /// <summary>
        /// Servicio de peticiónes POST
        /// </summary>
        /// <param name="serializedData"></param>
        /// <param name="parameters"></param>
        /// <param name="serviceAddress"></param>
        /// <param name="base64basicAuth"></param>
        /// <returns></returns>
        public async Task<T> PostRestServiceDataAsync(string serializedData, string serviceAddress, string parameters,
                    string base64basicAuth, ServiceSettings settings, string str_id_transaccion)
        {
            try
            {
                HttpClient client = new();
                client.BaseAddress = new Uri(serviceAddress);
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64basicAuth);
                client.Timeout = TimeSpan.FromSeconds(settings.timeOutHttp);
                var httpContent = new StringContent(serializedData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(parameters, httpContent).Result;
                string resultadoJson = await response.Content.ReadAsStringAsync();

                T respuesta = default(T)!;
                if (response.IsSuccessStatusCode)
                {
                    respuesta = JsonSerializer.Deserialize<T>(resultadoJson)!;
                }
                else
                {
                    var res_servicio = new { codigo = response.StatusCode, cabecera = response.Headers, cuerpo = response.Content.ReadAsStreamAsync() };
                    ServiceLogs.Init(settings);
                    ServiceLogs.SaveHttpErrorLogs(JsonSerializer.Deserialize<dynamic>(serializedData), MethodBase.GetCurrentMethod()!.Name, "ServiceHttp", res_servicio, str_id_transaccion);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                var data = JsonSerializer.Deserialize<dynamic>(serializedData);
                ServiceLogs.Init(settings);
                ServiceLogs.SaveHttpErrorLogs(data, MethodBase.GetCurrentMethod()!.Name, "ServiceHttp", ex, str_id_transaccion);
                throw new Exception(data!.str_id_transaccion)!;
            }
        }
        public async Task<T> PostRestServiceDataAsync(string serializedData, string serviceAddress, string parameters, string base64basicAuth, ServiceSettings settings)
        {
            return await PostRestServiceDataAsync(serializedData, serviceAddress, parameters, base64basicAuth, settings, String.Empty);
        }
    }
}

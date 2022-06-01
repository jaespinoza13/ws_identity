
using System.Text.Json;

namespace Infrastructure.Common.Tramas
{
    internal static class TextFiles
    {
        private static readonly object objetoBloqueoJson = new();
        private static object objetoBloqueo = new object();
        public static void RegistrarTramas(string strTramaRequest, string rutaArchivo)
        {
            try
            {
                lock (objetoBloqueo)
                {

                    if (!File.Exists(rutaArchivo))
                    {
                        Directory.CreateDirectory(rutaArchivo);
                    }

                    string fileName = Path.Combine(rutaArchivo, DateTime.Now.ToString("yyyyMMdd") + ".txt");

                    using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(DateTime.Now.ToString("HHmmssff") + " " + strTramaRequest + " ");
                        }
                        fs.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(String.Empty)!;
            }

        }

        public static void RegistrarTramas(String str_tipo, dynamic obj, string ruta)
        {
            string rutaArchivo = Directory.GetCurrentDirectory() + ruta;
            try
            {
                lock (objetoBloqueoJson)
                {

                    if (!File.Exists(rutaArchivo))
                    {
                        Directory.CreateDirectory(rutaArchivo);
                    }

                    string fileName = Path.Combine(rutaArchivo, DateTime.Now.ToString("yyyyMMdd") + ".txt");

                    using (var fs = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.WriteLine(DateTime.Now.ToString("HHmmssff") + " " + str_tipo + JsonSerializer.Serialize(obj) + " ");
                        }
                        fs.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception(String.Empty)!;
            }
        }
    }
}

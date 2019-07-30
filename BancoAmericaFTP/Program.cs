using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace BancoAmericaFTP
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //Ruta
            string NombreArchivo = @"InformeBancoAmerica-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt";

            //Credenciales
            var Credencial = new
            {
                Properties.Settings.Default.User,
                Properties.Settings.Default.Password,
                Properties.Settings.Default.Url
            };

            //Creando Informe Diario
            CrearInforme(NombreArchivo);

            //Subiendo archivo a servidor ftp
            PostDatatoFTP(NombreArchivo,Credencial.User,Credencial.Password, Credencial.Url);

            //Leyendo directorio
            ReadDirectory(Credencial.User,Credencial.Password, Credencial.Url);
         

            Console.ReadKey();
        }
        //private static void ConexionServerFTP(string user, string password)
        //{
        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://localhost");
        //    request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

        //    // This example assumes the FTP site uses anonymous logon.  
        //    request.Credentials = new NetworkCredential(user, password);
        //    request.KeepAlive = false;
        //    request.UseBinary = true;
        //    request.UsePassive = true;

        //    request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);

         
        //}
        public static void CrearInforme(string NombreArchivo)
        {
            Console.WriteLine("Creando informe...");
                System.Threading.Thread.Sleep(3000);

            if (!File.Exists(NombreArchivo))
                {
                using (StreamWriter sw = File.CreateText(NombreArchivo))
                {
                    sw.WriteLine("Informe general " + DateTime.Now.ToString("dd-MMM-yyyy"));
                }
            }

        }


        public static void ReadDirectory(String user, String password, string url)
        {
            try
            {
                Console.WriteLine("Leyendo Directorio.....");

                System.Threading.Thread.Sleep(3000);

                //ConexionServerFTP(user,password);

                // FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://localhost");    
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;


                request.Credentials = new NetworkCredential(user, password);
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;


                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                Console.WriteLine(reader.ReadToEnd());

                Console.WriteLine("Directory List Complete, status {0}", response.StatusDescription);

                reader.Close();
                response.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }


        }
        public static void PostDatatoFTP(string NombreArchivo, string user, string password, string url)
        {
            try
            {
                Console.WriteLine("Subiendo archivo.....");

                System.Threading.Thread.Sleep(3000);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create( url +"/"  + NombreArchivo);
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(user, password);
                // Copy the contents of the file to the request stream.  
                StreamReader sourceStream = new StreamReader(NombreArchivo);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message.ToString());
                String status = ((FtpWebResponse)e.Response).StatusDescription;
                Console.WriteLine(status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}

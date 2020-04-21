using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using RestSharp;

namespace SilentPackage.Controllers
{
    /// <summary>
    /// REST querying class
    /// </summary>
    class HttpClient
    {
        /// <summary>
        /// Creates an POST HTTP request.
        /// </summary>
        /// <param name="url">Address URL.</param>
        /// <param name="method">HTTP query type.</param>
        /// <param name="data">Data to be sent in RAW form.</param>
        /// <returns>Server response.</returns>
        public string MakeWebRequest(string url, string method, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine("Error");
                return "Error";
            }
            //Console.WriteLine(data + @"\n");
            var webRequest = WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.ContentType = @"application/json; charset=utf-8";
            try
            {
                using (var stream = new StreamWriter(webRequest.GetRequestStream()))
                {
                    stream.Write(data);
                }
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    Encoding enc = Encoding.GetEncoding("utf-8");
                    if (response != null)
                    {
                        StreamReader loResponseStream = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), enc);
                        string strResult = loResponseStream.ReadToEnd();
                        loResponseStream.Close();
                        response.Close();
                        return strResult;
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = (HttpWebResponse)we.Response;
                    return ((int)response.StatusCode).ToString();
                }
                else
                {
                    return we.Status.ToString();
                }
            }
            return null;
        }


        public string MakeWebRequest(string url, string method, bool returnOnlyHttpStatus)
        {
          
            //Console.WriteLine(data + @"\n");
            var webRequest = WebRequest.Create(url);
            webRequest.Method = method;
            webRequest.ContentType = @"application/json; charset=utf-8";
            try
            {
                using (HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse)
                {
                    if (returnOnlyHttpStatus)
                    {
                        if (response != null) return ((int) response.StatusCode).ToString();
                    }

                    Encoding enc = Encoding.GetEncoding("utf-8");
                    if (response != null)
                    {
                        StreamReader loResponseStream = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), enc);
                        string strResult = loResponseStream.ReadToEnd();
                        loResponseStream.Close();
                        response.Close();
                        return strResult;
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = (HttpWebResponse)we.Response;
                    return ((int)response.StatusCode).ToString();
                }
                else
                {
                    return we.Status.ToString();
                }
            }
            return null;
        }


        public string SendFile(string url, string path, bool returnOnlyHttpStatus)
        {
            if (!File.Exists(path))
            {
                return "Error";
            }
         
            var client = new RestClient(url);
            var request = new RestRequest(Method.PUT);
           
            request.AddFile("files", path);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            return returnOnlyHttpStatus ? response.StatusCode.ToString() : response.Content;
        }
    }
}

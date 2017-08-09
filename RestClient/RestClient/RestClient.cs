using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace pm.test.rest
{
    public class RestClient
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum RequestType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        private HttpContent PackageContent(string input)
        {
            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");
            return content;
        }

        private HttpResponseMessage MakeRequest(RequestType reqType, string uri, HttpContent content, string authToken = null)
        {
            var client = new HttpClient();

            if (authToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }
            
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //ACCEPT header
            
            var meth = new HttpMethod(Enum.GetName(typeof(RequestType), reqType));
            var req = new HttpRequestMessage(meth, uri);
            if (content != null)
            {
                req.Content = content;
            }
            var msg = client.SendAsync(req).Result;

            return msg;
        }

        public HttpResponseMessage Get(string uri, string authtoken = null)
        {
            return MakeRequest(RequestType.GET, uri, null,  authtoken);
        }

        public HttpResponseMessage Put(string uri, string json, string authtoken = null)
        {
            return MakeRequest(RequestType.PUT, uri, PackageContent(json), authtoken);
        }

        public HttpResponseMessage Post(string uri, string json, string authtoken = null)
        {
            return MakeRequest(RequestType.POST, uri, PackageContent(json), authtoken);
        }

        public HttpResponseMessage Delete(string uri, string authtoken = null)
        {
            return MakeRequest(RequestType.DELETE, uri, null, authtoken);
        }
    }

    public static class HttpResponseMessageExt
    {
        public static string GetContent(this HttpResponseMessage msg)
        {
            using (var receiveStream = msg.Content.ReadAsStreamAsync().Result)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }

        public static string GetContent(this HttpRequestMessage msg)
        {
            using (var receiveStream = msg.Content.ReadAsStreamAsync().Result)
            {
                using (var readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }
    }
}

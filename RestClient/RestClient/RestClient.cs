using System;
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

        private HttpResponseMessage MakeRequest(RequestType reqType, string uri, HttpContent content = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

            var meth = new HttpMethod(Enum.GetName(typeof(RequestType), reqType));
            var req = new HttpRequestMessage(meth, uri);
            if (content != null)
            {
                req.Content = content;
            }
            var msg = client.SendAsync(req).Result;

            return msg;
        }

        public HttpResponseMessage Get(string uri)
        {
            return MakeRequest(RequestType.GET, uri);
        }

        public HttpResponseMessage Put(string uri, string json)
        {
            return MakeRequest(RequestType.PUT, uri, PackageContent(json));
        }

        public HttpResponseMessage Post(string uri, string json)
        {
            return MakeRequest(RequestType.POST, uri, PackageContent(json));
        }

        public HttpResponseMessage Delete(string uri)
        {
            return MakeRequest(RequestType.DELETE, uri);
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace firstmile.services.DejeroApi
{
    public class ApiCaller<T>
    {
        public async Task<T> InvokeAPI(string uri, string method)
        {
            string payload = CreateMD5("");
            var currentDate = DateTime.UtcNow;
            string timeStamp = ConvertToGMT(currentDate);
            string cano = CreateCanonicalString(method, uri, string.Empty, timeStamp);
            byte[] key = Encoding.ASCII.GetBytes("gHSNJB5METcOkwSyH7oyRjdDEliyCFV/g5b4NBZIsXYnFk8Tc4FJKTdlNoWk+Uw315KIeXBSkHtKAiRiCWiqpA==");
            string signature = Encode(cano, key);
            var header = new DejeroHeader()
            {
                Accept = "application/json",
                Authorization = $"APIAuth 70627:{signature}",
                ContentMD5 = payload,
                ContentType = "application/json",
                Date = timeStamp
            };
            var result = await Invoke(uri, method, header, currentDate);
            if (result == string.Empty)
            { return default(T); };
            return JsonConvert.DeserializeObject<T>(result);
        }

        private async Task<string> Invoke(string url, string method, DejeroHeader header, DateTime currentDate)
        {
            try
            {
                HttpWebRequest client = (HttpWebRequest)WebRequest.Create($"https://api-control.dejero.com{url}");
                client.Method = method;
                client.ContentType = "application/json";
                client.Headers.Add("Authorization", header.Authorization);
                if (!string.IsNullOrEmpty(header.ContentMD5)) client.Headers.Add("Content-MD5", header.ContentMD5);
                client.Accept = "application/json";
                client.Date = currentDate;

                WebResponse webResponse = client.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = await responseReader.ReadToEndAsync();
                return response;
            }
            catch (WebException ex)
            {
                if(ex.Response != null)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            string error = reader.ReadToEnd();
                        }
                    }
                }

                return string.Empty;
            }
        }

        private string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string hash = Convert.ToBase64String(hashBytes);
                return hash;
            }
        }

        private string CreateCanonicalString(string httpMethod, string uri, string contentHash, string timestamp)
        {
            return $"{httpMethod},application/json,{contentHash},{uri},{timestamp}";
        }

        private string Encode(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return Convert.ToBase64String(myhmacsha1.ComputeHash(stream));
        }

        private string ConvertToGMT(DateTime datetime)
        {
            var months = new string[]
            {
                "","Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"
            };
            string time = $"{datetime:HH:mm:ss}";
            return $"{datetime.DayOfWeek.ToString().Substring(0, 3)}, {datetime.Day:00} {months[datetime.Month]} {datetime.Year} {time} GMT";
        }
    }

    public class DejeroHeader
    {
        public string Accept { get; set; }
        public string Authorization { get; set; }

        [JsonProperty(PropertyName = "Content-MD5")]
        public string ContentMD5 { get; set; }

        [JsonProperty(PropertyName = "Content-Type")]
        public string ContentType { get; set; }
        public string Date { get; set; }
    }
}

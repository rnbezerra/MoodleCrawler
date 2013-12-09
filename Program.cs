using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoodleCrawler
{
    class Program
    {

        static Dictionary<string, string> coockies = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            DoWithWebRequest();
        }

        static void SaveCookies(string cookieString)
        {
             //= "Header Name:Set-Cookie, Header value :MoodleSessionmdl1=8523e6999201fe96127ce3e3147358d9; path=/,MoodleSessionTestmdl1=iz6dxRM761; path=/,MOODLEID_mdl1=deleted; expires=Sun, 09-Dec-2012 03:01:09 GMT; path=/,MOODLEID_mdl1=%25ED%25C3%251CC%25B7d; expires=Fri, 07-Feb-2014 03:01:10 GMT; path=/";

            cookieString = cookieString.Replace("MOODLEID_mdl1=deleted;", "");

            var v = new Regex("MoodleSessionmdl1=(.*?);").Match(cookieString);
            coockies.Add("MoodleSessionmdl1", v.Groups[1].ToString());

            v = new Regex("MoodleSessionTestmdl1=(.*?);").Match(cookieString);
            coockies.Add("MoodleSessionTestmdl1", v.Groups[1].ToString());

            v = new Regex("MOODLEID_mdl1=(.*?);").Match(cookieString);
            coockies.Add("MOODLEID_mdl1", v.Groups[1].ToString());

        }

        static void DoWithWebRequest()
        {
            string postData = "username=rafael.bezerra&password=RNB3z3rr@&testcookies=1";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            //headers.Add("Cookie", "hide:inst4239=1; MoodleSessionmdl1=89e196ede8724847d4ff51cd2f1a7d4d; MOODLEID_mdl1=%25F1%25CD%2518M%25B6q%25FE%250C%25E5%255D%25E44%25F1U; MoodleSessionTestmdl1=z3jZpWVT06");

            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("http://sae.infnet.edu.br/moodle/login/index.php");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Convert POST data to a byte array.
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            
            //set custom headers
            request.Headers = GetCustomHeaders(headers);

            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);

            if (response.Headers.AllKeys.Any(header => header == "Set-Cookie"))
            {
                int index = response.Headers.AllKeys.ToList().IndexOf("Set-Cookie");
                string value = response.Headers[index];
                SaveCookies(value.ToString());
                //System.Diagnostics.Debug.WriteLine("\nHeader Name:{0}, Header value :{1}", response.Headers.Keys[i], response.Headers[i]);

            }

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(coockies));

            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            Console.ReadLine();
        }

        private static WebHeaderCollection GetCustomHeaders(Dictionary<string, string> headers)
        {
            WebHeaderCollection collection = new WebHeaderCollection();
            foreach (var header in headers)
            {
                collection.Add(header.Key, header.Value);
            }

            return collection;
        }
    }
}

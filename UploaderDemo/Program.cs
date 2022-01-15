using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UploaderDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string option = string.Empty;

            Console.WriteLine("Multi part file upload demo");
            do
            {

                Console.WriteLine("Press any key to invoke the api, type 'exit' to exit fromt the application");
                option = Console.ReadLine();
                if (option == "exit")
                    break;

                string json = "{ \"title\": \"Rocket Launch\", \"filename\": \"gslv.jpg\", \"body\": \"This is the GSLV rocket launch\", \"tags\": [ \"gslv\", \"isro\" ]}";
                HttpContent stringStreamContent = new StringContent(json);
                stringStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
                stringStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { Name = "meta" };
                using (var fileStream = new FileStream("gslv.jpg", FileMode.Open))
                {
                    HttpContent fileStreamContent = new StreamContent(fileStream);
                    fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { Name = "data" };

                    // Construct a MultiPart
                    // 1st : JSON Object with IN parameters
                    // 2nd : Octet Stream with file to upload
                    var content = new MultipartContent("mixed");
                    content.Add(stringStreamContent);
                    content.Add(fileStreamContent);

                    var client = new HttpClient();
                    // POST the request as "multipart/mixed"
                    var result = client.PostAsync("http://localhost:5000/upload", content).Result;
                    string response = await result.Content.ReadAsStringAsync();
                    Console.WriteLine(response);
                }
            }
            while (true);
            
        }
    }
}

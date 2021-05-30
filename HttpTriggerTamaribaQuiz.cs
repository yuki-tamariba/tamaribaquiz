using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;


namespace Company.Function
{
    public static class HttpTriggerTamaribaQuiz
    {
        [FunctionName("HttpTriggerTamaribaQuiz")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try{
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;
            string messeage = data.events[0].messeage.text;
            string replyToken = data.events[0].replytoken;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", $@"Bearer cafbe71401562209fee7ab8001fa6986");
            await client.PostAsJsonAsync("https://api.line.me/v2/bot/message/reply", 
            new { replyToken = replyToken, messages = new Object[]{
                new{
                  type = "text",
                  text = messeage
                },
                
            }});
            
            return new OkObjectResult(new {});
            }catch (Exception ex) {
                return new OkObjectResult(new {} );
            }
        }
    }
}

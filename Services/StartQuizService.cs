using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Play.Tamariba.QuizGame.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Play.Tamariba.QuizGame.Service
{
public class StartQuiz
{
  static readonly string ChannelAccessToken = "rnxbLwzfOzXTlgoy6hDCtNnqOYxT3UjEAqlPzb87pTCPdgHjGjoBrZvZGvpqq0JbqTVC79/H0hnkCU86vcy/85ihoHmgn6GbmJdv+e0OlvX3rlVpEeOuazrs6cclvawxRUA0kdAzu7xufmpku2EIFgdB04t89/1O/w1cDnyilFU="; 
  static readonly string LineEndpoint = "https://api.line.me/v2/bot/message/reply";
  public readonly static HttpClient client;
  static StartQuiz()
  {
      client = new HttpClient();
      client.DefaultRequestHeaders.Add("authorization", $@"Bearer {ChannelAccessToken}");
  }
  [FunctionName("StartQuiz")]
  public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ExecutionContext context, ILogger log) {
    using var stream = new System.IO.StreamReader(req.Body);
    {
      
      log.LogInformation("post");
      using var jsonReaer = new JsonTextReader(stream);
      dynamic json = JsonSerializer.CreateDefault().Deserialize(jsonReaer);
      JArray events = json.events;
      //LoadQuizService
      QuizService service = new QuizService(context);
      LoadQuizService QuizService_test = new LoadQuizService(service);
      foreach (dynamic item in events)
      {
        string reply_token = item.replyToken;
        string reply_text = item.message.text;
        
        Task<object[]> reply_quiz =  QuizService_test.LoadQuiz();

        //後で消す部分
        var quiz_text = await service.LoadQuiz();
        var selections = quiz_text.Selections.Select(kv => new
            {
                type = "text",
                size = "lg",
                text = @$"{kv.Key} {kv.Value}"
            });
        //
        try	
        {
          if(reply_text == "クイズ"){
            var payload = new{
              replyToken = reply_token,
              messages =  new object[]{
                new {
                    type = "flex",
                    altText = "問題",
                    contents = new {
                        type = "bubble",
                        body = new {
                            type = "box",
                            layout = "vertical",
                            contents = new object[] {
                                new {
                                    type = "text",
                                    text = "問題",
                                    weight = "bold",
                                    size = "xxl"
                                },
                                
                                new {
                                    type = "text",
                                    text = "quiz.Question",
                                    wrap = true,
                                    size = "lg",
                                    margin = "lg",
                                    flex = 5
                                },
                                new {
                                    type = "box",
                                    layout = "vertical",
                                    position = "relative",
                                    spacing = "sm",
                                    margin = "lg",
                                    contents = selections.ToArray()
                                }
                            }
                        },
                        
                        footer = new {
                            type = "box",
                            layout = "vertical",
                            contents = new object[] {
                                new {
                                    type = "button",
                                    action = new {
                                        type = "postback",
                                        label = "答えを見る",
                                        data = $@"questionId={1}"
                                    },
                                    gravity = "center",
                                    style = "primary",
                                    height = "md"
                                }
                              
                            }
                        }
                        
                    }
                }
              }
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(LineEndpoint, payload);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            log.LogInformation(responseBody);
          }
          else{
            var payload = new{
              replyToken = reply_token,
              messages =  new[]{
                new{
                  type = "text",
                  text =  reply_text
                }
              }
            };

            HttpResponseMessage response = await client.PostAsJsonAsync(LineEndpoint, payload);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            log.LogInformation(responseBody);
          }
        }
        catch(HttpRequestException e)
        {
          log.LogInformation("\nException Caught!");	
          log.LogInformation("Message :{0} ",e.Message);
        }
      }
    }
    return new OkObjectResult(new{});
  }
  } 
}
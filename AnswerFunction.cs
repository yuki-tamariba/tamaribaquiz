using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Play.Tamariba.QuizGame.Service;

namespace Company.Function
{
  public static class AnswerFunction
  {
    [FunctionName("GetAnswer")]
    public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
          ExecutionContext context, ILogger log)
    {
      using(var stream = new System.IO.StreamReader(req.Body)){
        var body = await stream.ReadToEndAsync();
        var parameters = QueryHelpers.ParseQuery(body);
        var quizId = parameters["quiz_id"];
      
      var service = new QuizService(context);
      var quiz = await service.LoadQuiz(quizId);
      return new OkObjectResult(
        new{
          type = "bubble",
          body = new{
            type = "box",
            layout = "vertical",
            contents = new object[]{
              new{
                type = "text",
                text = "答え"
              },
              new{
                type = "text",
                text = quiz.Answer
              },
              new{
                type = "text",
                text =  "解説"
              },
              new{
                type = "text",
                text = quiz.Comment ?? "",
                wrap = true
              }
    
            }
          }

      });}
    }
  }
}
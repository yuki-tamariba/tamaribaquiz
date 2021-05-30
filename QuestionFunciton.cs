using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Play.Tamariba.QuizGame.Service;

namespace Company.Function
{
  public static class QuestionFunction
  {
    [FunctionName("GetQuiz")]
    public static async Task<IActionResult> Run(
          [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
          ExecutionContext context, ILogger log)
    {
      var service = new QuizService(context);
      var quiz = await service.LoadQuiz();
      return new OkObjectResult(new
      {

        type = "bubble",
        body = new
        {
          type = "box",
          layout = "vertical",
          contents = new object[]{
            new{
              type = "text",
              text = quiz.Question,
              style = "normal",
              weight = "bold",
              decoration = "underline",
              size = "lg"
            },
            new{
              type = "text",
              text = quiz.Selections[Play.Tamariba.QuizGame.Models.Selection.A]
            },
            new{
              type = "text",
              text = quiz.Selections[Play.Tamariba.QuizGame.Models.Selection.B]
            },
            new{
              type = "text",
              text = quiz.Selections[Play.Tamariba.QuizGame.Models.Selection.C]
            },
            new{
              type = "text",
              text = quiz.Selections[Play.Tamariba.QuizGame.Models.Selection.D]
            },
            new{
                type = "button",
                action = new{
                type = "postback",
                label = "答えをみる",
                data = @$"quiz_id={quiz.Id}"
              }
            }
          }
        }
      });
    }
  }
}
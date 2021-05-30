using System.Linq;
using System.Threading.Tasks;
using Play.Tamariba.QuizGame.Models;

namespace Play.Tamariba.QuizGame.Service
{
    public class LoadQuizService
    {
        private readonly QuizService service;
        public LoadQuizService(QuizService service)
        {
            this.service = service;
        }

        public async Task<object[]> LoadQuiz()
        {
            var quiz = await service.LoadQuiz();
            return ToQuestionMessages(quiz);
        }

        private object[] ToQuestionMessages(Quiz quiz)
        {
            var selections = quiz.Selections.Select(kv => new
            {
                type = "text",
                size = "lg",
                text = @$"{kv.Key} {kv.Value}"
            });

            return new object[]{
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
                                    text = quiz.Question,
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
                                        data = $@"questionId={quiz.Id}"
                                    },
                                    gravity = "center",
                                    style = "primary",
                                    height = "md"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
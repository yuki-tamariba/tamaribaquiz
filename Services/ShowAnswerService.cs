using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Play.Tamariba.QuizGame.Models;

namespace Play.Tamariba.QuizGame.Service
{
    public class ShowAnswerService
    {
        private readonly QuizService service;
        public ShowAnswerService(QuizService service)
        {
            this.service = service;
        }
        public async Task<object[]> ShowAnswer(string data)
        {
            var dictionery = QueryHelpers.ParseQuery(data);
            if (!dictionery.TryGetValue("questionId", out var id))
            {
                throw new System.Exception("IDが指定されていません");
            }
            var quiz = await service.LoadQuiz(id);
            return ToAnswerMessages(quiz);
        }

        private object[] ToAnswerMessages(Quiz quiz) => new object[] {
                new {
                    type = "flex",
                    altText = "正解",
                    contents = new {
                        type = "bubble",
                        body = new {
                            type = "box",
                            layout = "vertical",
                            contents = new object [] {
                                new {
                                    type = "box",
                                    layout = "vertical",
                                    contents = new object[] {
                                        new {
                                            type = "text",
                                            text = "正解",
                                            weight = "bold",
                                            size = "xxl"
                                        },
                                        new {
                                            type = "text",
                                            text = quiz.Answer,
                                            margin = "md",
                                            wrap = true,
                                            size = "lg"
                                        },
                                    }
                                },
                                new {
                                    type = "box",
                                    layout = "vertical",
                                    spacing = "lg",
                                    contents = new object[] {
                                        new {
                                            type = "text",
                                            text = "解説",
                                            weight = "bold",
                                            size = "xxl"
                                        },
                                        new {
                                            type = "text",
                                            text = quiz.Comment,
                                            margin = "md",
                                            wrap = true,
                                            size = "lg"
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Azure.WebJobs;
using Play.Tamariba.QuizGame.Models;

namespace Play.Tamariba.QuizGame.Service
{
    public class QuizService
    {
        private const string ApplicationName = "Tamariba quiz bot";

        private readonly ExecutionContext context;

        public QuizService(ExecutionContext context)
        {
            this.context = context;
        }


        public async Task<Quiz> LoadQuiz(string id)
        {
            var values = await LoadFromSpreadSheet();
            var row = values?.Where(row => row.Count > 6)
                ?.FirstOrDefault(row => row[0].ToString() == id);
            return row != null ? Quiz.Create(row) : null;
        }

        public async Task<Quiz> LoadQuiz()
        {
            var values = await LoadFromSpreadSheet();
            var random = new Random();
            // ランダムに1問返す。
            // 被る確率はガチャシミュレーターで確認する
            var row = values?.Where(row => row.Count > 6)
                ?.OrderBy(_ => random.Next())
                ?.FirstOrDefault();
            return row != null ? Quiz.Create(row) : null;
        }

        private async Task<IEnumerable<IList<object>>> LoadFromSpreadSheet()
        {
            ServiceAccountCredential credential;

            var credPath = Path.Combine(context.FunctionAppDirectory, "credential.json");

            using (var stream =
                new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                credential = ServiceAccountCredential.FromServiceAccountData(stream);
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var spreadsheetId = Environment.GetEnvironmentVariable("SpreadsheetId");
            var range = Environment.GetEnvironmentVariable("SpreadsheetRange");
            var selected = string.Format(range, 1, "");
            var request =
                    service.Spreadsheets.Values.Get(spreadsheetId, selected);

            var response = await request.ExecuteAsync();
            return response.Values;
        }
    }
}
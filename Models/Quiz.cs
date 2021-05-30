using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Play.Tamariba.QuizGame.Models
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Selection
    {
        A, B, C, D, None
    }
    public class Quiz
    {
        private Quiz()
        {
            Selections = new Dictionary<Selection, string>();
        }
        public string Question
        {
            get; private set;
        }
        public string Comment
        {
            get; private set;
        }
        public IReadOnlyDictionary<Selection, string> Selections
        {
            get; private set;
        }
        private Selection AnswerKey
        {
            get; set;
        }
        public string Answer => Selections[AnswerKey];
        public string Correct => AnswerKey.ToString();
        public string Id
        {
            get; private set;
        }
        public string Questioner
        {
            get; private set;
        }
        public static Quiz Create(IList<object> row)
        {
            if (row.Count < 7)
            {
                throw new ArgumentException("行の値が足りません。");
            }
            var random = new Random();
            var selections = row.Skip(3).Take(4);
            var selection = new[] { Selection.A, Selection.B, Selection.C, Selection.D }
            .OrderBy(_ => random.Next())
            .Select((key, index) => (new { key, value = selections.ElementAtOrDefault(index) }))
            .ToList();

            var dictionary = selection.OrderBy(s => s.key).ToDictionary(kv => kv.key, kv => kv.value?.ToString());
            return new Quiz
            {
                Id = row[0].ToString(),
                Questioner = row[1].ToString(),
                Question = row[2].ToString(),
                Selections = dictionary,
                AnswerKey = selection.First().key,
                Comment = row.ElementAtOrDefault(7)?.ToString()
            };
        }
    }
}
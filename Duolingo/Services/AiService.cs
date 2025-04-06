using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using System.Text.Json.Serialization;
using Duolingo.Areas.Identity.Data;
using Duolingo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenAI.Assistants;
using OpenAI.Chat;

namespace Duolingo.Services
{
    public class AiService
    {
        string apiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");



        public async Task<List<SubjectModel>> GenerateSubjects(LevelEnum level, LanguagesEnum language)
        {
            ChatClient client = new("gpt-4o-mini", apiKey: apiKey);

            List<ChatMessage> messages = new()
    {
        new UserChatMessage($"Jestes nauczycielem jezyka obcego w Polsce, przygotuj mi 15 tematów po polsku do nauki jezyka {language} na poziomie {level}, niech tematy będą stricte do nauki  języka")
    };

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "subject_to_learn",
                    jsonSchema: BinaryData.FromBytes("""
                {
                    "type": "object",
                    "properties": {
                        "subjectsToLearn": {
                            "type": "array",
                            "items": {
                                "type": "object",
                                "properties": {
                                    "description": { "type": "string" },
                                    "subject": { "type": "string" }
                                },
                                "required": ["subject", "description"],
                                "additionalProperties": false
                            }
                        }
                    },
                    "required": ["subjectsToLearn"],
                    "additionalProperties": false
                }
            """u8.ToArray()),
                    jsonSchemaIsStrict: true
                )
            };

            ChatCompletion completion = await client.CompleteChatAsync(messages, options);

            using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);


            JsonElement subjectsArray = structuredJson.RootElement.GetProperty("subjectsToLearn");

            List<SubjectModel> subjects = new();

            foreach (JsonElement item in subjectsArray.EnumerateArray())
            {
                SubjectModel subjectModel = new SubjectModel();
                subjectModel.subject = item.GetProperty("subject").GetString()!;
                subjectModel.description = item.GetProperty("description").GetString()!;
                subjectModel.language = language.ToString();

                subjects.Add(subjectModel);


            }




            return subjects;
        }

        public async Task<ActionResult<ContentModel>> GenerateContent(string subject, string language, string level)
        {

            ChatClient client = new(model: "gpt-4o-mini", apiKey: apiKey);


            List<ChatMessage> messages =
           [

              new UserChatMessage("Jestes moim nauczycielem jezyka "+language+", dla tematu "+subject+" z  jezyka"+language+" na poziomie "+level+" wygeneruj bardzo rozbudowany materiał teoretyczny, 2 zadania praktyczne oraz quiz sprawdzajacy wiedze niech quiz ma postac pytanie A) odpowiedz B) odpowiedz C) odpowiedz. Twoi uczniowe mówią po polsku ale chcą się nauczyć "+language),
        ];

            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "subject_to_learn",
                    jsonSchema: BinaryData.FromBytes("""
                     {
                    "type": "object",
                        "properties": {
                            "ContentToLearn": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "title": { "type": "string" },
                                        "content": { "type": "string" }
                                    },
                                    "required": ["title", "content"],
                                    "additionalProperties": false
                                }
                            },
                            "PracticalTasks": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "TaskToDo": { "type": "string" }
                                    },
                                    "required": ["TaskToDo"],
                                    "additionalProperties": false
                                }
                            },
                            "Quiz": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                       
                                        "Questions": { "type": "string" }
                                    },
                                    "required": ["Questions"],
                                    "additionalProperties": false
                                }
                            }
                        },
                        "required": ["ContentToLearn", "PracticalTasks", "Quiz"],
                        "additionalProperties": false
                    }

                    """u8.ToArray()),
                    jsonSchemaIsStrict: true)
            };



            // Deserialize JSON response
            ChatCompletion completion = await client.CompleteChatAsync(messages, options);
            string jsonResponse = completion.Content[0].Text;

            var contentModel = JsonSerializer.Deserialize<ContentModel>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Example: Access data
            if (contentModel != null)
            {
                foreach (var item in contentModel.ContentToLearn)
                {
                    Console.WriteLine($"Title: {item.Title}");
                    Console.WriteLine($"Content: {item.Content}");
                }

                foreach (var task in contentModel.PracticalTasks)
                {
                    Console.WriteLine($"Task: {task.TaskToDo}");
                }

                foreach (var quiz in contentModel.Quiz)
                {
                    Console.WriteLine($"Question: {quiz.Questions}");
                }
            }



            return contentModel;
        }


        public async Task<ResponseModel> CheckTaskCorrectnessAsync(string task, string answer)
        {



            ChatClient client = new(model: "gpt-4o-mini", apiKey: apiKey);


            List<ChatMessage> messages =
           [

               new UserChatMessage(task+" Czy taka odpowiedź jest poprawna? "+answer+". Odpowiedz w jezyku polskim")
        ];
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
               jsonSchemaFormatName: "subject_to_learn",
               jsonSchema: BinaryData.FromString(@"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""TeacherResponse"": {
                        ""type"": ""object"",
                        ""properties"": {
                            ""review"": { ""type"": ""string"" },
                            ""isAnswerCorrect"": { ""type"": ""boolean"" }
                        },
                        ""required"": [""review"", ""isAnswerCorrect""],
                        ""additionalProperties"": false
                    }
                },
                ""required"": [""TeacherResponse""],
                ""additionalProperties"": false
            }"),
               jsonSchemaIsStrict: true
           )
            };

            ChatCompletion completion = await client.CompleteChatAsync(messages, options);

            if (completion.Content.Count > 0)
            {
                string jsonResponse = completion.Content[0].Text;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("TeacherResponse", out JsonElement teacherResponse))
                    {
                        string review = teacherResponse.GetProperty("review").GetString();
                        bool isAnswerCorrect = teacherResponse.GetProperty("isAnswerCorrect").GetBoolean();

                        var responseModel = new ResponseModel { IsCorrect = isAnswerCorrect, Review = review };
                        return responseModel;

                    }
                    else
                    {
                        Console.WriteLine("Błąd: Brak pola 'TeacherResponse' w JSON-ie.");
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Błąd parsowania JSON: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Błąd: Brak odpowiedzi od API.");
            }

            return null;




        }

    }


}




public class ResponseModel
{
    public bool IsCorrect { get; set; }
    public string Review { get; set; }
}
public class ContentModel
{
    public List<ContentToLearn> ContentToLearn { get; set; }
    public List<PracticalTask> PracticalTasks { get; set; }
    public List<QuizQuestion> Quiz { get; set; }
}



public class ContentToLearn
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int SubjectId { get; set; }
    public SubjectToLearn Subject { get; set; } // Nawigacja do encji nadrzędnej
}

public class PracticalTask
{
    public int Id { get; set; }
    public string TaskToDo { get; set; }

    public int SubjectId { get; set; }
    public SubjectToLearn Subject { get; set; } // Nawigacja do encji nadrzędnej

    public bool isPassed { get; set; } = false;
}

public class QuizQuestion
{
    public int Id { get; set; }
    public string Questions { get; set; }

    public int SubjectId { get; set; }
    public SubjectToLearn Subject { get; set; } // Nawigacja do encji nadrzędnej

    public bool isPassed { get; set; } = false;

}
public class SubjectModel
{

    public string subject { get; set; }
    public string description { get; set; }
    public int? CourseId { get; set; }
    public string? language { get; set; }

    public int? ChallengeToDo { get; set; }
    public int? PassedChalenges { get; set; }
}

public class SubjectsResponse
{
    [JsonPropertyName("subjectsToLearn")]
    public List<SubjectModel> SubjectsToLearn { get; set; }
}
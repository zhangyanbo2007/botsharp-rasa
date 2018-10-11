using BotSharp.Core;
using BotSharp.Core.Engines;
using BotSharp.Platform.Abstraction;
using BotSharp.Platform.Models;
using BotSharp.Platform.Models.AiRequest;
using BotSharp.Platform.Models.AiResponse;
using BotSharp.Platform.Models.MachineLearning;
using BotSharp.Platform.Rasa.Models;
using DotNetToolkit;
using EntityFrameworkCore.BootKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Platform.Rasa
{
    /// <summary>
    /// Rasa nlu >= 0.12
    /// </summary>
    public class RasaAi<TAgent> : 
        PlatformBuilderBase<TAgent>,
        IPlatformBuilder<TAgent>
        where TAgent : AgentModel
    {

        public RasaAi(IAgentStorageFactory<TAgent> agentStorageFactory)
          : base(agentStorageFactory)
        {

        }


        public async Task<AiResponse> TextRequest(AiRequest request)
        {
            AiResponse aiResponse = new AiResponse();

            /*string model = RasaRequestExtension.GetModelPerContexts(agent, AiConfig, request, dc);
            var result = CallRasa(agent.Id, request.Query.First(), model);

            result.Content.Log();

            RasaResponse response = result.Data;
            aiResponse.Id = Guid.NewGuid().ToString();
            aiResponse.Lang = agent.Language;
            aiResponse.Status = new AIResponseStatus { };
            aiResponse.SessionId = AiConfig.SessionId;
            aiResponse.Timestamp = DateTime.UtcNow;

            var intentResponse = RasaRequestExtension.HandleIntentPerContextIn(agent, AiConfig, request, result.Data, dc);

            RasaRequestExtension.HandleParameter(agent, intentResponse, response, request);

            RasaRequestExtension.HandleMessage(intentResponse);

            aiResponse.Result = new AIResponseResult
            {
                Source = "agent",
                ResolvedQuery = request.Query.First(),
                Action = intentResponse?.Action,
                Parameters = intentResponse?.Parameters?.ToDictionary(x => x.Name, x => (object)x.Value),
                Score = response.Intent.Confidence,
                Metadata = new AIResponseMetadata { IntentId = intentResponse?.IntentId, IntentName = intentResponse?.IntentName },
                Fulfillment = new AIResponseFulfillment
                {
                    Messages = intentResponse?.Messages?.Select(x => {
                        if (x.Type == AIResponseMessageType.Custom)
                        {
                            return (new
                            {
                                x.Type,
                                Payload = JsonConvert.DeserializeObject(x.PayloadJson)
                            }) as Object;
                        }
                        else
                        {
                            return (new { x.Type, x.Speech }) as Object;
                        }

                    }).ToList()
                }
            };

            RasaRequestExtension.HandleContext(dc, AiConfig, intentResponse, aiResponse);

            Console.WriteLine(JsonConvert.SerializeObject(aiResponse.Result));*/

            return aiResponse;
        }

        private IRestResponse<RasaResponse> CallRasa(string projectId, string text, string model)
        {
            var config = (IConfiguration)AppDomain.CurrentDomain.GetData("Configuration");
            var client = new RestClient($"{config.GetSection("RasaNlu:url").Value}");

            var rest = new RestRequest("parse", Method.POST);
            string json = JsonConvert.SerializeObject(new { Project = projectId, Q = text, Model = model },
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            rest.AddParameter("application/json", json, ParameterType.RequestBody);

            return client.Execute<RasaResponse>(rest);
        }

        public async Task<TrainingCorpus> ExtractorCorpus(TAgent agent)
        {
            var corpus = new TrainingCorpus()
            {
                Entities = new List<TrainingEntity>(),
                UserSays = new List<TrainingIntentExpression<TrainingIntentExpressionPart>>()
            };

            List<string> entities = new List<string>();

            // generate entity list
            agent.Intents.ForEach(intent =>
            {
                intent.Entities.ForEach(entity =>
                {
                    if (!entities.Contains(entity.Entity))
                    {
                        corpus.Entities.Add(new TrainingEntity
                        {
                            Entity = entity.Entity,
                            Values = new List<TrainingEntitySynonym>
                            {
                                new TrainingEntitySynonym
                                {
                                    Value = entity.Value,
                                    Synonyms = new List<string>
                                    {
                                        entity.Value
                                    }
                                }
                            }
                        });
                        entities.Add(entity.Entity);
                    }
                });
            });

            agent.Intents.ForEach(intent =>
            {
                var express = new TrainingIntentExpression<TrainingIntentExpressionPart>()
                {
                    Text = intent.Text,
                    Intent = intent.Intent,
                    Entities = intent.Entities
                };
                
                corpus.UserSays.Add(express);
            });

            return corpus;
        } 
    }
}

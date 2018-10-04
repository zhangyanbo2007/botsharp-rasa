﻿using BotSharp.Platform.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotSharp.Platform.Rasa.Models
{
    public class AgentModel : AgentBase
    {
        [JsonProperty("common_examples")]
        public List<RasaIntentExpression> UserSays { get; set; }

        [JsonProperty("entity_synonyms")]
        public List<RasaTrainingEntity> Entities { get; set; }

        [JsonProperty("regex_features")]
        public List<RasaTrainingRegex> Regex { get; set; }
    }

    public class RasaAgentImportModel
    {
        [JsonProperty("rasa_nlu_data")]
        public AgentModel Data { get; set; }
    }
}

using BotSharp.Core;
using BotSharp.Platform.Abstraction;
using BotSharp.Platform.Models;
using BotSharp.Platform.Rasa.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotSharp.Platform.Rasa
{
    public class AgentStorageFactory : IAgentStorageFactory
    {
        private readonly Func<string, IAgentStorage<AgentModel>> func;
        private readonly NLUSetting nLUSetting;

        public AgentStorageFactory(NLUSetting setting, Func<string, IAgentStorage<AgentModel>> serviceAccessor)
        {
            this.func = serviceAccessor;
            this.nLUSetting = setting;
        }

        public async Task<IAgentStorage<TAgent>> Get<TAgent>() where TAgent : AgentBase
        {
            IAgentStorage<AgentModel> storage = null;
            string storageName = this.nLUSetting.AgentStorage;
            storage = func(storageName);
            return storage as IAgentStorage<TAgent>;
        }
    }
}

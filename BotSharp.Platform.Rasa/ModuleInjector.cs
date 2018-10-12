using BotSharp.Core;
using BotSharp.Core.AgentStorage;
using BotSharp.Core.Modules;
using BotSharp.Platform.Abstraction;
using BotSharp.Platform.Rasa.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotSharp.Platform.Rasa
{
    public class ModuleInjector : IModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<RasaAi<AgentModel>>();
            AgentStorageServiceRegister.Register<AgentModel>(services);
            PlatformConfigServiceRegister.Register<PlatformSettings>("rasaAi", services, config);
        }

        public void Configure(IApplicationBuilder app)
        {

        }
    }
}

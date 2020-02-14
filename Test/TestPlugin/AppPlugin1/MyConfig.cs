using Hangfire.HttpJob.Agent.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppPlugin1
{
    public class MyConfig : IPluginConfigure
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IClassA, ClassA>();
        }
    }
}

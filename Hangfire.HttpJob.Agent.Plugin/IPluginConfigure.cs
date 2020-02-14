using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public interface IPluginConfigure
    {
        void Configure(IServiceCollection services);
    }
}

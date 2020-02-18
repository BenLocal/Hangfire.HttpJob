using Hangfire.HttpJob.Agent.Config;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public static class HttpJobPluginExtensions
    {

        public static void AddHangfireHttpJobAgentPlugins(this IServiceCollection services, Action<HttpJobPluginOptions> config)
        {
            var options = new HttpJobPluginOptions();
            config(options);

            if (options == null)
            {
                throw new ArgumentNullException($"{nameof(options)} is null");
            }

            var mvcBuilder = services.AddMvcCore()
                .ConfigureApplicationPartManager(manager =>
                {
                    manager.FeatureProviders.Add(new JobAgentFeatureProvider(options.JobAgentTypes));
                });

            // 添加Plugin
            var context = new PluginContext(services, options, mvcBuilder.PartManager);
            context.Loader().AddJobAgents();
            services.AddSingleton(context);

            // 修改默认IJobAgentService
            services.AddSingleton<IJobAgentService, JobAgentServiceWapper>();

            // 文件监视
            services.AddSingleton<DirectoryWatcherHandler>();
            services.AddHostedService<DirectoryWatcherService>();
        }

        public static void UseHangfireHttpJobAgentPlugins(this IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetRequiredService<PluginContext>();

            context.SetProvider(app.ApplicationServices);
        }
    }
}

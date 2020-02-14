using McMaster.NETCore.Plugins;
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

            // 将所有Plugin添加到ApplicationParts中
            foreach (var dir in Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, "plugins")))
            {
                var assemblyFile = Path.Combine(dir, Path.GetFileName(dir) + ".dll");
                if (File.Exists(assemblyFile))
                {
                    var plugin = PluginLoader.CreateFromAssemblyFile(
                        assemblyFile,
                        config =>
                        {
                            config.PreferSharedTypes = true;
                            config.EnableHotReload = true; // TODO
                        });

                    var pluginAssembly = plugin.LoadDefaultAssembly();
                    var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginAssembly);
                    foreach (var part in partFactory.GetApplicationParts(pluginAssembly))
                    {
                        mvcBuilder.PartManager.ApplicationParts.Add(part);
                    }

                    // 将所有IPluginConfigure中的配置DI到services中
                    foreach (var pluginType in pluginAssembly.GetTypes()
                        .Where(t => typeof(IPluginConfigure).IsAssignableFrom(t) &&
                            !t.IsAbstract && 
                            t != typeof(IPluginConfigure)))
                    {
                        var pluginConfig = Activator.CreateInstance(pluginType) as IPluginConfigure;
                        pluginConfig?.Configure(services);
                    }
                }
            }

            // 修改HttpAnget中默认配置
            services.AddHangfireHttpJobAgent(config => {
                var jobAgentFeature = JobAgentFeature.GetFromApplicationPart(mvcBuilder.PartManager);
                foreach (var type in jobAgentFeature.JobAgents)
                {
                    config.AddJobAgent(type);
                }
            });

            // 修改默认IJobAgentService
            services.AddSingleton<IJobAgentService, JobAgentServiceWapper>();
        }
    }
}

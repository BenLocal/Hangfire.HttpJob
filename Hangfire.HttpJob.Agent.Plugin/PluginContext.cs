using Hangfire.HttpJob.Agent.Config;
using McMaster.NETCore.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class PluginContext
    {
        private readonly IServiceCollection _services;
        private readonly ApplicationPartManager _partManager;
        private readonly HttpJobPluginOptions _options;
        private IServiceProvider _provider;

        public PluginContext(IServiceCollection services, HttpJobPluginOptions options,
            ApplicationPartManager partManager)
        {
            _services = services;
            _partManager = partManager;
            _options = options;
        }

        public PluginContext Loader()
        {
            foreach (var dir in Directory.GetDirectories(Path.Combine(AppContext.BaseDirectory, _options.SubPath)))
            {
                var assemblyFile = Path.Combine(dir, Path.GetFileName(dir) + ".dll");
                InnerLoader(assemblyFile);
            }

            return this;
        }

        public PluginContext SetProvider(IServiceProvider provider)
        {
            this._provider = provider;

            return this;
        }

        public PluginContext AddJobAgents(bool reload = false)
        {
            // 添加
            var configurer = new JobAgentServiceConfigurer(_services);

            JobAgentFeature jobAgentFeature = reload ? 
                JobAgentFeature.GetReloadFromApplicationPart(_partManager)
                : JobAgentFeature.GetFromApplicationPart(_partManager);

            foreach (var type in jobAgentFeature.JobAgents)
            {
                configurer.AddJobAgent(type);
            }

            return this;
        }

        public PluginContext Reload(string path)
        {
            var basePath = Path.Combine(AppContext.BaseDirectory, _options.SubPath);
            var fileInfo = new FileInfo(path);

            if (fileInfo.Extension != ".dll" || 
                fileInfo.Directory.Name + fileInfo.Extension != fileInfo.Name ||
                basePath != fileInfo.Directory.Parent.FullName)
            {
                return this;
            }

            InnerLoader(path);

            AddJobAgents(true);

            ReloadCallSiteFactory(_services);

            return this;
        }

        private void InnerLoader(string assemblyFile)
        {
            if (File.Exists(assemblyFile))
            {
                var plugin = PluginLoader.CreateFromAssemblyFile(
                    assemblyFile,
                    config =>
                    {
                        config.PreferSharedTypes = true;
                    });

                var pluginAssembly = plugin.LoadDefaultAssembly();
                var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginAssembly);
                foreach (var part in partFactory.GetApplicationParts(pluginAssembly))
                {
                    _partManager.ApplicationParts.Add(part);
                }

                // 将所有IPluginConfigure中的配置DI到services中
                foreach (var pluginType in pluginAssembly.GetTypes()
                    .Where(t => typeof(IPluginConfigure).IsAssignableFrom(t) &&
                        !t.IsAbstract &&
                        t != typeof(IPluginConfigure)))
                {
                    var pluginConfig = Activator.CreateInstance(pluginType) as IPluginConfigure;
                    pluginConfig?.Configure(_services);
                }
            }
        }

        /// <summary>
        /// TODO：ugly implementation
        /// </summary>
        /// <param name="provider"></param>
        private void ReloadCallSiteFactory(IEnumerable<ServiceDescriptor> newServiceDescriptors)
        {
            if (_provider == null)
            {
                return;
            }

            BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var type = _provider.GetType();
            PropertyInfo engineProperty = type.GetProperty("Engine", InstanceBindFlags);
            var engine = engineProperty.GetValue(_provider);


            var engineType = engine.GetType();
            PropertyInfo callSiteFactoryProperty = engineType.GetProperty("CallSiteFactory", InstanceBindFlags);
            var callSiteFactory = callSiteFactoryProperty.GetValue(engine);


            var callSiteFactoryType = callSiteFactory.GetType();

            var descriptorsField = callSiteFactoryType.GetField("_descriptors", InstanceBindFlags);
            //var value = (List<ServiceDescriptor>) descriptorsField.GetValue(callSiteFactory);
            descriptorsField.SetValue(callSiteFactory, newServiceDescriptors.ToList<ServiceDescriptor>());


            // 调用Populate方法
            var populateMethod = callSiteFactoryType.GetMethod("Populate", InstanceBindFlags);
            populateMethod.Invoke(callSiteFactory, null);
        }
    }
}

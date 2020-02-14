using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class JobAgentFeatureProvider: IApplicationFeatureProvider<JobAgentFeature>
    {
        private readonly IEnumerable<Type> _jobAgentTypes;

        private List<TypeInfo> _featureCache;

        public JobAgentFeatureProvider(IEnumerable<Type> jobAgentTypes)
        {
            if (jobAgentTypes == null)
            {
                throw new ArgumentNullException($"{nameof(jobAgentTypes)} is null.");
            }

            this._jobAgentTypes = jobAgentTypes;
        }

        protected bool IsController(TypeInfo typeInfo)
        {
            foreach (var type in _jobAgentTypes)
            {
                if (type.IsAssignableFrom(typeInfo) && 
                    !typeInfo.IsEquivalentTo(type) &&
                    !typeInfo.IsAbstract)
                {
                    return true;
                }
            }

            return false;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, JobAgentFeature feature)
        {
            if (_featureCache != null)
            {
                _featureCache.ForEach(x => feature.JobAgents.Add(x));
                return;
            }

            foreach (var part in parts.OfType<IApplicationPartTypeProvider>())
            {
                foreach (var type in part.Types)
                {
                    if (this.IsController(type) && !feature.JobAgents.Contains(type))
                    {
                        feature.JobAgents.Add(type);

                        // 保存在缓存中
                        if (_featureCache == null)
                        {
                            _featureCache = new List<TypeInfo>();
                        }
                        _featureCache.Add(type);
                    }
                }
            }
        }
    }
}

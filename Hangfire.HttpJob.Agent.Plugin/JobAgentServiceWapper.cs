using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class JobAgentServiceWapper : IJobAgentService
    {
        public (Type, string) GetAgentType(string agentClass, HttpContext context)
        {
            var _partManager = (ApplicationPartManager)context.RequestServices.GetRequiredService(typeof(ApplicationPartManager));
            var jobAgentFeature = JobAgentFeature.GetFromApplicationPart(_partManager);
            try
            {
                if (jobAgentFeature == null || jobAgentFeature.JobAgents == null)
                {
                    return (null, $"Type.GetType({agentClass}) = null!");
                }

                var fullName = agentClass.Split(",")[0];

                foreach (var type in jobAgentFeature.JobAgents)
                {
                    if (!typeof(JobAgent).IsAssignableFrom(type))
                    {
                        continue;
                    }

                    if (fullName == type.FullName)
                    {
                         return (type, null);
                    }
                }

                return (null, $"Type.GetType({agentClass}) = null!");
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }

        }
    }
}

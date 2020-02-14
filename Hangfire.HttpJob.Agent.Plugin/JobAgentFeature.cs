using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class JobAgentFeature
    {
        public IList<TypeInfo> JobAgents { get; } = new List<TypeInfo>();

        public static JobAgentFeature GetFromApplicationPart(ApplicationPartManager partManager)
        {
            var jobAgentFeature = new JobAgentFeature();
            partManager.PopulateFeature(jobAgentFeature);

            return jobAgentFeature;
        }
    }
}
 
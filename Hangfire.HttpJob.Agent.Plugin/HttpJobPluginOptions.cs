using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class HttpJobPluginOptions
    {
        public IEnumerable<Type> JobAgentTypes { get; set; }
    }
}

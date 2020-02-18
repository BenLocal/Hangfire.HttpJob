using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent.Plugin
{
    public class ServiceProviderWapper
    {
        private readonly IEnumerable<ServiceDescriptor> _serviceDescriptors;

        public ServiceProviderWapper(IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
        }

        
    }
}

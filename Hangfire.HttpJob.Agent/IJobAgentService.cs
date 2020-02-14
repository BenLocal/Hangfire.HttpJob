using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent
{
    public interface IJobAgentService
    {
        (Type, string) GetAgentType(string agentClass, HttpContext context);
    }

    class DefaultJobAgentService : IJobAgentService
    {
        /// <summary>
        /// 默认获取AgentType方法
        /// </summary>
        /// <param name="agentClass">HTTP请求中传入的AggentClass参数</param>
        /// <param name="context">该默认方法可以传入空值</param>
        /// <returns></returns>
        public (Type, string) GetAgentType(string agentClass, HttpContext context)
        {
            try
            {
                var type = Type.GetType(agentClass);
                if (type == null)
                {
                    return (null, $"Type.GetType({agentClass}) = null!");
                }

                if (!typeof(JobAgent).IsAssignableFrom(type))
                {
                    return (null, $"Type:({type.FullName}) is not AssignableFrom JobAgent !");
                }


                return (type, null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.HttpJob.Agent.Config
{
    /// <summary>
    /// JobAgent
    /// </summary>
    public class JobAgentOptions
    {
        public bool Enabled { get; set; } = true;
        public string SitemapUrl { get; set; } = "/jobagent";

        public bool EnabledBasicAuth { get; set; }
        public string BasicUserName { get; set; } 
        public string BasicUserPwd { get; set; }

        /// <summary>
        /// 获取AgentType
        /// </summary>
        public Func<string, HttpContext, (Type, string)> AgentTypeDelegate { get; set; } = GetAgentType;

        /// <summary>
        /// 默认获取AgentType方法
        /// </summary>
        /// <param name="agentClass">HTTP请求中传入的AggentClass参数</param>
        /// <param name="_"></param>
        /// <returns></returns>
        private static (Type, string) GetAgentType(string agentClass, HttpContext _)
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

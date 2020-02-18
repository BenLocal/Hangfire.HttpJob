using Hangfire.HttpJob.Agent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppPlugin2
{
    public class TestSecondJob : JobAgent
    {
        protected override void OnException(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override Task OnStart(JobContext jobContext)
        {
            Console.WriteLine("This is second test");

            return Task.CompletedTask;
        }

        protected override void OnStop(JobContext jobContext)
        {
            throw new NotImplementedException();
        }
    }
}

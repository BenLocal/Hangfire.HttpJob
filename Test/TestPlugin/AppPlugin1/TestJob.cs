using Hangfire.HttpJob.Agent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppPlugin1
{
    public class TestJob : JobAgent
    {
        private readonly IClassA _testDIClass;

        public TestJob(IClassA testDIClass)
        {
            _testDIClass = testDIClass;
        }

        protected override void OnException(Exception ex)
        {
            throw new NotImplementedException();
        }

        protected override Task OnStart(JobContext jobContext)
        {
            Console.WriteLine("This is Test1");

            var result = _testDIClass.SayHello();

            return Task.CompletedTask;
        }

        protected override void OnStop(JobContext jobContext)
        {
            throw new NotImplementedException();
        }
    }
}

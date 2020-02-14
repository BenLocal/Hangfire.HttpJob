using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.HttpJob.Agent;
using Hangfire.HttpJob.Agent.Plugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfireHttpJobAgentPlugins(options => {
                options.JobAgentTypes = new List<Type>() { typeof(JobAgent) };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireHttpJobAgent();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    //var _partManager = app.ApplicationServices.GetRequiredService<ApplicationPartManager>();
                    //var jobAgentFeature = new JobAgentFeature();
                    //_partManager.PopulateFeature(jobAgentFeature);
                    //var controllers = jobAgentFeature.JobAgents.ToList();
                    await context.Response.WriteAsync("Hello World!");
                });
            });


        }
    }
}

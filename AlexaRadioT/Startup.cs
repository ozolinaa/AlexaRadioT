using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AlexaRadioT.Store;
using AlexaRadioT.Models;

namespace AlexaRadioT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //https://www.youtube.com/watch?v=TNCdStJRW3s
            //https://stackoverflow.com/questions/45058527/proper-way-to-build-instantiate-a-static-class-in-aspnetcore
            //Could not configure dependency injection from articles aboce, will do basic stuff in next 2 lines
            SkillSettings skillSettings = Configuration.GetSection("SkillSettings").Get<SkillSettings>();
            ApplicationSettingsService.SetSkillSettings(skillSettings);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (ApplicationSettingsService.Skill.ProxyLiveStreamAudio || ApplicationSettingsService.Skill.ProxyPodcastAudio) {
                app.UseAudioProxyServer();
            }
            

            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReplitDbClient;

namespace ReplitDbTest {
  public class Startup {
    public Startup(IConfiguration configuration) {
      this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddCors(options => {
        options.AddPolicy("CorsPolicy",
        builder => {
          builder.AllowAnyOrigin();
          builder.AllowAnyMethod();
          builder.AllowAnyHeader();
        });
      });

      services.AddScoped<IReplitDbClient>(
        _ => new ReplitDBClient(Environment.GetEnvironmentVariable("REPLIT_DB_URL"))
      );

      services.AddControllersWithViews();

      services.Configure<ForwardedHeadersOptions>(options => {
        options.ForwardedHeaders =
          ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
      Console.Error.WriteLine($"Environment: {env.EnvironmentName}");

      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseForwardedHeaders();
      } else {
        app.UseExceptionHandler("/Home/Error");
        app.UseForwardedHeaders();
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      // AMcC - https redirection doesn't play nicely with repl.it
      //app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();
      app.UseCors("CorsPolicy");

      app.UseEndpoints(endpoints => {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}

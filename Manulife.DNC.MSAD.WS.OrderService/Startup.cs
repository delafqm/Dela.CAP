using Manulife.DNC.MSAD.WS.OrderService.Models;
using Manulife.DNC.MSAD.WS.OrderService.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Manulife.DNC.MSAD.WS.OrderService
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
            //设置跨域请求
            services.AddCors(
                option => option.AddPolicy("cors",
                    policy => policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .AllowAnyOrigin()
                )
            );

            services.AddMvc();

            // Repository
            services.AddScoped<IOrderRepository, OrderRepository>();

            // EF DbContext
            services.AddDbContext<OrderDbContext>();

            // Dapper-ConnString
            services.AddSingleton(Configuration["DB:OrderDB"]);

            // CAP
            services.AddCap(x =>
            {
                //x.UseEntityFramework<OrderDbContext>(); // EF

                //x.UseSqlServer(Configuration["DB:OrderDB"]); // SQL Server
                x.UseMySql(Configuration["DB:OrderDB"]); //my sql

                x.UseRabbitMQ(cfg =>
                {
                    cfg.HostName = Configuration["MQ:Host"];
                    cfg.VirtualHost = Configuration["MQ:VirtualHost"];
                    cfg.Port = Convert.ToInt32(Configuration["MQ:Port"]);
                    cfg.UserName = Configuration["MQ:UserName"];
                    cfg.Password = Configuration["MQ:Password"]; 
                }); // RabbitMQ

                x.UseDashboard(); // Dashboard

                // Below settings is just for demo
                x.FailedRetryCount = 2;
                x.FailedRetryInterval = 5;
            });

            // Swagger
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc(Configuration["Service:DocName"], new Info
                {
                    Title = Configuration["Service:Title"],
                    Version = Configuration["Service:Version"],
                    Description = Configuration["Service:Description"],
                    Contact = new Contact
                    {
                        Name = Configuration["Service:Contact:Name"],
                        Email = Configuration["Service:Contact:Email"]
                    }
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, Configuration["Service:XmlFile"]);
                s.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //cors跨域
            app.UseCors("cors");

            app.UseMvc();

            // CAP
            app.UseCap();

            // Swagger
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "doc/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/doc/{Configuration["Service:DocName"]}/swagger.json",
                    $"{Configuration["Service:Name"]} {Configuration["Service:Version"]}");
            });
        }
    }
}

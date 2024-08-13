using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Abp.AspNet.JwtBearer
{
    public class AbpAspNetJwtBearerModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            services.AddTransient<TokenCreateModel>();
            var configuration = services.GetConfiguration();
            // 注册jwt验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = configuration.GetValue<string>("JwtAuth:Audience"),
                    ValidIssuer = configuration.GetValue<string>("JwtAuth:Issuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetValue<string>("JwtAuth:SecurityKey")!)),
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        // 此处代码为终止.Net Core默认的返回类型和数据结果
                        context.HandleResponse();
                        var payload = "{\"ret\":203,\"err\":\"无登录信息或登录信息已失效，请重新登录。\"}";
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status203NonAuthoritative;
                        context.Response.WriteAsync(payload);
                        return Task.FromResult(0);
                    }
                };
            });
            base.ConfigureServices(context);
        }
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            base.OnApplicationInitialization(context);
        }
    }
}
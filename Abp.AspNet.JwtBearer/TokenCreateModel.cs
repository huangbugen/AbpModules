using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Abp.AspNet.JwtBearer
{
    public class TokenCreateModel
    {
        private readonly IConfiguration _configuration;

        public string UserId { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Ts { get; set; }

        public TokenCreateModel()
        {

        }

        public TokenCreateModel(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetToken(string customerNo, params Claim[] claims)
        {
            //从配置文件中获取token配置，并赋值到TokenManagement中
            TokenCreateModel tokenCreateModel = new TokenCreateModel
            {
                Audience = _configuration.GetValue<string>("JwtAuth:Audience")!,
                Issuer = _configuration.GetValue<string>("JwtAuth:Issuer")!,
                SecurityKey = _configuration.GetValue<string>("JwtAuth:SecurityKey")!,
                UserId = customerNo, //用户id可以从数据库中获取
                Ts = TimeSpan.FromHours(2),
            };
            return JwtCreator.CreateToken(tokenCreateModel, claims);
        }
    }
}
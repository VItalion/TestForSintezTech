using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForSintezTech
{
    public class AuthOptions
    {
        public const string ISSUER = "ApiService";
        public const string AUDIENCE = "http://localhost:63448/";
        const string KEY = "test_secret_token#322!";
        public const int LIFETIME = 60;

        public static int GetLifeTime()
        {
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            if (int.TryParse(configuration["TokenLifeTime"], out int tokentLiveTime))
                return tokentLiveTime;
            else
                return 60;
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

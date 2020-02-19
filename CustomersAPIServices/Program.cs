using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomersAPIServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //.UseHttpSys(options =>
            //{
            //    options.Authentication.Schemes = AuthenticationSchemes.NTLM;
            //    options.Authentication.AllowAnonymous = true;
            //    options.MaxConnections = 100;
            //    options.MaxRequestBodySize = 30000000;
            //})
                .UseStartup<Startup>()
                .UseUrls("http://localhost:9001");
    }
}

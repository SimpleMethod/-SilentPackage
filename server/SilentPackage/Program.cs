/*
 * Copyright  Micha³ M³odawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SilentPackage.Controllers;

namespace SilentPackage
{

    public class Program
    {
        readonly DatabaseManagement _databaseManagement = DatabaseManagement.GetInstance("main.db", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        public static void Main(string[] args)
        {
        #if !RELEASE
            CreateHostBuilder(args).Build().Run();
        #else
             var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();


        var hostUrl = configuration["hosturl"]; // add this line
        if (string.IsNullOrEmpty(hostUrl)) // add this line
            hostUrl = "http://0.0.0.0:5001"; // add this line


        var host = new WebHostBuilder()
            .UseKestrel()
            .UseUrls(hostUrl)   // // add this line
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .UseConfiguration(configuration)
            .Build();

        host.Run();
#endif

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}

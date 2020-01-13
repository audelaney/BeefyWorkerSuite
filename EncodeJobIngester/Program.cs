#nullable enable
using AppLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace EncodeJobIngester
{
	public class Program
	{
		public static void Main(string[] args)
		{
            if (args == null || args.Length == 0)
            {
                System.Console.WriteLine("Please enter '-c' with the location of the config file and run again");
                return;
            }

			if (args[0] == "-c") { AppConfigManager.SetConfig(args[1]); }

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.File(AppConfigManager.Instance.LogFilePath)
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSystemd()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Worker>();
				})
				.UseSerilog();
	}
}
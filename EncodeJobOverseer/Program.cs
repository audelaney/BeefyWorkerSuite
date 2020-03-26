using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using AppConfig;
using System;
using AppConfig.Models;

namespace EncodeJobOverseer
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args[0] == "-c" ||
				args[0] == "--config") { AppConfigManager.SetConfig(args[1]); }
			else if (args.Length == 1 ||
					args[0] == "--mock-logic") { AppConfigManager.SetConfig(BuildMockConfig()); }
			

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.File(AppConfigManager.Model.LogFilePath)
				.CreateLogger();

			CreateHostBuilder(args).Build().Run();
		}

		private static ConfigModel BuildMockConfig()
		{
			var cfg = new ConfigModel();
			cfg.Logic = LogicType.mock;
			return cfg;
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
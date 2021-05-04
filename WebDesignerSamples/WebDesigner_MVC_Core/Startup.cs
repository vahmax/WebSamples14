﻿using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

using GrapeCity.ActiveReports.Aspnetcore.Viewer;
using GrapeCity.ActiveReports.Aspnetcore.Designer;

using WebDesignerMvcCore.Services;
using WebDesignerMvcCore.Implementation;

namespace WebDesignerMvcCore
{
	public class Startup
	{
		private static readonly DirectoryInfo ResourcesRootDirectory = 
			new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "resources"));

		private static readonly DirectoryInfo TemplatesRootDirectory = 
			new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "templates"));

		private static readonly DirectoryInfo DataSetsRootDirectory = 
			new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "datasets"));

		
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddReporting()
				.AddDesigner()
				.AddSingleton<ITemplatesService>(new FileSystemTemplates(TemplatesRootDirectory))
				.AddSingleton<IDataSetsService>(new FileSystemDataSets(DataSetsRootDirectory))
				.AddMvc(options => options.EnableEndpointRouting = false)
				.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseReporting(config => config.UseFileStore(ResourcesRootDirectory));
			app.UseDesigner(config => config.UseFileStore(ResourcesRootDirectory, false));

			app.UseStaticFiles();
			app.UseMvc();
		}
	}
}

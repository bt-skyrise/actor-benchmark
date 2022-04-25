using System.Reflection;
using AkkaSut.Server;
using AkkaSut.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lcfg) =>
    lcfg
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.Seq(builder.Configuration["SeqUrl"])
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Service", Assembly.GetExecutingAssembly().GetName().Name));

builder.AddAkkaClusterSharding();
builder.Services.AddHostedService<AkkaClusterService>();

var app = builder.Build();
app.Run();
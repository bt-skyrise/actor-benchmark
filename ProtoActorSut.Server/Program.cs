using System.Reflection;
using Proto;
using ProtoActorSut.Contracts;
using ProtoActorSut.Server;
using ProtoActorSut.Shared;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, lcfg) =>
    lcfg
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.Seq(builder.Configuration["SeqUrl"])
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Service", Assembly.GetExecutingAssembly().GetName().Name));

var pingPongProps = Props
    .FromProducer(() => new PingPongActorActor((c, _) => new PingPongActor(c)));

builder.AddProtoActor((PingPongActorActor.Kind, pingPongProps));

var app = builder.Build();
app.Run();


using Microsoft.EntityFrameworkCore;
using ScintillaVitae.Database.Data;
using ScintillaVitae.Database.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddGrpc();

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new("CONNECTION_STRING is null");
builder.Services.AddDbContext<ChatContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Database Init
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChatContext>();
    context.Database.Migrate();
}

// gRPC Services Map
app.MapGrpcService<MessageService>();

app.Run();

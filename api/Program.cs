using api.Config;
using api.infrastructure.irc;
using api.infrastructure.repositories.twitch;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<TwitchRepository>();

builder.Services.Configure<TwitchConfig>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Machine) ?? "";
    options.Secret = Environment.GetEnvironmentVariable("Secret", EnvironmentVariableTarget.Machine) ?? "";
    options.GrantType = "client_credentials";
    options.RedirectUri = "http://localhost:3000";
    options.Scope = "chat:read";
    options.TokenType = "bearer";
    options.AccessToken = "";
    options.RefreshToken = "";
});

builder.Services.AddHostedService<TwitchChat>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
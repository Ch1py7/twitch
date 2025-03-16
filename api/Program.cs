using api.Application;
using api.Application.Services.TokenCache;
using api.Infrastructure.Irc;
using api.Infrastructure.Repositories.Twitch;

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
});

builder.Services.AddSingleton<TwitchChat>();
builder.Services.AddSingleton<ITokenCache, TokenCache>();

var app = builder.Build();

var twitchChat = app.Services.GetRequiredService<TwitchChat>();
_ = Task.Run(() => twitchChat.Start(CancellationToken.None));

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
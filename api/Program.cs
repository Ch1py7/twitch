using api.Application;
using api.Infrastructure.Irc;
using api.Infrastructure.Persistence;
using api.Infrastructure.Persistence.TokensRepository;
using api.Infrastructure.Persistence.TwitchRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<TwitchRepository>();

builder.Services.Configure<TwitchConfig>(options =>
{
    options.ClientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.Machine) ?? "";
    options.Secret = Environment.GetEnvironmentVariable("Secret", EnvironmentVariableTarget.Machine) ?? "";
});

builder.Services.AddDbContext<AppDbContext>()
.AddSingleton<ITokensRepository, TokensRepository>()
.AddSingleton<TwitchChat>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var twitchChat = app.Services.GetRequiredService<TwitchChat>();

await twitchChat.Start(CancellationToken.None);

app.Run();
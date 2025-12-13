using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Repositories;
using LifeForge.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure MongoDB settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// Add repositories
builder.Services.AddSingleton<IQuestRepository, QuestRepository>();
builder.Services.AddSingleton<IQuestRunRepository, QuestRunRepository>();
builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();
builder.Services.AddSingleton<IBuffRepository, BuffRepository>();
builder.Services.AddSingleton<IBuffInstanceRepository, BuffInstanceRepository>();
builder.Services.AddSingleton<IActionRepository, ActionRepository>();

// Add application services
builder.Services.AddScoped<IRewardApplicationService, RewardApplicationService>();
builder.Services.AddScoped<IBuffInstanceApplicationService, BuffInstanceApplicationService>();

// Add controllers
builder.Services.AddControllers();

// Add CORS for Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",
        policy =>
        {
            policy.WithOrigins("https://localhost:7295", "http://localhost:5009")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowBlazorWasm");

app.UseAuthorization();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

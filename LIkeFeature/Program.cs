using LIkeFeature.Data;
using LIkeFeature.Infrastructure;
using LIkeFeature.Interfaces.IRepository;
using LIkeFeature.Interfaces;
using LIkeFeature.Repositories;
using LIkeFeature.Services;
using StackExchange.Redis;
using LIkeFeature.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
if (!Directory.Exists(dataDirectory))
{
    Directory.CreateDirectory(dataDirectory);
}

var dbPath = Path.Join(builder.Environment.ContentRootPath, "Data", "articles.db");
builder.Services.AddDbContext<ArticleDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath};Mode=ReadWriteCreate;"));

builder.Services.AddMemoryCache();

//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfiguration>();
//    var configOptions = new ConfigurationOptions
//    {
//        AbortOnConnectFail = false,
//        ConnectTimeout = 5000,
//        ResponseTimeout = 5000,
//        ConnectRetry = 3,
//        KeepAlive = 5,
//        SyncTimeout = 5000
//    };

//    // Add Redis Sentinel endpoints if configured
//    if (redisConfig.UseSentinel)
//    {
//        foreach (var sentinel in redisConfig.Sentinels)
//        {
//            configOptions.EndPoints.Add(sentinel);
//        }
//        configOptions.ServiceName = redisConfig.MasterName;
//        configOptions.TieBreaker = "";  // Sentinel handles master election
//    }
//    else
//    {
//        configOptions.EndPoints.Add(redisConfig.ConnectionString);
//    }

//    if (!string.IsNullOrEmpty(redisConfig.Password))
//    {
//        configOptions.Password = redisConfig.Password;
//    }

//    return ConnectionMultiplexer.Connect(configOptions);
//});

builder.Services.AddScoped<IArticleLikeService, ArticleLikeService>();
builder.Services.AddScoped<IArticleLikeRepository, ArticleLikeRepository>();
//builder.Services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();
builder.Services.AddResponseCaching();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ArticleDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseAuthorization();

app.MapControllers();

app.Run();

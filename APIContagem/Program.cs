using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var connectionAppConfiguration =
    builder.Configuration.GetConnectionString("AppConfiguration");
var useAppConfiguration =
    !String.IsNullOrWhiteSpace(connectionAppConfiguration);
if (useAppConfiguration)
{
    builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(connectionAppConfiguration)
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("Mensagens:Saudacao").SetCacheExpiration(
                        TimeSpan.FromSeconds(20));
                });
            
            if (Convert.ToBoolean(builder.Configuration["UsingAzureKeyVault"]))
                options.ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
        }
    );
    /*builder.Host.ConfigureAppConfiguration(cfg => {
        cfg.AddAzureAppConfiguration(options =>
        {
            options.Connect(connectionAppConfiguration)
                .ConfigureRefresh(refresh =>
                {
                    refresh.Register("Mensagens:Aviso").SetCacheExpiration(
                        TimeSpan.FromSeconds(20));
                });
            
            if (Convert.ToBoolean(builder.Configuration["UsingAzureKeyVault"]))
                options.ConfigureKeyVault(kv =>
                    {
                        kv.SetCredential(new DefaultAzureCredential());
                    });
        });
    });*/

    builder.Services.AddAzureAppConfiguration();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!String.IsNullOrWhiteSpace(builder.Configuration.GetConnectionString("ApplicationInsights")))
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
    });
    
var app = builder.Build();

if (useAppConfiguration)
    app.UseAzureAppConfiguration();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
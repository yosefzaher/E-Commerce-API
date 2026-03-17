using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


var client = new AmazonSecretsManagerClient(RegionEndpoint.USEast1);
var request = new GetSecretValueRequest { SecretId = "ClickToBuy/Prod/DbCredentials" };
var response = client.GetSecretValueAsync(request).Result;

var secretData = JsonSerializer.Deserialize<Dictionary<string, object>>(response.SecretString);

if (secretData != null)
{
    foreach (var item in secretData)
    {
        builder.Configuration[item.Key] = item.Value?.ToString();
    }
}


builder.Services.AddDependencies(builder.Configuration);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "https://e-commerce-iti-six.vercel.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});


var app = builder.Build();

// Seed database with initial data
using (var scope = app.Services.CreateScope())
{
    await E_Commerce.API.Seed.DbSeeder.SeedAsync(scope.ServiceProvider);
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();


app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

// Configure Azure Text Analytics
builder.Services.AddAzureClients(clientBuilder =>
{
    var endpoint = builder.Configuration["AzureTextAnalytics:Endpoint"];
    var apiKey = builder.Configuration["AzureTextAnalytics:ApiKey"];

    if (string.IsNullOrEmpty(endpoint)) throw new ArgumentNullException("AzureTextAnalytics:Endpoint");
    if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException("AzureTextAnalytics:ApiKey");

    clientBuilder.AddClient<TextAnalyticsClient, TextAnalyticsClientOptions>(
        (_, _, _) => new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(apiKey)));
});

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
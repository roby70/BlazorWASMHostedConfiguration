using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


var app = builder.Build();





// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error");
}



app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// Place this as first to avoid Blazor client data to serve configuration
app.MapGet("/appsettings.server.json", async ctx => {
    var clientSettingsSection = app.Configuration.GetSection("ClientSettings");
    var clientJsonContent = Serialize(clientSettingsSection).ToString();
    await ctx.Response.WriteAsync(clientJsonContent);
});


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");



app.Run();

static JToken Serialize(IConfiguration config) {
    JObject obj = new JObject();
    foreach (var child in config.GetChildren()) {
        obj.Add(child.Key, Serialize(child));
    }

    if (!obj.HasValues && config is IConfigurationSection section)
        return new JValue(section.Value);

    return obj;
}
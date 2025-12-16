using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// 1. Disable reload-on-change for appsettings.json
// -----------------------------
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

// -----------------------------
// 2. Add services
// -----------------------------
builder.Services.AddRazorPages();

// -----------------------------
// 3. Build the app
// -----------------------------
var app = builder.Build();

// -----------------------------
// 4. Configure HTTP request pipeline
// -----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Disable static file watchers by using a physical file provider without change tracking
var fileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    ServeUnknownFileTypes = false,
    OnPrepareResponse = ctx => { } // no watcher events triggered
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

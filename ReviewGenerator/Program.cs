using ReviewGenerator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ITextGenerator, TextGenerator>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Initialize the text generator and process the text data.
try
{
    var textGen = app.Services.GetService<ITextGenerator>();
    if (textGen is { })
        textGen.TrainData(
            textGen.ProcessData(
                textGen.ReadData()));
}
catch (Exception)
{
    Environment.Exit(1);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();

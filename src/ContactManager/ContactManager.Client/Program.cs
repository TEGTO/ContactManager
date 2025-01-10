using ContactManager.Client.Services;

var builder = WebApplication.CreateBuilder(args);

#region Project Services

builder.Services.AddSingleton<IContactService, ContactService>();
builder.Services.AddSingleton<IFileValidationService, FileValidationService>();

#endregion

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var app = builder.Build();

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

await app.RunAsync();

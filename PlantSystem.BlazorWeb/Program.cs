using Blazorise;
using Blazorise.Tailwind;
using PlantSystem.BlazorWeb.Data;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlantSystem.BlazorWeb.Components;
using PlantSystem.BlazorWeb.Components.Account;
using Microsoft.AspNetCore.Components.Authorization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();
string connection_string = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection_string));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager().AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
}).AddTailwindProviders().AddFontAwesomeIcons();
WebApplication app = builder.Build();
if(app.Environment.IsDevelopment() is true)
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts(); //The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();
app.Run();
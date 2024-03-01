using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Movies.Interface;
using Movies.Service;
using Movies.Security;
using Streamit_movie_mvc.Models.Domain;
using Streamit_movie_mvc.Utilities;
using System.Text;
using Movies.Repository;
using Streamit_movie_mvc.Services.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ISeasonService, SeasonService>();
builder.Services.AddScoped<IEpisodeService, EpisodeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMovieCategoryService, MovieCategoryService>();
builder.Services.AddScoped<JWTGenerator, JWTConfig>();
builder.Services.AddScoped<IAuthentication, Authentication>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<INationService, NationService>();
builder.Services.AddScoped<ICastService, CastService>();
builder.Services.AddDbContext<MOVIESContext>();
builder.Services.AddFluentEmail(); 

builder.Services.AddHttpContextAccessor();

//set up watch dog (logs)
/*builder.Services.AddWatchDogServices(opt =>
{
    opt.IsAutoClear = true;
    opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Quarterly;
    opt.SetExternalDbConnString = builder.Configuration.GetConnectionString("Hangfire");
    opt.DbDriverOption = WatchDogDbDriverEnum.Mongo;
});*/

//set up configuration JWT

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JWTSetting:Issuer"],
        ValidAudience = builder.Configuration["JWTSetting:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSetting:Securitykey"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(Constraint.RoleUser.USER, policy => policy.RequireClaim("Role", Constraint.RoleUser.USER));
    opt.AddPolicy(Constraint.RoleUser.ADMIN, policy => policy.RequireClaim("Role", Constraint.RoleUser.ADMIN));
});

var app = builder.Build();

// Configure cors
app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//inject middleware watch dog logs
/*app.UseWatchDogExceptionLogger();
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "admin";
    opt.WatchPagePassword = "123";
    opt.Blacklist = "GET:/Home/Index, GET:/Home/Genres";
});*/

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

/*app.UseExceptionHandler("/Error/{0}");

app.UseStatusCodePagesWithRedirects("/Error/{0}");*/

app.UseAuthorization();

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Genres}/{id?}"
);*/

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

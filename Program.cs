using Microsoft.EntityFrameworkCore;
using To_Do.Authentication;
using To_Do.Authntication;
using To_Do.Data;
using To_Do.Data.Repositery;
using To_Do.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();    //json patch
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwtsettings")); //Ioption pattern
builder.Services.AddScoped<ITokenGenerater, TokenGenerater>();//add services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepositery, UserRepositery>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddDbContext<ToDoContext>(options =>             //add db context
   options.UseNpgsql(builder.Configuration.GetConnectionString("ToDoConection")));
builder.Services.AddAutoMapper(typeof(Program)); 
//builder.Services.AddSingleton<IUserServices, UserServices>(); singletone is for In memory HeHaHa
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "To-Do API",
        Version = "v1",
        Description = "API for managing To Do items",
    });
c.DocInclusionPredicate((docname, apiDesc) =>
{
    return docname == "v1"; // Grouping by version

});
    c.OrderActionsBy(endpoint => endpoint.GroupName ?? "");  // this mean each point in contrller with group name will be ordered


});

  // Register AutoMapper

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

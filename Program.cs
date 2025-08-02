using Microsoft.EntityFrameworkCore;
using To_Do.Authentication;
using To_Do.Authntication;
using To_Do.Data;
using To_Do.Data.Repositery;
using To_Do.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwtsettings"));
builder.Services.AddScoped<ITokenGenerater, TokenGenerater>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepositery, UserRepositery>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddDbContext<ToDoContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("ToDoConection")));
//builder.Services.AddSingleton<IUserServices, UserServices>(); singletone is for In memory HeHaHa
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

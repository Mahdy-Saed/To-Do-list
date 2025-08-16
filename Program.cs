using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using To_Do.Authentication;
using To_Do.Authntication;
using To_Do.Data;
using To_Do.Data.Repositery;
using To_Do.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ تمكين عرض معلومات حساسة في الأخطاء - مهم جداً في مرحلة التطوير
IdentityModelEventSource.ShowPII = true;

// ✅ إعدادات Logging تفصيلي
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication.JwtBearer", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.IdentityModel", LogLevel.Trace);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();    // JSON Patch
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwtsettings")); // IOptions pattern
builder.Services.AddScoped<ITokenGenerater, TokenGenerater>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepositery, UserRepositery>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddDbContext<ToDoContext>(options =>
   options.UseNpgsql(builder.Configuration.GetConnectionString("ToDoConection")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ITaskRepositer, TaskRepositer>();
builder.Services.AddScoped<ITaskServices, TaskServices>();
// ✅ إعدادات JWT
var jwtSettings = builder.Configuration.GetSection("jwtsettings");
var key = jwtSettings["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
        ClockSkew = TimeSpan.Zero
    };

    // ✅ تمكين التفاصيل في حالة فشل المصادقة
    options.IncludeErrorDetails = true;

    // ✅ تسجيل الأحداث لمزيد من التشخيص
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"🔴 Authentication failed: {context.Exception.GetType().Name} - {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"✅ Token validated for: {context.Principal.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "To-Do API",
        Version = "v1",
        Description = "API for managing To Do items",
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Just Enter Your Token Here"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    c.DocInclusionPredicate((docname, apiDesc) =>
    {
        return docname == "v1";
    });

    c.OrderActionsBy(endpoint => endpoint.GroupName ?? "");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // ✅ مهم قبل Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();

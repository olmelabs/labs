using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

//var bytes = Base64UrlEncoder.DecodeBytes("UnguQXV0aFNlcnZlclsyMDE3XS5Kd3QuUmVzcG9uc2U=");
//var signingKey = new SymmetricSecurityKey(bytes);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(o =>
//{
//    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(o =>
//{
//    o.SaveToken = true;
//    o.RequireHttpsMetadata = false;
//    o.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateLifetime = true,

//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
//        ValidateIssuerSigningKey = true,

//        ValidateIssuer = true,
//        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],

//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["JWT:ValidAudience"]
//    };
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllers();

app.Run();

global using back.Services;
global using back.Models;
global using Newtonsoft.Json;
global using back.securite;
global using back.ImportModels;
global using back.ExportModels;
global using System.Text;
global using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// connection a la base de donnée
builder.Services.AddDbContext<GestionMdpContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("ionos")));

ConfigurationManager config = builder.Configuration;

string cleSecrete = config["token:cleSecrete"];
var cle = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cleSecrete));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            // se qu'on veut valider
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            // valider les données
            ValidIssuer = config["token:issuer"],
            ValidAudience = config["token:audience"],
            IssuerSigningKey = cle
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    // genere un XML et permet de voir le sumary dans swagger pour chaque fonctions dans le controller
    string xmlNomFichier = $"{ Assembly.GetExecutingAssembly().GetName().Name }.xml";
    //swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlNomFichier));
});

builder.Services.AddCors(options => options.AddPolicy("CORS", c => c.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // creer un JSON pour swagger
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "mon API V1");

        // cache les shemas des classes en bas de page
        c.DefaultModelsExpandDepth(-1);
    });
}


app.UseCors("CORS");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Scaffold-DbContext "Data Source=DESKTOP-U41J905\SQLEXPRESS;Initial Catalog=GestionMdp;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
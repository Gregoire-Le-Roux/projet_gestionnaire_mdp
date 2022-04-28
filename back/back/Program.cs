global using back.Services;
global using back.Models;
global using Newtonsoft.Json;
global using back.securite;
global using back.ImportModels;
global using back.ExportModels;
global using System.Text;

using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// connection a la base de donnée
builder.Services.AddDbContext<GestionMdpContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("ionos")));

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

app.UseAuthorization();

app.MapControllers();

app.Run();

// Scaffold-DbContext "Data Source=DESKTOP-U41J905\SQLEXPRESS;Initial Catalog=GestionMdp;Integrated Security=True" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
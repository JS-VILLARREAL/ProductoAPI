using Microsoft.EntityFrameworkCore;
using WebAPIProducto;
using WebAPIProducto.Data;
using WebAPIProducto.Repository;
using WebAPIProducto.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(
    option => option.UseSqlite(connectionString)
);*/

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Agregar un contexto de base de datos al contenedor de inyección de dependencia usando el método
`AddDbContext`. La clase `DataContext` se utiliza como contexto para la base de datos. El método
`UseSqlServer` especifica que se usará SQL Server como proveedor de la base de datos y la cadena de
conexión se recupera del archivo de configuración usando la clave "DefaultConnection". */
builder.Services.AddDbContext<DataContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

/*` agrega AutoMapper al contenedor de inyección de dependencia.
El parámetro `typeof(mappingConfig)` especifica la clase que contiene la
configuración de AutoMapper. Esto permite que AutoMapper se use en toda la aplicación para mapear
entre diferentes tipos de objetos. */
builder.Services.AddAutoMapper(typeof(mappingConfig));

/*` está registrando la clase `ProductRepository` como la implementación de la interfaz `IProductRepository` en el contenedor de inyección de dependencia. Esto permite que otras clases en la aplicación usen la interfaz
`IProductRepository` sin tener que conocer los detalles de implementación de `ProductRepository`. El
tiempo de vida de `Scoped` especifica que se creará una nueva instancia de `ProductRepository` para
cada solicitud HTTP. */
builder.Services.AddScoped<IProductRepository, ProductRepository>();

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

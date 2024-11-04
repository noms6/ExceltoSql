using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý ekleniyor
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controller'larý ekleyin (API için gerekli)
builder.Services.AddControllers();

// Swagger'ý ekleyin
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Uygulamanýn middleware yapýlandýrmasý
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Swagger arayüzünün URL'si
});


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers(); // API controller'larýný uygulamaya dahil et

app.Run();

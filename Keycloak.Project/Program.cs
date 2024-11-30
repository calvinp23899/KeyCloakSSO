using Keycloak.Project.Interfaces;
using Keycloak.Project.ServiceExtensions;
using Keycloak.Project.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IKeyCloakClient, KeycloakClientService>();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureSwaggerGen();
builder.Services.ConfigureAuthorize(builder.Configuration); var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using AuthService.Common.Postgres;
using AuthService.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJwt(builder.Configuration);

builder.Services.AddPostgres(builder.Configuration).AddPostgresRepository();

builder.Services.RegisterServices().ConfigureOptions(builder.Configuration, builder.Environment);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseErrorHandlerMiddleware();
app.UseAuthorization();

app.MapControllers();

app.Run();
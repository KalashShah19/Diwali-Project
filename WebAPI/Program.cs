var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
// builder.Services.AddCors(policy => {
//     policy.AddPolicy("CorsPolicy", rules => {
//         rules.AllowAnyMethod();
//         rules.AllowAnyOrigin();
//     });
// });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5255") // Allow only the origin you need
              .AllowAnyMethod()                     // Allows all HTTP methods (GET, POST, etc.)
              .AllowAnyHeader();                    // Allows all headers, including Content-Type
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.MapControllers();
//app.UseCors("CorsPolicy");
app.UseCors("AllowSpecificOrigin"); 

app.Run();

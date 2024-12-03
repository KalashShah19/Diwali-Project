using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;                // Secure the cookie
    options.Cookie.IsEssential = true;            // Required for GDPR compliance
});

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", rules =>
    {
        rules.AllowAnyMethod();
        rules.AllowAnyHeader();
        rules.AllowAnyOrigin(); // Restrict this in production!
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseSession(); // Ensure this is before `MapControllers()`
app.MapControllers();

app.Run();

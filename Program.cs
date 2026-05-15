using acepickle_chat_api.ChatHub;
using acepickle_chat_api.Dependencies;

var builder = WebApplication.CreateBuilder(args);

// Configure Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAppCors(builder.Configuration);
builder.Services.AddAppSwagger();
builder.Services.AddAppDbContext(builder.Configuration);
builder.Services.AddServiceDependency();

var app = builder.Build();

app.UseAppEnvironment();
app.UseAppMiddleware();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();

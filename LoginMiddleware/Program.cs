using LoginMiddleware.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseLoginMiddleware();

app.Run(async context =>
{
    await context.Response.WriteAsync("No Response");
});

app.Run();

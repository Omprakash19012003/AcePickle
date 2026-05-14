
namespace softskiller_chat_api.Dependencies
{
public static class ApplicationBuilder
{
    public static IApplicationBuilder UseAppEnvironment(this IApplicationBuilder app)
    {
        var env = app.ApplicationServices.GetService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }

    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseCors("defaultCorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
}
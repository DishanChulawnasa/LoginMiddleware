using Microsoft.Extensions.Primitives;

namespace LoginMiddleware.Middleware
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/" && httpContext.Request.Method == "POST")
            {
                StreamReader streamReader = new StreamReader(httpContext.Request.Body);
                string body = await streamReader.ReadToEndAsync();

                Dictionary<string, StringValues> queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);
                string? email = null;
                string? password = null;

                if (queryDictionary.ContainsKey("email"))
                {
                    email = Convert.ToString(queryDictionary["email"][0]);
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                    await httpContext.Response.WriteAsync("Invalid input for 'email'\n");
                }

                if (queryDictionary.ContainsKey("password"))
                {
                    password = Convert.ToString(queryDictionary["password"][0]);
                }
                else
                {
                    if (httpContext.Response.StatusCode == 200)
                    {
                        httpContext.Response.StatusCode = 400;
                    }
                    await httpContext.Response.WriteAsync("Invalid input for 'password'\n");
                }

                if(string.IsNullOrEmpty(email) == false && string.IsNullOrEmpty(password) == false)
                {
                    string passEmail = "admin@example.com";
                    string passPassword = "admin1234";
                    bool isValidLogin;

                    if (email == passEmail && password == passPassword)
                    {
                        isValidLogin = true;
                    }
                    else
                    {
                        isValidLogin = false;
                    }

                    if (isValidLogin)
                    {
                        await httpContext.Response.WriteAsync("Successful login\n");
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 400;
                        await httpContext.Response.WriteAsync("Invalid login\n");
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
        }
    }

    public static class LoginMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}

using FluentValidation;
using OrcShackApi.Core.Helper;
using OrcShackApi.Core.Models;
using System.Text.Json;

namespace OrcShackApi.Web.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly IValidator<UserDto> _userDtoValidator = new UserDtoValidator();
        private static readonly IValidator<PasswordUpdateDto> _passwordUpdateDtoValidator = new PasswordUpdateDtoValidator();
        private static readonly IValidator<UpdateDishRatingDto> _updateDishRatingDtoValidator = new UpdateDishRatingDtoValidator();

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/users/register") &&
                context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var userDto = await JsonSerializer.DeserializeAsync<UserDto>(context.Request.Body);

                var validationResult = _userDtoValidator.Validate(userDto);
                if (!validationResult.IsValid)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync(JsonSerializer.Serialize(validationResult.Errors));
                    return;
                }

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }
            else if (context.Request.Path.StartsWithSegments("/api/users/updatePassword") &&
                     context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                var passwordUpdateDto = await JsonSerializer.DeserializeAsync<PasswordUpdateDto>(context.Request.Body);

                var validationResult = _passwordUpdateDtoValidator.Validate(passwordUpdateDto);
                if (!validationResult.IsValid)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync(JsonSerializer.Serialize(validationResult.Errors));
                    return;
                }

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }
            else if (context.Request.Path.StartsWithSegments("/api/dishes/rating") &&
                     context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                var updateDishRatingDto = await JsonSerializer.DeserializeAsync<UpdateDishRatingDto>(context.Request.Body);

                var validationResult = _updateDishRatingDtoValidator.Validate(updateDishRatingDto);
                if (!validationResult.IsValid)
                {
                    context.Response.StatusCode = 400; // Bad Request
                    await context.Response.WriteAsync(JsonSerializer.Serialize(validationResult.Errors));
                    return;
                }

                // Reset the request body stream position so the next middleware can read it
                context.Request.Body.Position = 0;
            }

            await _next(context);
        }
    }

}

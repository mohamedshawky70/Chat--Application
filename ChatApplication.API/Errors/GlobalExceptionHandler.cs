using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Errors;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;
	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		_logger.LogError(exception, "Something went wrong {Message}", exception.Message);
		var problemDetails = new ProblemDetails()
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Internal server error",
			Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1" //Link to RFC for 500 error to explain the error
		};
		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError; //Set all responses to 500
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken); //Write the problem details as JSON response
		return true;
	}
}


using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.API.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger = logger;

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		_logger.LogError(exception, "Something went wrong {Message}", exception.Message);// Log the exception in cmd or any logging provider not in response
		var problemDetails = new ProblemDetails()
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Internal server error",
			Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1" //Link to RFC for 500 error to explain the error
		};
		//var result=Result.Failure<bool>(UserError.Ex); //If you don't want problemDetails
		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError; //Set all responses to 500
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken); //Write the problem details as JSON response
		return true;
	}
}


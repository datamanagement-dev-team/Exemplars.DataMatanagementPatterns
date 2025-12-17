using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Api.ActionResults
{
	internal class InvalidModelStateActionResult : IActionResult
	{
		private readonly ILogger<InvalidModelStateActionResult> _logger;

		public InvalidModelStateActionResult(ILogger<InvalidModelStateActionResult> logger)
		{
			_logger = logger;
		}

		public async Task ExecuteResultAsync(ActionContext actionContext)
		{
			var errors = actionContext.ModelState
				.SelectMany(_modelStateEntry => _modelStateEntry.Value!.Errors
					.Select(_modelError => new
					{
						Property = _modelStateEntry.Key,
						_modelError.ErrorMessage,
					})
					.ToList())
				.ToList();

			var errorMessage = JsonConvert.SerializeObject(errors);

			var exeption = new Exception(errorMessage);

			_logger.LogError(exeption, string.Empty);

			actionContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

			await actionContext.HttpContext.Response.WriteAsJsonAsync(errorMessage, actionContext.HttpContext.RequestAborted);
		}
	}
}

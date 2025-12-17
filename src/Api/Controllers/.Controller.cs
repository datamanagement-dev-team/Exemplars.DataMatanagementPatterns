using BlueBrown.Data.DataManagementPatterns.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlueBrown.Data.DataManagementPatterns.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public abstract class Controller : ControllerBase
	{
		private readonly ILogger<Controller> _logger;

		protected Controller(ILogger<Controller> logger)
		{
			_logger = logger;
		}

		protected void LogRequest(Request request)
		{
			_logger.LogInformation("HttpRequest content : {0}", JsonConvert.SerializeObject(request));
		}

		protected void LogResponse(Response response)
		{
			_logger.LogInformation("HttpResponse content : {0}", JsonConvert.SerializeObject(response));
		}
	}
}

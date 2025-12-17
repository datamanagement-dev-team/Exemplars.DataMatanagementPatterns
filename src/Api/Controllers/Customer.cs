using BlueBrown.Data.DataManagementPatterns.Api.HttpResults;
using BlueBrown.Data.DataManagementPatterns.Shared.Customer;
using Microsoft.AspNetCore.Mvc;

namespace BlueBrown.Data.DataManagementPatterns.Api.Controllers
{
	public class CustomerController : Controller
	{
		public CustomerController(ILogger<Controller> logger)
			: base(logger)
		{ }

		[HttpGet("[action]")]
		public HttpResult<GetCustomerResult> Get(long customerId, CancellationToken cancellationToken = default)
		{
			var response = Shared.Response<GetCustomerResult>.Create(statusCode: 200, new GetCustomerResult(customerId));

			LogResponse(response);

			return HttpResult<GetCustomerResult>.Create(response);
		}

		[HttpPost("[action]")]
		public HttpResult Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken = default)
		{
			LogRequest(request);

			var response = Shared.Response.Create(statusCode: 200);

			LogResponse(response);

			return HttpResult.Create(response);
		}
	}
}

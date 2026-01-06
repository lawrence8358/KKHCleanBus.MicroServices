using KKHCleanBus.MicroServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace KKHCleanBus.MicroServices.Controllers;

[ApiController]
public class ApiBaseController : ControllerBase
{
    public override OkObjectResult Ok([ActionResultObjectValue] object? value)
    {
        var response = new ResponseModel
        {
            Type = ResponseType.Ok.ToString(),
            Data = value
        };
        return base.Ok(response);
    }

    protected OkObjectResult Ok(object? value, object? expando)
    {
        var response = new ResponseExpandoModel
        {
            Type = ResponseType.Ok.ToString(),
            Data = value,
            Expando = expando
        };
        return base.Ok(response);
    }

    protected IActionResult BadRequest(List<string> error)
    {
        var response = new ResponseModel
        {
            Type = ResponseType.BadRequest.ToString(),
            Errors = error
        };
        return new JsonResult(response) { StatusCode = StatusCodes.Status400BadRequest };
    }
}

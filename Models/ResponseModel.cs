namespace KKHCleanBus.MicroServices.Models;

public enum ResponseType
{
    InvalidModel,
    BadRequest,
    Exception,
    Upgrade,
    Ok
}

public class ResponseModel
{
    public string Type { get; set; } = ResponseType.Ok.ToString();
    public object? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class ResponseExpandoModel : ResponseModel
{
    public object? Expando { get; set; }
}

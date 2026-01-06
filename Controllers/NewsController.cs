using System.Net;
using KKHCleanBus.MicroServices.Data.Entities;
using KKHCleanBus.MicroServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace KKHCleanBus.MicroServices.Controllers;

[Route("api/[controller]")]
public class NewsController : ApiBaseController
{
    private readonly NewsService _service;

    public NewsController(NewsService service)
    {
        _service = service;
    }

    /// <summary>
    /// 取得前5則新聞標題
    /// </summary>
    [HttpGet, Route("Topic")]
    public IActionResult GetTopic()
    {
        var result = _service.GetAll().Take(5).Select(x => new
        {
            x.Id,
            x.Title
        });
        return Ok(result);
    }

    /// <summary>
    /// 取得所有新聞
    /// </summary>
    [HttpGet, Route("All")]
    [ProducesResponseType(typeof(IEnumerable<News>), (int)HttpStatusCode.OK)]
    public IActionResult GetAll()
    {
        var result = _service.GetAll();
        return Ok(result);
    }

    /// <summary>
    /// 取得單一新聞
    /// </summary>
    [HttpGet, Route("{id}")]
    [ProducesResponseType(typeof(News), (int)HttpStatusCode.OK)]
    public IActionResult Get(Guid id)
    {
        var result = _service.Get(id);
        return Ok(result);
    }
}

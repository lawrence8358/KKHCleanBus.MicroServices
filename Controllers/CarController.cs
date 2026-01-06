using KKHCleanBus.MicroServices.Models;
using KKHCleanBus.MicroServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace KKHCleanBus.MicroServices.Controllers;

[Route("api/[controller]")]
public class CarController : ApiBaseController
{
    private readonly ArrivalTimeService _arrivalTimeService;

    public CarController(ArrivalTimeService arrivalTimeService)
    {
        _arrivalTimeService = arrivalTimeService;
    }

    /// <summary>
    /// 取得垃圾車沿線資訊 (舊版 API)
    /// </summary>
    [HttpGet, Route("route/{carLicence}")]
    public IActionResult GetRouteInfo(string carLicence)
    {
        var result = _arrivalTimeService.GetArrivalTimeView(carLicence);
        return FormatResult(result);
    }

    /// <summary>
    /// 取得垃圾車沿線資訊 (依時段)
    /// </summary>
    [HttpGet, Route("route/{carLicence}/{type}")]
    public IActionResult GetRouteInfo(string carLicence, ArrivalTimeType type)
    {
        // ErrorCode E01 : 參數不正確，本模式不支援指定分鐘
        if (type != ArrivalTimeType.Morning && type != ArrivalTimeType.Afternoon && type != ArrivalTimeType.Evening)
            return BadRequest(new List<string> { "E01" });

        var result = _arrivalTimeService.GetArrivalTimeView(carLicence, type);
        return ConvertToWeekendResult(result);
    }

    /// <summary>
    /// 取得垃圾車沿線資訊 (進階查詢)
    /// </summary>
    [HttpPost, Route("route")]
    public IActionResult GetRouteInfo(ArrivalTimeAdvQueryModel model)
    {
        // ErrorCode E01 : 參數不正確，必須指定幾分鐘內的資料
        if ((model.Type == ArrivalTimeType.Specify30 ||
             model.Type == ArrivalTimeType.Specify60 ||
             model.Type == ArrivalTimeType.Specify120 ||
             model.Type == ArrivalTimeType.Specify180) && !model.InMinutes.HasValue)
        {
            return BadRequest(new List<string> { "E01" });
        }

        var result = _arrivalTimeService.GetArrivalTimeView(model);

        // 轉換星期幾
        int? weekendDay = null;
        switch (model.Type)
        {
            case ArrivalTimeType.MonMorning:
            case ArrivalTimeType.MonAfternoon:
            case ArrivalTimeType.MonEvening:
                weekendDay = 1;
                break;
            case ArrivalTimeType.TueMorning:
            case ArrivalTimeType.TueAfternoon:
            case ArrivalTimeType.TueEvening:
                weekendDay = 2;
                break;
            case ArrivalTimeType.WedMorning:
            case ArrivalTimeType.WedAfternoon:
            case ArrivalTimeType.WedEvening:
                weekendDay = 3;
                break;
            case ArrivalTimeType.ThuMorning:
            case ArrivalTimeType.ThuAfternoon:
            case ArrivalTimeType.ThuEvening:
                weekendDay = 4;
                break;
            case ArrivalTimeType.FriMorning:
            case ArrivalTimeType.FriAfternoon:
            case ArrivalTimeType.FriEvening:
                weekendDay = 5;
                break;
            case ArrivalTimeType.SatMorning:
            case ArrivalTimeType.SatAfternoon:
            case ArrivalTimeType.SatEvening:
                weekendDay = 6;
                break;
            case ArrivalTimeType.SunMorning:
            case ArrivalTimeType.SunAfternoon:
            case ArrivalTimeType.SunEvening:
                weekendDay = 0;
                break;
        }

        return ConvertToWeekendResult(result, weekendDay);
    }

    private IActionResult ConvertToWeekendResult(List<ArrivalTimeView>? model, int? weekendDay = null)
    {
        var weekend = (int)DateTime.Today.DayOfWeek;
        string? weekendString = null;

        if (weekendDay.HasValue) weekend = weekendDay.Value;

        weekendString = weekend switch
        {
            0 => "周日",
            1 => "周一",
            2 => "周二",
            3 => "周三",
            4 => "周四",
            5 => "周五",
            6 => "周六",
            _ => null
        };

        if (model != null && model.Count == 0) model = null;

        return Ok(model, new ArrivalTimeExpandoResponse { Weekend = weekendString });
    }

    private IActionResult FormatResult(List<ArrivalTimeView>? result)
    {
        if (result != null && result.Count == 0) result = null;
        return Ok(result);
    }
}

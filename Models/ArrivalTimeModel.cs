namespace KKHCleanBus.MicroServices.Models;

public enum ArrivalTimeType
{
    Morning = 0,
    Afternoon = 1,
    Evening = 2,
    Specify30 = 3,
    Specify60 = 4,
    Specify120 = 5,
    Specify180 = 6,

    MonMorning = 10,
    MonAfternoon = 11,
    MonEvening = 12,
    TueMorning = 20,
    TueAfternoon = 21,
    TueEvening = 22,
    WedMorning = 30,
    WedAfternoon = 31,
    WedEvening = 32,
    ThuMorning = 40,
    ThuAfternoon = 41,
    ThuEvening = 42,
    FriMorning = 50,
    FriAfternoon = 51,
    FriEvening = 52,
    SatMorning = 60,
    SatAfternoon = 61,
    SatEvening = 62,
    SunMorning = 70,
    SunAfternoon = 71,
    SunEvening = 72
}

public class ArrivalTimeAdvQueryModel
{
    /// <summary>緯度</summary>
    public decimal Lat { get; set; }

    /// <summary>經度</summary>
    public decimal Lng { get; set; }

    /// <summary>距離(公尺)</summary>
    public int Distance { get; set; }

    /// <summary>類型</summary>
    public ArrivalTimeType? Type { get; set; }

    /// <summary>幾分鐘內的班次</summary>
    public int? InMinutes { get; set; }
}

public class ArrivalTimeExpandoResponse
{
    /// <summary>星期幾</summary>
    public string? Weekend { get; set; }
}

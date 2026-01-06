using KKHCleanBus.MicroServices.Data;
using KKHCleanBus.MicroServices.Data.Entities;
using KKHCleanBus.MicroServices.Models;

namespace KKHCleanBus.MicroServices.Services;

public class ArrivalTimeService
{
    private readonly CleanBusDbContext _dbContext;

    public ArrivalTimeService(CleanBusDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>取得垃圾車沿線資訊</summary>
    /// <param name="carLicence">車號</param>
    public List<ArrivalTimeView>? GetArrivalTimeView(string carLicence)
    {
        // 使用 AsEnumerable() 切換到 client-side 排序，因為 SQLite 不支援 DateTimeOffset 的 ORDER BY
        var parent = _dbContext.ArrivalTime.AsEnumerable()
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefault();
        if (parent != null)
        {
            var query = GetArrivalTimeViewQuery(parent.Id);
            query = query.Where(x => x.CarLicence == carLicence || x.ReplaceCarLicence == carLicence);
            return query.OrderBy(x => x.TimeRange).ToList();
        }

        return null;
    }

    /// <summary>取得垃圾車沿線資訊</summary>
    /// <param name="carLicence">車號</param>
    /// <param name="type">類型</param>
    public List<ArrivalTimeView>? GetArrivalTimeView(string carLicence, ArrivalTimeType type)
    {
        var parent = _dbContext.ArrivalTime.AsEnumerable()
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefault();
        if (parent != null)
        {
            var query = GetArrivalTimeViewQuery(parent.Id);
            query = query.Where(x => x.CarLicence == carLicence || x.ReplaceCarLicence == carLicence);
            query = QueryByType(query, type, null);
            return query.OrderBy(x => x.TimeRange).ToList();
        }

        return null;
    }

    /// <summary>取得垃圾車沿線資訊 (進階查詢)</summary>
    public List<ArrivalTimeView>? GetArrivalTimeView(ArrivalTimeAdvQueryModel model)
    {
        var parent = _dbContext.ArrivalTime.AsEnumerable()
            .OrderByDescending(x => x.CreatedDate)
            .FirstOrDefault();
        if (parent == null) return null;

        // 轉換列舉類型
        int? weekendDay = null;
        ArrivalTimeType? type = model.Type;

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
                weekendDay = 7;
                break;
        }

        switch (model.Type)
        {
            case ArrivalTimeType.MonMorning:
            case ArrivalTimeType.TueMorning:
            case ArrivalTimeType.WedMorning:
            case ArrivalTimeType.ThuMorning:
            case ArrivalTimeType.FriMorning:
            case ArrivalTimeType.SatMorning:
            case ArrivalTimeType.SunMorning:
                type = ArrivalTimeType.Morning;
                break;

            case ArrivalTimeType.MonAfternoon:
            case ArrivalTimeType.TueAfternoon:
            case ArrivalTimeType.WedAfternoon:
            case ArrivalTimeType.ThuAfternoon:
            case ArrivalTimeType.FriAfternoon:
            case ArrivalTimeType.SatAfternoon:
            case ArrivalTimeType.SunAfternoon:
                type = ArrivalTimeType.Afternoon;
                break;

            case ArrivalTimeType.MonEvening:
            case ArrivalTimeType.TueEvening:
            case ArrivalTimeType.WedEvening:
            case ArrivalTimeType.ThuEvening:
            case ArrivalTimeType.FriEvening:
            case ArrivalTimeType.SatEvening:
            case ArrivalTimeType.SunEvening:
                type = ArrivalTimeType.Evening;
                break;
        }

        var query = GetArrivalTimeViewQuery(parent.Id, model.Lat, model.Lng, weekendDay);
        query = query.Where(x => x.Distance <= model.Distance);
        query = QueryByType(query, type, model.InMinutes);

        return query.OrderBy(x => x.TimeRange).ToList();
    }

    private IEnumerable<ArrivalTimeView> GetArrivalTimeViewQuery(Guid parentId, decimal? lat = null, decimal? lng = null, int? weekendDay = null)
    {
        var weekend = (int)DateTime.Today.DayOfWeek;
        if (weekend == 0) weekend = 7;

        if (weekendDay.HasValue) weekend = weekendDay.Value;

        var details = _dbContext.ArrivalTimeDetail
            .Where(x => x.ParentId == parentId && x.CrossDeptName == null)
            .ToList();

        var result = details.Select(x =>
        {
            var timeRange = GetTimeRangeByWeekend(x, weekend);
            var (startMinute, endMinute) = ParseTimeRange(timeRange);

            int? distance = null;
            if (lat.HasValue && lng.HasValue)
            {
                distance = (int)CalculateDistance((double)lat.Value, (double)lng.Value, (double)x.Latitude, (double)x.Longitude);
            }

            return new ArrivalTimeView
            {
                Seq = x.Seq,
                CarLicence = x.CarLicence,
                Caption = x.Caption,
                DeptName = x.DeptName,
                VillageName = x.VillageName,
                TaskType = x.TaskType,
                Longitude = x.Longitude,
                Latitude = x.Latitude,
                RecycleDay = x.RecycleDay,
                TimeRange = timeRange,
                StartMinute = startMinute,
                EndMinute = endMinute,
                ReplaceCarLicence = x.ReplaceCarLicence,
                ReplaceLongitude = x.ReplaceLongitude,
                ReplaceLatitude = x.ReplaceLatitude,
                Distance = distance
            };
        })
        .Where(x => x.TimeRange != null);

        return result;
    }

    private static string? GetTimeRangeByWeekend(ArrivalTimeDetail detail, int weekend)
    {
        return weekend switch
        {
            1 => detail.GDay1,
            2 => detail.GDay2,
            3 => detail.GDay3,
            4 => detail.GDay4,
            5 => detail.GDay5,
            6 => detail.GDay6,
            7 => detail.GDay7,
            _ => null
        };
    }

    private static (int startMinute, int endMinute) ParseTimeRange(string? timeRange)
    {
        if (string.IsNullOrEmpty(timeRange) || timeRange.Length < 11) return (0, 0);

        try
        {
            var startHour = int.Parse(timeRange.Substring(0, 2));
            var startMin = int.Parse(timeRange.Substring(3, 2));
            var endHour = int.Parse(timeRange.Substring(6, 2));
            var endMin = int.Parse(timeRange.Substring(9, 2));

            return (startHour * 60 + startMin, endHour * 60 + endMin);
        }
        catch
        {
            return (0, 0);
        }
    }

    private IEnumerable<ArrivalTimeView> QueryByType(IEnumerable<ArrivalTimeView> query, ArrivalTimeType? type, int? inMinutes = 10)
    {
        if (type == null) return query;

        int startMinute, endMinute;

        switch (type)
        {
            case ArrivalTimeType.Morning:
                startMinute = 0;
                endMinute = 12 * 60;
                break;
            case ArrivalTimeType.Afternoon:
                startMinute = 12 * 60;
                endMinute = 18 * 60;
                break;
            case ArrivalTimeType.Evening:
                startMinute = 18 * 60;
                endMinute = 24 * 60;
                break;
            default:
                // Specify modes - use current Taiwan time
                var taiwanTime = DateTime.UtcNow.AddHours(8);
                startMinute = taiwanTime.Hour * 60 + taiwanTime.Minute;
                endMinute = startMinute + (inMinutes ?? 10);
                break;
        }

        return query.Where(x => endMinute >= x.StartMinute && startMinute <= x.EndMinute);
    }

    /// <summary>
    /// 使用 Haversine 公式計算兩點之間的距離 (公尺)
    /// </summary>
    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // 地球半徑 (公尺)

        var lat1Rad = lat1 * Math.PI / 180;
        var lat2Rad = lat2 * Math.PI / 180;
        var deltaLat = (lat2 - lat1) * Math.PI / 180;
        var deltaLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }
}

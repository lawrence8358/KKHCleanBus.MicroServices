using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KKHCleanBus.MicroServices.Data.Entities;

[Table("ArrivalTimeDetail")]
public class ArrivalTimeDetail
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid ParentId { get; set; }

    /// <summary>序號</summary>
    [Required]
    public string Seq { get; set; } = string.Empty;

    /// <summary>車輛 ID</summary>
    [Required]
    public string CarId { get; set; } = string.Empty;

    /// <summary>車牌號碼</summary>
    [Required]
    public string CarLicence { get; set; } = string.Empty;

    /// <summary>停放路口</summary>
    [Required]
    public string Caption { get; set; } = string.Empty;

    /// <summary>行政區名稱</summary>
    [Required]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>村里</summary>
    [Required]
    public string VillageName { get; set; } = string.Empty;

    /// <summary>定點/沿街</summary>
    public string? TaskType { get; set; }

    public string? GDay1 { get; set; }
    public string? GDay2 { get; set; }
    public string? GDay3 { get; set; }
    public string? GDay4 { get; set; }
    public string? GDay5 { get; set; }
    public string? GDay6 { get; set; }
    public string? GDay7 { get; set; }

    public string? RDay1 { get; set; }
    public string? RDay2 { get; set; }
    public string? RDay3 { get; set; }
    public string? RDay4 { get; set; }
    public string? RDay5 { get; set; }
    public string? RDay6 { get; set; }
    public string? RDay7 { get; set; }

    /// <summary>回收日</summary>
    public string? RecycleDay { get; set; }

    /// <summary>經度 ex.12x.xxxxx</summary>
    [Required]
    public decimal Longitude { get; set; }

    /// <summary>緯度 ex 23.xxxx</summary>
    [Required]
    public decimal Latitude { get; set; }

    public string? Sort { get; set; }

    /// <summary>鄰近行政區</summary>
    public string? CrossDeptName { get; set; }

    /// <summary>鄰近村里</summary>
    public string? CrossVillageName { get; set; }

    /// <summary>本日清運開始時間</summary>
    public string? TodayStart { get; set; }

    /// <summary>本日清運開始結束時間</summary>
    public string? TodayRange { get; set; }

    /// <summary>WGS 經度</summary>
    public decimal? WGSLongitude { get; set; }

    /// <summary>WGS 緯度</summary>
    public decimal? WGSLatitude { get; set; }

    /// <summary>是否抵達</summary>
    public string? GotoFire { get; set; }

    /// <summary>替代車輛 ID</summary>
    public string? ReplaceCarid { get; set; }

    /// <summary>替代車牌號碼</summary>
    public string? ReplaceCarLicence { get; set; }

    /// <summary>替代經度</summary>
    public decimal? ReplaceLongitude { get; set; }

    /// <summary>替代緯度</summary>
    public decimal? ReplaceLatitude { get; set; }

    public string? Memo { get; set; }

    public string? PointNumber { get; set; }
}

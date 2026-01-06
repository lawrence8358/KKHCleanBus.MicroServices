namespace KKHCleanBus.MicroServices.Models;

/// <summary>
/// 到達時間查詢結果視圖
/// </summary>
public class ArrivalTimeView
{
    /// <summary>序號</summary>
    public string Seq { get; set; } = string.Empty;

    /// <summary>車牌號碼</summary>
    public string CarLicence { get; set; } = string.Empty;

    /// <summary>停放路口</summary>
    public string Caption { get; set; } = string.Empty;

    /// <summary>行政區名稱</summary>
    public string DeptName { get; set; } = string.Empty;

    /// <summary>村里</summary>
    public string VillageName { get; set; } = string.Empty;

    /// <summary>定點/沿街</summary>
    public string? TaskType { get; set; }

    /// <summary>經度 ex.12x.xxxxx</summary>
    public decimal Longitude { get; set; }

    /// <summary>緯度 ex 23.xxxx</summary>
    public decimal Latitude { get; set; }

    /// <summary>回收日</summary>
    public string? RecycleDay { get; set; }

    /// <summary>開始清運的分鐘(小時已轉換)</summary>
    public int StartMinute { get; set; }

    /// <summary>結束清運的分鐘(小時已轉換)</summary>
    public int EndMinute { get; set; }

    /// <summary>本日清運開始結束時間</summary>
    public string? TimeRange { get; set; }

    /// <summary>替代車牌號碼</summary>
    public string? ReplaceCarLicence { get; set; }

    public decimal? ReplaceLongitude { get; set; }

    public decimal? ReplaceLatitude { get; set; }

    /// <summary>與當前座標的距離(公尺)</summary>
    public int? Distance { get; set; }
}

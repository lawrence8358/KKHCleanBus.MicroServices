using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KKHCleanBus.MicroServices.Data.Entities;

[Table("News")]
public class News
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public Guid? SystemId { get; set; }

    public Guid? TypeId { get; set; }

    public string? Description { get; set; }

    [Required]
    public bool IsTop { get; set; }

    /// <summary>開始日期 (ISO 8601 格式字串)</summary>
    public string? StartDate { get; set; }

    /// <summary>結束日期 (ISO 8601 格式字串)</summary>
    public string? EndDate { get; set; }

    [Required]
    public bool Enabled { get; set; }

    /// <summary>建立日期 (ISO 8601 格式字串)</summary>
    [Required]
    public string CreatedDate { get; set; } = string.Empty;
}

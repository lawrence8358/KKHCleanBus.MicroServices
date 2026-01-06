using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KKHCleanBus.MicroServices.Data.Entities;

[Table("ArrivalTime")]
public class ArrivalTime
{
    [Key]
    public Guid Id { get; set; }

    /// <summary>建立日期 (ISO 8601 格式字串)</summary>
    [Required]
    public string CreatedDate { get; set; } = string.Empty;
}

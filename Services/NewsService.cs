using KKHCleanBus.MicroServices.Data;
using KKHCleanBus.MicroServices.Data.Entities;

namespace KKHCleanBus.MicroServices.Services;

public class NewsService
{
    private readonly CleanBusDbContext _dbContext;
    private static readonly Guid SystemId = Guid.Parse("61832ddf-4cc5-465d-9817-0533e93708d6");

    public NewsService(CleanBusDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<News> GetAll()
    {
        var now = DateTimeOffset.UtcNow.ToString("O");

        // SQLite 需要使用 client-side evaluation 進行複雜的日期比較
        var result = _dbContext.News
            .Where(x => x.Enabled && x.SystemId == SystemId)
            .AsEnumerable() // 切換到 client-side evaluation
            .Where(x =>
                (string.IsNullOrEmpty(x.StartDate) || string.Compare(x.StartDate, now, StringComparison.Ordinal) <= 0) &&
                (string.IsNullOrEmpty(x.EndDate) || string.Compare(x.EndDate, now, StringComparison.Ordinal) >= 0)
            )
            .OrderByDescending(x => x.IsTop)
            .ThenByDescending(x => x.StartDate ?? string.Empty)
            .ThenBy(x => x.EndDate ?? string.Empty)
            .ToList();

        return result;
    }

    public News? Get(Guid id)
    {
        return _dbContext.News.FirstOrDefault(x => x.Enabled && x.Id == id && x.SystemId == SystemId);
    }
}

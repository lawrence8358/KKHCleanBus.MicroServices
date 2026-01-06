using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

// MSSQL Connection (source)
var mssqlConn = "Data Source=.;Initial Catalog=PrimeEagleX;Persist Security Info=True;User ID=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True";
// SQLite Connection (destination)
var sqliteConn = @"Data Source=d:\Project\Github\KKHCleanBus\KKHCleanBus.MicroServices\CleanBus.db";

Console.WriteLine("=== KKHCleanBus Data Migration Tool ===");
Console.WriteLine();

// Create SQLite database and tables
using var sqliteConnection = new SqliteConnection(sqliteConn);
sqliteConnection.Open();

Console.WriteLine("Creating SQLite tables...");

// Create tables
var createTablesSql = @"
CREATE TABLE IF NOT EXISTS News (
    Id TEXT PRIMARY KEY,
    Title TEXT NOT NULL,
    SystemId TEXT,
    TypeId TEXT,
    Description TEXT,
    IsTop INTEGER NOT NULL,
    StartDate TEXT,
    EndDate TEXT,
    Enabled INTEGER NOT NULL,
    CreatedDate TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ArrivalTime (
    Id TEXT PRIMARY KEY,
    CreatedDate TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS ArrivalTimeDetail (
    Id TEXT PRIMARY KEY,
    ParentId TEXT NOT NULL,
    Seq TEXT NOT NULL,
    CarId TEXT NOT NULL,
    CarLicence TEXT NOT NULL,
    Caption TEXT NOT NULL,
    DeptName TEXT NOT NULL,
    VillageName TEXT NOT NULL,
    TaskType TEXT,
    GDay1 TEXT,
    GDay2 TEXT,
    GDay3 TEXT,
    GDay4 TEXT,
    GDay5 TEXT,
    GDay6 TEXT,
    GDay7 TEXT,
    RDay1 TEXT,
    RDay2 TEXT,
    RDay3 TEXT,
    RDay4 TEXT,
    RDay5 TEXT,
    RDay6 TEXT,
    RDay7 TEXT,
    RecycleDay TEXT,
    Longitude REAL NOT NULL,
    Latitude REAL NOT NULL,
    Sort TEXT,
    CrossDeptName TEXT,
    CrossVillageName TEXT,
    TodayStart TEXT,
    TodayRange TEXT,
    WGSLongitude REAL,
    WGSLatitude REAL,
    GotoFire TEXT,
    ReplaceCarid TEXT,
    ReplaceCarLicence TEXT,
    ReplaceLongitude REAL,
    ReplaceLatitude REAL,
    Memo TEXT,
    PointNumber TEXT
);

CREATE INDEX IF NOT EXISTS IX_ArrivalTimeDetail_ParentId ON ArrivalTimeDetail(ParentId);
CREATE INDEX IF NOT EXISTS IX_ArrivalTimeDetail_CarLicence ON ArrivalTimeDetail(CarLicence);
CREATE INDEX IF NOT EXISTS IX_ArrivalTimeDetail_ReplaceCarLicence ON ArrivalTimeDetail(ReplaceCarLicence);
CREATE INDEX IF NOT EXISTS IX_News_SystemId ON News(SystemId);
CREATE INDEX IF NOT EXISTS IX_News_Enabled ON News(Enabled);
";

using (var cmd = new SqliteCommand(createTablesSql, sqliteConnection))
{
    cmd.ExecuteNonQuery();
}

Console.WriteLine("Tables created successfully.");
Console.WriteLine();

// Connect to MSSQL and migrate data
using var mssqlConnection = new SqlConnection(mssqlConn);
mssqlConnection.Open();

Console.WriteLine("Connected to MSSQL database.");
Console.WriteLine();

// 1. Migrate News (SystemId = '61832ddf-4cc5-465d-9817-0533e93708d6')
Console.WriteLine("Migrating News...");
var newsCount = 0;
using (var mssqlCmd = new SqlCommand(@"
    SELECT Id, Title, SystemId, TypeId, Description, IsTop, StartDate, EndDate, Enabled, CreatedDate 
    FROM Base_News 
    WHERE SystemId = '61832ddf-4cc5-465d-9817-0533e93708d6'", mssqlConnection))
using (var reader = mssqlCmd.ExecuteReader())
{
    while (reader.Read())
    {
        var insertSql = @"INSERT OR REPLACE INTO News (Id, Title, SystemId, TypeId, Description, IsTop, StartDate, EndDate, Enabled, CreatedDate) 
                          VALUES (@Id, @Title, @SystemId, @TypeId, @Description, @IsTop, @StartDate, @EndDate, @Enabled, @CreatedDate)";
        using var insertCmd = new SqliteCommand(insertSql, sqliteConnection);
        insertCmd.Parameters.AddWithValue("@Id", reader["Id"].ToString());
        insertCmd.Parameters.AddWithValue("@Title", reader["Title"].ToString());
        insertCmd.Parameters.AddWithValue("@SystemId", reader["SystemId"] is DBNull ? DBNull.Value : reader["SystemId"].ToString());
        insertCmd.Parameters.AddWithValue("@TypeId", reader["TypeId"] is DBNull ? DBNull.Value : reader["TypeId"].ToString());
        insertCmd.Parameters.AddWithValue("@Description", reader["Description"] is DBNull ? DBNull.Value : reader["Description"].ToString());
        insertCmd.Parameters.AddWithValue("@IsTop", Convert.ToInt32(reader["IsTop"]));
        insertCmd.Parameters.AddWithValue("@StartDate", reader["StartDate"] is DBNull ? DBNull.Value : ((DateTimeOffset)reader["StartDate"]).ToString("O"));
        insertCmd.Parameters.AddWithValue("@EndDate", reader["EndDate"] is DBNull ? DBNull.Value : ((DateTimeOffset)reader["EndDate"]).ToString("O"));
        insertCmd.Parameters.AddWithValue("@Enabled", Convert.ToInt32(reader["Enabled"]));
        insertCmd.Parameters.AddWithValue("@CreatedDate", ((DateTimeOffset)reader["CreatedDate"]).ToString("O"));
        insertCmd.ExecuteNonQuery();
        newsCount++;
    }
}
Console.WriteLine($"  Migrated {newsCount} news records.");

// 2. Get latest ArrivalTime parent record
Console.WriteLine("Migrating ArrivalTime...");
Guid? latestParentId = null;
using (var mssqlCmd = new SqlCommand(@"
    SELECT TOP 1 Id, CreatedDate 
    FROM KKH_CleanBus_ArrivalTime 
    ORDER BY CreatedDate DESC", mssqlConnection))
using (var reader = mssqlCmd.ExecuteReader())
{
    if (reader.Read())
    {
        latestParentId = (Guid)reader["Id"];
        var insertSql = @"INSERT OR REPLACE INTO ArrivalTime (Id, CreatedDate) VALUES (@Id, @CreatedDate)";
        using var insertCmd = new SqliteCommand(insertSql, sqliteConnection);
        insertCmd.Parameters.AddWithValue("@Id", latestParentId.ToString());
        insertCmd.Parameters.AddWithValue("@CreatedDate", ((DateTimeOffset)reader["CreatedDate"]).ToString("O"));
        insertCmd.ExecuteNonQuery();
        Console.WriteLine($"  Migrated 1 ArrivalTime parent record (Id: {latestParentId}).");
    }
}

// 3. Migrate ArrivalTimeDetail for the latest parent
if (latestParentId.HasValue)
{
    Console.WriteLine("Migrating ArrivalTimeDetail...");
    var detailCount = 0;
    using (var mssqlCmd = new SqlCommand($@"
        SELECT * FROM KKH_CleanBus_ArrivalTimeDetail 
        WHERE ParentId = '{latestParentId}'", mssqlConnection))
    using (var reader = mssqlCmd.ExecuteReader())
    {
        while (reader.Read())
        {
            var insertSql = @"INSERT OR REPLACE INTO ArrivalTimeDetail 
                (Id, ParentId, Seq, CarId, CarLicence, Caption, DeptName, VillageName, TaskType,
                 GDay1, GDay2, GDay3, GDay4, GDay5, GDay6, GDay7,
                 RDay1, RDay2, RDay3, RDay4, RDay5, RDay6, RDay7,
                 RecycleDay, Longitude, Latitude, Sort, CrossDeptName, CrossVillageName,
                 TodayStart, TodayRange, WGSLongitude, WGSLatitude, GotoFire,
                 ReplaceCarid, ReplaceCarLicence, ReplaceLongitude, ReplaceLatitude, Memo, PointNumber) 
                VALUES 
                (@Id, @ParentId, @Seq, @CarId, @CarLicence, @Caption, @DeptName, @VillageName, @TaskType,
                 @GDay1, @GDay2, @GDay3, @GDay4, @GDay5, @GDay6, @GDay7,
                 @RDay1, @RDay2, @RDay3, @RDay4, @RDay5, @RDay6, @RDay7,
                 @RecycleDay, @Longitude, @Latitude, @Sort, @CrossDeptName, @CrossVillageName,
                 @TodayStart, @TodayRange, @WGSLongitude, @WGSLatitude, @GotoFire,
                 @ReplaceCarid, @ReplaceCarLicence, @ReplaceLongitude, @ReplaceLatitude, @Memo, @PointNumber)";

            using var insertCmd = new SqliteCommand(insertSql, sqliteConnection);
            insertCmd.Parameters.AddWithValue("@Id", reader["Id"].ToString());
            insertCmd.Parameters.AddWithValue("@ParentId", reader["ParentId"].ToString());
            insertCmd.Parameters.AddWithValue("@Seq", reader["Seq"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@CarId", reader["CarId"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@CarLicence", reader["CarLicence"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@Caption", reader["Caption"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@DeptName", reader["DeptName"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@VillageName", reader["VillageName"]?.ToString() ?? "");
            insertCmd.Parameters.AddWithValue("@TaskType", reader["TaskType"] is DBNull ? DBNull.Value : reader["TaskType"]);

            for (int i = 1; i <= 7; i++)
            {
                insertCmd.Parameters.AddWithValue($"@GDay{i}", reader[$"GDay{i}"] is DBNull ? DBNull.Value : reader[$"GDay{i}"]);
                insertCmd.Parameters.AddWithValue($"@RDay{i}", reader[$"RDay{i}"] is DBNull ? DBNull.Value : reader[$"RDay{i}"]);
            }

            insertCmd.Parameters.AddWithValue("@RecycleDay", reader["RecycleDay"] is DBNull ? DBNull.Value : reader["RecycleDay"]);
            insertCmd.Parameters.AddWithValue("@Longitude", Convert.ToDecimal(reader["Longitude"]));
            insertCmd.Parameters.AddWithValue("@Latitude", Convert.ToDecimal(reader["Latitude"]));
            insertCmd.Parameters.AddWithValue("@Sort", reader["Sort"] is DBNull ? DBNull.Value : reader["Sort"]);
            insertCmd.Parameters.AddWithValue("@CrossDeptName", reader["CrossDeptName"] is DBNull ? DBNull.Value : reader["CrossDeptName"]);
            insertCmd.Parameters.AddWithValue("@CrossVillageName", reader["CrossVillageName"] is DBNull ? DBNull.Value : reader["CrossVillageName"]);
            insertCmd.Parameters.AddWithValue("@TodayStart", reader["TodayStart"] is DBNull ? DBNull.Value : reader["TodayStart"]);
            insertCmd.Parameters.AddWithValue("@TodayRange", reader["TodayRange"] is DBNull ? DBNull.Value : reader["TodayRange"]);
            insertCmd.Parameters.AddWithValue("@WGSLongitude", reader["WGSLongitude"] is DBNull ? DBNull.Value : reader["WGSLongitude"]);
            insertCmd.Parameters.AddWithValue("@WGSLatitude", reader["WGSLatitude"] is DBNull ? DBNull.Value : reader["WGSLatitude"]);
            insertCmd.Parameters.AddWithValue("@GotoFire", reader["GotoFire"] is DBNull ? DBNull.Value : reader["GotoFire"]);
            insertCmd.Parameters.AddWithValue("@ReplaceCarid", reader["ReplaceCarid"] is DBNull ? DBNull.Value : reader["ReplaceCarid"]);
            insertCmd.Parameters.AddWithValue("@ReplaceCarLicence", reader["ReplaceCarLicence"] is DBNull ? DBNull.Value : reader["ReplaceCarLicence"]);
            insertCmd.Parameters.AddWithValue("@ReplaceLongitude", reader["ReplaceLongitude"] is DBNull ? DBNull.Value : reader["ReplaceLongitude"]);
            insertCmd.Parameters.AddWithValue("@ReplaceLatitude", reader["ReplaceLatitude"] is DBNull ? DBNull.Value : reader["ReplaceLatitude"]);
            insertCmd.Parameters.AddWithValue("@Memo", reader["Memo"] is DBNull ? DBNull.Value : reader["Memo"]);
            insertCmd.Parameters.AddWithValue("@PointNumber", reader["PointNumber"] is DBNull ? DBNull.Value : reader["PointNumber"]);

            insertCmd.ExecuteNonQuery();
            detailCount++;
        }
    }
    Console.WriteLine($"  Migrated {detailCount} ArrivalTimeDetail records.");
}

Console.WriteLine();
Console.WriteLine("=== Migration completed successfully! ===");

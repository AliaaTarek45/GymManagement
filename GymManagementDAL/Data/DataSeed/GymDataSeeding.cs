using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class GymDataSeeding
{
    public static async Task SeedAsync(
        GymDbContext dbContext,
        string seedFilesPath,
        ILogger logger,
        CancellationToken ct = default)
    {
        try
        {
            if (!await dbContext.Categories.AnyAsync(ct))
            {
                var categories = LoadDataFromJsonFile<CategoryEntity>("categories.json", seedFilesPath);
                if (categories.Count > 0)
                {
                    dbContext.Categories.AddRange(categories);
                    logger.LogInformation("Seeded {Count} categories.", categories.Count);
                }
            }

            if (!await dbContext.Plans.AnyAsync(ct))
            {
                var plans = LoadDataFromJsonFile<PlanEntity>("plans.json", seedFilesPath);
                if (plans.Count > 0)
                {
                    dbContext.Plans.AddRange(plans);
                    logger.LogInformation("Seeded {Count} plans.", plans.Count);
                }
            }

            if (dbContext.ChangeTracker.HasChanges())
                await dbContext.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Gym data seeding failed.");
            throw;
        }
    }

    private static List<T> LoadDataFromJsonFile<T>(string fileName, string FolderPath)
    {

        var filePath = Path.Combine(FolderPath, fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Seed data file not found: {filePath}");

        var data = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(new JsonStringEnumConverter());

        return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskTracker.Domain;

namespace TaskTracker.Api.Json;

public static class TaskTrackerJsonSerializerOptions
{
    public static void Configure(JsonSerializerOptions options)
    {
        options.Converters.Add(
            new JsonStringEnumConverter<TaskPriority>(
                namingPolicy: null,
                allowIntegerValues: false));
    }

    public static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        Configure(options);

        return options;
    }
}

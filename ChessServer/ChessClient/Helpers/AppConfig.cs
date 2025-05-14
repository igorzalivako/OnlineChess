using Microsoft.Extensions.Configuration;

public static class AppConfig
{
    public static string BaseUrl { get; }

    static AppConfig()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        BaseUrl = config["ApiSettings:BaseUrl"];
    }
}
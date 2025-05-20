using Microsoft.Extensions.Configuration;

public static class AppConfig
{
    public static string BaseUrl { get; }

    static AppConfig()
    {
        /*var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        BaseUrl = config["ApiSettings:BaseUrl"];*/
        //BaseUrl = "http://192.168.100.4:5054";
        BaseUrl = "http://192.168.172.53:5054";
        
    }
}
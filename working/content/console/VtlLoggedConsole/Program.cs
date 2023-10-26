using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VtlSoftware.Aspects.Logging;
#if((LoggingFramework) == Nlog)
using NLog;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging;
#endif
#if((LoggingFramework) == Serilog)
using Serilog;
#endif


namespace VtlLoggedConsole;
class Program
{

    static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
        optional: true)
    .AddEnvironmentVariables();
}
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        BuildConfig(builder);

        // This line provides access to the application configuration.
        IConfiguration configuration = builder.Build();

#if ((LoggingFramework) == Serilog)
        Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Build())
        .CreateLogger();

        // Add an application startup message to the log.
        Log.Logger.Information("VtlLoggedConsole Starting");

        // Now the various services are setup and we instruct the logger to use Serilog.
        var appHost = Host.CreateDefaultBuilder()
        .ConfigureServices(
         (context, services) =>
         {
             services.AddSingleton<IConfiguration>(configuration);
             services.AddScoped<ILoggingAspect, LoggingAspect>();
             // You can now add you own types here ie;

             //services.AddTransient<MyTestClass>();
             
         })
        .UseSerilog()
        .Build();

        // and then instantiate and run them as illustrated below.

        //var mtc = ActivatorUtilities.CreateInstance<MyTestClass>(appHost.Services);
        //mtc.DoSomething();
        //mtc.ConvertToUpper("frank");

        // Add a closing message to the log
        Log.Logger.Information("VtlLoggedConsole Closing");

        //finally shut down serilog
        Log.CloseAndFlush();
#endif

#if ((LoggingFramework) == Nlog)

        // configure the Nlog framework
        var logger = LogManager.GetCurrentClassLogger();


        var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

        using var servicesProvider = new ServiceCollection()
        .AddLogging(
        loggingBuilder =>
        {
            // configure Logging with NLog
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            loggingBuilder.AddNLog(config);
        }).BuildServiceProvider();



        //use services here

        var host = Host.CreateDefaultBuilder()
        .ConfigureServices(
        (context, services) =>
        {
          services.AddSingleton<IConfiguration>(configuration);
          services.AddScoped<ILoggingAspect, LoggingAspect>();
          // You can now add you own types here ie;
          
          //services.AddTransient<MyTestClass>();
        })
        .Build();

        // and then instantiate and run them as illustrated below.

        //var mtc = ActivatorUtilities.CreateInstance<MyTestClass>(host.Services);
        //mtc.DoSomething();
        //mtc.ConvertToUpper("frank");

        //prior to application exit shut down Nlog
        LogManager.Shutdown();
#endif
    }
}
// Classes that use aspects should be marked as partial.
internal partial class MyTestClass
{
   
    [Log]
    public string ConvertToUpper(string value) 
    { 
        return value.ToUpper(); 
    }

    [Log]
    public void DoSomething() 
    { 
        // Do some work here

        // You can add you own custom log messages as illustrated below.
        // NB: You should make sure that the class is marked as partial first.
        // This allows you to reference the logger provided via Dependency Injection

        //logger.LogString(LogLevel.Information, "Hi There"); 
    }
}

//  The class below can be used to automate the logging.  
//  Before uncommenting it remove the [Log] attributes from myTestClass
//  then just build and run the program.

//public class Configure : ProjectFabric
//{        
//    public override void AmendProject(IProjectAmender amender) { amender.LogEverything(); }    
//}


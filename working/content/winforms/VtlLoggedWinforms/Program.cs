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

namespace VtlLoggedWinforms;

static class Program
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

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        AppDomain currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += new UnhandledExceptionEventHandler(Unhandled);
        Application.ApplicationExit += Application_ApplicationExit;
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

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

        
#endif

        
        Application.Run(new Form1());
    }    

    private static void Application_ApplicationExit(object? sender, EventArgs e)
    {
#if ((LoggingFramework) == Serilog)
          // Add a closing message to the log
        Log.Logger.Information("VtlLoggedConsole Closing");

        //finally shut down serilog
        Log.CloseAndFlush();
#endif
#if ((LoggingFramework) == Nlog)
        //prior to application exit shut down Nlog
        LogManager.Shutdown();
#endif
    }


    static void Unhandled(object sender, UnhandledExceptionEventArgs args)
   {
      Exception e = (Exception) args.ExceptionObject;

      #if ((LoggingFramework) == Serilog)
          // Add a closing message to the log
        Log.Logger.Fatal("VtlLoggedConsole has encountered an error {0}",e);

        //finally shut down serilog
        Log.CloseAndFlush();
#endif
#if ((LoggingFramework) == Nlog)

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        logger.Fatal("VtlLoggedConsole has encountered an error {0}",e);
        //prior to application exit shut down Nlog
        NLog.LogManager.Shutdown();
#endif
   }
}
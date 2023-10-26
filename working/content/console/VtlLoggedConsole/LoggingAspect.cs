using Microsoft.Extensions.Configuration;
using VtlSoftware.Aspects.Logging;

namespace VtlLoggedConsole
{
  public class LoggingAspect : ILoggingAspect
  {

           private readonly IConfiguration configuration;

  
   public LoggingAspect(IConfiguration configuration)
   {
       this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
       LoggingEnabled = CheckAppSettingKeyExistsAndReturnValueIfItDoesOrDefaultToTrue();
   }

 
   private bool CheckAppSettingKeyExistsAndReturnValueIfItDoesOrDefaultToTrue()
   {
       var section = configuration.GetSection("LoggingEnabled");
       var sectionExists = section.Exists();
       if(sectionExists)
       {
           return Convert.ToBoolean(configuration.GetSection("LoggingEnabled").Value);
       } else
       {
           return true;
       }
   }

 
   public bool LoggingEnabled { get; set; }

   
  }
}
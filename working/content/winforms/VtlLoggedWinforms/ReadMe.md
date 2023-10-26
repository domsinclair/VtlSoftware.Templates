# Vtl Logged Console

This is a basic winforms application that will initialise either Serilog or Nlog depending on choice that you make when creating it.

The appsettings file contains basic configurations for both NLog and Serilog.

In the Csproj file you will find the SensitiveDataParameters property that the VtlSoftware.Aspects.Logging library uses to redact parameter values.

There is an example of a ProjectFabric that can automate the process of adding logging to your application.

> :warning: **Using fabrics requires a metalama paid licence**: In most cases a basic Starter licence will suffice.

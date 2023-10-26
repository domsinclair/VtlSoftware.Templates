# Vtl Logged Console

This is a basic console application that will initialise either Serilog or Nlog depending on choice that you make when creating it.

The appsettings file contains basic configurations for both NLog and Serilog.

In the Csproj file you will find the SensitiveDataParameters property that the VtlSoftware.Aspects.Logging library uses to redact parameter values.

There is a small example class (MyTestClass) that illustrates both using the preconfigured log templates and adding a custom log entry.

There is also an example of a ProjectFabric that can automate the process of adding logging to your application.

> :warning: **Using fabrics requires a metalama paid licence**: In most cases a basic Starter licence will suffice.

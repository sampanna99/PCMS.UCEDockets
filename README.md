PCMS.UCEDockets
===============

A database cache and REST API that consumes the UMCS-UCE data feed

Features
--------

* Manages scheduling and incremental synchronization from SFTP source
* REST API to query by DocketID or Changes by date
* Supports both XML and JSON responses
* Swagger API Specification preserves .xsd documentation (intellisense on clients)
* Docker/Kubernetes support
* Monitoring and Logging
* Easy to update the .xsd in the future

Unsupported:

* The server opens unauthenticated HTTP ports that must be 
  protected through standard server hosting solutions
* The database is a single table. The XML is stored as XML in a column,
  not expanded into a comparable schema

Database
--------
The dockets will be maintained in a database through EntityFramework. Initial
support is for MS SQL Server, however supporting other databases would be trivial.

It's important to understand that it's the XML that is stored in this database. It
does NOT expand the schema into comparable tables within the database.

![Database Table showing columns stored](docs/table.png)

The database has a single data table, that stores the XML keyed by the 
UCMS Docket-ID.

Prerequisites
--------
Install the following:
    [.NET Core SDK](https://dotnet.microsoft.com/download) The SDK also includes the Runtime.

Building
--------

Restore nuget packages and build tools
```
dotnet restore
dotnet tool restore
```

Build
```
dotnet build
```

Configuration
-------------
```json
{
  "UCEDockets": {
      "LocalPath": "data",
      "Counties": ["Richmond", "Albany"],
      "Host": "sftp.nycourts.gov",
      
      "Username": "UCE-********",
      "Password": "**********"
  },
  "ConnectionStrings": {
      "DefaultConnection": ""
  }
}
```

Libraries
---------

In addition to many libraries from Microsoft, these libraries are in use:
- *SSH.NET* - SSH.NET is a Secure Shell (SSH) library for .NET - [github](https://github.com/sshnet/SSH.NET) / [nuget](https://www.nuget.org/packages/SSH.NET/)
- *prometheus-net* - .NET library to instrument your code with Prometheus metrics - [github](https://github.com/prometheus-net/prometheus-net) / [nuget](https://www.nuget.org/packages/prometheus-net/)
- *Swashbuckle.AspNetCore* - Swagger tools for documenting API's built on ASP.NET Core - [github](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) / [nuget](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)
- *XmlSchemaClassGenerator* - Generate C# classes from XML Schema files - [github](XmlSchemaClassGenerator) / [nuget](https://www.nuget.org/packages/dotnet-xscgen/)



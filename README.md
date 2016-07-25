NLibuv [![Build Status](https://travis-ci.org/neris/NLibuv.svg?branch=master)](https://travis-ci.org/neris/NLibuv) [![Build Status](https://ci.appveyor.com/api/projects/status/4vcw8d72fvtgwuhf?svg=true)](https://ci.appveyor.com/project/neris/nlibuv) [![NuGet](https://img.shields.io/nuget/v/NLibuv.svg?label=nuget:%20NLibuv)](https://www.nuget.org/packages/NLibuv/)
========

A cross-platform bindings to the [libuv library](http://libuv.org/) for .NET.

### Warning! Work in progress!

This project currently is in the development stage. Use at your own risk.

Not all libuv API methods are implemented yet, and many of them will not be implemented in this package at all since .NET platform provides similar functionality (for example, Threading and synchronization utilities, Process handles, etc).

Please refer to the project's GitHub issues for more information about the development progress.



Installation
------------

All you need to do is simply install a [NuGet package](https://www.nuget.org/packages/NLibuv/)
from the package manager console:
```
PM> Install-Package NLibuv
```
Or add the `NLibuv` dependency to your `project.json` file.

### Prerequisites

 * This package requires the [libuv library](http://libuv.org/) installed in your system (.NET Core redistributes it since version 1.0.0).




NLibuv [![Build Status](https://travis-ci.org/VitaliiTsilnyk/NLibuv.svg?branch=master)](https://travis-ci.org/VitaliiTsilnyk/NLibuv) [![Build Status](https://ci.appveyor.com/api/projects/status/4vcw8d72fvtgwuhf?svg=true)](https://ci.appveyor.com/project/VitaliiTsilnyk/nlibuv) [![NuGet](https://img.shields.io/nuget/v/NLibuv.svg?label=nuget:%20NLibuv)](https://www.nuget.org/packages/NLibuv/) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/VitaliiTsilnyk/NLibuv/master/LICENSE)
========

A cross-platform bindings to the [libuv library](http://libuv.org/) for .NET.

The purpose of this library is to provide the libuv API on .NET platform, as simple as possible.
It requires you to write your code as if you were using libuv directly.
NLibuv takes care of platform invoke, marshaling, error handling and memory management,
allowing you to consume libuv API as it is, safely on any platform supported by .NET.

### Thread safety

This library is **NOT** thread safe. Like the original libuv library, it has only
one thread safe method: `UvAsync.Send` (which represents libuv's `uv_async_send` function).
If you're using NLibuv in multithreaded environment, you have to take care of synchronization by yourself,
NLibuv will help you with this only by throwing an exception if you try to call non-thread-safe method from incorrect thread.


### Warning! Work in progress!

This project currently is in the development stage. Use at your own risk.

Not all libuv API methods are implemented yet, and many of them will not be implemented in this
package at all since .NET platform provides similar functionality (for example, Threading and synchronization utilities, Process handles, etc).

Please refer to the [project's GitHub issues page](https://github.com/VitaliiTsilnyk/NLibuv/issues) for more information about the development progress.



Installation
------------

All you need to do is simply install a [NuGet package](https://www.nuget.org/packages/NLibuv/)
from the package manager console:
```
PM> Install-Package NLibuv
```
or through .NET CLI utility:
```
$ dotnet add package NLibuv
```




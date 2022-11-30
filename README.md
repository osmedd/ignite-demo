# Apache Ignite Cross Platform Demo

An Apache Ignite cross platform demonstration, demonstrating C#/C++ shared cache and distributed computing functions.

Currently, build and run is only supported on Windows.

PowerShell is a prerequisite to run all the mentioned scripts.  Java 1.8+ is required, as well as .NET Core 6.0,
cmake and a C++ compiler.

## Project components

### RequestProcessor

An ASP.NET Core 6.0 web api service that provides API endpoints that hit Apache Ignite data stores and computing functions.

### Worker.NET

.NET Core 6.0 console application that stores it's status in Apache Ignite and provides one distributed computing function, AssignTopic.

### Worker

C++ console application that stores it's status in Apache Ignite.

## Build

### Build toolbox

```
> projbin/build_toolbox
```

### Compile

```
> projbin/compile
```

## Run

In separate terminals/consoles (or run each with redirection), start each component.

```
> projbin/start_ignite
> projbin/start_requestprocessor
> projbin/start_worker
> projbin/start_worker -CPP
```

Each worker prints out it's unique worker id.

To get the list of workers, use:
```
> curl http://localhost:5000/demo/workers
```

Note this will currently fail if you started the C++ worker and succeed if you only started the .NET worker.

To see the status of one worker, for example, using an example Guid worker id:
```
> curl http://localhost:5000/demo/workers/8e74f96c-117d-42e2-96b7-71926fbcab5e
```

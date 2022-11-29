# Apache Ignite Cross Platform Demo

An Apache Ignite cross platform demonstration, demonstrating C#/C++ shared cache and distributed computing functions.

Currently, build and run is only supported on Windows.

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

In separate terminals/consoles (or run each with redirection), run each component.

```
> projbin/start_ignite
> projbin/start_requestprocessor
> projbin/start_worker
> projbin/start_worker -CPP
```

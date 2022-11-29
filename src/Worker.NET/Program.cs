using Cfg.Common;
using Cfg.Worker.NET;

Console.WriteLine("Assign topic function name: {0}", AssignTopicFunc.FunctionName);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IDataGrid, DataGrid>();
        services.AddSingleton<IWorkerStatus, WorkerStatus>();

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();

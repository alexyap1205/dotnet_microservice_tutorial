# Pre-requisite
* .NET 7 installed
* Docker installed

# Tutorial for .NET Core Microservice Web API

## First, let's build a basic CRUD Web API
-- Source code in 1_Basic_Dotnet_WebApi --


1. Create a new web api .NET Core app using dotnet cli
    https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-new
2. Create a Todo Model we could use.
```
public class Todo
{
    public int Id { get; set; }
    
    public string Summary { get; set; }

    public string Description { get; set; }
}
```
3. Create a Todo Service layer. Let's create the methods all asynchronous in preparation of reaching out to a database.
```
public interface ITodoService
{
    Task<Todo> createAsync(Todo todo);
    Task<Todo> retrieveAsync(int id);
    Task<Todo> updateAsync(int id, Todo todo);
    Task<Todo> deleteAsync(int id);
}

public class TodoService : ITodoService
{
    public async Task<Todo> createAsync(Todo todo)
    {
        await Task.Run(() => Console.WriteLine($"Creating {JsonSerializer.Serialize(todo)}"));
        return todo;
    }

    public async Task<Todo> retrieveAsync(int id)
    {
        await Task.Run(() => Console.WriteLine($"Retrieving Todo ID {id}"));
        return new Todo
        {
            Id = id
        };
    }

    public async Task<Todo> updateAsync(int id, Todo todo)
    {
        await Task.Run(() => Console.WriteLine($"Updating {JsonSerializer.Serialize(todo)} with id {id}"));
        return todo;
    }

    public async Task<Todo> deleteAsync(int id)
    {
        await Task.Run(() => Console.WriteLine($"Deleting Todo ID {id}"));
        return new Todo
        {
            Id = id
        };
    }
}
```
4. Add the service to DI in Program.cs

```
builder.Services.AddTransient<ITodoService, TodoService>();
```

5. Create a Todo Controller.

```
public class ToDoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public ToDoController(ITodoService todoService)
    {
        _todoService = todoService;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> GetTodoItem(int id)
    {
        return await _todoService.retrieveAsync(id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Todo>> PutTodoItem(int id, Todo todo)
    {
        return await _todoService.updateAsync(id, todo);
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> PostTodoItem(Todo todo)
    {
        return await _todoService.createAsync(todo);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Todo>> DeleteTodoItem(int id)
    {
        return await _todoService.deleteAsync(id);
    }

}
```

6. Run.

```
$ dotnet run
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5267

```

7. Try the swagger endpoint http://localhost:5267/swagger/index.html

8. Try out end of the endpoint, you should see the following from console.

```
Retrieving Todo ID 1
Updating {"Id":1,"Summary":"string","Description":"string"} with id 1
Creating {"Id":0,"Summary":"string","Description":"string"}
Deleting Todo ID 1
```

## Let's run this basic microservice in a docker container
-- Source code in 2_dockerised_web_api --

1. Add a Dockerfile into the project, and on the dockerfile let's do a 2 step build:
* Build with the dotnet sdk docker image.
```
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
# TODO change linux-arm64 to linux-amd64 if not running on m1
RUN dotnet publish -c Release -o out -r linux-arm64
```

* With the output of the above build, create another image using the runtime docker image. This makes our final docker image smaller, and containing only runtime libraries.
```
# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "DockerisedWebApi.dll"]
```

2. Build the docker image
```
$ docker build -t dotnet_test -f Dockerfile 
 => [internal] load build definition from Dockerfile  2.0s
 ...
 => => exporting layers 3.0s
 => => writing image sha256:866bf0dcd655a92b81fa873e125e82a30db526ee96fdc9e829e018a79a9af4ec 1.0s
 => => naming to docker.io/library/dotnet_test
```

> Note: -t dotnet_test means, tag the resulting docker image as dotnet_test
> More information on https://docs.docker.com/engine/reference/commandline/build/


3. Run the docker image locally
```
$ docker run -p 8080:80 dotnet_test:latest
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://[::]:80
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /app
```

> Note: -p 8080:80 means, do a port mapping from local port 8080 on the host, to port 80 of the docker container.
> More information on https://docs.docker.com/engine/reference/commandline/run/


4. Navigate to `http://localhost:8080/swagger/index.html` and again trigger all endpoints.
```
...
Retrieving Todo ID 1
Updating {"Id":1,"Summary":"string","Description":"string"} with id 1
Deleting Todo ID 1
Creating {"Id":0,"Summary":"string","Description":"string"}
```

# Pre-requisite
* .NET 7 installed

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


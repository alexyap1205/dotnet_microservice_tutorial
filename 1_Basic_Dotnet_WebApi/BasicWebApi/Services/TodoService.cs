using System.Text.Json;
using BasicWebApi.Models;

namespace BasicWebApi.Services;

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
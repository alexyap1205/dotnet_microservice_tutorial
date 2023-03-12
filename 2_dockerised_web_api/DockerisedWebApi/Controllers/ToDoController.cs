using DockerisedWebApi.Models;
using DockerisedWebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DockerisedWebApi.Controllers;

[ApiController]
[Route("[controller]")]
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
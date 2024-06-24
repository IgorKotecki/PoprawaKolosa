using System.Transactions;
using KolPoprawa.Models_DTOs;
using KolPoprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace KolPoprawa.Controller;
[Route("api/[controller]")]
[ApiController]
public class ProjectContoller : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectContoller(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectsTasks(int id)
    {
        if (!await _projectService.ChceckIfProjectExists(id))
        {
            return NotFound();
        }

        var results = _projectService.GetTaskByProjectId(id);

        return Ok(results);
    }

    [HttpPost]

    public async Task<IActionResult> AddNewTask(NewTaskDTO newTaskDTO)
    {
        if (!await _projectService.CheckIfUserExists(newTaskDTO.IdAssignee))
        {
            return NotFound();
        }

        if (!await _projectService.CheckIfUserExists(newTaskDTO.IdReporter))
        {
            return NotFound();
        }

        await _projectService.AddNewTask(newTaskDTO);

        return Created();
    }
}
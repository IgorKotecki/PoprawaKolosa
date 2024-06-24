using KolPoprawa.Models_DTOs;
using KolPoprawa.Repository;

namespace KolPoprawa.Services;

public interface IProjectService
{
    Task<bool> ChceckIfProjectExists(int id);
    Task<bool> CheckIfUserExists(int id);
    Task<Projetct> GetTaskByProjectId(int id);
    Task AddNewTask(NewTaskDTO newTaskDto);
}

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<bool> ChceckIfProjectExists(int id)
    {
        return await _projectRepository.ChceckIfProjectExists(id);
    }

    public async Task<bool> CheckIfUserExists(int id)
    {
        return await _projectRepository.CheckIfUserExists(id);
    }

    public async Task<Projetct> GetTaskByProjectId(int id)
    {
        return await _projectRepository.GetTaskByProjectId(id);
    }

    public async Task AddNewTask(NewTaskDTO newTaskDto)
    {
        await _projectRepository.AddNewTask(newTaskDto);
    }
}
using System.Data;
using KolPoprawa.Models_DTOs;
using Microsoft.Data.SqlClient;

namespace KolPoprawa.Repository;

public interface IProjectRepository
{
    Task<bool> ChceckIfProjectExists(int id);
    Task<bool> CheckIfUserExists(int id);
    Task<Projetct> GetTaskByProjectId(int id);
    Task AddNewTask(NewTaskDTO newTaskDto);
}

public class ProjectRepository : IProjectRepository
{
    private readonly IConfiguration _configuration;

    public ProjectRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> ChceckIfProjectExists(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        var query = "SELECT 1 FROM Project WHERE IdProject = @id";
        await connection.OpenAsync();
        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        var results = (int)await cmd.ExecuteScalarAsync();

        return results > 0;
    }

    public async Task<bool> CheckIfUserExists(int id)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        var query = "SELECT 1 FROM User WHERE IdUser = @id";
        await connection.OpenAsync();
        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        var results = (int)await cmd.ExecuteScalarAsync();

        return results > 0;
    }

    public async Task<Projetct> GetTaskByProjectId(int id)
    {
        
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        var query = "SELECT t.IdTask,t.Name,t.Description,t.CreatedAt,t.IdProject,t.IdReporter,u1.FirstName as ReporterName ,u1.LastName as ReporterLastName,t.IdAssignee,u2.FirstName as AssigneeName,u2.LastName as AssigneeLastName FROM TASK " +
                    "INNER JOIN USER u1 ON u1.IdUser = t.IdReporter " +
                    "INNER JOIN USER u2 ON u2.IdUser = t.IdAssigne " +
                    "WHERE t.IdProject = @id";
        await connection.OpenAsync();
        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", id);

        var reader = await cmd.ExecuteReaderAsync();

        var IdTaskOrdinal = reader.GetOrdinal("IdTask");
        var NameOrdinal = reader.GetOrdinal("Name");
        var DescriptionOrdinal = reader.GetOrdinal("Description");
        var admissionDateOrdinal = reader.GetOrdinal("CreatedAt");
        var IdProjectOrdinal = reader.GetOrdinal("IdProject");
        var IdReporterOrdinal = reader.GetOrdinal("IdReporter");
        var ReporterNameOrdinal = reader.GetOrdinal("ReporterName");
        var ReporterLastNameOrdinal = reader.GetOrdinal("ReporterLastName");
        var IdAssigneeOrdinal = reader.GetOrdinal("IdAssignee");
        var AssigneeNameOrdinal = reader.GetOrdinal("AssigneeName");
        var AssigneeLastNameOrdinal = reader.GetOrdinal("AssigneeLastName");
        
        Projetct zadanie = new Projetct();
        
        while (await reader.ReadAsync())
        {
            zadanie.Zadania.Add(new Zadanie()
            {
                IdTask = reader.GetInt32(IdTaskOrdinal),
                Name = reader.GetString(NameOrdinal),
                Description = reader.GetString(DescriptionOrdinal),
                CreatedAt = reader.GetDateTime(admissionDateOrdinal),
                IdProject = reader.GetInt32(IdProjectOrdinal),
                IdReporter = reader.GetInt32(IdReporterOrdinal),
                Reporter = new User()
                {
                    FirstName = reader.GetString(ReporterNameOrdinal),
                    LastName = reader.GetString(ReporterLastNameOrdinal)
                },
                IdAssignee = reader.GetInt32(IdAssigneeOrdinal),
                Assignee = new User()
                {
                    FirstName = reader.GetString(AssigneeNameOrdinal),
                    LastName = reader.GetString(AssigneeLastNameOrdinal)
                }
            });
        }

        if (zadanie is null)
        {
            throw new Exception();
        }

        return zadanie;
    }

    public async Task AddNewTask(NewTaskDTO newTaskDto)
    {
        await using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);

        var query = "Select idDefaultAssignee from Project where IdProject = @id";
        await connection.OpenAsync();
        await using var cmd = new SqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", newTaskDto.IdProject);

        var idDeafultAssignee = (int)await cmd.ExecuteScalarAsync();
        
        cmd.Parameters.Clear();
        
        query = "INSERT INTO TASK VALUES (@Name,@Description,@CreatedAt,@IdProject,@IdReporter,@IdAssignee)";

        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@Name", newTaskDto.Name);
        cmd.Parameters.AddWithValue("@Description", newTaskDto.Description);
        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        cmd.Parameters.AddWithValue("@IdProject", newTaskDto.IdProject);
        cmd.Parameters.AddWithValue("@IdReporter", newTaskDto.IdReporter);
        if (newTaskDto.IdAssignee == 0)
        {
            cmd.Parameters.AddWithValue("@IdAssignee", idDeafultAssignee);
        }

        cmd.Parameters.AddWithValue("@IdAssignee", newTaskDto.IdAssignee);
        
        await connection.OpenAsync();
        
        
        
        var transaction = await connection.BeginTransactionAsync();
        cmd.Transaction = transaction as SqlTransaction;

        try
        {
            await cmd.ExecuteReaderAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
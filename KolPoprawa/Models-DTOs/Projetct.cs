namespace KolPoprawa.Models_DTOs;

public class Projetct
{
    public List<Zadanie> Zadania { get; set; }
}

public class Zadanie
{
    public int IdTask { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int IdProject { get; set; }
    public int IdReporter { get; set; }
    public User Reporter { get; set; }
    public int IdAssignee { get; set; }
    public User Assignee { get; set; }
}

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
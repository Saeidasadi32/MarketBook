namespace MarketBook.Tools.ProjectInspector.Models;

public sealed class ProjectReport
{
    public string Root { get; init; } = string.Empty;

    public List<ProjectFolder> Folders { get; } = [];

    public List<ProjectFile> Files { get; } = [];

    public List<ProjectClass> Classes { get; } = [];
    public List<ProjectIssue> Issues { get; } = [];

    public int TotalProjects { get; set; }

    public int TotalFiles { get; set; }

    public int TotalClasses { get; set; }

    public int TotalRecords { get; set; }

    public int TotalEnums { get; set; }

    public int TotalInterfaces { get; set; }

    public int TotalValueObjects { get; set; }

    public int TotalEntities { get; set; }

    public int TotalAggregates { get; set; }

    public int DocumentationScore { get; set; }

    public int ArchitectureScore { get; set; }

    public int DddScore { get; set; }
}
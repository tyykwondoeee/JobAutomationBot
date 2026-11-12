public class AdzunaResponse
{
    public List<AdzunaJob>? results { get; set; }
}

public class AdzunaJob
{
    public string? title { get; set; }
    public AdzunaCompany? company { get; set; }
    public AdzunaLocation? location { get; set; }
    public string? description { get; set; }
    public string? redirect_url { get; set; }
    public double? salary_min { get; set; }
    public double? salary_max { get; set; }
}

public class AdzunaCompany
{
    public string? display_name { get; set; }
}

public class AdzunaLocation
{
    public string? display_name { get; set; }
}

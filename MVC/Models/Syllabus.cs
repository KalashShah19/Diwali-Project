public class Syllabus
{
    public int SyllabusID { get; set; }
    public int CWSID { get; set; }
    public string? title { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public double? percentComplete { get; set; }
}
namespace MonApi.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Titre { get; set; } = string.Empty;
        public bool EstTerminée { get; set; } = false;
    }
}
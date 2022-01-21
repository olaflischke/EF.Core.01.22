namespace WorkPlanDal
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public List<Person> Worker { get; set; } = new List<Person>();
    }
}
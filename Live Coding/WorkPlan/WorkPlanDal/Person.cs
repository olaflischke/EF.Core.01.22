using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkPlanDal
{
    [Table("Personen")]
    public class Person
    {
        [Column("Description")]
        [StringLength(200)]
        public string Name { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Guid { get; set; }
        //public int PersonId { get; set; }

        [NotMapped]
        public string PKZ { get; set; }

    }
}
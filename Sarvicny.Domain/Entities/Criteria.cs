using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities;

public class Criteria
{
    public Criteria()
    {
        Services = new List<Service>();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string CriteriaID { get; set; }
    public string CriteriaName { get; set; }

    public string Description { get; set; }

    public List<Service>? Services { get; set; }

}
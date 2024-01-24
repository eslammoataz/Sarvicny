using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sarvicny.Domain.Entities;

public class Service
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ServiceID { get; set; }
    public string ServiceName { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string AvailabilityStatus { get; set; }


    // Explicit foreign key for the Criteria relationship
    public string? CriteriaID { get; set; }

    // Navigation property for the Criteria relationship
    public Criteria? Criteria { get; set; }


    // Foreign key for self-referencing relationship
    public string? ParentServiceID { get; set; }

    // Navigation property for the parent service
    public Service? ParentService { get; set; }

    // Navigation property for the child services
    public List<Service> ChildServices { get; set; } = new List<Service>();

    // public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    // public List<ProviderService> ProviderServices { get; set; } = new List<ProviderService>();
}
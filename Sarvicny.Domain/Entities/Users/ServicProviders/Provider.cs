﻿namespace Sarvicny.Domain.Entities.Users.ServicProviders;

public abstract class Provider : User
{
    public Provider()
    {
        // Availabilities = new List<ProviderAvailability>();
        // ProviderServices = new List<ProviderService>();
    }
    public bool isVerified { get; set; }


    // public List<ProviderService> ProviderServices { get; set; }
    // public List<ProviderAvailability> Availabilities { get; set; } // Make sure this property is present
    //list<Service>
    //image (profile)
    //image (license)
}
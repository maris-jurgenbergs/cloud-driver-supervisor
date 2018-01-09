namespace User.Service.Domain.Entities
{
    using System;

    public class User
    {
        public Guid AzureId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public double CreatedAt { get; set; }

        public string Phone { get; set; }
    }
}
namespace Common.Contracts.User
{
    using System;
    using System.Collections.Generic;

    public class UserDto
    {
        public Guid AzureId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public double CreatedAt { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public string Phone { get; set; }
    }
}
using System;

namespace NSE.Identidade.API.Models.Responses
{
    public class UserDataResponse
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime? Birthdate { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }
    }
}
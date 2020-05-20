using System.Collections.Generic;

namespace NSE.Identidade.API.Models.Responses
{
    public class UserTokenResponse
    {
        public string Id { get; set; }

        public string Email { get; set; }
        
        public IEnumerable<UserClaimResponse> Claims { get; set; }
    }
}
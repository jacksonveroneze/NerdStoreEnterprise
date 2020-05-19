using System.Collections.Generic;

namespace NSE.Identidade.API.Models.Responses
{
    public class UsuarioToken
    {
        public string Id { get; set; }

        public string Email { get; set; }
        
        public IEnumerable<UsuarioClaim> Claims { get; set; }
    }
}
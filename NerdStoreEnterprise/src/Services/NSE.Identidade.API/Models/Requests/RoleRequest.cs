using System.ComponentModel.DataAnnotations;

namespace NSE.Identidade.API.Models.Requests
{
    public class RoleRequest
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Name { get; set; }
    }
}
namespace NSE.Identidade.API.Models.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }

        public double ExpiresIn { get; set; }

        public UserTokenResponse UsuarioToken { get; set; }
    }
}

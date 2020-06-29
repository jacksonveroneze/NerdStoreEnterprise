using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSE.Identidade.API.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
                return Ok(result);

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponseCreated(string url, object result = null)
        {
            return Created(url, result);
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            IEnumerable<ModelError> erros = modelState.Values.SelectMany(e => e.Errors);

            foreach (ModelError erro in erros)
                AdicionarErroProcessamento(erro.ErrorMessage);

            return CustomResponse();
        }

        protected bool OperacaoValida()
            => !Erros.Any();

        protected void AdicionarErroProcessamento(string erro)
            => Erros.Add(erro);

        protected void LimparErrosProcessamento()
            => Erros.Clear();
    }
}

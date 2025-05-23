using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Common;

namespace Questao5.Infrastructure.Services.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BaseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected IActionResult Retorno<T>(Resultado<T> resultado)
        {
            if (resultado.Sucesso)
            {
                return Ok(resultado);
            }

            return BadRequest(resultado);
        }
    }
}

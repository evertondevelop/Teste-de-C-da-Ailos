using MediatR;
using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commands.Responses;
using Questao5.Application.Common;
using Questao5.Application.Queries.Requests;
using Questao5.Application.Queries.Responses;

namespace Questao5.Infrastructure.Services.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ContaCorrenteController : BaseController
    {
        public ContaCorrenteController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Consulta o saldo da conta corrente
        /// </summary>
        /// <param name="request">Id da conta corrente</param>
        /// <returns>Saldo bancário de conta corrente</returns>
        /// <response code="200">Retorna dados da conta e saldo bancário.</response>
        /// <response code="400">Retorna uma lista de erros de validação</response>
        [HttpGet("BuscarSaldoConta/{ContaCorrenteId}")]
        [ProducesResponseType(typeof(Resultado<SaldoContaCorrenteQueryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Resultado<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BuscarSaldoContaRequest([FromRoute] SaldoContaCorrenteQueryRequest request)
        {
            var result = await _mediator.Send(request);
            return Retorno(result);
        }

        /// <summary>
        /// Realiza movimentação de crédito ou débito na conta corrente
        /// </summary>
        /// <param name="request">Dados requeridos para movimentação</param>
        /// <returns>Id da movimentação realizada</returns>
        /// <response code="200">Retorna o id da movimentação realizada.</response>
        /// <response code="400">Retorna uma lista de erros de validação</response>
        [HttpPost("MovimentarConta")]
        [ProducesResponseType(typeof(Resultado<MovimentarContaCorrenteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Resultado<>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MovimentarContaRequest([FromBody] MovimentarContaCorrenteRequest request)
        {
            var result = await _mediator.Send(request);
            return Retorno(result);
        }
    }
}
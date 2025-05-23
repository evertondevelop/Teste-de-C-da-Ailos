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
    [Route("[controller]")]
    public class ContaCorrenteController : BaseController
    {
        public ContaCorrenteController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Consulta o saldo da conta corrente
        /// </summary>
        /// <param name="ContaCorrenteId">B6BAFC09-6967-ED11-A567-055DFA4A16C9</param>
        /// <returns>Saldo bancário de conta corrente</returns>
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
        /// <param name="IdentificadorRequisicao">111379f4-e79b-4139-af29-bdd9a425909e</param>
        /// <param name="ContaCorrenteId">FA99D033-7067-ED11-96C6-7C5DFA4A16C9</param>
        /// <param name="Valor">100</param>
        /// <param name="TipoMovimento">C</param>
        /// <returns>Id da movimentação realizada</returns>
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
using NSubstitute;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Domain.Entities;
using Xunit;
using Questao5.Application.Handlers;
using Questao5.Application.Queries.Requests;
using NSubstitute.ReturnsExtensions;
using FluentAssertions;
using Questao5.Application.Commons.Enumerators;

namespace Questao5.Tests.Units.Handlers
{
    public class SaldoContaCorrenteQueryHandlerTests
    {
        [Fact(DisplayName = "Retornar erro quando não encontrado conta corrente")]
        public async Task Saldo_Conta_Corrente_Quando_Conta_Inexistente()
        {
            // Arrange
            var command = new SaldoContaCorrenteQueryRequest() { ContaCorrenteId = Guid.NewGuid().ToString() };

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId).ReturnsNull();

            var handler = new SaldoContaCorrenteQueryHandler(contaCorrenteQueryStoreFake);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INVALID_ACCOUNT.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando conta corrente está inativa")]
        public async Task Saldo_Conta_Corrente_Quando_Conta_Inativa()
        {
            // Arrange
            var command = new SaldoContaCorrenteQueryRequest() { ContaCorrenteId = Guid.NewGuid().ToString() };

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId)
                .Returns(new ContaCorrente
                {
                    IdContaCorrente = command.ContaCorrenteId,
                    Ativo = false
                });

            var handler = new SaldoContaCorrenteQueryHandler(contaCorrenteQueryStoreFake);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INACTIVE_ACCOUNT.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando conta corrente id está nulo")]
        public async Task Saldo_Conta_Corrente_Quando_Conta_ID_Nulo()
        {
            // Arrange
            var command = new SaldoContaCorrenteQueryRequest();
            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            var handler = new SaldoContaCorrenteQueryHandler(contaCorrenteQueryStoreFake);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.VALIDATION_ERROR.ToString());
        }
    }
}

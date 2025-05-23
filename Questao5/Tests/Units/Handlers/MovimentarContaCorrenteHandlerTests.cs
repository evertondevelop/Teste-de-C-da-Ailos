using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Questao5.Application.Commands.Requests;
using Questao5.Application.Commons.Enumerators;
using Questao5.Application.Handlers;
using Questao5.Application.Interfaces;
using Questao5.Application.Interfaces.QueryStore;
using Questao5.Domain.Entities;
using System.Text.Json;
using Xunit;

namespace Questao5.Tests.Units.Handlers
{
    public class MovimentarContaCorrenteHandlerTests
    {
        [Fact(DisplayName = "Retonar sucesso ao movimentar conta corrente")]
        public async Task Movimentar_Conta_Corrente_Sucesso()
        {
            // Arrange
            var command = CreateCommand();

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            idempotenciaQueryStoreFake
                .BuscarIdempotenciaPorID(command.IdentificadorRequisicao)
                .ReturnsNull();

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId)
                .Returns(new ContaCorrente
                {
                    IdContaCorrente = command.ContaCorrenteId,
                    Ativo = true
                });

            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();
            inserirMovimentoServiceFake
                .Execute(Arg.Any<Movimento>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeTrue();
        }

        [Fact(DisplayName = "Retornar sucesso quando encontrado idempotencia id")]
        public async Task Movimentar_Conta_Corrente_Quando_Requisicao_Repetida()
        {
            // Arrange
            var command = CreateCommand();
            var resultadoIdempotencia = Guid.NewGuid().ToString();

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            idempotenciaQueryStoreFake
                .BuscarIdempotenciaPorID(command.IdentificadorRequisicao)
                .Returns(new Idempotencia() { Id = command.IdentificadorRequisicao, Resultado = JsonSerializer.Serialize(resultadoIdempotencia) });

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeTrue();
            result.Value!.MovimentoId.Should().Be(resultadoIdempotencia);
        }

        [Fact(DisplayName = "Retornar erro quando não encontrado conta corrente")]
        public async Task Movimentar_Conta_Corrente_Quando_Conta_Inexistente()
        {
            // Arrange
            var command = CreateCommand();

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            idempotenciaQueryStoreFake
                .BuscarIdempotenciaPorID(command.IdentificadorRequisicao)
                .ReturnsNull();

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId)
                .ReturnsNull();

            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INVALID_ACCOUNT.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando conta corrente está inativa")]
        public async Task Movimentar_Conta_Corrente_Quando_Conta_Inativa()
        {
            // Arrange
            var command = CreateCommand();

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            idempotenciaQueryStoreFake
                .BuscarIdempotenciaPorID(command.IdentificadorRequisicao)
                .ReturnsNull();

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId)
                .Returns(new ContaCorrente
                {
                    IdContaCorrente = command.ContaCorrenteId,
                    Ativo = false
                });

            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INACTIVE_ACCOUNT.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando ocorrer falha ao inserir movimentação de conta corrente")]
        public async Task Movimentar_Conta_Corrente_Quando_Falha_Inserir_Movimento()
        {
            // Arrange
            var command = CreateCommand();

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            idempotenciaQueryStoreFake
                .BuscarIdempotenciaPorID(command.IdentificadorRequisicao)
                .ReturnsNull();

            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            contaCorrenteQueryStoreFake.BuscarContaCorrentePorID(command.ContaCorrenteId)
                .Returns(new ContaCorrente
                {
                    IdContaCorrente = command.ContaCorrenteId,
                    Ativo = true
                });

            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.ERROR_INSERTING_MOVEMENT.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando inserir um valor de movimento menor que 1")]
        public async Task Movimentar_Conta_Corrente_Quando_Valor_Menor_Um()
        {
            // Arrange
            var command = CreateCommand(0);

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INVALID_VALUE.ToString());
        }

        [Fact(DisplayName = "Retornar erro quando inserir um tipo inválido de movimento")]
        public async Task Movimentar_Conta_Corrente_Quando_Tipo_Invalido()
        {
            // Arrange
            var command = CreateCommand(tipoMovimento: "W");

            var idempotenciaQueryStoreFake = Substitute.For<IIdempotenciaQueryStore>();
            var contaCorrenteQueryStoreFake = Substitute.For<IContaCorrenteQueryStore>();
            var inserirMovimentoServiceFake = Substitute.For<IInserirMovimentoService>();

            var handler = CreateHandler(
                idempotenciaQueryStoreFake,
                contaCorrenteQueryStoreFake,
                inserirMovimentoServiceFake
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Sucesso.Should().BeFalse();
            result.Value.Should().BeNull();
            result.Errors.Should().Contain(erro => erro.Tipo == ETipoErro.INVALID_TYPE.ToString());
        }

        #region Funções auxiliares
        private static MovimentarContaCorrenteRequest CreateCommand(decimal value = 100, string tipoMovimento = "C")
        {
            return new MovimentarContaCorrenteRequest
            {
                IdentificadorRequisicao = Guid.NewGuid().ToString(),
                ContaCorrenteId = Guid.NewGuid().ToString(),
                TipoMovimento = tipoMovimento,
                Valor = value
            };
        }

        private static MovimentarContaCorrenteHandler CreateHandler(IIdempotenciaQueryStore idempotenciaQueryStore, IContaCorrenteQueryStore contaCorrenteQueryStore, IInserirMovimentoService inserirMovimentoService)
        {
            return new MovimentarContaCorrenteHandler(
                idempotenciaQueryStore,
                contaCorrenteQueryStore,
                inserirMovimentoService
            );
        }
        #endregion
    }
}

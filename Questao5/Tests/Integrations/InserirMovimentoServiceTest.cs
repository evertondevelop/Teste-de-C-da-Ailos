using Dapper;
using Microsoft.Data.Sqlite;
using NSubstitute;
using Questao5.Domain.Entities;
using Questao5.Infrastructure.Sqlite;
using System.Data;
using Xunit;
using Questao5.Application.Services;
using Questao5.Application.Interfaces.CommandStore;
using Questao5.Domain.Enumerators;
using System.Text.Json;
using FluentAssertions;
using Questao5.Infrastructure.Database.CommandStore;
using NSubstitute.ExceptionExtensions;

namespace Questao5.Tests.Integrations
{
    public class InserirMovimentoServiceTest : IAsyncLifetime
    {
        private IDbConnection? _conn;

        #region Configuração de banco de dados
        public Task InitializeAsync()
        {
            var bootstrap = new DatabaseBootstrap(new DatabaseConfig { Name = "Data Source=integration_test.db" });
            bootstrap.Setup();

            var connection = new SqliteConnection("Data Source=integration_test.db");
            connection.Open();
            _conn = connection;

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _conn?.Dispose();
            if (File.Exists("integration_test.db")) File.Delete("integration_test.db");
            return Task.CompletedTask;
        }
        #endregion

        [Fact(DisplayName = "Retornar sucesso ao inserir uma movimentação de conta corrente")]
        public async Task Inserir_Movimento_Conta_Sucesso()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                DataMovimento = DateTime.Now,
                TipoMovimento = ETipoMovimento.C.ToString(),
                Valor = 100
            };
            var idRequisicao = Guid.NewGuid().ToString();

            var movimentoCommandStoreFake = new MovimentoCommandStore(_conn!);
            var idempotenciaCommandStoreFake = new IdempotenciaCommandStore(_conn!);

            var handler = new InserirMovimentoService(_conn!,
                movimentoCommandStoreFake,
                idempotenciaCommandStoreFake
            );

            // Act
            var result = await handler.Execute(
                movimento,
                idRequisicao,
                JsonSerializer.Serialize(Arg.Any<string>()),
                JsonSerializer.Serialize(Arg.Any<string>())
            );
            var movimentoInserido = await _conn.QueryFirstOrDefaultAsync<Movimento>(
                "SELECT * FROM movimento WHERE idmovimento = @IdMovimento",
                new { movimento.IdMovimento }
            );
            var idempotenciaInserida = await _conn.QueryFirstOrDefaultAsync<Idempotencia>(
                "SELECT chave_idempotencia as Id FROM idempotencia WHERE chave_idempotencia = @IdRequisicao",
                new { IdRequisicao = idRequisicao }
            );

            // Assert
            result.Should().BeTrue();
            movimentoInserido.Should().NotBeNull();
            movimentoInserido.Should().BeEquivalentTo(movimento);
            idempotenciaInserida.Id.Should().Be(idRequisicao);
        }

        [Fact(DisplayName = "Retornar erro ao inserir uma movimentação e não inserir idempotência")]
        public async Task Inserir_Movimento_Conta_Erro_Inserir_Idempotencia()
        {
            // Arrange
            var movimento = new Movimento
            {
                IdMovimento = Guid.NewGuid().ToString(),
                IdContaCorrente = "B6BAFC09-6967-ED11-A567-055DFA4A16C9",
                DataMovimento = DateTime.Now,
                TipoMovimento = ETipoMovimento.C.ToString(),
                Valor = 100
            };
            var idRequisicao = Guid.NewGuid().ToString();

            var movimentoCommandStoreFake = new MovimentoCommandStore(_conn!);
            var idempotenciaCommandStoreFake = Substitute.For<IIdempotenciaCommandStore>();
            idempotenciaCommandStoreFake.SalvaIdempotencia(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Throws(new InvalidOperationException("Erro ao inserir idempotencia"));

            var handler = new InserirMovimentoService(_conn!,
                movimentoCommandStoreFake,
                idempotenciaCommandStoreFake
            );

            // Act
            var result = await handler.Execute(
                movimento,
                idRequisicao,
                JsonSerializer.Serialize(Arg.Any<string>()),
                JsonSerializer.Serialize(Arg.Any<string>())
            );
            var movimentoInserido = await _conn.QueryFirstOrDefaultAsync<Movimento>(
                "SELECT * FROM movimento WHERE idmovimento = @IdMovimento",
                new { movimento.IdMovimento }
            );
            var idempotenciaInserida = await _conn.QueryFirstOrDefaultAsync<Idempotencia>(
                "SELECT chave_idempotencia as Id FROM idempotencia WHERE chave_idempotencia = @IdRequisicao",
                new { IdRequisicao = idRequisicao }
            );

            // Assert
            result.Should().BeFalse();
            movimentoInserido.Should().BeNull();
            idempotenciaInserida.Should().BeNull();
        }
    }
}

using System;

namespace Questao1
{
    public class ContaBancaria
    {
        public long Numero { get; init; }
        public string Titular { get; private set; }
        public double Saldo { get; private set; } = 0;

        public ContaBancaria(long numero, string titular)
        {
            if (numero <= 0)
                throw new ArgumentOutOfRangeException(nameof(numero), "O número da conta deve ser positivo.");

            if (string.IsNullOrWhiteSpace(titular))
                throw new ArgumentException("O titular não pode ser vazio ou nulo.", nameof(titular));

            Numero = numero;
            Titular = titular;
        }

        public ContaBancaria(long numero, string titular, double depositoInicial) : this(numero, titular)
        {
            if (depositoInicial < 0)
                throw new ArgumentOutOfRangeException(nameof(depositoInicial), "O depósito inicial não pode ser negativo.");

            Deposito(depositoInicial);
        }

        public void TrocarNomeTitular(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome do titular não pode ser vazio ou nulo.", nameof(nome));

            Titular = nome;
        }

        public void Deposito(double quantia)
        {
            if (quantia <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantia), "A quantia do depósito deve ser positiva.");

            Saldo += quantia;
        }

        public void Saque(double quantia)
        {
            if (quantia <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantia), "A quantia do saque deve ser positiva.");                

            Saldo -= quantia + 3.50;
        }

        public override string ToString()
        {
            return $"Conta: {Numero}, Titular: {Titular}, Saldo: {Saldo:C}";
        }
    }
}

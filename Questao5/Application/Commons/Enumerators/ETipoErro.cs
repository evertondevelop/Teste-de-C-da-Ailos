namespace Questao5.Application.Commons.Enumerators
{
    public enum ETipoErro
    {
        /// <summary>
        /// A conta não foi encontrada
        /// </summary>
        INVALID_ACCOUNT,

        /// <summary>
        /// A conta está inativa
        /// </summary>
        INACTIVE_ACCOUNT,

        /// <summary>
        /// O valor não é positivo
        /// </summary>
        INVALID_VALUE,

        /// <summary>
        /// Tipo de movimento é inválido
        /// </summary>
        INVALID_TYPE,

        /// <summary>
        /// Erro ao inserir o movimento da conta corrente
        /// </summary>
        ERROR_INSERTING_MOVEMENT,

        /// <summary>
        /// Erros de validação de propriedades da requisição
        /// </summary>
        VALIDATION_ERROR
    }
}

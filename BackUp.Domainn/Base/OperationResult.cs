
namespace BackUp.Domain.Base
{

    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public dynamic? Data { get; set; }


        // Constructores estáticos para claridad al retornar
        public static OperationResult Success(dynamic? data = null, string message = "Operación exitosa.")
        {
            return new OperationResult
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        public static OperationResult Failure(string message = "Ocurrió un error.")
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = message
            };
        }

      

    }
}

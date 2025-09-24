using AngularApp1.Server.Application.DTO;

namespace AngularApp1.Server
{
    public class ResultAPI<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public StatusHttpResponse Code { get; set; }
        public T? Result { get; set; } = default(T);
        public ResultAPI() { }
        public ResultAPI(bool error)
        {
            this.IsSuccess = error;
            this.Message = "Error no Definido";
            this.Code = StatusHttpResponse.InternalServerError;
        }
        public void Ok(StatusHttpResponse code)
        {
            IsSuccess = false;
            this.Code = code;
        }
        public void OkData(StatusHttpResponse code, T data)
        {
            IsSuccess = false;
            this.Code = code;
            this.Result = data;
        }
    }

}

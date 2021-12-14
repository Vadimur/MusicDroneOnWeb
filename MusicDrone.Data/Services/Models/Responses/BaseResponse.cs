namespace MusicDrone.Data.Services.Models.Responses
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        private BaseResponse(bool success, T data = default, string message = default)
        {
            Data = data;
            Success = success;
            Message = message;
        }

        public static BaseResponse<T> Fail(string message) => new BaseResponse<T>(false, default, message);
        public static BaseResponse<T> Ok(T data = default, string message = default) => new BaseResponse<T>(true, data, message);
    }
}

namespace BusinessLayer.Model.Models
{
    public class SaveResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public SaveResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public SaveResult(bool isSuccess, string message, object data)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }
    }
}
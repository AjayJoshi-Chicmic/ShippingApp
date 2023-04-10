namespace BlogApplication.Models
{
    public class ResponseModel
    {
        public int statusCode { get; set; } = 200;
        public string message { get; set; } = "Success";
        public object? data { get; set; } = null;
        public bool isSuccess { get; set; } = true;
        public ResponseModel()
        {
        }
        public ResponseModel(string message)
        {
            this.message = message;
        }
        public ResponseModel(int statusCode,string message, object Data, bool isSuccess)
        {
            this.statusCode = statusCode;
            this.message = message;
            this.data = Data;
            this.isSuccess = isSuccess;
        }
        public ResponseModel(int statusCode, string message, bool isSuccess)
        {
            this.statusCode = statusCode;
            this.message = message;
            this.isSuccess = isSuccess;
        }
        public ResponseModel(string message, object data)
        {
            this.message = message;
            this.data = data;
        }
        public ResponseModel( object data)
        {
            this.data = data;
        }
    }
}

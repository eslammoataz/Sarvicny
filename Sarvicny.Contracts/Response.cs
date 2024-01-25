namespace Sarvicny.Contracts
{
    public class Response<T>
    {
   

        public string Status { get; set; }
        public bool isError { get; set; } = false;

        public string Message { get; set; } = "";
        public List<string> Errors { get; set; } = new List<string>();

        public T? Payload { get; set; }



    }
}

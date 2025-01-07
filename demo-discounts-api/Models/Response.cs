namespace demo_discounts_api.Models
{
    public class Response<T>
    {
        public Response(bool success, string reason, T payload)
        {
            Success = success;
            Reason = reason;
            Payload = payload;
        }

        public bool Success { get; set; }

        public string Reason { get; set; }

        public T Payload { get; set; }
    }
}

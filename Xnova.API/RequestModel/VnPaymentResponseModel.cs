namespace Xnova.API.RequestModel
{
    public class VnPaymentResponseModel
    {

        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponsecode { get; set; }
        public int Amount { get; set; }
        public string OrderInfo { get; set; }

        public string TxnRef { get; set; }

    }
    public class VnPaymentRequestModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Fullname { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

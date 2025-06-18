using Xnova.API.RequestModel;

namespace Xnova.API.Services
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}

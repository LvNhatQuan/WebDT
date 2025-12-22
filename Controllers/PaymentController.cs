//using Microsoft.AspNetCore.Mvc;
//using System.Security.Cryptography;
//using System.Text;
//using System.Text.Json;
//using WebDT.DAL;

//namespace WebDT.Controllers
//{
//    public class PaymentController : Controller
//    {
//        private readonly IConfiguration _config;
//        private readonly OrderDAL _orderDal = new OrderDAL();

//        public PaymentController(IConfiguration config)
//        {
//            _config = config;
//        }

//        // ================= MOMO CREATE =================
//        public async Task<IActionResult> MoMoPay(int orderId)
//        {
//            var order = _orderDal.GetOrderById(orderId);
//            if (order == null) return NotFound();

//            string partnerCode = _config["MoMo:PartnerCode"]!;
//            string accessKey = _config["MoMo:AccessKey"]!;
//            string secretKey = _config["MoMo:SecretKey"]!;
//            string endpoint = _config["MoMo:Endpoint"]!;
//            string returnUrl = _config["MoMo:ReturnUrl"]!;
//            string notifyUrl = _config["MoMo:NotifyUrl"]!;

//            string requestId = Guid.NewGuid().ToString();
//            string orderInfo = $"Thanh toán đơn hàng #{order.Id}";
//            string amount = ((int)order.GrandTotal).ToString();
//            string requestType = "captureWallet";

//            string rawHash =
//                $"accessKey={accessKey}" +
//                $"&amount={amount}" +
//                $"&extraData=" +
//                $"&ipnUrl={notifyUrl}" +
//                $"&orderId={order.Id}" +
//                $"&orderInfo={orderInfo}" +
//                $"&partnerCode={partnerCode}" +
//                $"&redirectUrl={returnUrl}" +
//                $"&requestId={requestId}" +
//                $"&requestType={requestType}";

//            string signature = HmacSHA256(rawHash, secretKey);

//            var payload = new
//            {
//                partnerCode,
//                accessKey,
//                requestId,
//                amount,
//                orderId = order.Id.ToString(),
//                orderInfo,
//                redirectUrl = returnUrl,
//                ipnUrl = notifyUrl,
//                requestType,
//                signature
//            };

//            using var client = new HttpClient();
//            var response = await client.PostAsync(
//                endpoint,
//                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
//            );

//            var result = await response.Content.ReadAsStringAsync();
//            using var doc = JsonDocument.Parse(result);

//            string payUrl = doc.RootElement.GetProperty("payUrl").GetString()!;
//            return Redirect(payUrl);
//        }

//        // ================= MOMO RETURN =================
//        public IActionResult MoMoReturn(int orderId, int resultCode)
//        {
//            if (resultCode == 0)
//            {
//                _orderDal.UpdateStatus(orderId, "paid");
//                return RedirectToAction("Success", "Order", new { id = orderId });
//            }

//            _orderDal.UpdateStatus(orderId, "failed");
//            return RedirectToAction("Index", "Cart");
//        }

//        private static string HmacSHA256(string data, string key)
//        {
//            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
//            return BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(data)))
//                .Replace("-", "")
//                .ToLower();
//        }
//    }
//}

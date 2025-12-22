using Microsoft.AspNetCore.Http;

namespace WebDT.Helper
{
    public class ImageHelper
    {
        public static string UpLoadImage(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File rỗng");

            // 1️⃣ Danh sách MIME cho phép
            var allowedMimeTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/jpg",
                "image/webp"
            };

            // 2️⃣ Danh sách đuôi cho phép
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedMimeTypes.Contains(file.ContentType) ||
                !allowedExtensions.Contains(extension))
            {
                throw new Exception("File upload phải là ảnh (jpg, png, webp)");
            }

            // 3️⃣ Kiểm tra header thật (chống đổi đuôi)
            byte[] header = new byte[4];
            using (var stream = file.OpenReadStream())
            {
                stream.Read(header, 0, header.Length);
            }

            bool isImage =
                (header[0] == 0xFF && header[1] == 0xD8) || // JPG
                (header[0] == 0x89 && header[1] == 0x50) || // PNG
                (header[0] == 0x52 && header[1] == 0x49);   // WEBP (RIFF)

            if (!isImage)
                throw new Exception("File giả mạo ảnh");

            // 4️⃣ Tạo tên file an toàn
            var safeFileName = Guid.NewGuid().ToString("N") + extension;

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "Images", folder);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fullPath = Path.Combine(folderPath, safeFileName);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(fs);
            }

            // 5️⃣ Trả về path để lưu DB
            return $"/Images/{folder}/{safeFileName}";
        }
    }
}

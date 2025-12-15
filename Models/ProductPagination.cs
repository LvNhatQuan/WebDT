using System.Collections.Generic;

namespace WebDT.Models
{
    public class ProductPagination
    {
        // Danh sách sản phẩm của trang hiện tại
        public List<Product> Products { get; set; } = new();

        // Trang hiện tại (1-based)
        public int CurrentPageIndex { get; set; }

        // Tổng số trang
        public int PageCount { get; set; }

        // Helper cho View
        public bool HasPreviousPage => CurrentPageIndex > 1;
        public bool HasNextPage => CurrentPageIndex < PageCount;
    }
}

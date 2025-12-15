-- ===================================================
-- DATABASE: DT - E-commerce System
-- CREATED: 12/11/2025
-- DESCRIPTION: Database for E-commerce Application
-- ===================================================

USE [master]
GO

-- Create database if not exists
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'DT')
BEGIN
    CREATE DATABASE [DT]
    CONTAINMENT = NONE
    ON PRIMARY 
    (
        NAME = N'DT', 
        FILENAME = N'E:\DsqlServer\MSSQL16.SQLEXPRESS\MSSQL\DATA\DT.mdf', 
        SIZE = 8192KB, 
        MAXSIZE = UNLIMITED, 
        FILEGROWTH = 65536KB
    )
    LOG ON 
    (
        NAME = N'DT_log', 
        FILENAME = N'E:\DsqlServer\MSSQL16.SQLEXPRESS\MSSQL\DATA\DT_log.ldf', 
        SIZE = 8192KB, 
        MAXSIZE = 2048GB, 
        FILEGROWTH = 65536KB
    )
    WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
END
GO

USE [DT]
GO

-- ===================================================
-- DATABASE SETTINGS
-- ===================================================
ALTER DATABASE [DT] SET COMPATIBILITY_LEVEL = 160
GO

-- Enable Full-Text Search if installed
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC sp_fulltext_database @action = 'enable'
END
GO

-- Database configuration
ALTER DATABASE [DT] SET ANSI_NULL_DEFAULT OFF 
ALTER DATABASE [DT] SET ANSI_NULLS OFF 
ALTER DATABASE [DT] SET ANSI_PADDING OFF 
ALTER DATABASE [DT] SET ANSI_WARNINGS OFF 
ALTER DATABASE [DT] SET ARITHABORT OFF 
ALTER DATABASE [DT] SET AUTO_CLOSE ON 
ALTER DATABASE [DT] SET AUTO_SHRINK OFF 
ALTER DATABASE [DT] SET AUTO_UPDATE_STATISTICS ON 
ALTER DATABASE [DT] SET CURSOR_CLOSE_ON_COMMIT OFF 
ALTER DATABASE [DT] SET CURSOR_DEFAULT GLOBAL 
ALTER DATABASE [DT] SET CONCAT_NULL_YIELDS_NULL OFF 
ALTER DATABASE [DT] SET NUMERIC_ROUNDABORT OFF 
ALTER DATABASE [DT] SET QUOTED_IDENTIFIER OFF 
ALTER DATABASE [DT] SET RECURSIVE_TRIGGERS OFF 
ALTER DATABASE [DT] SET ENABLE_BROKER 
ALTER DATABASE [DT] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
ALTER DATABASE [DT] SET DATE_CORRELATION_OPTIMIZATION OFF 
ALTER DATABASE [DT] SET TRUSTWORTHY OFF 
ALTER DATABASE [DT] SET ALLOW_SNAPSHOT_ISOLATION OFF 
ALTER DATABASE [DT] SET PARAMETERIZATION SIMPLE 
ALTER DATABASE [DT] SET READ_COMMITTED_SNAPSHOT OFF 
ALTER DATABASE [DT] SET HONOR_BROKER_PRIORITY OFF 
ALTER DATABASE [DT] SET RECOVERY SIMPLE 
ALTER DATABASE [DT] SET MULTI_USER 
ALTER DATABASE [DT] SET PAGE_VERIFY CHECKSUM  
ALTER DATABASE [DT] SET DB_CHAINING OFF 
ALTER DATABASE [DT] SET FILESTREAM(NON_TRANSACTED_ACCESS = OFF) 
ALTER DATABASE [DT] SET TARGET_RECOVERY_TIME = 60 SECONDS 
ALTER DATABASE [DT] SET DELAYED_DURABILITY = DISABLED 
ALTER DATABASE [DT] SET ACCELERATED_DATABASE_RECOVERY = OFF  
ALTER DATABASE [DT] SET QUERY_STORE = ON
ALTER DATABASE [DT] SET QUERY_STORE (
    OPERATION_MODE = READ_WRITE, 
    CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), 
    DATA_FLUSH_INTERVAL_SECONDS = 900, 
    INTERVAL_LENGTH_MINUTES = 60, 
    MAX_STORAGE_SIZE_MB = 1000, 
    QUERY_CAPTURE_MODE = AUTO, 
    SIZE_BASED_CLEANUP_MODE = AUTO, 
    MAX_PLANS_PER_QUERY = 200, 
    WAIT_STATS_CAPTURE_MODE = ON
)
GO

-- ===================================================
-- CREATE TABLES
-- ===================================================

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Drop tables if they exist (in correct order to avoid foreign key conflicts)
IF OBJECT_ID('dbo.reviews', 'U') IS NOT NULL DROP TABLE dbo.reviews
IF OBJECT_ID('dbo.order_items', 'U') IS NOT NULL DROP TABLE dbo.order_items
IF OBJECT_ID('dbo.orders', 'U') IS NOT NULL DROP TABLE dbo.orders
IF OBJECT_ID('dbo.cart', 'U') IS NOT NULL DROP TABLE dbo.cart
IF OBJECT_ID('dbo.products', 'U') IS NOT NULL DROP TABLE dbo.products
IF OBJECT_ID('dbo.addresses', 'U') IS NOT NULL DROP TABLE dbo.addresses
IF OBJECT_ID('dbo.menu', 'U') IS NOT NULL DROP TABLE dbo.menu
IF OBJECT_ID('dbo.coupons', 'U') IS NOT NULL DROP TABLE dbo.coupons
IF OBJECT_ID('dbo.categories', 'U') IS NOT NULL DROP TABLE dbo.categories
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users
GO

-- ===================================================
-- TABLE: users
-- ===================================================
CREATE TABLE [dbo].[users](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [username] [varchar](50) NOT NULL,
    [email] [varchar](100) NOT NULL,
    [password] [nvarchar](200) NULL,
    [full_name] [nvarchar](100) NULL,
    [phone_number] [varchar](20) NULL,
    [address] [nvarchar](255) NULL,
    [avatar] [nvarchar](255) NULL,
    [role] [varchar](20) NULL DEFAULT 'customer',
    [created_at] [datetime] NULL DEFAULT GETDATE(),
    [is_active] [bit] NULL DEFAULT 1,
    [is_locked] [bit] NOT NULL DEFAULT 0,
    
    CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [UQ_users_username] UNIQUE ([username]),
    CONSTRAINT [UQ_users_email] UNIQUE ([email]),
    CONSTRAINT [CK_users_role] CHECK ([role] IN ('staff', 'admin', 'customer'))
)
GO

-- ===================================================
-- TABLE: addresses
-- ===================================================
CREATE TABLE [dbo].[addresses](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [user_id] [int] NOT NULL,
    [address_line] [nvarchar](255) NOT NULL,
    [city] [nvarchar](100) NULL,
    [district] [nvarchar](100) NULL,
    [phone_receiver] [nvarchar](20) NULL,
    [is_default] [bit] NULL DEFAULT 0,
    
    CONSTRAINT [PK_addresses] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_addresses_users] FOREIGN KEY([user_id]) 
        REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE
)
GO

-- ===================================================
-- TABLE: categories
-- ===================================================
CREATE TABLE [dbo].[categories](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [name] [nvarchar](100) NOT NULL,
    [description] [nvarchar](max) NULL,
    
    CONSTRAINT [PK_categories] PRIMARY KEY CLUSTERED ([id] ASC)
)
GO

-- ===================================================
-- TABLE: coupons
-- ===================================================
CREATE TABLE [dbo].[coupons](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [event_name] [nvarchar](100) NOT NULL,
    [discount_value] [int] NOT NULL,
    [start_date] [datetime] NULL,
    [end_date] [datetime] NULL,
    [is_active] [bit] NULL DEFAULT 1,
    
    CONSTRAINT [PK_coupons] PRIMARY KEY CLUSTERED ([id] ASC)
)
GO

-- ===================================================
-- TABLE: menu
-- ===================================================
CREATE TABLE [dbo].[menu](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [title] [nvarchar](100) NOT NULL,
    [parent_id] [int] NULL,
    [menu_url] [nvarchar](255) NULL,
    [menu_index] [int] NOT NULL DEFAULT 0,
    [isVisible] [bit] NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_menu] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_menu_parent] FOREIGN KEY([parent_id]) 
        REFERENCES [dbo].[menu] ([id])
)
GO

-- ===================================================
-- TABLE: products
-- ===================================================
CREATE TABLE [dbo].[products](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [category_id] [int] NULL,
    [coupon_id] [int] NULL,
    [name] [nvarchar](200) NOT NULL,
    [description] [nvarchar](max) NULL,
    [price] [decimal](10, 2) NOT NULL,
    [stock_quantity] [int] NULL DEFAULT 0,
    [image_url] [nvarchar](255) NULL,
    [is_active] [bit] NULL DEFAULT 1,
    [created_at] [datetime] NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_products] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_products_categories] FOREIGN KEY([category_id]) 
        REFERENCES [dbo].[categories] ([id]) ON DELETE SET NULL,
    CONSTRAINT [FK_products_coupons] FOREIGN KEY([coupon_id]) 
        REFERENCES [dbo].[coupons] ([id]) ON DELETE SET NULL
)
GO

-- ===================================================
-- TABLE: cart
-- ===================================================
CREATE TABLE [dbo].[cart](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [customerId] [int] NULL,
    [coupon_id] [int] NULL,
    [createAt] [datetime] NULL,
    [productId] [int] NULL,
    [quantity] [int] NULL,
    
    CONSTRAINT [PK_cart] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_cart_users] FOREIGN KEY([customerId]) 
        REFERENCES [dbo].[users] ([id]),
    CONSTRAINT [FK_cart_coupons] FOREIGN KEY([coupon_id]) 
        REFERENCES [dbo].[coupons] ([id]),
    CONSTRAINT [FK_cart_products] FOREIGN KEY([productId]) 
        REFERENCES [dbo].[products] ([id])
)
GO

-- ===================================================
-- TABLE: orders
-- ===================================================
CREATE TABLE [dbo].[orders](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [user_id] [int] NULL,
    [order_date] [datetime] NULL DEFAULT GETDATE(),
    [sub_total] [decimal](10, 2) NOT NULL,
    [shipping_fee] [decimal](10, 2) NULL DEFAULT 0,
    [discount_amount] [decimal](10, 2) NULL DEFAULT 0,
    [grand_total] [decimal](10, 2) NOT NULL,
    [shipping_address] [nvarchar](max) NOT NULL,
    [status] [nvarchar](20) NULL DEFAULT 'pending',
    
    CONSTRAINT [PK_orders] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_orders_users] FOREIGN KEY([user_id]) 
        REFERENCES [dbo].[users] ([id]) ON DELETE SET NULL
)
GO

-- ===================================================
-- TABLE: order_items
-- ===================================================
CREATE TABLE [dbo].[order_items](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [order_id] [int] NOT NULL,
    [coupon_id] [int] NULL,
    [product_id] [int] NULL,
    [quantity] [int] NOT NULL,
    [price] [decimal](10, 2) NOT NULL,
    [total_price] [decimal](10, 2) NOT NULL,
    
    CONSTRAINT [PK_order_items] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_order_items_orders] FOREIGN KEY([order_id]) 
        REFERENCES [dbo].[orders] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_order_items_coupons] FOREIGN KEY([coupon_id]) 
        REFERENCES [dbo].[coupons] ([id]) ON DELETE SET NULL,
    CONSTRAINT [FK_order_items_products] FOREIGN KEY([product_id]) 
        REFERENCES [dbo].[products] ([id]) ON DELETE SET NULL
)
GO

-- ===================================================
-- TABLE: reviews
-- ===================================================
CREATE TABLE [dbo].[reviews](
    [id] [int] IDENTITY(1,1) NOT NULL,
    [user_id] [int] NOT NULL,
    [product_id] [int] NOT NULL,
    [order_id] [int] NOT NULL,
    [rating] [tinyint] NOT NULL,
    [comment] [nvarchar](max) NULL,
    [created_at] [datetime] NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_reviews] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_reviews_users] FOREIGN KEY([user_id]) 
        REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_reviews_products] FOREIGN KEY([product_id]) 
        REFERENCES [dbo].[products] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_reviews_orders] FOREIGN KEY([order_id]) 
        REFERENCES [dbo].[orders] ([id]),
    CONSTRAINT [CK_reviews_rating] CHECK ([rating] BETWEEN 1 AND 5)
)
GO

-- ===================================================
-- INSERT SAMPLE DATA
-- ===================================================

-- Insert users (updated with address and avatar)
SET IDENTITY_INSERT dbo.users ON
INSERT INTO dbo.users ([id], [username], [email], [password], [full_name], [phone_number], [address], [avatar], [role], [created_at], [is_active], [is_locked]) VALUES
(1, 'admin', 'admin@dt.com', 'Admin@123', N'Quản trị viên', '0900000000', N'123 Đường Lê Lợi, Hà Nội', N'admin_avatar.jpg', 'admin', '2025-12-09T15:20:43.003', 1, 0),
(2, 'staff01', 'staff@dt.com', '$2y$10$HxWlQ6P3hZnY7V7s8K9Z/OF6dLmWq1A2bC3dE4fG5hI6jK7L8M9N0', N'Trần Thị Nhân Viên', '0912345678', N'456 Đường Nguyễn Huệ, HCM', N'staff_avatar.jpg', 'staff', '2025-12-09T15:20:43.003', 1, 0),
(3, 'khach01', 'customer1@email.com', '$2y$10$HxWlQ6P3hZnY7V7s8K9Z/OF6dLmWq1A2bC3dE4fG5hI6jK7L8M9N0', N'Lê Văn Khách', '0909123456', N'789 Đường Trần Hưng Đạo, Đà Nẵng', N'customer1_avatar.jpg', 'customer', '2025-12-09T15:20:43.003', 1, 0),
(4, 'khach02', 'customer2@email.com', '$2y$10$HxWlQ6P3hZnY7V7s8K9Z/OF6dLmWq1A2bC3dE4fG5hI6jK7L8M9N0', N'Phạm Thị Mua Hàng', '0938123456', N'654 Đường Lý Thường Kiệt, Hà Nội', N'customer2_avatar.jpg', 'customer', '2025-12-09T15:20:43.003', 1, 0),
(5, 'khach03', 'customer3@email.com', '123456', N'Hoàng Văn Tiêu Dùng', '0977123456', N'987 Đường Hai Bà Trưng, HCM', N'customer3_avatar.jpg', 'customer', '2025-12-09T15:20:43.003', 1, 0),
(6, 'QUANLE', 'quanle@gmail.com', '170704', N'le van nhat quan', '0364424198', N'123 Nguyễn Văn Linh, Đà Nẵng', N'quanle_avatar.jpg', 'admin', '2025-12-09T22:42:50.727', 1, 0)
SET IDENTITY_INSERT dbo.users OFF
GO

-- Insert addresses
SET IDENTITY_INSERT dbo.addresses ON
INSERT INTO dbo.addresses ([id], [user_id], [address_line], [city], [district], [phone_receiver], [is_default]) VALUES
(1, 1, N'123 Đường Lê Lợi', N'Hà Nội', N'Quận Hoàn Kiếm', N'0987654321', 1),
(2, 2, N'456 Đường Nguyễn Huệ', N'Hồ Chí Minh', N'Quận 1', N'0912345678', 1),
(3, 3, N'789 Đường Trần Hưng Đạo', N'Đà Nẵng', N'Quận Hải Châu', N'0909123456', 1),
(4, 3, N'321 Đường Hoàng Diệu', N'Đà Nẵng', N'Quận Thanh Khê', N'0909123456', 0),
(5, 4, N'654 Đường Lý Thường Kiệt', N'Hà Nội', N'Quận Đống Đa', N'0938123456', 1),
(6, 5, N'987 Đường Hai Bà Trưng', N'Hồ Chí Minh', N'Quận 3', N'0977123456', 1)
SET IDENTITY_INSERT dbo.addresses OFF
GO

-- Insert categories
SET IDENTITY_INSERT dbo.categories ON
INSERT INTO dbo.categories ([id], [name], [description]) VALUES
(1, N'DTNEST', N'Sản phẩm yến sào thương hiệu DTNEST'),
(2, N'KHANEST', N'Sản phẩm yến sào thương hiệu KHANEST'),
(3, N'OKINAWA', N'Thạch rong nho thương hiệu OKINAWA')
SET IDENTITY_INSERT dbo.categories OFF
GO

-- Insert coupons
SET IDENTITY_INSERT dbo.coupons ON
INSERT INTO dbo.coupons ([id], [event_name], [discount_value], [start_date], [end_date], [is_active]) VALUES
(1, N'Khuyến mãi Tết Nguyên Đán', 20, '2024-01-01T00:00:00.000', '2024-02-28T00:00:00.000', 1),
(2, N'Sự kiện Chào Hè Sôi Động', 15, '2024-06-01T00:00:00.000', '2024-08-31T00:00:00.000', 1),
(3, N'Quốc tế Thiếu Nhi', 10, '2024-06-01T00:00:00.000', '2024-06-07T00:00:00.000', 1),
(4, N'Tết Trung Thu Đoàn Viên', 25, '2024-09-01T00:00:00.000', '2024-09-30T00:00:00.000', 1),
(5, N'Tri ân Khách hàng cuối năm', 10, '2024-12-01T00:00:00.000', '2024-12-31T00:00:00.000', 1)
SET IDENTITY_INSERT dbo.coupons OFF
GO

-- Insert menu
SET IDENTITY_INSERT dbo.menu ON
INSERT INTO dbo.menu ([id], [title], [parent_id], [menu_url], [menu_index], [isVisible]) VALUES
(1, N'Trang chủ', NULL, N'/', 1, 1),
(2, N'Sản phẩm', NULL, N'/Product', 2, 1),
(3, N'Danh mục', NULL, N'#', 3, 1),
(4, N'Yến sào', 3, N'/Product?categoryId=1', 1, 1),
(5, N'Khanest', 3, N'/Product?categoryId=2', 2, 1),
(6, N'Okinawa', 3, N'/Product?categoryId=3', 3, 1)
SET IDENTITY_INSERT dbo.menu OFF
GO

-- Insert products (updated with coupon_id)
SET IDENTITY_INSERT dbo.products ON
INSERT INTO dbo.products ([id], [category_id], [coupon_id], [name], [description], [price], [stock_quantity], [image_url], [is_active], [created_at]) VALUES
(1, 1, 1, N'DTNEST TỔ YẾN CHƯNG KHÔNG ĐƯỜNG', N'Thành phần: Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 6g , Isomalt 0,6g , Rong nho 250mg , Acesulfam kali , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 45000.00, 109, N'20251209152814Screenshot 2025-09-13 215844.png', 1, '2025-12-09T15:20:43.007'),
(2, 1, 1, N'DTNEST YẾN SỮA DÀNH CHO TRẺ EM', N'Thành phần: Mỗi lọ 70ml chứa: Nước, sợi yến tươi 6g, đường phèn, bột sữa (7%) (Whole milk, Premium Creamer), Taurin 50mg, Canxi 30mg, Calci lactat, Agar, Guar gum, Xanthan gum, Natri alginat, Chất nhũ hóa Acidmilk, Natri benzoat, Hương liệu tổng hợp dùng cho thực phẩm.', 60000.00, 100, N'yen_sua.jpg', 1, '2025-12-09T15:20:43.007'),
(3, 1, 2, N'DTNEST TỔ YẾN CHƯNG ĐƯỜNG PHÈN', N'Thành phần: Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 6g , Đường phèn 6g , Rong nho 250mg , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 44000.00, 100, N'to_yen_chung_duong_phen.jpg', 1, '2025-12-09T15:20:43.007'),
(4, 1, 2, N'DTNEST TỔ YẾN CHƯNG ĐÔNG TRÙNG HẠ THẢO', N'Thành phần: Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 6g , Đường phèn , Rong nho 250mg , Đông trùng hạ thảo 0.2g , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 60000.00, 100, N'dong_trung_ha_thao.jpg', 1, '2025-12-09T15:20:43.007'),
(5, 1, 3, N'DTNEST TỔ YẾN CHƯNG TỨ VỊ', N'Thành phần: Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 6g , Đường phèn , Rong nho 250mg , Long nhãn 1,1g , Hạt sen 0,5g , Kỷ tử 0,4g , Đông trùng hạ thảo 0,07g , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm', 50000.00, 100, N'to_yen_chung_tu_vi.jpg', 1, '2025-12-09T15:20:43.007'),
(6, 1, 3, N'DTNEST TỔ YẾN CHƯNG DÀNH CHO TRẺ EM', N'Thành phần: Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 6g , Đường phèn , Rong nho 250mg , Taurine 35mg , Lysine 35mg , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 46000.00, 100, N'to_yen_chung_danh_cho_te_em.jpg', 1, '2025-12-09T15:20:43.007'),
(7, 1, 4, N'DTNEST - CHÈ YẾN RONG NHO', N'Thành phần: Nước tinh kiết vừa đủ 150ml, tổ yến 10%, tinh chất rong nho 5%, đường phèn 3%, chất định ổn INS (406, 401, 415, 466), hượng liệu tổng hợp dung trong thực phẩm.', 99000.00, 50, N'che_yen_rong_nho.jpg', 1, '2025-12-09T15:20:43.007'),
(8, 1, 4, N'DTNEST - CHÈ YẾN NGŨ VỊ', N'Thành phần: Nước tinh kiết vừa đủ 150ml, tổ yến 10%, táo đỏ 4%, hạt sen 3%, long nhãn 3%, đường phèn 3%, kỉ tử 1%, hạt chia 1%, chất ổn định INS (406, 401, 415, 466), hượng liệu tổng hợp dung trong thực phẩm.', 99000.00, 50, N'che_yen_ngu_vi.jpg', 1, '2025-12-09T15:20:43.007'),
(9, 2, 5, N'KHANEST – TỔ YẾN CHƯNG ĐƯỜNG PHÈN', N'Mỗi lọ 70ml chứa: Nước, Đường phèn 5,4g , Sợi yến tươi 3,6g , Rong nho 250mg , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 36000.00, 100, N'KHANEST_duong_phen.jpg', 1, '2025-12-09T15:20:43.007'),
(10, 2, NULL, N'KHANEST - TỔ YẾN CHƯNG DÀNH CHO TRẺ EM - CANXI', N'Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 3,6g , Đường phèn , Rong nho 250mg , Canxi 35mg , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 38000.00, 100, N'tre_em_Canxi.jpg', 1, '2025-12-09T15:20:43.007'),
(11, 2, NULL, N'KHANEST - TỔ YẾN CHƯNG DÀNH CHO TRẺ EM - TAURIN LYSIN', N'Mỗi lọ 70ml chứa: Nước, Sợi yến tươi 3,6g , Đường phèn , Rong nho 250mg , Lysine 40mg , Taurine 40mg , Calci lactat , Agar , Guar gum , Xanthan gum , Natri alginat , Natri benzoat , Hương liệu tổng hợp dùng trong thực phẩm.', 38000.00, 100, N'tre_em_TAURIN_LYSIN.jpg', 1, '2025-12-09T15:20:43.007'),
(12, 3, 1, N'OKINAWA THẠCH RONG NHO VỊ XOÀI', N'Nước , Đường , Rong nho 3g , Nước cốt xoài 300mg , Taurine , Lysine , Bột rau câu dẻo , Carrageenan , Canxi , Xanthan gum , Natri alginat , Acid malic , Acid citric , Trinatri citrat , Kali sorbat , Natri benzoat , Chlorophyllins , hương xoài tổng hợp dùng trong thực phẩm.', 45000.00, 200, N'thach_rong_nho_vi_xoai.jpg', 1, '2025-12-09T15:20:43.007'),
(13, 3, 2, N'OKINAWA THẠCH RONG NHO VỊ TÁO', N'Nước , Đường , Rong nho 3g , Nước cốt táo 300mg , Taurine , Lysine , Bột rau câu dẻo , Carrageenan , Canxi , Xanthan gum , Natri alginat , Acid malic , Acid citric , Trinatri citrat , Kali sorbat , Natri benzoat , Chlorophyllins , hương táo tổng hợp dùng trong thực phẩm.', 45000.00, 200, N'thach_rong_nho_vi_cam.jpg', 1, '2025-12-09T15:20:43.007'),
(14, 3, 3, N'OKINAWA THẠCH RONG NHO VỊ DÂU', N'Nước , Đường , Rong nho 3g , Nước cốt dâu 300mg , Taurine , Lysine , Bột rau câu dẻo , Carrageenan , Canxi , Xanthan gum , Natri alginat , Acid malic , Acid citric , Trinatri citrat , Kali sorbat , Natri benzoat , Chlorophyllins , hương dâu tổng hợp dùng trong thực phẩm.', 45000.00, 200, N'thach_rong_nho_vi_dau.jpg', 1, '2025-12-09T15:20:43.007')
SET IDENTITY_INSERT dbo.products OFF
GO

-- Insert orders (removed coupon_id column)
SET IDENTITY_INSERT dbo.orders ON
INSERT INTO dbo.orders ([id], [user_id], [order_date], [sub_total], [shipping_fee], [discount_amount], [grand_total], [shipping_address], [status]) VALUES
(1, 3, '2025-12-09T15:20:43.010', 225000.00, 15000.00, 45000.00, 195000.00, N'789 Đường Trần Hưng Đạo, Quận Hải Châu, Đà Nẵng', 'completed'),
(2, 3, '2025-12-09T15:20:43.010', 90000.00, 15000.00, 0.00, 105000.00, N'321 Đường Hoàng Diệu, Quận Thanh Khê, Đà Nẵng', 'pending'),
(3, 4, '2025-12-10T10:30:00.000', 120000.00, 20000.00, 12000.00, 128000.00, N'654 Đường Lý Thường Kiệt, Quận Đống Đa, Hà Nội', 'shipped'),
(4, 5, '2025-12-11T14:45:00.000', 180000.00, 15000.00, 0.00, 195000.00, N'987 Đường Hai Bà Trưng, Quận 3, Hồ Chí Minh', 'processing')
SET IDENTITY_INSERT dbo.orders OFF
GO

-- Insert order_items (added coupon_id column)
SET IDENTITY_INSERT dbo.order_items ON
INSERT INTO dbo.order_items ([id], [order_id], [coupon_id], [product_id], [quantity], [price], [total_price]) VALUES
(1, 1, 1, 1, 5, 45000.00, 225000.00),
(2, 2, NULL, 12, 2, 45000.00, 90000.00),
(3, 3, 3, 3, 3, 44000.00, 132000.00),
(4, 4, NULL, 2, 2, 60000.00, 120000.00),
(5, 4, NULL, 5, 1, 50000.00, 50000.00)
SET IDENTITY_INSERT dbo.order_items OFF
GO

-- Insert reviews
SET IDENTITY_INSERT dbo.reviews ON
INSERT INTO dbo.reviews ([id], [user_id], [product_id], [order_id], [rating], [comment], [created_at]) VALUES
(1, 3, 1, 1, 5, N'Yến chưng rất thanh mát, không quá ngọt, đóng gói đẹp.', '2025-12-09T15:20:43.010'),
(2, 3, 12, 2, 4, N'Thạch dai ngon, vị xoài thơm nhưng hơi ít.', '2025-12-09T15:20:43.010'),
(3, 4, 3, 3, 5, N'Sản phẩm chất lượng, giao hàng nhanh.', '2025-12-10T16:30:00.000'),
(4, 5, 2, 4, 4, N'Bé nhà mình rất thích, sẽ mua lại.', '2025-12-11T18:00:00.000')
SET IDENTITY_INSERT dbo.reviews OFF
GO

-- Insert sample cart data
SET IDENTITY_INSERT dbo.cart ON
INSERT INTO dbo.cart ([id], [customerId], [coupon_id], [createAt], [productId], [quantity]) VALUES
(1, 3, 1, '2025-12-12T09:30:00.000', 1, 2),
(2, 3, NULL, '2025-12-12T09:30:00.000', 12, 1),
(3, 4, 3, '2025-12-12T10:15:00.000', 3, 3),
(4, 5, NULL, '2025-12-12T11:20:00.000', 2, 1)
SET IDENTITY_INSERT dbo.cart OFF
GO

-- ===================================================
-- CREATE INDEXES
-- ===================================================

-- Index for products table
CREATE NONCLUSTERED INDEX [IX_products_category] 
ON [dbo].[products] ([category_id])
INCLUDE ([name], [price], [is_active])
GO

CREATE NONCLUSTERED INDEX [IX_products_coupon] 
ON [dbo].[products] ([coupon_id])
WHERE [coupon_id] IS NOT NULL
GO

-- Index for orders table
CREATE NONCLUSTERED INDEX [IX_orders_user] 
ON [dbo].[orders] ([user_id])
INCLUDE ([order_date], [grand_total], [status])
GO

CREATE NONCLUSTERED INDEX [IX_orders_status] 
ON [dbo].[orders] ([status])
INCLUDE ([order_date], [grand_total])
GO

-- Index for order_items table
CREATE NONCLUSTERED INDEX [IX_order_items_order] 
ON [dbo].[order_items] ([order_id])
INCLUDE ([product_id], [quantity], [total_price])
GO

CREATE NONCLUSTERED INDEX [IX_order_items_coupon] 
ON [dbo].[order_items] ([coupon_id])
WHERE [coupon_id] IS NOT NULL
GO

-- Index for reviews table
CREATE NONCLUSTERED INDEX [IX_reviews_product] 
ON [dbo].[reviews] ([product_id])
INCLUDE ([rating], [created_at])
GO

-- ===================================================
-- CREATE VIEWS FOR REPORTING
-- ===================================================

-- View for product sales summary
CREATE VIEW [dbo].[vw_product_sales] AS
SELECT 
    p.id AS product_id,
    p.name AS product_name,
    c.name AS category_name,
    COUNT(oi.id) AS total_orders,
    SUM(oi.quantity) AS total_quantity_sold,
    SUM(oi.total_price) AS total_revenue,
    AVG(r.rating) AS average_rating
FROM dbo.products p
LEFT JOIN dbo.categories c ON p.category_id = c.id
LEFT JOIN dbo.order_items oi ON p.id = oi.product_id
LEFT JOIN dbo.reviews r ON p.id = r.product_id
GROUP BY p.id, p.name, c.name
GO

-- View for customer orders summary
CREATE VIEW [dbo].[vw_customer_orders] AS
SELECT 
    u.id AS user_id,
    u.username,
    u.email,
    u.full_name,
    COUNT(o.id) AS total_orders,
    SUM(o.grand_total) AS total_spent,
    MAX(o.order_date) AS last_order_date
FROM dbo.users u
LEFT JOIN dbo.orders o ON u.id = o.user_id
WHERE u.role = 'customer'
GROUP BY u.id, u.username, u.email, u.full_name
GO

-- View for active coupons
CREATE VIEW [dbo].[vw_active_coupons] AS
SELECT 
    id,
    event_name,
    discount_value,
    start_date,
    end_date
FROM dbo.coupons
WHERE is_active = 1 
    AND GETDATE() BETWEEN start_date AND end_date
GO

-- ===================================================
-- FINAL DATABASE SETTINGS
-- ===================================================
USE [master]
GO
ALTER DATABASE [DT] SET READ_WRITE 
GO

PRINT '==================================================='
PRINT 'Database [DT] created successfully!'
PRINT 'Tables created: 10'
PRINT 'Sample data inserted successfully'
PRINT 'Views created: 3'
PRINT '==================================================='
PRINT 'Changes made:'
PRINT '1. Added [address] and [avatar] columns to users table'
PRINT '2. Added [coupon_id] column to products table'
PRINT '3. Removed [coupon_id] column from orders table'
PRINT '4. Added [coupon_id] column to order_items table'
PRINT '5. Added [status] column to orders table'
PRINT '6. Added indexes for better performance'
PRINT '7. Added reporting views'
PRINT '==================================================='
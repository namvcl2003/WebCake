CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Tự động tăng Id
    FirstName NVARCHAR(100) NOT NULL, -- Tên
    LastName NVARCHAR(100) NOT NULL, -- Họ
    Email NVARCHAR(100) NOT NULL UNIQUE, -- Email (phải duy nhất)
    Phone NVARCHAR(50) NOT NULL, -- Số điện thoại
    Username NVARCHAR(100) NOT NULL UNIQUE, -- Tên đăng nhập (phải duy nhất)
    Password NVARCHAR(255) NOT NULL -- Mật khẩu
);


-- Bảng CakeModel để lưu thông tin về bánh
CREATE TABLE Cakes (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Tự động tăng Id
    Name NVARCHAR(255) NOT NULL, -- Tên bánh
    Price DECIMAL(18, 2) NOT NULL, -- Giá bánh
    ImageUrl NVARCHAR(255) -- Đường dẫn ảnh
);

-- Bảng BillingDetailsModel lưu thông tin khách hàng và sử dụng làm dữ liệu đăng nhập
CREATE TABLE BillingDetails (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Tự động tăng Id
    FirstName NVARCHAR(100) NOT NULL, -- Tên
    LastName NVARCHAR(100) NOT NULL, -- Họ
    Company NVARCHAR(255), -- Công ty (nếu có)
    Address NVARCHAR(255) NOT NULL, -- Địa chỉ
    Address2 NVARCHAR(255), -- Địa chỉ thứ hai (tùy chọn)
    City NVARCHAR(100) NOT NULL, -- Thành phố
    Email NVARCHAR(100) NOT NULL, -- Email
    Phone NVARCHAR(50) NOT NULL, -- Số điện thoại
    OrderNotes NVARCHAR(MAX), -- Ghi chú đơn hàng (tùy chọn)
    TimeOrder DATETIME , -- Thời gian đặt hàng
  --  Username NVARCHAR(100) NOT NULL UNIQUE, -- Tên đăng nhập (lưu ý phải duy nhất)
  --  Password NVARCHAR(255) NOT NULL -- Mật khẩu
  FOREIGN KEY (UserId) REFERENCES Users(Id) -- Tạo liên kết khóa ngoại tới bảng Users
);

-- Bảng CartModel lưu thông tin về giỏ hàng
CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY(1,1), -- Tự động tăng Id
    ProductName NVARCHAR(255) NOT NULL, -- Tên sản phẩm
    ImageUrl NVARCHAR(255), -- Đường dẫn ảnh
    Price DECIMAL(18, 2) NOT NULL, -- Giá của sản phẩm
    Quantity INT NOT NULL, -- Số lượng sản phẩm
    BillingDetailsId INT, -- Khóa ngoại liên kết tới bảng BillingDetails
    FOREIGN KEY (BillingDetailsId) REFERENCES BillingDetails(Id), -- Tạo liên kết khóa ngoại
    TotalPrice AS (Price * Quantity) -- Tính tổng giá trị sản phẩm tự động
);




INSERT INTO Cakes (Name, Price, ImageUrl)
VALUES 
('Chocolate Cake', 15.50, '~/TemplateShop/img/cake-feature/c-feature-1.jpg'),
('Vanilla Cake', 12.00, '~/TemplateShop/img/cake-feature/c-feature-2.jpg'),
('Strawberry Cake', 18.25, '~/TemplateShop/img/cake-feature/c-feature-3.jpg'),
('Red Velvet Cake', 20.00, '~/TemplateShop/img/cake-feature/c-feature-4.jpg'),
('Carrot Cake', 14.50, '~/TemplateShop/img/cake-feature/c-feature-5.jpg');


INSERT INTO BillingDetails (FirstName, LastName, Company, Address, Address2, City, Email, Phone, OrderNotes, TimeOrder, Username, Password)
VALUES 
('John', 'Doe', 'Doe Inc.', '123 Elm St', 'Apt 4', 'New York', 'john.doe@example.com', '1234567890', 'Deliver in the morning', GETDATE(), 'johndoe', 'password123'),
('Jane', 'Smith', 'Smith Co.', '456 Oak Ave', NULL, 'Los Angeles', 'jane.smith@example.com', '9876543210', 'Leave at the front desk', GETDATE(), 'janesmith', 'password456'),
('Michael', 'Johnson', NULL, '789 Maple Rd', NULL, 'Chicago', 'michael.j@example.com', '5556667777', 'No contact delivery', GETDATE(), 'michaelj', 'password789'),
('Emily', 'Davis', NULL, '101 Pine St', 'Suite 10', 'Houston', 'emily.d@example.com', '4443332222', 'Please call before delivery', GETDATE(), 'emilyd', 'password321'),
('David', 'Williams', 'Williams LLC', '202 Cedar Blvd', NULL, 'Miami', 'david.w@example.com', '2221110000', 'Deliver after 6 PM', GETDATE(), 'davidw', 'password654');



-- Giỏ hàng của John Doe
INSERT INTO Carts (ProductName, ImageUrl, Price, Quantity, BillingDetailsId)
VALUES 
('Chocolate Cake', '~/TemplateShop/img/cake-feature/c-feature-1.jpg', 15.50, 2, 1),
('Vanilla Cake', '~/TemplateShop/img/cake-feature/c-feature-2.jpg', 12.00, 1, 1);

-- Giỏ hàng của Jane Smith
INSERT INTO Carts (ProductName, ImageUrl, Price, Quantity, BillingDetailsId)
VALUES 
('Strawberry Cake', '~/TemplateShop/img/cake-feature/c-feature-3.jpg', 18.25, 1, 2),
('Red Velvet Cake', '~/TemplateShop/img/cake-feature/c-feature-4.jpg', 20.00, 1, 2);

-- Giỏ hàng của Michael Johnson
INSERT INTO Carts (ProductName, ImageUrl, Price, Quantity, BillingDetailsId)
VALUES 
('Carrot Cake', '~/TemplateShop/img/cake-feature/c-feature-5.jpg', 14.50, 3, 3);
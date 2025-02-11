-- Tạo bảng TypeCake
CREATE TABLE TypeCake (
    TypeCakeId INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);

-- Thêm cột TypeCakeId vào bảng Cakes và thiết lập khóa ngoại
ALTER TABLE Cakes
ADD TypeCakeId INT;

ALTER TABLE Cakes
ADD CONSTRAINT FK_Cakes_TypeCake
FOREIGN KEY (TypeCakeId) REFERENCES TypeCake(TypeCakeId);


-- Thêm dữ liệu mẫu vào TypeCake
INSERT INTO TypeCake (TypeName) VALUES ('Chocolate');
INSERT INTO TypeCake (TypeName) VALUES ('CupCake');
INSERT INTO TypeCake (TypeName) VALUES ('BirthDay');
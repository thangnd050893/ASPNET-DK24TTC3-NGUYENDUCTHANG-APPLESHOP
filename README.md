# ASPNET-DK24TTC3-NGUYENDUCTHANG-APPLESHOP

## Đồ án môn học Chuyên đề ASP.NET – Xây dựng website bán sản phẩm công nghệ của Apple

## Thông tin tác giả

* **Họ và tên:** Nguyễn Đức Thắng
* **Lớp:** DK24TTC3
* **MSSV:** 170124337
* **Email:** thangnd050893@tvu-onschool.edu.vn
* **Điện thoại:** 0972667461
* **Trường:** Đại học Trà Vinh
* **GVHD:** TS. Đoàn Phước Miền

## 1. Giới thiệu
Đồ án được thực hiện nhằm xây dựng một hệ thống website bán hàng sử dụng ASP.NET Core MVC.
Hệ thống bao gồm các chức năng cơ bản của một website thương mại điện tử như quản lý sản phẩm, quản lý danh mục, giỏ hàng, đặt hàng, theo dõi đơn hàng và trang quản trị dành cho quản trị viên.
------------------------------------------------------------------------
## 2. Công nghệ sử dụng
-   ASP.NET Core MVC (.NET 8)
-   Entity Framework Core
-   SQL Server 2019
-   HTML, CSS, Bootstrap 5
-   JavaScript, jQuery
-   Visual Studio 2022
-   SQL Server Management Studio (SSMS)
------------------------------------------------------------------------
## 4. Cấu trúc thư mục

    ├── docker/               chứa file phục vụ triển khai Docker 
    ├── progress-report/      chứa báo cáo tiến độ hằng tuần
    ├── soft/                 chứa phần mềm hỗ trợ trong quá trình làm đồ án 
    ├── src/
    │   └── AppleShop/        chứa toàn bộ mã nguồn dự án
    ├── thesis/               chứa tài liệu đồ án
    │   ├── doc/              file tài liệu dạng .doc, .docx
    │   ├── pdf/              file tài liệu dạng .pdf
    │   ├── html/             tài liệu dạng HTML
    │   ├── abs/              slide trình bày hoặc video demo
    │   └── refs/             tài liệu tham khảo
    └── README.md             tài liệu hướng dẫn
------------------------------------------------------------------------
## 5. Yêu cầu hệ thống
-   .NET SDK 8.0
-   SQL Server Management Studio (SSMS) 20 hoặc mới hơn SQL Server Management Studio (SSMS) 22
-   Visual Studio 2022 hoặc Visual Studio 2026
-   Hệ điều hành Windows 10 trở lên
------------------------------------------------------------------------
## 6. Hướng dẫn cài đặt

### 6.1. Tải source
1. Download ZIP từ GitHub.
2. Giải nén.

### 6.2. Cài đặt database

#### Bước 1 — Mở SSMS 20
- Server type: Database Engine  
- Server name:
```
localhost\SQLEXPRESS
```
- Authentication: Windows Authentication  
- Tick: Trust server certificate  
→ Connect

#### Bước 2 — Mở file SQL
File → Open → File → Chọn `AppleShopDbnew.sql`

#### Bước 3 — Chạy SQL
- Chọn database AppleShopDb
- Nhấn Execute (F5)
- Khi hiện “Commands completed successfully.” là đã tạo database xong.

#### Bước 4 — Kiểm tra bảng
Mở:
```
Databases → AppleShopDb → Tables
```

### 6.3. Cập nhật Connection String nếu Server name đã thay đổi 
Mở `appsettings.json` và thay: 
```json
"ConnectionStrings": {
  "AppleShopContext": "Server=localhost\SQLEXPRESS;Database=AppleShopDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 6.4. Chạy dự án

1. **Mở Visual Studio 2022**
   - Nhấn Start → gõ **Visual Studio 2022** → mở ứng dụng.

2. **Mở solution của dự án**
   - Chọn:
     ```
     File → Open → Project/Solution...
     ```
   - Chọn file:
     ```
     src/AppleShop/AppleShop.sln
     ```

3. **Chọn cấu hình chạy**
   - Ở góc trên Visual Studio, chọn:
     ```
     IIS Express
     ```

4. **Chạy website**
   - Nhấn **F5** hoặc nhấn nút **Run (IIS Express)**.

5. **Truy cập website**
   - Visual Studio sẽ tự mở trình duyệt.
   - Nếu không, bạn có thể truy cập:
     ```
     https://localhost:xxxx
     ```
6. **Tài Khoản Admin **
- https://localhost:xxxx/admin
- tài khoản: admin
- mật khẩu : admin@123
### 7. Link Demo trên Hosting
- [https://thangnd.com](```
     https://thangnd.com
     ```)


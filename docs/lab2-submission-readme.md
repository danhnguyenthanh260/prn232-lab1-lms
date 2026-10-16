# PRN232 Lab 2 — Advanced REST API & Security

Sinh viên: **chưa cung cấp — bản nháp, chưa nộp**. Điền thông tin thật trong submission.json và tài liệu này trước khi đóng gói cuối.

## Chạy bằng Docker Desktop

Yêu cầu Docker Desktop ở chế độ Linux containers, Docker Compose và cổng trống.
Trong PowerShell tại thư mục bài nộp:

```powershell
$env:API_PORT='8093'
docker compose -p prn232-lab2 up --build -d
```

- Swagger: http://localhost:8093/swagger
- Giao diện: http://localhost:8093/students
- Đăng nhập: http://localhost:8093/login.html
- Health: http://localhost:8093/health
- API v1: http://localhost:8093/api/v1/students

API chờ database sẵn sàng, tạo schema, tự seed và chạy trước khi nhận request.
Compose truyền khóa JWT qua `Jwt__Key`, có thể ghi đè bằng biến `JWT_KEY`.
Khóa và tài khoản mặc định chỉ dành cho môi trường thực hành cục bộ.
Không dùng chung Compose project/volume với Lab 1. Dừng bằng `docker compose -p prn232-lab2 stop` để giữ dữ liệu.

## Kiến trúc

Ba project .NET 8: API (HTTP, binding, validation, middleware, Swagger), Services (nghiệp vụ và phát token), Repositories (EF Core, schema, truy vấn và persistence). SQL Server 2022 chạy trong container riêng. Giao diện tĩnh được API phục vụ.

Tái sử dụng Semester, Subject, Course, Student, Enrollment của Lab 1. Thêm User và RefreshToken. Mật khẩu lưu bằng PasswordHasher; refresh token ngẫu nhiên chỉ lưu SHA-256 trong DB. Schema upgrade chỉ thêm bảng/cột, không xóa dữ liệu cũ. Student có thêm phoneNumber tùy chọn.

## Đăng nhập và phân quyền

| Tài khoản thực hành | Mật khẩu | Role |
|---|---|---|
| admin | Admin@123 | Admin |
| student | Student@123 | Student |

1. POST `/api/v1/auth/login` với `username`, `password`.
2. Lấy `data.accessToken`, bấm **Authorize** trong Swagger và dán token.
3. GET `/api/v1/auth/me` yêu cầu đăng nhập.
4. GET `/api/v1/enrollments` và POST/PUT/DELETE students chỉ dành cho Admin. Các danh mục còn lại cho phép đọc công khai trong phạm vi dữ liệu thực hành.
5. POST `/api/v1/auth/refresh-token` với `refreshToken` để đổi cả access token và refresh token mới. Token refresh cũ không thể dùng lại.

Access token có thời hạn 1 giờ, refresh token 7 ngày. Không có/token hỏng/hết hạn: 401. Token hợp lệ nhưng sai quyền: 403. Sai thông tin đăng nhập: 401.

## API và validation

- Năm resource giữ search, sort, page, size, fields, expand; size tối đa 100. `fields` quyết định tập thuộc tính cuối cùng.
- `/api/...` là đường dẫn tương thích với Lab 1, cùng quy tắc phân quyền.
- `/api/v1/courses/1/students` trả danh sách sinh viên thuộc lớp, 404 nếu lớp không tồn tại.
- Student POST/PUT: fullName, email, dateOfBirth bắt buộc; phoneNumber tùy chọn. Không nhận field JSON không khai báo.
- DataAnnotations kiểm trường bắt buộc, độ dài, email, phone, range và regex. FluentValidation chạy trên StudentRequestModel. PastDateAttribute kiểm ngày sinh từ 1753 đến hôm nay.
- `Accept: application/json` hoặc `application/xml`; định dạng khác trả 406. XML giữ cấu trúc response và các field được chọn.
- `X-Request-Id` được nhận và trả lại. Logs stdout gồm method, path, status và thời gian ms. Không log mật khẩu/token/body; query chỉ ghi marker kiểm tra được giới hạn độ dài.
- Envelope: success, message, data, errors; collection thêm pagination. Lỗi không lộ chi tiết server.

## Demo

Health → Swagger → JSON/XML/406 → validation 400 → login → Authorize → Admin 200 → Student 403 → token hỏng 401 → refresh token → search/sort/paging/fields/expand → nested resource → Docker logs.

Source đề bài: LAB 2 - Advanced REST API & Security (9 trang) và PRN232_LAB2_Submission_Guideline (6 trang), trong Lab2.zip. Không áp dụng yêu cầu Final Assignment.

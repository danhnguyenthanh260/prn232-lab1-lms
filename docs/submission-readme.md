# PRN232 Lab 1 — LMS

MSSV / Họ tên / Lớp: **chưa cung cấp — bản nháp, chưa nộp**. Điền thông tin thật trong submission.json và tài liệu này trước khi đóng gói cuối.

Stack: .NET 8, EF Core, SQL Server 2022; ba project API → Services → Repositories.

Chạy bằng PowerShell khi Docker Desktop đã hoạt động:

```powershell
docker compose up --build -d
```

UI: http://localhost:8080/students · Swagger: http://localhost:8080/swagger · Readiness: /health. Đặt API_PORT nếu cần đổi cổng host. API publish cổng host theo đề; chỉ chạy trong mạng tin cậy, không mở ra Internet.

API có 5 list/detail resources và POST/PUT/DELETE students. Tất cả list hỗ trợ search, sort, page, size, fields, expand. Seed lần đầu: 5 semesters, 10 subjects, 20 courses, 50 students, 500 enrollments; knownIds ban đầu đều là 1. Startup dùng EnsureCreated và seed transaction, không cần lệnh migration thủ công. Không xóa/reset DB dùng chung; Compose chỉ dùng volume riêng.

Giới hạn: không auth; chỉ dùng local/class demo. Mật khẩu Compose mặc định là giá trị demo công khai, có thể thay bằng MSSQL_SA_PASSWORD. size>100 được cap100; fields quyết định property set cuối cùng kể cả expand; Subject độc lập; xóa Student sẽ xóa enrollments của student. Chưa có optimistic concurrency. UI tĩnh không thêm project .NET thứ tư. Nếu thay schema sau khi DB đã tạo, phải bổ sung migration hoặc dùng database mới.

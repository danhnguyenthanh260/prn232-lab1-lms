# PRN232 Lab 1 — LMS REST API

Ứng dụng ASP.NET Core REST API LMS và giao diện quản lý nhỏ gọn, theo đề Lab 1.

[GitHub Project](https://github.com/users/danhnguyenthanh260/projects/5) · [Issue tổng](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) · [Toàn bộ issues](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues)

| Tài liệu | Nội dung |
|---|---|
| [Coverage](docs/coverage.md) | 79 mục phân tích → issue → cách kiểm tra; đủ 21 tiêu chí và 6 penalty |
| [Kế hoạch](docs/plan.md) | 19 issues, 3 milestone, dependency và 10 điểm cần quyết định |
| [Nguồn](docs/sources.md) | Bộ tài liệu đã đọc, SHA-256, bản trùng và giới hạn của screenshot |
| [Dữ liệu kế hoạch](docs/planning-data.json) | Dữ liệu cấu trúc dùng để kiểm tra coverage |
| [Mô hình DB](docs/database-model.md) | ERD, 5 bảng/20 cột/3 FK và các quyết định chưa chốt |
| [API routes](docs/api-routes.md) | 13 operations, routes hỗ trợ và query/related-data contract |
| [Design pattern LMS](docs/design-pattern.md) | Brief thiết kế gốc; bản UI tối giản và kết quả thực tế xem implementation |

## Trạng thái

Đã có 3 tầng, SQL Server/EF Core, 13 operations, Swagger và UI danh sách/chi tiết/thêm/sửa/xóa Student. Xem [kết quả kiểm tra](docs/implementation.md). Build và kiểm tra HTTP đã chạy trên SQL Server LocalDB; Docker build bị chặn bởi ổ C gần đầy, **chưa nghiệm thu full Docker grader**. Coverage kế hoạch 79/79 không phải line/branch coverage.

Chạy nhanh trên máy Windows đã có LocalDB:

```powershell
./tools/run-local.ps1
```

Mở [LMS](http://localhost:8088/students) hoặc [Swagger](http://localhost:8088/swagger). Database riêng PRN232Lab1, không dùng DB dự án khác.

Chạy đúng môi trường Docker khi Docker Desktop/ổ đĩa sẵn sàng:

```powershell
$env:API_PORT='8088'
docker compose -p prn232-lab1 up --build -d
```

API chỉ bind host loopback; DB không publish port. Mật khẩu mặc định trong Compose chỉ là giá trị demo công khai, không dùng cho hệ thống thật.

## Phạm vi bài

- Đúng 3 project: API, Services, Repositories; 4 loại model tách biệt.
- 5 resource: Semester, Subject, Course, Student, Enrollment.
- 5 GET list + 5 GET detail + POST/PUT/DELETE Student.
- List hỗ trợ search, sort, paging, fields, expand.
- API và SQL Server chạy Docker Compose; tự tạo schema/seed; có /health và Swagger.
- Gói nộp ZIP sạch, submission.json và README theo phụ lục.

Không có auth/JWT, frontend hoặc cloud hosting trong phạm vi bắt buộc.

Theo yêu cầu triển khai ngày 2026-09-22, UI dùng HTML/CSS/JavaScript ngay trong API/wwwroot. Đây là phần mở rộng ngoài rubric; không thêm application project thứ tư hoặc toolchain frontend.

## Bắt đầu

Xem [repo map](REPO-MAP.md), [issue tổng](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) và các phụ thuộc trong Project. Bản gốc PDF/ZIP giảng viên được giữ local; không phân phối lại cùng source.

Kiểm tra tính đầy đủ của kế hoạch bằng:

```powershell
python tools/validate_plan.py
```

Lệnh này chỉ kiểm dữ liệu kế hoạch, không chấm bài API. Khi triển khai xong, issue QA yêu cầu full grader trên ZIP cuối cùng và bổ sung những trường hợp grader chưa kiểm tra.

Kiểm API bằng `python tools/check_api.py http://127.0.0.1:8088`. Tạo ZIP bằng `python tools/package.py`; script chỉ lấy ba project và cấu hình nộp bài, loại bin/obj/DB/QA artifacts. Identity thật lấy từ `submission.local.json` (ignored); public template không chứa thông tin sinh viên. Nếu thiếu identity, chỉ tạo ZIP DRAFT.

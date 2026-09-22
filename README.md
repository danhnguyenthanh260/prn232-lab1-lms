# PRN232 Lab 1 — LMS REST API

Kế hoạch làm bài ASP.NET Core REST API LMS theo đề, phụ lục nộp bài và autograder được cung cấp.

[GitHub Project](https://github.com/users/danhnguyenthanh260/projects/5) · [Issue tổng](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) · [Toàn bộ issues](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues)

| Tài liệu | Nội dung |
|---|---|
| [Coverage](docs/coverage.md) | 79 mục phân tích → issue → cách kiểm tra; đủ 21 tiêu chí và 6 penalty |
| [Kế hoạch](docs/plan.md) | 19 issues, 3 milestone, dependency và 10 điểm cần quyết định |
| [Nguồn](docs/sources.md) | Bộ tài liệu đã đọc, SHA-256, bản trùng và giới hạn của screenshot |
| [Dữ liệu kế hoạch](docs/planning-data.json) | Dữ liệu cấu trúc dùng để kiểm tra coverage |

## Trạng thái

Hiện mới có tài liệu và backlog; **chưa có code ứng dụng, chưa chạy build, Docker hoặc grader**. Tất cả 19 issues ở Todo. Coverage kế hoạch 79/79 không phải phần trăm code hoàn thành hay test coverage.

## Phạm vi bài

- Đúng 3 project: API, Services, Repositories; 4 loại model tách biệt.
- 5 resource: Semester, Subject, Course, Student, Enrollment.
- 5 GET list + 5 GET detail + POST/PUT/DELETE Student.
- List hỗ trợ search, sort, paging, fields, expand.
- API và SQL Server chạy Docker Compose; tự tạo schema/seed; có /health và Swagger.
- Gói nộp ZIP sạch, submission.json và README theo phụ lục.

Không có auth/JWT, frontend hoặc cloud hosting trong phạm vi bắt buộc.

## Bắt đầu

Xem [issue #2: nền tảng](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) và các phụ thuộc trong Project. Đọc nguồn và decision log trước khi code. Bản gốc PDF/ZIP được giữ ở Downloads của chủ repo; public repo chỉ chứa phân tích và hash nhận dạng.

Kiểm tra tính đầy đủ của kế hoạch bằng:

```powershell
python tools/validate_plan.py
```

Lệnh này chỉ kiểm dữ liệu kế hoạch, không chấm bài API. Khi triển khai xong, issue QA yêu cầu full grader trên ZIP cuối cùng và bổ sung những trường hợp grader chưa kiểm tra.

README hiện tại mô tả repository lập kế hoạch. Issue đóng gói sẽ tạo README nộp bài tối đa một trang với MSSV, họ tên, lệnh chạy, seed counts và limitations thực tế.

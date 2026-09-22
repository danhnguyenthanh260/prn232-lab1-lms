# Routes và hợp đồng Web API

Bài lab bắt buộc là **backend web: ASP.NET Core REST API**, không phải website có màn hình nghiệp vụ sẵn. Swagger là giao diện thử API. UI quản lý LMS là phần thiết kế bổ sung người dùng chọn, tách khỏi rubric và gói nộp: [design pattern](design-pattern.md).

Luồng: HTTP client → API controller → Services → Repositories/EF Core → SQL Server. Đúng 3 application projects; UI chưa chọn công nghệ và chưa được triển khai.

## 13 operations nghiệp vụ bắt buộc

| Method | Route | Thành công | Chủ issue |
|---|---|---|---|
| GET | /api/semesters | 200, danh sách | #8 |
| GET | /api/semesters/{id} | 200, chi tiết | #8 |
| GET | /api/subjects | 200, danh sách | #8 |
| GET | /api/subjects/{id} | 200, chi tiết | #8 |
| GET | /api/courses | 200, danh sách | #8 |
| GET | /api/courses/{id} | 200, chi tiết | #8 |
| GET | /api/students | 200, danh sách | #7 |
| GET | /api/students/{id} | 200, chi tiết | #7 |
| GET | /api/enrollments | 200, danh sách | #9 |
| GET | /api/enrollments/{id} | 200, chi tiết | #9 |
| POST | /api/students | 201, Location trỏ tới Student đã tạo | #15 |
| PUT | /api/students/{id} | 200, cập nhật | #15 |
| DELETE | /api/students/{id} | 200, xóa | #15 |

Detail hoặc mutation trên ID không tồn tại trả 404; input/query không hợp lệ trả 400 và error envelope. Không tạo POST/PUT/DELETE cho bốn resource còn lại. POST/PUT dùng business fields của Student; ID URL/server-managed, không dùng làm input nhập tay khi tạo.

## Routes hỗ trợ

| Method | Route | Mục đích / giới hạn |
|---|---|---|
| GET | /health | Readiness; 200 khi sẵn sàng, không báo sẵn sàng nếu DB chưa dùng được (#17) |
| GET | /swagger hoặc /swagger/index.html | Swagger UI phải dùng được trong container (#16) |
| GET | /swagger/v1/swagger.json | OpenAPI document (#16); không bọc envelope nghiệp vụ |

Không tính các route hỗ trợ vào 13 operations. Envelope của health xem DEC-10; Swagger HTML/JSON giữ định dạng chuẩn.

## Query contract cho cả 5 list

| Tham số | Quy tắc | Chủ issue |
|---|---|---|
| search | Contains, không phân biệt hoa thường trên main text fields | #10 |
| sort | Tên field camelCase; dấu `-` giảm dần; nhiều field ngăn bởi dấu phẩy | #11 |
| page | 1-based, mặc định 1; 0/âm/sai kiểu → 400 | #12 |
| size | Mặc định 10, tối đa 100; không dương/sai kiểu → 400; >100 xem DEC-03 | #12 |
| fields | Exact property set trên mọi item, không bỏ envelope/pagination | #13 |
| expand | Chỉ quan hệ được yêu cầu, tên không hợp lệ → 400 | #14 |

Sort/fields/expand dùng allowlist theo resource; không chèn tên field trực tiếp vào SQL. Query model thuộc đúng tầng theo kế hoạch #6. Ví dụ hợp lệ riêng lẻ: `/api/students?search=an&sort=fullName,-dateOfBirth&page=1&size=10`, `/api/students?fields=studentId,fullName`, `/api/enrollments?expand=student,course`.

Thứ tự đề xuất: validate → search → count → sort ổn định (PK làm tie-breaker) → page → projection/expand theo contract. `totalItems` đếm sau search, trước page. Query `fields+expand` còn mâu thuẫn nguồn: phải chốt DEC-01, không tự gọi một cách hiểu là đáp án bắt buộc.

## Quan hệ trả về

| Resource | Detail: quan hệ trực tiếp đề xuất theo ERD | List expand allowlist đề xuất |
|---|---|---|
| Semester | courses | courses |
| Subject | Không có FK trong schema | Rỗng; expand quan hệ bất kỳ → 400 |
| Course | semester, enrollments | semester, enrollments |
| Student | enrollments | enrollments |
| Enrollment | student, course | student, course |

`Enrollment expand=student,course` được phụ lục nêu rõ. Các tên quan hệ khác là thiết kế theo schema cần thống nhất trong DTO/OpenAPI, không phải toàn bộ đều được grader kiểm. Detail có related data theo yêu cầu; list không expand thì related absent/null. Nested DTO chỉ chứa scalar cần thiết, không serialize vòng Student→Enrollment→Student; không tự hỗ trợ đường dẫn expand nhiều tầng.

## Gate coverage

Issue #19 kiểm: 5 list, 5 detail found + 5 missing, POST đọc lại persisted data và Location, PUT đọc lại, DELETE đọc lại 404; malformed inputs và missing ID riêng. Matrix 5 resource × search/sort/paging/fields/expand; invalid allowlists kể cả DB rỗng; nhiều field sort có tie; expanded → plain → expanded để bắt state leak. Query tổ hợp chờ DEC-01 chốt. Xem [coverage gốc](coverage.md); không đánh dấu PASS khi chưa chạy API.

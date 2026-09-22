# Kế hoạch PRN232 Lab 1

Project: [PRN232 Lab 1 — LMS REST API](https://github.com/users/danhnguyenthanh260/projects/5). Phạm vi hiện tại là backlog và phân tích. 18 issue triển khai/nghiệm thu cộng 1 tracker; mọi issue ở Todo. Không có thời hạn/assignee tự đặt.

## Sản phẩm cần đạt

ASP.NET Core REST API LMS, đúng 3 tầng và 4 loại model; DB và API chạy qua Docker Compose. Nghiệp vụ bắt buộc gồm 5 GET list, 5 GET detail và 3 thao tác thêm/sửa/xóa Student. Có health và Swagger. Seed tối thiểu 5 semesters, 10 subjects, 20 courses, 50 students và 500 enrollments.

API chịu trách nhiệm HTTP/request/response; Services xử lý validation và nghiệp vụ; Repositories thực hiện EF query và persistence, giữ entity/data-access contract. API gọi Repositories chỉ để DI. Không truyền API Request/Response vào repository hoặc đưa DbContext lên controller.

Tài liệu/issue paths là **dự kiến**, do repo chưa có source ứng dụng. Mỗi issue phải xác minh path và SDK thực tế trước khi sửa.

## Công việc và phụ thuộc

| Key | Công việc | Milestone | Priority | Blocked by |
|---|---|---|---|---|
| [#1 TRACK](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) | [Theo dõi] Coverage toàn bộ đề bài, rubric và nghiệm thu Lab 1 | M3 - Đóng gói và nghiệm thu | P1 | — |
| [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | [Nền tảng] Solution 3 layer, DI và 4 loại model | M1 - Nền tảng | P0 | — |
| [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | [Dữ liệu] Schema LMS, khóa ngoại và truy cập qua EF Core | M1 - Nền tảng | P0 | [#2](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) |
| [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | [Khởi động] Migration, seed lặp lại an toàn và readiness | M1 - Nền tảng | P0 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) |
| [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | [API] Envelope thống nhất, camelCase và phản hồi lỗi | M1 - Nền tảng | P0 | [#2](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) |
| [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | [API] Query model, allowlist và quy tắc kết hợp tham số | M1 - Nền tảng | P0 | [#2](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) |
| [#7 STUDENT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7) | [Sinh viên] GET list và detail có enrollment | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#4](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#8 CATALOG](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8) | [Danh mục] GET Semester, Subject và Course | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#4](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#9 ENROLL](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/9) | [Đăng ký học] GET Enrollment và dữ liệu Student/Course | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#4](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#10 SEARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/10) | [Query] Search không phân biệt hoa thường cho 5 resource | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#11 SORT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/11) | [Query] Sort tăng/giảm, nhiều trường và thứ tự ổn định | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | [Query] Paging, metadata và dữ liệu rỗng | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) |
| [#13 SELECT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/13) | [Query] Fields trả đúng property set, giữ metadata | M2 - API và query | P1 | [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) |
| [#14 EXPAND](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14) | [Query] Expand quan hệ đúng request và ngăn recursion | M2 - API và query | P1 | [#3](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3), [#6](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6), [#5](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) |
| [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | [Sinh viên] POST, PUT, DELETE và xác thực đầu vào cơ bản | M2 - API và query | P1 | [#7](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7) |
| [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | [Tài liệu API] Swagger dùng được trong container | M3 - Đóng gói và nghiệm thu | P1 | [#7](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7), [#8](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8), [#9](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/9), [#15](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) |
| [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | [Docker] Build sạch, SQL Server, cổng động và /health | M1 - Nền tảng | P0 | [#2](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2), [#4](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) |
| [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | [Nộp bài] submission.json, README và ZIP sạch đúng tên | M3 - Đóng gói và nghiệm thu | P1 | [#16](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16), [#17](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) |
| [#19 QA](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/19) | [Nghiệm thu] Full grader và kiểm tra phần grader bỏ sót | M3 - Đóng gói và nghiệm thu | P1 | [#18](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18), [#10](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/10), [#11](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/11), [#12](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12), [#13](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/13), [#14](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14), [#16](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16), [#7](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7), [#8](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8), [#9](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/9), [#15](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15), [#17](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17), [#4](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) |

Thứ tự thực hiện: ARCH → DB → BOOT → DOCKER để kiểm chứng khả năng khởi động sớm. Hoàn thiện CONTRACT → QUERY rồi luồng Student xuyên suốt ba tầng. Sau đó hoàn tất Catalog, Enrollment và 5 khả năng truy vấn; tiếp theo là CRUD, Swagger; cuối cùng PACKAGE → QA.

QA phụ thuộc trực tiếp vào artifact đóng gói và tất cả hành vi chính. TRACK là parent quản lý scope, không là blocker giả cho mọi issue. Native issue dependencies biểu diễn input bắt buộc trước nghiệm thu; có thể chuẩn bị code dựa trên contract rõ ràng.

## Các điểm chưa rõ và quyết định thiết kế

### DEC-01: fields kết hợp expand

- Trạng thái: Cần xác nhận với giảng viên.
- Căn cứ: G §5 vừa yêu cầu exact property set vừa dùng fields=enrollmentId,status&expand=student,course.
- Hướng xử lý: Selection quyết định property set cuối cùng. Expand chỉ hiển thị nếu relation nằm trong fields; xác nhận cách xử lý ví dụ trước nghiệm thu.
- Owner: QUERY.

### DEC-02: Subject độc lập và relation allowlist

- Trạng thái: Đề xuất thiết kế.
- Căn cứ: B p1 không có SubjectId trên Course hoặc bảng liên kết khác, trong khi mọi list cần expansion.
- Hướng xử lý: Giữ schema gốc; Subject không có relation để expand và trả 400 cho relation lạ. Xác nhận nếu giảng viên muốn thêm quan hệ Course–Subject.
- Owner: QUERY.

### DEC-03: size vượt 100

- Trạng thái: Đề xuất thiết kế.
- Căn cứ: G §5 ghi capped at 100; mock_api.py trả 400 khi size > 100; dynamic grader không kiểm trường hợp này.
- Hướng xử lý: Giới hạn size > 100 xuống 100 và metadata dùng kích thước thực tế; đổi nếu giảng viên yêu cầu trả 400.
- Owner: PAGING.

### DEC-04: Xóa Student có enrollments

- Trạng thái: Cần chốt khi triển khai.
- Căn cứ: G yêu cầu DELETE trả 200 nhưng không quy định cascade/restrict, trong khi schema có khóa ngoại.
- Hướng xử lý: Xóa enrollment thuộc student và student trong transaction để không mồ côi; chỉ QA trên dữ liệu thử.
- Owner: CRUD.

### DEC-05: MSSV, họ tên, lớp, hạn nộp

- Trạng thái: Chưa có thông tin.
- Căn cứ: Tài liệu chỉ có HE170123/Nguyen Van An/SE1701 mẫu.
- Hướng xử lý: Không đặt assignee/deadline giả. Thu thập identity thật trước tạo submission.json/ZIP cuối.
- Owner: PACKAGE.

### DEC-06: Phiên bản .NET/EF và DB

- Trạng thái: Cần chọn khi triển khai.
- Căn cứ: B bắt buộc ASP.NET Core và Docker; phụ lục/template dùng SQL Server 2022; README grader gợi ý .NET 8 nhưng đề không cố định phiên bản.
- Hướng xử lý: Dùng SQL Server 2022 làm baseline; chọn phiên bản .NET SDK, EF và container phù hợp môi trường môn học và khóa thống nhất ở issue nền tảng.
- Owner: ARCH.

### DEC-07: Tổng penalty

- Trạng thái: Sai lệch nguồn đã xác minh.
- Căn cứ: G §11/README ghi tối đa −2,0; sáu khoản trừ trong rubric cộng lại là −2,25; Report.ps1 chỉ giới hạn điểm cuối trong khoảng 0..10.
- Hướng xử lý: Kiểm tra đủ sáu khoản trừ điểm, ghi sai lệch nếu cần hỏi điểm; giữ grader nguyên bản.
- Owner: TRACK.

### DEC-08: Screenshot có lỗi 404/DY-04 và mock

- Trạng thái: Cần kiểm khi chạy grader.
- Căn cứ: Screenshot gọi giống README self-test trên cổng 8099; Common.ps1 dùng nhánh đọc error body khác giữa PowerShell 5/7; mock trả 404 có envelope và thao tác expand làm thay đổi dữ liệu dùng chung.
- Hướng xử lý: So raw response 404 trên PowerShell 7 trước khi quy lỗi API. DY-11 = 80% phù hợp lỗi làm lộ related data được cài cố ý trong mock. Ảnh chưa đủ để kết luận chất lượng bài sinh viên.
- Owner: QA.

### DEC-09: Vị trí Dockerfile

- Trạng thái: Sai lệch tài liệu đã giải quyết.
- Căn cứ: G §0 nói root, nhưng §1.2 cho phép API/Dockerfile hoặc root; grader tìm recursively.
- Hướng xử lý: Chọn API/Dockerfile và compose dockerfile path đúng, theo hướng dẫn chi tiết.
- Owner: DOCKER.

### DEC-10: Health/Swagger và yêu cầu mọi response có envelope

- Trạng thái: Đề xuất giới hạn.
- Căn cứ: G §6 nói mọi response; OpenAPI/Swagger có định dạng tiêu chuẩn; grader health chỉ kiểm 2xx, mock dùng object health riêng.
- Hướng xử lý: Dùng envelope cho API nghiệp vụ và error; health 200 có thể dùng envelope. Swagger HTML/OpenAPI giữ định dạng tiêu chuẩn.
- Owner: CONTRACT.


## Kiểm tra theo từng lớp

1. Khi có solution: `dotnet build PRN232.LMS.sln`; review reference/model/controller/repository boundaries.
2. Chạy API+DB trong Compose project riêng, kiểm readiness/count/ID, từng endpoint và query combination.
3. Kiểm hợp đồng bằng HTTP/manual hoặc script ngoài submission. Không bắt buộc thêm unit/integration test project.
4. Đóng gói ZIP sạch và chạy full grader của giảng viên bằng PowerShell 7+, không chỉ DynamicChecks/SkipDynamic.
5. Đối chiếu report.md/raw.json/grader.log với [coverage](coverage.md), rồi bổ sung các trường hợp grader thiếu.
6. Nếu sửa code/artifact, chạy lại gate liên quan trên đúng revision/ZIP hash trước nghiệm thu.

Các command exact cho grader/build runtime được issue thực hiện chốt khi source/SDK và artifact đã tồn tại. Trong PowerShell, truyền port bằng `$env:API_PORT='8080'` trước `docker compose up --build -d`; không dùng cú pháp shell Linux `API_PORT=8080 command`.

Không sử dụng Docker global prune. Reset chỉ volume/container của Compose project bài học; QA mutations chỉ dùng student thử. Giữ tài liệu/grader nguồn nguyên bản.

## Ngoài phạm vi bắt buộc

Auth/JWT, frontend, cloud hosting, framework validation/exception nâng cao, test framework unit/integration, CRUD toàn bộ resource. Basic input 400 và envelope lỗi vẫn phải làm. Không tự tăng scope để trông giống hệ thống production.

## Definition of done

Mỗi tiêu chí nghiệm thu có bằng chứng; đủ 21 tiêu chí chấm, không bị khoản trừ điểm nào, các phần grader bỏ sót đã được kiểm tra. DB/API chạy từ ZIP sạch trên môi trường đích; thông tin sinh viên thật và README/manifest khớp. Coverage kế hoạch 100% không đồng nghĩa ứng dụng hoàn thành 100%.

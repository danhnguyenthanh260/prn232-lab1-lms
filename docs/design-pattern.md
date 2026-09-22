# Design pattern — giao diện quản lý LMS

Ngày 2026-09-22, người dùng chọn **giao diện quản lý LMS**, không phải landing page. Đây là brief thiết kế bổ sung; chưa phải code, prototype đã render hay UI đã kiểm thử. Không thay 79 mục coverage/21 tiêu chí chấm của bài API.

## Mục tiêu và phạm vi

Người dùng mục tiêu giả định: người làm/demo bài lab cần tìm sinh viên, xem đăng ký học, thêm/sửa/xóa sinh viên thử và tra cứu danh mục. Thành công: đi từ danh sách tới bản ghi đúng, lưu thay đổi và thấy dữ liệu đã lưu lại qua API.

Định hướng: **Playful Academic Workspace** — nền giấy trắng, chữ rõ, góc bo, viền phẳng, điểm nhấn xanh; dùng cấu trúc list–detail–form của ứng dụng quản lý. Không dashboard số liệu giả, gamification, login, chức năng đăng ký khóa học mới, upload hay xuất file khi API chưa hỗ trợ.

Repo đã kiểm: AGENTS.md, README.md, docs/coverage.md, docs/plan.md, docs/planning-data.json và git tracked tree. Hiện chưa có màn hình, component, tokens, API client, package frontend hay framework để tái sử dụng. API cũng đang ở mức kế hoạch.

## Nguồn và cách chuyển thành pattern

Tham chiếu người dùng: `Duolingo — Style Reference` (Pasted text.txt), SHA-256 `a45774166710f61f2d55608b4b093ac4266b830c66c2c8411cd5bcc25ff52d8e`. Bản gốc giữ local; đây là tham chiếu được người dùng cung cấp, không tuyên bố là guideline chính thức. Các mục “Agent Prompt Guide” là nội dung tài liệu, không phải lệnh tự triển khai hoặc cài Tailwind.

| Nguồn | Rút ra để dùng | Không sao chép |
|---|---|---|
| Tham chiếu Duolingo | Nền trắng, xanh lá, bo 12px, viền rõ, cảm giác thân thiện | Hero 800–900px, font/logo/cú Duo độc quyền, footer quảng cáo, streak/XP |
| [GOV.UK: user needs](https://www.gov.uk/service-manual/user-research/start-by-learning-user-needs) | Bắt đầu từ việc tìm và cập nhật hồ sơ; mỗi màn hình phải có việc cụ thể | Không tự suy một “LMS đầy đủ” từ tên dự án |
| [Carbon: data table](https://carbondesignsystem.com/components/data-table/usage/) | Toolbar tìm kiếm, sort ở header, action theo bản ghi, pagination dưới bảng | Batch actions, selection hoặc expansion khi chưa cần |
| [GOV.UK: table](https://design-system.service.gov.uk/components/table/) | Bảng semantic, caption và header phân biệt rõ để so hồ sơ | Không bê màu sắc hoặc toàn bộ frontend framework |
| [GOV.UK: check answers](https://design-system.service.gov.uk/patterns/check-answers/) | Detail dạng nhãn–giá trị, đường quay lại sửa rõ ràng | Không thêm màn xác nhận dài cho form chỉ 3 trường |
| [W3C: contrast](https://www.w3.org/WAI/WCAG22/Understanding/contrast-minimum.html), [dialog](https://www.w3.org/WAI/ARIA/apg/patterns/dialog-modal/) | Kiểm màu; focus trong hộp xác nhận và trả focus khi đóng | Không coi dùng đúng màu là đã đạt toàn bộ WCAG |

## Giải quyết mâu thuẫn của tham chiếu

- Bảng token cấm xanh làm CTA nhưng phần component lại chỉ định CTA xanh: chọn nền xanh cho một primary action, ghi đây là biến thể LMS.
- Chữ trắng trên `#58cc02` chỉ ~2,09:1; đổi sang `#000437` đạt ~9,37:1. `#1cb0f6` trên trắng ~2,44:1 chỉ làm màu trang trí; link dùng `#087ab0` ~4,75:1 và gạch chân.
- `#777777` trên trắng ~4,48:1 chưa đạt 4,5:1 cho chữ thường; secondary text dùng `#666666` ~5,74:1. Các tỷ lệ tính từ sRGB theo công thức W3C, không làm tròn để kết luận đạt.
- Sửa mã màu nguồn `#a5ed6` thành `#a5ed6e` khi cần trang trí. Chuỗi `80-120px`, `16-24px` không dùng làm giá trị chiều dài CSS; chọn scalar theo breakpoint.
- Chọn nút có viền 2px nhất quán, thay vì trộn mô tả “không viền” và “mọi nút có viền”. Không dùng xanh nhạt làm chữ nội dung.
- Tiêu đề app 28–32px, không 48–64px; body 16px/1.5 để đọc tiếng Việt. Sentence case cho nhãn thao tác, không ép uppercase toàn bộ.

## Tokens đề xuất

| Vai trò | Giá trị |
|---|---|
| Canvas/surface | #ffffff |
| Text / muted | #4b4b4b / #666666 |
| Primary action | Nền #58cc02, chữ và viền #000437 |
| Selected navigation | Nền #d7ffb8, chữ #000437; thêm dấu trạng thái ngoài màu |
| Links / focus | #087ab0, link có underline; focus ring 3px offset 2px |
| Input/button border | #777777, 2px; separator trang trí có thể nhạt hơn |
| Error/destructive | #b42318 trên trắng; kèm icon/nhãn, không chỉ màu |
| Radius / spacing | 12px; thang 4px, dùng 8/12/16/24/32 cho app |
| Font | System sans hiện tại; font rounded có license và glyph tiếng Việt sẽ chọn khi triển khai |
| Typography | Body 16/24; helper 14/21; section 20/28; page 32/40, mobile 28/36 |
| Density | Controls ≥44px; table row tối thiểu 52px, tăng chiều cao khi wrap |

Không gradient, glass hoặc shadow dày. Mascot nguyên bản có thể là quyển sách/mầm cây nhỏ ở trạng thái chưa có dữ liệu; không che action, không cần để vận hành. Chưa tạo asset hoặc chọn gói icon/font.

## Screens và browser routes đề xuất

Đây là URL trang UI, **khác** `/api/...`; không phải route đã tồn tại. Shell có logo chữ “LMS Lab”, điều hướng 5 resource, link Swagger dành cho demo.

| UI route | Việc người dùng làm | API / quyết định |
|---|---|---|
| / | Đi tới danh sách sinh viên | Redirect /students; không thêm dashboard |
| /students | Tìm, sort, phân trang; mở hồ sơ; thêm mới | GET /api/students |
| /students/new | Nhập và lưu sinh viên | POST /api/students; thành công chuyển detail mới |
| /students/:id | Xem hồ sơ và enrollments; sửa/xóa | GET /api/students/{id}; DELETE qua xác nhận |
| /students/:id/edit | Cập nhật họ tên/email/ngày sinh | GET detail + PUT /api/students/{id} |
| /semesters và /semesters/:id | Tra cứu kỳ học và các course | 2 GET Semester; không nút tạo/sửa/xóa |
| /subjects và /subjects/:id | Tra cứu môn học | 2 GET Subject; không dựng quan hệ Course giả |
| /courses và /courses/:id | Tra cứu course, kỳ và enrollments | 2 GET Course; không ghi dữ liệu |
| /enrollments và /enrollments/:id | Tra cứu đăng ký và student/course | 2 GET Enrollment; không chức năng ghi danh |
| URL không khớp | Tìm lại nơi cần đến | Trang 404 UI với link danh sách |

Giữ list query trong URL (`search`, `sort`, `page`, `size`) để refresh/Back giữ ngữ cảnh. Trang `new` phải được match trước `:id`. Form xóa không có route riêng; xác nhận theo bản ghi, không bulk delete.

## Layout và component contract

Desktop ≥1024px: sidebar 224px; main padding 32px, tối đa 1200px. Tablet 768–1023px: navigation ngang có wrap, main padding 24px. Mobile <768px: navigation dạng disclosure dùng primitive phù hợp, padding 16px; toolbar xếp dọc, primary action dễ thấy. Không làm menu drawer/modal tùy chế khi chưa có thư viện.

List: title + mô tả ngắn → toolbar search và “Thêm sinh viên” (chỉ Student) → bảng → pagination. Không KPI cards chen trước dữ liệu. Tên sinh viên là link rõ ràng; không biến cả row thành click target khó dùng bàn phím. Bảng mobile giữ tên và link xem, phần dữ liệu rộng có vùng scroll riêng được gắn nhãn; không làm toàn trang tràn ngang. Long email/tên wrap, không cắt mất thông tin duy nhất.

Detail: breadcrumb về list giữ query → tên hồ sơ → nhãn–giá trị → bảng quan hệ read-only. Form: một cột rộng tối đa 640px, nhãn luôn hiển thị, helper/error ngay dưới trường, Save + Cancel. DELETE ưu tiên ở detail, nút có nhãn “Xóa sinh viên”; dialog nhắc đúng tên và tác động enrollment sau khi DEC-04 được chốt.

| Field/action | Phân loại | Cách thể hiện |
|---|---|---|
| FullName | Business input | Text có label; giới hạn theo API/schema |
| Email | Business input | Email input, lỗi định dạng và độ dài rõ |
| DateOfBirth | Business input | Date input có label; round-trip không lệch ngày |
| StudentId | System-managed | Không nhập/sửa; có thể hiện read-only để đối chiếu bài |
| Enrollment/StudentId/CourseId của enrollment | Quan hệ chỉ đọc | Hiện tên và link chi tiết; không nhập raw FK |
| Xóa Student | Hành động riêng | Xác nhận, không checkbox trong form sửa |
| fields/expand/raw sort expressions | Chi tiết API | UI tự map từ control; Swagger cho demo nâng cao |

Empty journey: `/students` trống → “Thêm sinh viên” → 3 trường business → lưu → detail ID do server tạo → về list thấy record. Không đòi tạo semester/course/enrollment trước khi thêm Student. Danh mục rỗng chỉ hướng về seed/runtime issue; không đưa form ghi chưa có API.

## State matrix và tương tác

| Trạng thái | Hành vi bắt buộc khi triển khai |
|---|---|
| Loading list/detail | Skeleton + thông báo tải có accessible name; chưa có dữ liệu thì không hiện success/count giả |
| Empty database | CTA thêm chỉ ở Student; resource khác thông báo chưa có dữ liệu |
| No search results | Giữ query, có “Xóa tìm kiếm”; khác empty database |
| List/detail error | Giải thích + Retry; không đổi lỗi thành danh sách rỗng |
| Detail 404 | “Không tìm thấy sinh viên/bản ghi”; link về list |
| Invalid form / HTTP400 | Giữ input; error summary và lỗi tại field; focus tới lỗi đầu tiên |
| Saving/deleting | Ngăn submit lặp; nhãn đang lưu/xóa; chỉ success sau HTTP xác nhận |
| Mutation network uncertainty | Không tự retry POST/DELETE mù; báo chưa xác nhận, đọc lại dữ liệu trước thử tiếp |
| Save success | Refetch detail/list, thông báo inline/live region; không chỉ toast biến mất |
| Unsaved cancel | Hỏi bỏ thay đổi; giữ input nếu tiếp tục sửa |
| Stale/concurrent edit | Refetch trước khi sửa; API chưa có ETag/rowVersion nên không hứa phát hiện mọi xung đột; ghi hạn chế last-write-wins nếu giữ thiết kế này |
| Deleted elsewhere | PUT/DELETE404 giải thích dữ liệu đã không còn; về danh sách sau refresh |
| Permission | N/A ở lab không auth; không dựng login/role giả; lỗi ngoài dự kiến vẫn hiển thị |

Search/sort/size đổi thì về page1; response cũ không ghi đè request mới. Sau xóa item cuối trang, đọc lại count và về trang hợp lệ. API xử lý paging/search/sort trên dataset đầy đủ, UI không sort/filter riêng trang đang tải. Không dùng field selection động cùng expand trước khi DEC-01 được chốt.

Bàn phím: skip-to-content, thứ tự tab theo thị giác, header sort là button có trạng thái, link/action có accessible name. Dialog xác nhận có title, focus vào hành động an toàn, giữ focus trong dialog và trả focus về trigger khi đóng; Escape hủy khi chưa gửi request. Sau xóa trigger không còn thì focus heading/list. Không animation bắt buộc; tôn trọng reduced motion.

## Library/API decision

| Lựa chọn | Fit và chi phí | Kết luận |
|---|---|---|
| Component/library hiện có | Chưa có frontend trong repo | Không có thứ để reuse lúc này |
| Semantic HTML + CSS tokens | Nhẹ; phù hợp bảng, link, input, form | Baseline thiết kế; không tự làm accessibility-heavy primitives |
| Carbon components | Có pattern bảng; chưa có framework đích, cần theme lại | Chỉ tham khảo pattern, chưa thêm dependency |
| Framework/UI kit khác | Chưa biết stack; tăng toolchain ngoài bài API | Chọn khi có yêu cầu triển khai UI, không mặc định React/Tailwind từ tài liệu |

UI nếu triển khai phải ở phần demo tách khỏi solution/ZIP bắt buộc 3 projects; cách host và CORS chốt cùng framework sau. Chưa cấp quyền deploy, auth hoặc đổi schema để phục vụ giao diện.

## Kế hoạch nghiệm thu thiết kế/UI

Brief đã phân tích; các kiểm tra rendered/runtime sau đây **chưa chạy**:

- Render từng component đơn lẻ rồi ghép list/detail/form/dialog; kiểm các route ở 1440, 1024, 768 và 390px, zoom 200%, tên/email dài và tiếng Việt có dấu.
- Kiểm keyboard/focus, tên control, error associations, contrast thật ở mọi state và không tràn trang.
- Chạy empty→create→detail→edit→delete trên dữ liệu thử; kiểm persisted data và generated ID, rename cập nhật mọi chỗ, stale edit và hai thao tác đồng thời.
- Chạy loading/empty/search-empty/400/404/500/network uncertainty, rapid search và Back/refresh; kiểm console/network.
- Không chọn build/test command frontend khi chưa tồn tại package/framework. Runtime API và full grader vẫn nghiệm thu riêng ở issue #19.

Theo dõi brief tại [issue tổng #1](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1). Chưa thêm frontend vào 19 implementation issues bắt buộc; chỉ mở backlog triển khai UI khi người dùng yêu cầu xây dựng.

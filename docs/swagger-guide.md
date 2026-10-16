# Hướng dẫn sử dụng Swagger LMS

Mở http://localhost:8080/swagger/index.html khi Docker API và DB đang chạy.

## Tham số danh sách

### students

Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. Tên sort/fields/expand không hợp lệ trả **400**.

Ví dụ: `/api/students?sort=fullName,-dateOfBirth&page=1&size=10&fields=studentId,fullName`. Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng.

- **search**: Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `fullName, email`. Bỏ trống để không lọc. Ví dụ: `nguyen`.
- **sort**: Các trường hợp lệ: `studentId,fullName,email,dateOfBirth`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `fullName,-dateOfBirth`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.
- **page**: Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.
- **size**: Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.
- **fields**: Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `studentId,fullName,email,dateOfBirth`. Quan hệ có thể chọn: `enrollments` (cần expand tương ứng, nếu không giá trị là null). Ví dụ: `studentId,fullName`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.
- **expand**: Quan hệ hợp lệ: `enrollments`. Phân cách bằng dấu phẩy. Ví dụ: `enrollments`. Bỏ trống: không trả quan hệ. Khi dùng fields, phải chọn cả tên quan hệ trong fields.

### semesters

Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. Tên sort/fields/expand không hợp lệ trả **400**.

Ví dụ: `/api/semesters?sort=semesterName,-startDate&page=1&size=10&fields=semesterId,semesterName`. Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng.

- **search**: Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `semesterName`. Bỏ trống để không lọc. Ví dụ: `semester`.
- **sort**: Các trường hợp lệ: `semesterId,semesterName,startDate,endDate`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `semesterName,-startDate`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.
- **page**: Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.
- **size**: Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.
- **fields**: Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `semesterId,semesterName,startDate,endDate`. Quan hệ có thể chọn: `courses` (cần expand tương ứng, nếu không giá trị là null). Ví dụ: `semesterId,semesterName`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.
- **expand**: Quan hệ hợp lệ: `courses`. Phân cách bằng dấu phẩy. Ví dụ: `courses`. Bỏ trống: không trả quan hệ. Khi dùng fields, phải chọn cả tên quan hệ trong fields.

### subjects

Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. Tên sort/fields/expand không hợp lệ trả **400**.

Ví dụ: `/api/subjects?sort=-credit,subjectName&page=1&size=10&fields=subjectId,subjectName`. Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng.

- **search**: Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `subjectCode, subjectName`. Bỏ trống để không lọc. Ví dụ: `subject`.
- **sort**: Các trường hợp lệ: `subjectId,subjectCode,subjectName,credit`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `-credit,subjectName`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.
- **page**: Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.
- **size**: Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.
- **fields**: Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `subjectId,subjectCode,subjectName,credit`. Không có trường quan hệ. Ví dụ: `subjectId,subjectName`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.
- **expand**: Subject không có quan hệ trong schema đề bài: để trống, không gửi expand. Mọi giá trị không rỗng đều trả 400.

### courses

Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. Tên sort/fields/expand không hợp lệ trả **400**.

Ví dụ: `/api/courses?sort=semesterId,courseName&page=1&size=10&fields=courseId,courseName`. Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng.

- **search**: Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `courseName`. Bỏ trống để không lọc. Ví dụ: `course`.
- **sort**: Các trường hợp lệ: `courseId,courseName,semesterId`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `semesterId,courseName`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.
- **page**: Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.
- **size**: Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.
- **fields**: Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `courseId,courseName,semesterId`. Quan hệ có thể chọn: `semester,enrollments` (cần expand tương ứng, nếu không giá trị là null). Ví dụ: `courseId,courseName`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.
- **expand**: Quan hệ hợp lệ: `semester,enrollments`. Phân cách bằng dấu phẩy. Ví dụ: `semester,enrollments`. Bỏ trống: không trả quan hệ. Khi dùng fields, phải chọn cả tên quan hệ trong fields.

### enrollments

Nhấn **Try it out**, nhập giá trị (không nhập `sort=` hoặc dấu `?`), rồi **Execute**. Có thể kết hợp các tham số. Bỏ trống tham số tùy chọn nếu không dùng. Tên trường phân biệt hoa/thường, dùng camelCase. Tên sort/fields/expand không hợp lệ trả **400**.

Ví dụ: `/api/enrollments?sort=-enrollDate,enrollmentId&page=1&size=10&fields=enrollmentId,status`. Nếu kết hợp fields và expand, fields quyết định tập khóa cuối cùng; phải thêm tên quan hệ vào fields để thấy dữ liệu mở rộng.

- **search**: Tìm chuỗi chứa, không phân biệt hoa/thường, trên: `status`. Bỏ trống để không lọc. Ví dụ: `active`.
- **sort**: Các trường hợp lệ: `enrollmentId,studentId,courseId,enrollDate,status`. Không có dấu = tăng dần; tiền tố `-` = giảm dần. Phân cách nhiều trường bằng dấu phẩy, ưu tiên từ trái sang phải. Ví dụ: `-enrollDate,enrollmentId`. Bỏ trống: ID tăng dần; ID cũng dùng để ổn định thứ tự khi bằng nhau.
- **page**: Trang bắt đầu từ 1. Mặc định 1; nhỏ hơn 1 hoặc sai kiểu trả 400. Trang vượt dữ liệu trả danh sách rỗng.
- **size**: Số dòng/trang, mặc định 10. Từ 1 đến 100; lớn hơn 100 tự giới hạn về 100; nhỏ hơn 1 hoặc sai kiểu trả 400. pagination.pageSize là kích thước thực tế.
- **fields**: Chọn đúng các khóa muốn nhận, cách nhau bằng dấu phẩy. Trường: `enrollmentId,studentId,courseId,enrollDate,status`. Quan hệ có thể chọn: `student,course` (cần expand tương ứng, nếu không giá trị là null). Ví dụ: `enrollmentId,status`. Bỏ trống: trả đủ trường scalar và các quan hệ đã expand. Không làm mất pagination ở root.
- **expand**: Quan hệ hợp lệ: `student,course`. Phân cách bằng dấu phẩy. Ví dụ: `student,course`. Bỏ trống: không trả quan hệ. Khi dùng fields, phải chọn cả tên quan hệ trong fields.

## Chi tiết và thay đổi sinh viên

### GET /api/students/{id}

Nhập ID hiện có (ví dụ 1). ID không tồn tại trả 404. Trả các quan hệ trực tiếp, không lặp vòng.

### POST /api/students

Body JSON phải có đủ fullName, email, dateOfBirth; không gửi studentId. Ngày dùng ISO 8601. Thành công: 201 kèm Location. Dữ liệu không hợp lệ: 400. Thao tác lưu dữ liệu thật.

### PUT /api/students/{id}

Body JSON phải có đủ fullName, email, dateOfBirth; không gửi studentId. Ngày dùng ISO 8601. Gửi đủ cả 3 trường để cập nhật; thành công: 200; ID không tồn tại: 404. Dữ liệu không hợp lệ: 400. Thao tác lưu dữ liệu thật.

### DELETE /api/students/{id}

Xóa sinh viên và các enrollments của sinh viên đó. Thành công: 200; ID không tồn tại: 404. Thao tác thay đổi dữ liệu thật.

### Body sinh viên

Dữ liệu tạo/cập nhật sinh viên. Cả 3 trường bắt buộc; ID do server cấp.

- **fullName**: Họ tên sau khi bỏ khoảng trắng đầu/cuối: 1–100 ký tự, không được chỉ gồm khoảng trắng.
- **email**: Email hợp lệ, tối đa 100 ký tự ASCII. Không bắt buộc duy nhất.
- **dateOfBirth**: Ngày sinh ISO 8601, từ 1753-01-01 đến hôm nay; server chỉ lưu phần ngày. Ví dụ: 2003-01-15.

```json
{
  "fullName": "Nguyen Van An",
  "email": "an@example.edu",
  "dateOfBirth": "2003-01-15"
}
```

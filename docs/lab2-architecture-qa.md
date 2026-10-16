# Lab 2 — Ôn kiến trúc và luồng bảo mật

Tài liệu mô tả code hiện có, không phải yêu cầu Final Assignment.

## 1. Kiến trúc mấy tầng?

Ba project: API nhận HTTP, binding/validation và trả response; Services xử lý nghiệp vụ; Repositories truy cập SQL Server bằng EF Core. Browser gọi API, không truy cập DB trực tiếp. Đây là một ứng dụng triển khai cùng nhau, không phải ba microservice. Controller dùng interface service qua DI; service dùng interface repository. DbContext, repository và service có lifetime scoped theo request; JwtSettings singleton. DatabaseInitializer và AuthSeedService là hosted service chạy khởi tạo, không phải middleware.

## 2. Có bao nhiêu middleware? Thứ tự thế nào?

Có **hai lớp middleware tự viết**: RequestLoggingMiddleware và ExceptionHandlingMiddleware. Ngoài ra có một middleware inline chuyển URL Student cũ và các middleware framework. Thứ tự đăng ký trong Program.cs:

1. RequestLoggingMiddleware: đo thời gian, request ID, ghi status khi request kết thúc.
2. ExceptionHandlingMiddleware: bắt exception downstream, chuyển thành error envelope; lỗi bất ngờ trả 500 không lộ stack trace.
3. StatusCodePages: bổ sung error envelope cho response lỗi chưa có body.
4. Swagger và SwaggerUI.
5. DefaultFiles và StaticFiles.
6. Middleware inline đổi /api/students thành /api/v1/students.
7. Routing: xác định endpoint và metadata.
8. Authentication: đọc Bearer JWT, xác thực và tạo HttpContext.User.
9. Authorization: kiểm tra yêu cầu đăng nhập/role của endpoint.
10. Endpoint MVC: binding, validation, controller → service → repository → EF Core → SQL Server.

MapControllers/MapGet/MapFallback là đăng ký endpoint; AddScoped/AddAuthentication là đăng ký dịch vụ, không nên đếm như lớp middleware tự viết. Response đi ngược ra ngoài; Swagger/static files có thể kết thúc sớm. Request bị 401/403 không chạy controller. Validation filter nằm trong MVC, không phải middleware.

## 3. Ví dụ POST Student

Browser gửi POST /api/v1/students, JSON body và Authorization: Bearer <accessToken>. Sau routing, JWT được kiểm tra chữ ký, issuer, audience, thời hạn; authorization yêu cầu Admin. MVC bind body, chạy DataAnnotations và StudentValidationFilter/FluentValidation. Controller gọi LmsService, repository lưu qua EF Core; API trả 201 cùng Location theo named route. Input sai trả 400, không có JWT hợp lệ trả 401, Student có JWT hợp lệ nhưng thiếu role Admin trả 403.

Các API đọc catalog/student công khai là lựa chọn của bài lab. Enrollments và thao tác ghi yêu cầu Admin; không phải mọi endpoint đều cần đăng nhập.

## 4. Token và password nằm đâu?

| Dữ liệu | Browser của ứng dụng | Server |
|---|---|---|
| Access token | sessionStorage, key lms.lab2.session | Không lưu DB; kiểm tra JWT bằng signing key |
| Refresh token | Cùng object sessionStorage, giữ token gốc để gửi đổi token | Bảng RefreshTokens chỉ lưu SHA-256 hash, UserId, ExpiresAt, RevokedAt và ID |
| Password | Người dùng nhập lúc login | Bảng Users lưu PasswordHash từ PasswordHasher, không lưu password gốc |
| Signing key | Không gửi xuống browser | Configuration Jwt:Key, biến môi trường Jwt__Key; Compose có fallback demo |

JWT có sub, name, role, jti và thời gian; ký HS256, hạn một giờ. JWT **được ký, không được mã hóa**: không đưa password hay bí mật vào payload. Refresh token là 64 byte ngẫu nhiên mã hóa Base64, hạn bảy ngày. Password dùng hàm hash chuyên dụng có salt; refresh token đủ entropy cao nên lưu SHA-256 để đối chiếu.

Storage này là của UI ứng dụng; Swagger Authorize là luồng nhập token riêng. sessionStorage không phải cookie hoặc localStorage; reload vẫn còn dữ liệu trong phiên tab.

## 5. Login và refresh đi như thế nào?

Login → AuthController → AuthService → AuthRepository tìm User → kiểm tra PasswordHash → tạo JWT và refresh token → lưu hash refresh vào DB → trả cặp token → browser lưu sessionStorage.

authenticatedFetch gắn access token vào Authorization header. Khi nhận 401 và có refresh token, browser gọi POST /api/v1/auth/refresh-token, lưu cặp token mới rồi thử lại request gốc một lần. 403 không kích hoạt refresh.

Server hash refresh token nhận vào, dùng UPDATE có điều kiện TokenHash khớp, chưa revoked và chưa hết hạn để đặt RevokedAt. Chỉ khi đúng một dòng được cập nhật mới cấp cặp token mới. Đây là rotation: token cũ dùng lại trả 401; hai request đồng thời dùng cùng token chỉ một request thành công. Việc consume và cấp token mới chưa nằm trong một transaction chung; lỗi sau consume có thể buộc người dùng login lại.

## 6. Refresh token khác reset token không?

Có. Refresh token dùng lấy access token mới. Password-reset token dùng đặt lại mật khẩu khi quên mật khẩu. **Bài hiện tại chưa có chức năng quên/reset mật khẩu hoặc reset token.** Không gọi refresh token là reset token khi trình bày.

## 7. Logout có vô hiệu hóa token không?

Hiện tại logout chỉ xóa sessionStorage và chuyển về login. Nó chưa revoke token trên server. Access token bị lấy cắp vẫn có hiệu lực đến khi hết hạn; refresh token chưa dùng vẫn có thể dùng đến khi hết hạn. Role trong JWT cũng không tự đổi khi role trong DB đổi. JWT validation thông thường không query DB mỗi request.

Đây là giới hạn cần trả lời trung thực. Nếu phát triển tiếp: bổ sung endpoint revoke/logout, chiến lược thu hồi phiên và refresh cookie HttpOnly/Secure với kiểm soát CSRF phù hợp. sessionStorage hiện tại đọc được bằng JavaScript nên XSS có thể lấy token; demo credentials và signing key mặc định chỉ dành cho lab.

## Câu trả lời ngắn để trình bày

“Em dùng kiến trúc ba tầng API–Services–Repositories, EF Core truy cập SQL Server. Request qua logging và xử lý exception, routing, authentication, authorization rồi vào MVC/controller. Access token JWT hạn một giờ được browser gửi bằng Bearer header, server không lưu access token trong DB. Refresh token hạn bảy ngày có bản gốc ở sessionStorage và hash trong DB; mỗi lần đổi sẽ thu hồi token cũ và cấp cặp mới. Hiện logout mới xóa phiên phía browser, chưa có password-reset và server-side logout.”

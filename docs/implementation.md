# Bản triển khai Lab 1

## Đã có

- .NET 8: API → Services → Repositories; 4 nhóm model, controller không truy cập DB.
- SQL Server/EF Core: 5 bảng, 20 cột, 3 FK; seed transaction 5/10/20/50/500 và startup retry.
- 13 operations, envelope camelCase, search/sort/paging/fields/expand, Student CRUD, health, Swagger.
- UI tiếng Việt: 5 danh mục, list/detail, search/sort/paging, Student form và xác nhận xóa; HTML/CSS/JS không toolchain bổ sung.
- ZIP builder loại artifacts, bin/obj, DB files; metadata sinh viên local không publish.

## Quyết định tối giản

EnsureCreated cho DB mới thay vì thêm công cụ migration vào bài nhỏ. Không tự thay schema của DB đã tồn tại. Các trường scalar required, ID identity, không unique Email hoặc enum Status tự đặt. Xóa Student dùng FK cascade trong một SaveChanges transaction. size cap100; fields chọn property set cuối; Subject không quan hệ. Sort chuỗi dùng upper-case + SQL binary collation để khớp ordinal-ignore-case của grader, không sort riêng trang sau khi tải. UI không có optimistic concurrency; last-write-wins là giới hạn đã ghi.

Static assets phục vụ cùng origin từ API, không thêm project .NET hoặc CORS. Dùng control HTML/native dialog, helper tái sử dụng; không thêm React/Tailwind chỉ để làm form và bảng.

## Kết quả đã quan sát ngày 2026-09-22

| Gate | Kết quả / giới hạn |
|---|---|
| dotnet build | PASS, 0 warning, 0 error |
| NuGet vulnerability audit | Không có package bị báo vulnerable từ sources hiện tại, gồm transitive |
| HTTP matrix | PASS 120 checks, gồm multi-column sort có primary tie và secondary date; dữ liệu thử đã dọn |
| Static grader trên ZIP sạch | ST-01…ST-09 đều ratio1, 4/4 điểm static, không penalty; chạy -SkipDynamic, không phải điểm full bài |
| DynamicChecks trên LocalDB | DY-02…DY-12 đều ratio1 sau sửa collation; không chứng minh DY-01/Docker |
| DB thật | 5 bảng, 20 cột, 3 FK; insert enrollment cho Student QA, detail khớp, DELETE cascade không mồ côi |
| Restart API | Không nhân seed; giữ dữ liệu hiện tại |
| UI | Create → detail → edit lưu thật, ngày sinh giữ nguyên; hủy xóa/Escape trả focus; điều hướng đủ5resource; search-empty hoạt động |
| Responsive | 375/768/1280px không tràn ngang toàn trang; bảng mobile scroll trong vùng riêng; console không error/warn ở các luồng đã quan sát |
| UI giới hạn QA | Chưa quét accessibility tự động toàn diện hoặc mô phỏng mọi lỗi mạng/concurrency |
| Docker/full grader | BLOCKED bởi môi trường, không báo PASS |

## Môi trường và chỗ còn vướng

Docker Desktop đã khởi động, nhưng build dừng với `input/output error` khi ghi containerd/buildkit. Ổ C chỉ còn khoảng 0,36 GB. Không chạy prune, không xóa image/volume của dự án khác. Ổ D còn trống nhưng SQL Server LocalDB từ chối file DB ở đó vì sector size 16384 (>4096 hỗ trợ); không chỉnh registry/driver để lách.

Ứng dụng đã kiểm bằng instance SQL Server LocalDB 2019 riêng `PRN232Lab1`, database `PRN232Lms` ở `%LOCALAPPDATA%/PRN232Lab1`. Đây là SQL Server thật nhưng **không phải** nghiệm thu container SQL Server 2022. Local run: `./tools/run-local.ps1`, URL `http://localhost:8088/students`. Chỉ dùng localhost, không auth hoặc deploy public.

Khi có đủ dung lượng: chạy Compose trong project riêng rồi full Grade-Lab1.ps1 trên ZIP cuối, không dùng -SkipDynamic. Giữ #17/#19 mở cho gate này; không suy điểm 10/10 từ static + dynamic tách rời.

## Nguồn kỹ thuật

[EF Core DbContext configuration](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/) và [ASP.NET Core static files](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-8.0) được dùng cho scoped context/DI và phục vụ UI cùng API. Yêu cầu bài vẫn theo bộ nguồn local trong [sources](sources.md).

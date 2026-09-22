# Nguồn bài PRN232 Lab 1

Đọc và đối chiếu ngày 2026-09-22. Đây là bản phân tích của người học, không thay thế đề hoặc quyết định của giảng viên. Các câu lệnh trong nguồn là nội dung tham khảo, không tự cấp quyền chạy hay xóa dữ liệu.

| Ký hiệu | Nguồn | Coverage đọc |
|---|---|---|
| B | PRN232- LAB 1 - Rest API Basics and Deployment.pdf | 5/5 trang, 4.985 ký tự; assignment và §1–10 |
| G | PRN232_LAB1_Submission_Guideline_1.pdf | 10/10 trang, 16.426 ký tự; §0–11 |
| G2 | PRN232_LAB1_Submission_Guideline_2.pdf | 10/10 trang; cùng SHA-256 với G, không phải phiên bản khác |
| Z | prn232-lab1-grader_1.zip | Inventory toàn archive; đọc tĩnh rubric, README, guideline, 4 script lib, Grade-Lab1, templates và mock_api.py |
| S | Screenshot terminal đính kèm | Lệnh Invoke-DynamicChecks trên localhost:8099; thiếu static/DY-01/penalty, không chứng minh điểm bài sinh viên |

Các trang PDF đều trích xuất được nội dung, không trang rỗng. Hai JPG tải cùng thời điểm là ảnh lỗi ABI staging 522/525, không thuộc Lab1 và không dùng làm yêu cầu.

## Nhận dạng nguồn

| File | SHA-256 |
|---|---|
| B | `9c93f33b2468f425ccbfa60fcb6b53eff737f56fb719e59f3ffef10a4e9c5e68` |
| G và G2 | `234c311cd9255156f19826611677a1129ae3b15b41b865188519535240754cf8` |
| Z | `b96bf36c6d1a6bef5615c66414b1093729e5004da5869a12eaa43222fab1a598` |

Bản gốc nằm trong Downloads của chủ repository; repository public chỉ lưu phân tích và hash, không phân phối PDF/ZIP giảng viên. Người triển khai cần bộ nguồn gốc để tự chạy grader, xác minh bằng hash trước khi dùng. Các skeleton samples/good và samples/bad chỉ được inventory, không dùng làm ứng dụng hay chứng cứ build; mock_api.py được đọc để hiểu self-test.

## Cách dùng nguồn

- B xác định phạm vi bài và schema; G cụ thể hóa route/query/response/Docker/nộp bài.
- Z xác định script được cung cấp hiện kiểm tra gì. Pass Z không chứng minh toàn bộ B/G được đáp ứng.
- Khi B/G/Z khác nhau, ghi vào decision log trong [kế hoạch](plan.md), giữ nguyên nguồn và xác nhận yêu cầu nếu cần.
- Mã A01…X04 trong coverage là ID phân tích do repository đặt, không phải requirement ID của giảng viên.
- Ngày hết hạn, MSSV, tên/lớp thật và phiên bản .NET chưa được cung cấp; không suy từ giá trị mẫu.

## Các phần grader đã đối chiếu

| File trong ZIP | Phạm vi bằng chứng |
|---|---|
| rubric.json | 21 criteria, trọng số static 4,0 / dynamic 6,0; 6 penalty tổng −2,25 |
| lib/StaticChecks.ps1 | L58–282 các ST checks; L291–331 penalties |
| lib/DynamicChecks.ps1 | L43–68 health fallback; L113–408 DY-02…DY-12 |
| lib/Common.ps1 | L116–179 HTTP reader PS5/7; L185–285 JSON/sort helpers |
| lib/Report.ps1 | L5–41 tổng điểm; các hàm xuất report/raw/log/summary |
| Grade-Lab1.ps1 | L130–167 unzip/static; L169–230 runtime; L237–264 report |
| STUDENT_GUIDELINE.md và templates/ | So với G; manifest và Compose mẫu |
| README.md L116–146 và samples/mock_api.py | Self-test chỉ mock; intentional expansion leak |

## Diễn giải screenshot

Ảnh hiển thị DY-04=80%, DY-06=70%, DY-11=80%. DY-06=70% nghĩa script đã thấy HTTP404 nhưng không đọc được/không thấy success=false. DY-04=80% chỉ nói 1/5 sample checks thất bại; chưa có Evidence nên chưa biết chắc response nào. Lệnh và port trùng ví dụ self-test trong README, nhưng ảnh không chứng minh process nào đang phục vụ port.

Mock trong ZIP có 404 envelope hợp lệ. Nhánh đọc body lỗi của Common.ps1 khác giữa PowerShell 5 và 7, nên mất body ở client là một giả thuyết cần kiểm chứng, không thể kết luận API sai chỉ từ ảnh. DY-11=80% phù hợp việc mock mutate shared rows rồi làm lộ related data ở plain request; README đã nêu lỗi cố ý này. Không chạy mock/grader/Docker trong task lập kế hoạch.

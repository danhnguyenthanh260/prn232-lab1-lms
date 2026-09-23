# Coverage yêu cầu và giới hạn autograder

Bản này ánh xạ **79/79 mục phân tích** tới công việc, bao phủ **21/21 tiêu chí rubric** và **6/6 penalty**. Đây là coverage kế hoạch, không phải line/branch coverage. Kết quả triển khai và kiểm tra hiện tại được ghi riêng tại [implementation](implementation.md).

[Nguồn, hash và giới hạn screenshot](sources.md) · [Kế hoạch và quyết định](plan.md) · [Dữ liệu để kiểm tra coverage](planning-data.json)

## Ma trận yêu cầu

Mỗi dòng có một owner chính; QA kiểm chéo toàn bộ khi nghiệm thu. Bảng dưới giữ yêu cầu và cách kiểm tra, không tự biểu diễn PASS. B/G là ký hiệu nguồn trong sources.md; các dòng ghi suy ra/đề xuất/ngoài phạm vi được phân biệt với yêu cầu trực tiếp của đề.

| ID | Yêu cầu hoặc ranh giới | Nguồn | Issue key | Rubric | Bằng chứng cần có |
|---|---|---|---|---|---|
| A01 | Đúng ba project PRN232.LMS.API/Services/Repositories; projectName khớp | B §1; G §1.3,2 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-01 | Đếm .csproj trong ZIP và build solution. |
| A02 | API gọi Services; Services gọi Repositories; API→Repositories chỉ đăng ký DI; không reference ngược | G §2 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-02 | Đọc ProjectReference và chỗ dùng repository trong API. |
| A03 | Controller không chứa nghiệp vụ/truy cập dữ liệu hoặc token bị cấm | B §1; G §2.1 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-03 | Đọc toàn bộ controller kể cả comment; service/repository thật. |
| A04 | Repository không chứa nghiệp vụ hoặc Request/Response/Dto | B §1–2; G §2.2 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-04 | Review repository signatures và implementation. |
| A05 | Entity tại Repositories/Entities | G §3 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-05 | Kiểm folder và mapping DB thực tế. |
| A06 | Business Model tại Services/BusinessModels | G §3 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-05 | Kiểm model nghiệp vụ được dùng qua service. |
| A07 | Request Model tại API/RequestModels; Response Model tại API/ResponseModels | G §3 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-05 | Đối chiếu action input/output và mapping. |
| A08 | Không trả Entity trực tiếp; Controllers không using *.Entities | B §2; G §3 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | ST-06 | Review type graph và JSON thực tế. |
| D01 | Semester: SemesterId int, SemesterName nvarchar(100), StartDate datetime, EndDate datetime | B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | So EF mapping/migration và schema SQL. |
| D02 | Course: CourseId int, CourseName nvarchar(100), SemesterId int | B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | So EF mapping và FK SemesterId. |
| D03 | Subject: SubjectId int, SubjectCode varchar(20), SubjectName nvarchar(100), Credit int | B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | So schema; giữ Subject độc lập nếu chưa có quyết định thêm quan hệ. |
| D04 | Student: StudentId int, FullName nvarchar(100), Email varchar(100), DateOfBirth datetime | B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | So schema/cột/length với migration. |
| D05 | Enrollment: EnrollmentId int, StudentId int, CourseId int, EnrollDate datetime, Status varchar(20) | B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | So schema và FK Student/Course. |
| D06 | Dữ liệu có khóa/quan hệ hợp lệ, không bản ghi mồ côi | Suy ra từ schema B p1 | [#3 DB](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/3) | — | Join kiểm FK và đối chiếu nested detail với DB. |
| D07 | Seed ≥5 semesters,10 subjects,20 courses,50 students,500 enrollments | B p1; G §7.3 | [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | DY-03 | DB count thật và totalItems của cả5 resource. |
| D08 | Startup tự migrate/tạo schema, seed và retry DB 5–10 lần | G §7.3 | [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | DY-01,DY-03 | Up với DB rỗng, không lệnh migration thủ công. |
| D09 | Seed idempotent; knownIds có thật; reset stack riêng vẫn tự tạo lại | G §1.3,7.3 | [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | DY-03,DY-05 | So counts/IDs trước-sau restart và fresh volume. |
| E01 | GET list và /{id} cho semesters | G §4 | [#8 CATALOG](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8) | ST-07,DY-03 | 200/list/detail; ID thiếu404; đủ5 query features. |
| E02 | GET list và /{id} cho subjects | G §4 | [#8 CATALOG](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8) | ST-07,DY-03 | 200/list/detail; ID thiếu404; expand theo schema. |
| E03 | GET list và /{id} cho courses | G §4 | [#8 CATALOG](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/8) | ST-07,DY-03 | Detail có related đúng; list không expand mặc định. |
| E04 | GET list và /{id} cho students | G §4,6.5 | [#7 STUDENT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7) | ST-07,DY-05,DY-06 | Known student có enrollments; missing404. |
| E05 | GET list và /{id} cho enrollments | G §4,6.5 | [#9 ENROLL](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/9) | ST-07,DY-03 | Detail có student/course khớp FK; missing404. |
| E06 | Routes /api/<plural>, lowercase, không verbs/trailing-slash/version segment; controller plural | G §4 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | ST-07 | Kiểm cả route template và OpenAPI actual path. |
| E07 | GET-by-id trả related data đầy đủ theo schema, không cắt mẫu 5 record | B §4; G §6.5 | [#7 STUDENT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7) | DY-05 | Kiểm10 detail success/missing của5 resource; compare DB. |
| E08 | Không vòng lặp hay $id/$ref/$values; nested response hữu hạn | G §6.5 | [#14 EXPAND](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14) | DY-05 | Inspect payload mọi detail/expand, depth và size. |
| E09 | POST students tạo thật,201+Location; GET Location thấy dữ liệu | G §4,6.4 | [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | DY-12 | Lifecycle trên record mới; kiểm DB persistence. |
| E10 | PUT students/{id} cập nhật thật,200; missing404 | G §4,6.4 | [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | — | GET trước/sau PUT; invalid body400. |
| E11 | DELETE students/{id} xóa thật,200; missing404; FK policy rõ | G §4,6.4; FK policy là thiết kế | [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | — | DELETE rồi GET404; test student có enrollment theo quyết định. |
| Q01 | Search contains không phân biệt hoa thường trên main text fields của cả5 resource | G §5 | [#10 SEARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/10) | DY-09 | Matrix5 resource matching/nonmatching/mixed case. |
| Q02 | Sort tăng/giảm và comma-separated nhiều field; - biểu thị giảm | G §5 | [#11 SORT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/11) | DY-08 | Ties primary, secondary date, numeric sort và mixed directions. |
| Q03 | Query tên chính xác search,sort,page,size,fields,expand; JSON field camelCase | G §5 | [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | DY-07..DY-11 | Kiểm binding/OpenAPI và response casing. |
| Q04 | Page 1-based, default page=1,size=10 | G §5 | [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | DY-07 | Omit params, so default và explicit values trên5 resource. |
| Q05 | Size tối đa100; hành vi >100 cần quyết định nhất quán | G §5; mock khác cách diễn giải | [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | — | Thử100/101, đối chiếu decision log và metadata. |
| Q06 | page=0 và size=-1 trả400; sai kiểu không gây500 | G §5 | [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | DY-07 | Kiểm từng tham số riêng và combined invalid. |
| Q07 | Paging đúng số lượng, khác trang, thứ tự ổn định | G §5; stability là hệ quả paging | [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | DY-07 | Compare IDs2 page và page cuối. |
| Q08 | totalItems sau filter trước paging; totalPages=ceil(totalItems/pageSize) | G §6.2; ý nghĩa metadata | [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | DY-07 | Count DB filtered; empty/page beyond cuối. |
| Q09 | fields trả đúng property set trên mọi item | G §5 | [#13 SELECT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/13) | DY-10 | Set equality mọi row trong5 resource, không chỉ row0. |
| Q10 | fields không bỏ pagination/envelope ở root | G §5–6 | [#13 SELECT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/13) | DY-04 | Kiểm root sau selection và empty result. |
| Q11 | expand chỉ thêm related objects được yêu cầu; không expand thì absent/null | G §5 | [#14 EXPAND](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14) | DY-11 | Expanded→plain→expanded để phát hiện state leak. |
| Q12 | enrollments expand=student,course có đủ2object đúng FK | G §5 | [#14 EXPAND](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14) | DY-11 | So mọi returned row với student/course endpoint. |
| Q13 | sort/fields/expand lạ trả400 và envelope lỗi | G §5 | [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | DY-08; DY-10 chỉ ghi nhận | Ba request riêng + query trên empty collection. |
| Q14 | Combined search/sort/page/size/fields/expand hoạt động | B §5; G §5 | [#9 ENROLL](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/9) | — | Chạy nguyên URL ví dụ với semantics được chốt. |
| Q15 | fields+expand: exact-key rule và ví dụ mâu thuẫn cần quyết định | G §5 example + rule4 | [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | — | Ghi câu trả lời giảng viên hoặc proposal rõ ràng trước acceptance tổ hợp. |
| Q16 | Expand cho Subject không có FK; relation allowlist cho mỗi resource cần ghi | B p1; G §5 | [#6 QUERY](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/6) | — | Đối chiếu schema, không tự coi quan hệ thêm là đề yêu cầu. |
| R01 | Envelope success,message,data,errors thống nhất mọi API nghiệp vụ | G §6.1 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | DY-04 | Check key+type+meaning, list/detail/mutation/error. |
| R02 | Collection data là array; pagination ở root với đủ4key | G §6.2 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | DY-04,DY-07 | Reject data.items/pagination lồng mặc dù helper có thể chấp nhận. |
| R03 | Error success=false,data=null,errors array; 404 resource không tồn tại | G §6.3–6.5 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | DY-06 | GET5missing IDs; body thật không rỗng/ProblemDetails. |
| R04 | GET/PUT/DELETE200, POST201+Location, invalid400,missing404, unexpected500 | G §6.4 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | DY-05,DY-06,DY-12 | Status matrix và full envelope; không dùng DELETE204. |
| R05 | Input cơ bản fullName rỗng/email sai phải400, không500 | G §6.4; DynamicChecks L403–405 | [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | DY-12 | Invalid POST/PUT/body malformed; không nâng thành validation framework bắt buộc. |
| R06 | JSON camelCase; không config serialization làm mất null keys required | G §5 rule5; §6 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | DY-04 | So exact casing/keys trên success/error/selection. |
| K01 | API và DB đều trong Docker; single command up --build -d | B §7; G §7 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | DY-01 | Build trên context sạch không local build artifacts. |
| K02 | Services chính xác api và db | G §7.1 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | ST-08 | docker compose config và runtime services. |
| K03 | Container API listen8080, host ${API_PORT:-8080}:8080 | G §7.1 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | ST-08,DY-01 | Default port và remapped free port đều health200. |
| K04 | DB không publish host port; connection host db, không localhost | G §7.1 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | ST-08 | Compose config+network test API→DB. |
| K05 | depends_on service_healthy và DB healthcheck thực sự chạy | G §7.1 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | ST-08,DY-01 | DB warming-up có logs, không chỉ token healthcheck. |
| K06 | Không bind mount máy cá nhân hoặc phụ thuộc file ngoài clone | G §7.1–7.2 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | — | Clean clone/build context và compose config. |
| K07 | Dockerfile multistage sdk→aspnet,EXPOSE8080,ENTRYPOINT đúng DLL | G §7.2 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | ST-08 kiểm tồn tại | Review Dockerfile và inspect/running container. |
| K08 | /health200 chỉ khi DB reachable và seed xong; deadline health300s | G §7.4; rubric.settings | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | DY-01 | Early startup không200, DB mất kết nối phản ánh readiness. |
| K09 | Fresh volume/restart hoạt động; startup không manual steps | G §7.3 | [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | DY-01,DY-03 | Isolated down/up, count trước-sau; không xóa global resources. |
| K10 | Tránh fixed container_name trong template gây collision | Template compose; đề xuất vận hành | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | — | Chạy compose project riêng và kiểm generated names. |
| W01 | Swagger UI /swagger và OpenAPI /swagger/v1/swagger.json | B §8; G §8 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | ST-09,DY-02 | GET UI/spec200 và document đúng OpenAPI. |
| W02 | Swagger enabled trong container, đúng environment | G §8 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | DY-02 | Chạy container artifact, không lấy IDE làm bằng chứng. |
| W03 | Document endpoints,input/output,status; ProducesResponseType phù hợp | B §8; G §8 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | DY-02 kiểm paths | So đủ 13 operations và schema/status với actual HTTP. |
| W04 | Hiện đủ6query parameters và API testing qua UI | B §8; G §8 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | — | Try it out và inspect OpenAPI parameters. |
| P01 | Một ZIP không password, đúng tên/MSSV/fullname không dấu và khoảng trắng | G §1.1 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-NAMING | Regex code2letters+6digits; metadata user thật; không dùng placeholder. |
| P02 | ZIP root đúng solution; tối đa1wrapper folder | G §1.2 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | — | Inventory ZIP trước khi chạy grader. |
| P03 | Có submission.json,README,.sln,compose,.dockerignore; Dockerfile root hoặc API | G §1.2,7.2 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-MANIFEST,PEN-README,ST-08 | Readback file list và Dockerfile build path. |
| P04 | submission.json đủ identity/project/paths/database/resources/implemented/knownIds; notes optional | G §1.3 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-MANIFEST | Parse JSON; khớp actual paths/assembly/seed, không sửa truth bằng flags. |
| P05 | implemented=false tự loại điểm feature tương ứng; true phải hoạt động | G §1.3; script chỉ dùng6feature+create | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | DY-07..DY-12 | Flags thật; update/delete vẫn phải làm dù grader chưa dùng flags đó. |
| P06 | README tối đa1trang: identity,DB,run command,seed counts,known limitations | G §1.4 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-README kiểm tồn tại | Review đủ nội dung và PowerShell syntax hợp lệ. |
| P07 | Không bin/obj/.vs/node_modules trong ZIP | G §1.5 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-BINOBJ | Scan archive paths. |
| P08 | Không DB backup .mdf/.ldf/.bak/.bacpac; seed bằng code | G §1.5; StaticChecks L312–314 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-DBFILE | Scan archive extension và chạy fresh DB. |
| P09 | Không secrets thật/cloud connection/API keys | G §1.5 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) | PEN-SECRET | Review all staged/ZIP config, không chỉ appsettings regex. |
| P10 | Chấm full ZIP cuối cùng, giữ report/raw/log đúng artifact | G §9–10 | [#19 QA](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/19) | ST-01..ST-09,DY-01..DY-12 | Record SHA256+revision+command+exit+reports; no Resume stale report. |
| P11 | Penalty docs −2,0 so với rubric/code −2,25 phải ghi rõ | G §11; rubric; Report L32–33 | [#1 TRACK](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) | 6 penalties | Tính tổng rubric và xác minh không cap penalty riêng. |
| X01 | Không yêu cầu Authentication/Authorization/JWT | B §10 | [#1 TRACK](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) | Ngoài phạm vi | Backlog không có auth/login/security feature ngoài bài. |
| X02 | Không yêu cầu Advanced Validation/Global Exception Handling framework | B §10 | [#1 TRACK](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) | Ngoài phạm vi | Vẫn giữ basic400/error contract, không thêm scope framework. |
| X03 | Không yêu cầu unit/integration test suite | B §10 | [#19 QA](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/19) | Ngoài phạm vi framework | HTTP/manual contract verification vẫn cần cho nghiệm thu. |
| X04 | Không frontend, cloud deployment hoặc CRUD thêm cho mọiresource thành bắt buộc | B assignment; G §4,7; phân tích scope | [#1 TRACK](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/1) | Giới hạn phạm vi | Chỉ13 operations nghiệp vụ+health+Swagger được yêu cầu. |

## Ma trận 21 tiêu chí chấm

| ID | Điểm | Issue key | Grader chưa chứng minh được gì |
|---|---:|---|---|
| ST-01 | 0.50 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Nhận diện suffix project, không chứng minh exactly3 hoặc tên project đồng nhất |
| ST-02 | 0.50 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Kiểm references cần thiết và upward ref; không enforce mọi ref bị cấm hay DI-only |
| ST-03 | 0.50 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Regex raw source; partial credit, false positive trong comment; không semantic business logic |
| ST-04 | 0.25 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Regex Request/Response/Dto; không chứng minh repository không có nghiệp vụ |
| ST-05 | 0.50 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Folder hoặc suffix presence; không chứng minh dùng model đúng luồng |
| ST-06 | 0.50 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | Chỉ detect using *.Entities/Entity; không đầy đủ entity-leak proof |
| ST-07 | 0.50 | [#2 ARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/2) | Route regex; không verify mọi path/method/casing runtime |
| ST-08 | 0.50 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | Dockerfile presence + compose regex; không đầy đủ multistage/readiness/bind mount validation |
| ST-09 | 0.25 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | Package/token wiring; không verify schema/status annotations |
| DY-01 | 0.75 | [#17 DOCKER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/17) | Health fallback /healthz,Swagger,students cho60%; bất kỳ2xx trên health được coi strict |
| DY-02 | 0.50 | [#16 SWAGGER](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/16) | Spec marker/resource-prefix paths+UI marker; không đủ 13 operations hoặc schema correctness |
| DY-03 | 0.25 | [#4 BOOT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/4) | Tin totalItems 5 resources; không query DB/relationships/idempotency |
| DY-04 | 0.50 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | 4sample envelopes + student pagination key presence; case-insensitive, không types/meaning |
| DY-05 | 0.50 | [#7 STUDENT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/7) | Chỉ student knownId, nested bất kỳ; depth sample tối đa 5 elements, không complete related data |
| DY-06 | 0.25 | [#5 CONTRACT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/5) | Student999999404(70%)+success=false(30%); không đủ error contract; phụ thuộc error-body reader |
| DY-07 | 0.75 | [#12 PAGING](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/12) | Students size5,page1/2; chỉ compare first item; combined invalid, không default/cap/other resources |
| DY-08 | 0.75 | [#11 SORT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/11) | Students fullName asc/desc; multi chỉ primary; invalid sort; null values có thể bị bỏ |
| DY-09 | 0.50 | [#10 SEARCH](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/10) | Students token lấy từ fullName, matches trong toàn JSON; không complete field/case semantics |
| DY-10 | 0.50 | [#13 SELECT](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/13) | Student row0 property set case-insensitive; unknown fields chỉ ghi warning500, không trừ điểm |
| DY-11 | 0.50 | [#14 EXPAND](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/14) | Enrollment row0 và2 relations; no invalid expand/otherresources/combined fields check |
| DY-12 | 0.25 | [#15 CRUD](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/15) | Chỉ POST201+Location và invalidPOST400; không PUT/DELETE/GET-persistence |

Static tổng **4,00**; dynamic tổng **6,00**. Khi Docker up thất bại hoặc không probe nào thành công, các mục dynamic còn lại không chạy và tối đa chỉ có4 điểm static trước penalty. Grader có fallback health nên vẫn có trường hợp chạy dynamic dù /health không đạt; dự án phải đáp ứng /health theo đề.

## Penalty

| ID | Điểm trừ | Issue key |
|---|---:|---|
| PEN-MANIFEST | -0.25 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |
| PEN-README | -0.25 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |
| PEN-BINOBJ | -0.25 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |
| PEN-NAMING | -0.50 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |
| PEN-DBFILE | -0.50 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |
| PEN-SECRET | -0.50 | [#18 PACKAGE](https://github.com/danhnguyenthanh260/prn232-lab1-lms/issues/18) |

Tổng thực tế **−2,25**; Report.ps1 chỉ giới hạn điểm cuối trong khoảng 0..10, không giới hạn khoản trừ ở −2,0. Đây là sai lệch đã kiểm bằng code nguồn, không tự sửa rubric.

## Ma trận hành vi nghiệm thu

Tất cả ô dưới đây đều **NOT RUN**. Mỗi tổ hợp cần evidence riêng, không suy từ việc Students đã pass.

| Resource | List 200 | Detail 200 + related | ID thiếu: 404 | Search | Sort | Paging | Fields | Expand |
|---|---|---|---|---|---|---|---|---|
| semesters | cần | courses | cần | semesterName | field allowlist | cần | exact keys | courses theo DEC-02 |
| subjects | cần | scalar, không FK gốc | cần | subjectCode/subjectName | field allowlist | cần | exact keys | empty allowlist theo DEC-02 |
| courses | cần | semester/enrollments | cần | courseName | field allowlist | cần | exact keys | semester,enrollments theo DEC-02 |
| students | cần | enrollments | cần | fullName/email | cần secondary ties | cần | mọi item | enrollments theo DEC-02 |
| enrollments | cần | student/course | cần | status | enrollDate và field khác | cần | mọi item | student,course |

Các lựa chọn field search ngoài ví dụ và relation allowlist là thiết kế được ghi rõ ở QUERY, không giả định đã được giảng viên duyệt.

- GET coverage: 5 list + 5 detail operations, mỗi detail kiểm found/missing; data và related phải khớp DB.
- Mutation coverage: POST→GET→PUT→GET→DELETE→GET404 trên student mới; invalid input và missing ID; xử lý FK theo DEC-04.
- Query boundaries: tham số lạ, default, cap, invalid riêng từng tham số, empty result/page cuối, secondary sort, exact fields mọi row, expanded → plain và tổ hợp 6 tham số.
- Runtime: clean build, health sau seed, DB retry, port remap, startup không manual, seed counts, IDs và FK, restart không tăng rows, reset volume của bài.
- Packaging: unpack ZIP cuối, đủ 3 projects, root đúng, manifest identity/flags thật, README đủ, không junk/backup/secrets.
- Swagger: đủ 13 operations; schema, status và query parameter thực tế, UI chạy trong container.
- Evidence: revision + ZIP SHA-256 + command/exit + raw request/response + reports. Check dự kiến không được đánh dấu PASS.

## Khoảng trống quan trọng của grader

1. PUT và DELETE hoàn toàn không được gọi; POST cũng chưa xác minh dữ liệu được lưu.
2. Phần lớn query test chỉ gọi Students; coverage chéo 5 resource phải tự kiểm.
3. Sort nhiều trường chỉ kiểm fullName; không test secondary key, số/ngày và stable paging.
4. Fields chỉ kiểm item đầu; không yêu cầu unknown field trả 400 khi tính điểm. Expand không kiểm unknown relation trả 400.
5. Envelope chỉ kiểm key tồn tại, không phân biệt hoa thường; chưa kiểm đủ kiểu dữ liệu, ý nghĩa null và status trên mọi operation.
6. Seed tin pagination.totalItems; không chứng minh DB thật, FK, restart idempotency.
7. Kiểm tĩnh bằng regex không chứng minh architecture semantic; comment có thể tạo false positive.
8. /health fallback và chấp nhận 2xx yếu hơn yêu cầu 200 và readiness của đề.
9. Không kiểm combined-query semantics và Subject expansion ambiguity.
10. Scoring screenshot/mock không phải acceptance của ZIP bài sinh viên.

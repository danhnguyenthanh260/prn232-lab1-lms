# Lab 2 implementation and acceptance

Verified 2026-10-16. Scope: Advanced REST API & Security, upgrading the existing LMS Lab 1. No Final Assignment services.

## Sources

Read all 9 pages of the Lab 2 handout and all 6 pages of its submission guideline from the original Downloads/Lab2.zip. Original PDFs and grader remain unmodified and excluded from public source/submission.

- `prn232-lab2-grader.zip`: SHA-256 `7102123d4c3c56c7b1308a86bb3e7702677a51056fa7e5ac10e434bef85ff8dc`
- `PRN232_LAB2_Submission_Guideline.pdf`: SHA-256 `2f5727967a9a6b6310a0edaea6e0b0ce876398ca7c79a4fa2e432de20059c9e7`
- `LAB 2 - Advanced REST API & Security.pdf`: SHA-256 `63cd2fc5874dc42838080d6b03c952049c5c7d3374284bd3c1dfa0c36ea413cb`

## Requirement coverage

| Requirement | Implementation | Evidence |
|---|---|---|
| Three layers and existing LMS | API / Services / Repositories | Build; ST-01/02; 120 inherited HTTP checks |
| User and model separation | Entities/User.cs, AuthService, request/response models | ST-03; login and DB persistence |
| JSON/XML and 406 | EnvelopeXmlFormatter + MVC negotiation | ST-08, DY-03; XML checks for all 5 resources |
| Binding and routing | Route/query/body/header, named Student route, nested Course students | ST-07/09; DY-06/07/08 |
| Validation | DataAnnotations, StudentRequestValidator, PastDateAttribute | ST-04/05/06; DY-04/05; live phone/date/custom-rule checks |
| API versioning | Asp.Versioning, /api/v1, compatible legacy resource URLs | ST-10, DY-02; inherited routes checked |
| Middleware | ExceptionHandlingMiddleware, RequestLoggingMiddleware | ST-11; DY-13/14; sanitized errors and completion logs |
| JWT and roles | PasswordHasher, JwtBearer, Admin/Student, auth/me | ST-12/13/14; DY-09/10/11/15 |
| Refresh token | Hashed token persistence, expiry, atomic consume, rotation | DY-12; replay 401 and concurrent exchange 200/401 |
| Docker and Swagger | API + SQL Server, environment JWT key, Authorize | Full unmodified grader on submission ZIP |

## Acceptance

- Debug and Release builds: 0 warnings, 0 errors.
- Original full Lab 2 grader: **10/10** (static 4, dynamic 6, penalty 0); all 14 static + 15 dynamic criteria at 100%. Reports: `artifacts/lab2-grading-delivery/`.
- Additional checks on final Docker build: **120 Lab 1 HTTP contracts + 49 Lab 2 HTTP checks PASS** (`tools/check_lab2.py`).
- NuGet audit including transitive packages: no known vulnerabilities reported by configured sources at verification time.
- Upgrade rehearsal: original Lab 1 acceptance image seeded a separate `PRN232Lab2UpgradeCheck` database inside the disposable Lab 2 SQL container. New image preserved the old values for 5 semesters, 10 subjects, 20 courses and 50 students; admin login returned 200. Temporary upgrade API container removed; original Lab 1 volume untouched.
- Browser: Admin login, Student creation with phone, logout, Student login and forbidden enrollment page verified. Browser-created record cleaned up by exact name/email match.
- Slides: 12 editable slides plus PDF exported from rendered slide images. Vietnamese text, tables and layout inspected. No invented team members.
- ZIP: 45 files, exactly 3 application projects, no build/cache/QA/course-source files; application and configuration bytes match the working source.
- ZIP SHA-256: `b73dbfae7f510b27ec8a2eb109f1fc62502f93db45cb372c0e85da146fbd8ac7`.

The first run scored 9.9 because the grader selected ASP.NET's start log before the middleware completion log. Framework request chatter was suppressed; original grader was not edited. Subsequent full runs scored 10. Grader score is evidence from the supplied tool, not an instructor-issued mark.

## Run and deliver

- Current demo: http://localhost:8093/swagger and http://localhost:8093/login.html.
- ZIP: `artifacts/lab2-submission/` (private student identity injected from existing submission.local.json).
- Slides: `artifacts/lab2-presentation/PRN232_Lab2_Presentation.pptx` and `.pdf`.
- Build: `dotnet build PRN232.LMS.sln -c Release`.
- HTTP acceptance: `python tools/check_lab2.py http://127.0.0.1:8093` against the disposable stack only.
- Packaging: `python tools/package.py --lab2`. Do not use the Lab 1 packaging mode for this branch.
- Source publication is authorized on the separate `feature/lab2-security` branch. No merge into Lab 1 or main. Prior unrelated dirty Lab 1 edits remain local. See `lab2-architecture-qa.md` for the implementation walkthrough.

## Design limits

Public reads for basic catalog/student data are a lab choice; enrollments and writes require Admin. Default accounts and Compose fallback JWT key are only local classroom fixtures. The browser stores tokens in sessionStorage and clears them on logout; logout does not revoke server tokens. Group roster, assignment responsibilities and teacher-specific slide requirements have not been supplied.

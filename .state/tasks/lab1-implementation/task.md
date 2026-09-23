# Lab 1 implementation

- User authorized implementation of API and simple LMS UI; prioritize working scope, no embellishment.
- Repo: D:/Coding_learning/prn232-lab1-lms, feature/lab1-app, start 1e96ed6; coordination session 643.
- Plan: three .NET 8 projects + SQL Server; static frontend served by API (no fourth project); Docker run, HTTP contract checks and UI smoke test.
- Decisions for this build: size capped100; fields determines final keys; Subject independent; Student deletion cascades own enrollments; integer identity; required scalar fields; no added uniqueness constraint.
- Instructor confirmation of ambiguous fields+expand remains an academic limitation, not a blocker to a working implementation.
- Student identity received and stored only in ignored submission.local.json; package injects it into ZIP, not public git.
- Implemented: three layers, EF SQL Server schema/seed, query options, CRUD, Swagger, static UI, packaging and HTTP checks.
- Verified: Debug/Release build 0 warnings/errors; 120 HTTP checks; ST01-09 full static and DY02-12 direct dynamic ratios 1; schema count and cascade; browser CRUD/empty/navigation/responsive smoke.
- Runtime: API http://127.0.0.1:8088 uses dedicated PRN232Lab1 LocalDB. App left running for user; LocalDB files under LOCALAPPDATA/PRN232Lab1. Temporary preview HTTP server is not needed for delivery.
- Docker build blocked: C disk ~0.36GB free; BuildKit/containerd IO errors. No prune or global cleanup. D sector size16384 rejects SQL LocalDB files. Full Docker grader/DY01 NOT PASS.
- Remaining gate: full Compose/ZIP grader after storage repair; no authority assumed to move Docker disk or delete other projects.
- Publication: implementation commit e7a70fc pushed on feature/lab1-app; PR https://github.com/danhnguyenthanh260/prn232-lab1-lms/pull/20 open, not merged.
- GitHub readback: progress comments on #1/#17/#18/#19 verified exactly; all 19 Project items In Progress; existing issue bodies and other project fields unchanged. No issues closed.
- Delivery: local ZIP artifacts/PRN232_LAB1_SE193274_NguyenThanhDanh.zip; SHA256 B0ED3B94691E2196EB14E17C07CBAC8D311010520A8477AEF1062E655AE168F0. Identity remains private/local. Browser opened at /students; viewport override reset; /health healthy at handoff.
- Handoff: implemented and locally verified; Docker acceptance BLOCKED on storage. Next authorized technical check after storage repair is full Compose/ZIP grader; no extra features required.

## 2026-09-23 — Swagger usage help (local follow-up)

- User requested understandable sort and other input fields. Scope: documentation only, no API behavior or prior compliance fixes.
- Added Swagger operation/schema filters: per-resource allowlists, sort directions/multiple fields, defaults, limits, errors, fields/expand interaction, Subject exception, ID and Student JSON body examples/validation descriptions.
- Verification: build 0 warnings/errors after stopping the exact old API process (initial build failed only from DLL file lock); tools/check_swagger_docs.py PASS (30 parameter descriptions, 29 read-only HTTP examples, 5 combined queries, body schema); existing 120 HTTP checks PASS. Browser /swagger shows the Students guide correctly.
- Serving modified feature/lab1-app source at 127.0.0.1:8088. LocalDB discovery reported stopped/start error575 even though the dedicated PRN232Lab1 SQL process32708 was alive. Verified its logged named pipe and PRN232Lms/50 students, then connected directly to that existing pipe for this runtime (exec74587). No DB reset or system setting change. Pipe is ephemeral: not committed into config; normal run-local startup discovery still needs separate investigation if it recurs.
- Source changes remain local/uncommitted; no GitHub writes, ZIP regeneration or full Docker grading in this follow-up. Previously delivered ZIP does not include this Swagger improvement.

## 2026-09-23 — Submission compliance repair

- User authorized all four source fixes and full Docker grading. Repo/branch unchanged; coordination647. Prior Swagger edits preserved.
- Exact Compose port syntax; lowercase controller convention plus api/[controller] on all five controllers; startup schema/seed moved to IHostedService registered by AddRepositories; success errors=null including health/delete; error arrays preserved. Updated HTTP assertions; UI parser already accepts nullable errors.
- Release build PASS 0 warnings/errors; package audit no vulnerable dependencies reported. Added read-only ZIP regression checker.
- New ZIP includes Swagger and compliance source, 31 files; SHA256 F5E63A5FA93E8CDC38E5298D6023E5AF920A2A3A7B6A9130193D7C979354B844. Previous ZIP preserved under artifacts/previous-submission/. Reader review: identity and concise Docker README verified, no QA/internal files in ZIP.
- Full original grader running: artifacts/grading-full-compliance, work artifacts/grading-work-compliance, isolated Compose prn232lab1_se193274 on8092. No pre-existing containers/volumes for this project. Static9 PASS; Docker downloading/building; final acceptance pending. Docker29.8.0 responsive, C now ~70GiB free. -KeepContainers used for post-grader HTTP/UI verification.

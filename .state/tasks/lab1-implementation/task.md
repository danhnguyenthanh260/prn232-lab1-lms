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

# Repository map

| Need | Location |
|---|---|
| HTTP routes, validation binding, response envelope | PRN232.LMS.API/Controllers, RequestModels, ResponseModels, Program.cs |
| Business validation, query allowlists, finite projections | PRN232.LMS.Services/LmsService.cs, BusinessModels |
| Schema, SQL query, persistence and seed | PRN232.LMS.Repositories/Entities, LmsDbContext.cs, LmsRepository.cs, DatabaseSetup.cs |
| Simple Vietnamese UI | PRN232.LMS.API/wwwroot/index.html, app.js, ui.js, styles.css |
| Docker | docker-compose.yml, PRN232.LMS.API/Dockerfile |
| Local run / HTTP tests / package | tools/run-local.ps1, check_api.py, package.py |
| Coverage and evidence | docs/coverage.md, docs/implementation.md |
| Student metadata | submission.json template; ignored submission.local.json override |

No fourth application project. Startup uses EnsureCreated for fresh databases;
schema evolution requires migrations or a new isolated DB, never silent reset.

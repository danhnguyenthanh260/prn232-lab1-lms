"""Read-only submission layout and exact compliance regression checks."""
import hashlib
import json
import sys
import zipfile
from pathlib import Path

root = Path(__file__).resolve().parents[1]
if len(sys.argv) > 1:
    archive = Path(sys.argv[1])
else:
    candidates = [p for p in (root / 'artifacts').glob('PRN232_LAB1_*.zip') if 'DRAFT' not in p.name]
    assert len(candidates) == 1, 'Pass the exact submission ZIP path when there is not exactly one candidate.'
    archive = candidates[0]
with zipfile.ZipFile(archive) as z:
    names = z.namelist()
    read = lambda name: z.read(name).decode('utf-8-sig')
    assert sum(n.endswith('.csproj') for n in names) == 3
    assert {'submission.json', 'README.md', 'docker-compose.yml', '.dockerignore', 'PRN232.LMS.sln'} <= set(names)
    assert not any(set(Path(n).parts) & {'bin', 'obj', '.git', '.vs', 'node_modules', '.state', 'artifacts', 'tools'} for n in names)
    assert not any(Path(n).suffix.lower() in {'.mdf', '.ldf', '.bak', '.bacpac'} for n in names)
    assert not any(n.endswith('components.html') for n in names)
    manifest = json.loads(read('submission.json'))
    assert all(manifest[k] for k in ['studentCode', 'fullName', 'className'])
    assert manifest['studentCode'] in archive.name and 'DRAFT' not in read('submission.json')
    assert all(manifest[k] in read('README.md') for k in ['studentCode', 'fullName', 'className'])
    compose = read('docker-compose.yml')
    assert '"${API_PORT:-8080}:8080"' in compose and '127.0.0.1:' not in compose
    program = read('PRN232.LMS.API/Program.cs')
    assert 'AddRepositories(' in program and 'InitializeDatabaseAsync' not in program
    setup = read('PRN232.LMS.Repositories/DatabaseSetup.cs')
    assert 'AddHostedService<DatabaseInitializer>()' in setup and ': IHostedService' in setup
    assert 'EnsureCreatedAsync(cancellationToken)' in setup
    controllers = [n for n in names if '/Controllers/' in n and n.endswith('.cs')]
    assert len(controllers) == 5 and all('[Route("api/[controller]")]' in read(n) for n in controllers)
    assert 'ToLowerInvariant()' in read('PRN232.LMS.API/Routing/LowercaseControllerConvention.cs')
    assert 'string[]? Errors' in read('PRN232.LMS.API/ResponseModels/Models.cs')
    assert 'PRN232.LMS.API/Swagger/UsageDocumentation.cs' in names
    for name in names:
        if name not in {'submission.json', 'README.md'}:
            assert z.read(name) == (root / name).read_bytes(), f'ZIP source stale: {name}'
print(f'PASS: {len(names)} files, exactly 3 projects, identity/README, clean layout, exact Compose/route/layering rules, Swagger included and packaged sources match checkout.')
print('SHA256:', hashlib.sha256(archive.read_bytes()).hexdigest().upper())

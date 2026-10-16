"""Create a clean submission; refuse a final name until real student identity exists."""
import json
import sys
import re
import shutil
import tempfile
import zipfile
import unicodedata
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
LAB2 = '--lab2' in sys.argv
manifest = json.loads((ROOT / 'submission.json').read_text(encoding='utf-8'))
if (ROOT / 'submission.local.json').exists():
    manifest.update(json.loads((ROOT / 'submission.local.json').read_text(encoding='utf-8')))
    manifest['notes'] = manifest['notes'].replace('DRAFT: student identity pending. ', '')
if LAB2:
    manifest['apiBasePath'] = manifest['apiVersionBase'] = '/api/v1'
    manifest['resources'] = {k:'/api/v1/'+k for k in manifest['resources']}
    manifest['auth'] = {'loginPath':'/api/v1/auth/login','refreshPath':'/api/v1/auth/refresh-token','adminUsername':'admin','adminPassword':'Admin@123','userUsername':'student','userPassword':'Student@123'}
    manifest['endpoints'] = {'protected':'/api/v1/enrollments','adminOnly':'/api/v1/enrollments','nested':'/api/v1/courses/1/students','createTarget':'/api/v1/students'}
    manifest['implemented'].update({k:True for k in ['contentNegotiation','validation','fluentValidation','customValidation','versioning','middleware','jwt','roleAuthorization','refreshToken']})
destination = ROOT / ('artifacts/lab2-submission' if LAB2 else 'artifacts')
destination.mkdir(parents=True, exist_ok=True)
staging = Path(tempfile.mkdtemp(prefix='submission-', dir=destination))
files = ['PRN232.LMS.sln', 'Directory.Build.props', 'global.json', 'docker-compose.yml', '.dockerignore', 'submission.json']
for path in files:
    shutil.copy2(ROOT / path, staging / path)
for name in ['PRN232.LMS.API', 'PRN232.LMS.Services', 'PRN232.LMS.Repositories']:
    shutil.copytree(ROOT / name, staging / name, ignore=shutil.ignore_patterns('bin','obj','components.html'))
shutil.copy2(ROOT / ('docs/lab2-submission-readme.md' if LAB2 else 'docs/submission-readme.md'), staging / 'README.md')
(staging / 'submission.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding='utf-8')
readme = (staging / 'README.md').read_text(encoding='utf-8').replace('**chưa cung cấp — bản nháp, chưa nộp**. Điền thông tin thật trong submission.json và tài liệu này trước khi đóng gói cuối.',
    f"{manifest['studentCode']} / {manifest['fullName']} / {manifest['className']}." if manifest['studentCode'] else '**chưa cung cấp — bản nháp, chưa nộp**.')
(staging / 'README.md').write_text(readme, encoding='utf-8')
code = manifest['studentCode']
name = ''.join(c for c in unicodedata.normalize('NFD', manifest['fullName'].replace('Đ','D').replace('đ','d')) if unicodedata.category(c) != 'Mn').replace(' ', '')
valid = re.fullmatch(r'[A-Za-z]{2}\d{6}', code) and re.fullmatch(r'[A-Za-z]+', name) and manifest['className']
lab = 'LAB2' if LAB2 else 'LAB1'
archive = destination / (f'PRN232_{lab}_{code}_{name}.zip' if valid else f'PRN232_{lab}_DRAFT.zip')
with zipfile.ZipFile(archive, 'w', zipfile.ZIP_DEFLATED) as output:
    for path in staging.rglob('*'):
        if path.is_file():
            output.write(path, path.relative_to(staging).as_posix())
with zipfile.ZipFile(archive) as check:
    paths = check.namelist()
    assert sum(p.endswith('.csproj') for p in paths) == 3
    assert not any(set(Path(p).parts) & {'bin','obj','.vs','node_modules','artifacts'} for p in paths)
    assert not any(Path(p).suffix.lower() in {'.mdf','.ldf','.bak','.bacpac'} for p in paths)
print(json.dumps({'staging':str(staging),'zip':str(archive),'finalIdentity':bool(valid),'files':len(paths)}))

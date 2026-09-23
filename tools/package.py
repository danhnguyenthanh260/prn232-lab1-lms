"""Create a clean submission; refuse a final name until real student identity exists."""
import json
import re
import shutil
import tempfile
import zipfile
import unicodedata
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
manifest = json.loads((ROOT / 'submission.json').read_text(encoding='utf-8'))
if (ROOT / 'submission.local.json').exists():
    manifest.update(json.loads((ROOT / 'submission.local.json').read_text(encoding='utf-8')))
    manifest['notes'] = manifest['notes'].replace('DRAFT: student identity pending. ', '')
destination = ROOT / 'artifacts'
destination.mkdir(exist_ok=True)
staging = Path(tempfile.mkdtemp(prefix='submission-', dir=destination))
files = ['PRN232.LMS.sln', 'Directory.Build.props', 'global.json', 'docker-compose.yml', '.dockerignore', 'submission.json']
for path in files:
    shutil.copy2(ROOT / path, staging / path)
for name in ['PRN232.LMS.API', 'PRN232.LMS.Services', 'PRN232.LMS.Repositories']:
    shutil.copytree(ROOT / name, staging / name, ignore=shutil.ignore_patterns('bin','obj','components.html'))
shutil.copy2(ROOT / 'docs/submission-readme.md', staging / 'README.md')
(staging / 'submission.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding='utf-8')
readme = (staging / 'README.md').read_text(encoding='utf-8').replace('**chưa cung cấp — bản nháp, chưa nộp**. Điền thông tin thật trong submission.json và tài liệu này trước khi đóng gói cuối.',
    f"{manifest['studentCode']} / {manifest['fullName']} / {manifest['className']}." if manifest['studentCode'] else '**chưa cung cấp — bản nháp, chưa nộp**.')
(staging / 'README.md').write_text(readme, encoding='utf-8')
code = manifest['studentCode']
name = ''.join(c for c in unicodedata.normalize('NFD', manifest['fullName'].replace('Đ','D').replace('đ','d')) if unicodedata.category(c) != 'Mn').replace(' ', '')
valid = re.fullmatch(r'[A-Za-z]{2}\d{6}', code) and re.fullmatch(r'[A-Za-z]+', name) and manifest['className']
archive = destination / (f'PRN232_LAB1_{code}_{name}.zip' if valid else 'PRN232_LAB1_DRAFT.zip')
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

"""Validate planning coverage and internal links; does not grade application code."""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
plan = json.loads((ROOT / 'docs/planning-data.json').read_text(encoding='utf-8'))
issues = {i['key']: i for i in plan['issues']}
assert len(issues) == len(plan['issues']) == 19
mapping = json.loads((ROOT / 'docs/github-issues.json').read_text(encoding='utf-8'))
assert set(mapping) == set(issues)
assert len({i['number'] for i in mapping.values()}) == 19
for key, item in mapping.items():
    assert item['url'] == f'https://github.com/{plan["owner"]}/{plan["repo"]}/issues/{item["number"]}'
assert len(plan['requirements']) == 79
assert len({r['id'] for r in plan['requirements']}) == 79
assert len(plan['rubric']) == 21
assert {r['id'] for r in plan['rubric']} == {f'ST-{i:02}' for i in range(1, 10)} | {f'DY-{i:02}' for i in range(1, 13)}
assert sum(r['points'] for r in plan['rubric'] if r['id'].startswith('ST-')) == 4
assert sum(r['points'] for r in plan['rubric'] if r['id'].startswith('DY-')) == 6
assert len(plan['penalties']) == 6
assert sum(r['points'] for r in plan['penalties']) == -2.25
for row in plan['requirements'] + plan['rubric'] + plan['penalties'] + plan['decisions']:
    assert row['owner'] in issues, row
for r in plan['requirements']:
    assert all(r.get(k) for k in ('text', 'source', 'verification'))
    assert f'| {r["id"]} |' in (ROOT / 'docs/coverage.md').read_text(encoding='utf-8')
    row = next(line for line in (ROOT / 'docs/coverage.md').read_text(encoding='utf-8').splitlines() if line.startswith(f'| {r["id"]} |'))
    assert mapping[r['owner']]['url'] in row

visited, active = set(), set()
def visit(key):
    assert key not in active, 'Dependency cycle: ' + key
    if key in visited:
        return
    active.add(key)
    for dep in issues[key]['deps']:
        assert dep in issues and dep != key
        visit(dep)
    active.remove(key)
    visited.add(key)

for key, issue in issues.items():
    visit(key)
    assert issue['accept'] and issue['scope'] and issue['outcome']

for path in [ROOT / 'README.md', *sorted((ROOT / 'docs').glob('*.md'))]:
    body = path.read_text(encoding='utf-8')
    assert '\ufffd' not in body, path
    for target in re.findall(r'\]\(([^)]+)\)', body):
        if target.startswith(('https://', 'http://', '#')):
            continue
        assert (path.parent / target.split('#')[0]).exists(), (path, target)

print('PASS: 19 issue definitions, 79 requirement mappings, 21 rubric criteria (4+6), 6 penalties (-2.25), acyclic dependencies and local links.')
routes = (ROOT / 'docs/api-routes.md').read_text(encoding='utf-8')
operations = re.findall(r'^\| (GET|POST|PUT|DELETE) \| (/api/[^ |]+) \|', routes, re.M)
expected = [('GET', '/api/' + name + suffix) for name in ['semesters', 'subjects', 'courses', 'students', 'enrollments'] for suffix in ['', '/{id}']]
expected += [('POST', '/api/students'), ('PUT', '/api/students/{id}'), ('DELETE', '/api/students/{id}')]
assert len(operations) == 13 and set(operations) == set(expected)
db = (ROOT / 'docs/database-model.md').read_text(encoding='utf-8')
erd = re.search(r'```mermaid\n(.*?)```', db, re.S).group(1)
tables = re.findall(r'^    (\w+) \{\n(.*?)^    \}', erd, re.M | re.S)
assert {name for name, _ in tables} == {'Semester', 'Course', 'Student', 'Enrollment', 'Subject'}
assert sum(len(fields.strip().splitlines()) for _, fields in tables) == 20
assert len(re.findall(r' PK$', erd, re.M)) == 5 and len(re.findall(r' FK$', erd, re.M)) == 3
assert len(re.findall(r'\|\|--o\{', erd)) == 3
print('PASS: documented 13 API operations, 5 entities, 20 columns, 5 PKs and 3 FKs; documentation consistency only, not rendered/runtime verification.')
print('This command validates planning documents only; application/build/runtime evidence is recorded separately in docs/implementation.md.')

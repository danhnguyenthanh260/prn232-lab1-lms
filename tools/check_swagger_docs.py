"""Read-only checks: documentation and executable query examples on all resources."""
import json
import sys
import urllib.parse
import urllib.request

base = sys.argv[1] if len(sys.argv) > 1 else 'http://127.0.0.1:8088'
def get(path):
    with urllib.request.urlopen(base + path, timeout=20) as response:
        assert response.status == 200
        return json.load(response)

doc = get('/swagger/v1/swagger.json')
checks = 0
for resource in ['students', 'semesters', 'subjects', 'courses', 'enrollments']:
    route = '/api/' + resource
    operation = doc['paths'][route]['get']
    assert operation['summary'] and 'Try it out' in operation['description']
    params = {p['name']: p for p in operation['parameters']}
    assert set(params) == {'search', 'sort', 'page', 'size', 'fields', 'expand'}
    for name, param in params.items():
        assert len(param['description']) > 40, (resource, name)
        if resource == 'subjects' and name == 'expand':
            assert 'example' not in param and '400' in param['description']
            continue
        example = param['example']
        get(route + '?' + urllib.parse.urlencode({name: example}))
        checks += 1
    for name, default in [('page', 1), ('size', 10)]:
        assert params[name]['schema']['default'] == default
        assert params[name]['schema']['minimum'] == 1
    fields = params['fields']['example']
    combo = {'sort': params['sort']['example'], 'page': 1, 'size': 2, 'fields': fields}
    if resource != 'subjects':
        relations = params['expand']['example']
        combo.update(expand=relations, fields=fields + ',' + relations)
    rows = get(route + '?' + urllib.parse.urlencode(combo))['data']
    assert rows and all(set(r) == set(combo['fields'].split(',')) for r in rows)
    if resource != 'subjects':
        assert all(row[r] is not None for row in rows for r in relations.split(','))
    detail = doc['paths'][route + '/{id}']['get']
    assert detail['parameters'][0]['example'] == 1 and detail['description']
schema = doc['components']['schemas']['StudentRequestModel']
assert set(schema['example']) == {'fullName', 'email', 'dateOfBirth'}
assert set(schema['required']) == set(schema['example'])
assert all(p['description'] for p in schema['properties'].values())
for method in ['post', 'put']:
    path = '/api/students' + ('/{id}' if method == 'put' else '')
    assert doc['paths'][path][method]['requestBody']['content']['application/json']['schema']['$ref'].endswith('/StudentRequestModel')
print(f'PASS: 5 resource guides, 30 parameter descriptions, {checks} HTTP examples, 5 combined queries, ID docs and Student body schema/example.')

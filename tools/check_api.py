"""HTTP acceptance on disposable lab data; never targets anything except explicit base URL."""
import json
import sys
import urllib.request
import urllib.error
import urllib.parse
from datetime import datetime

sys.stdout.reconfigure(encoding='utf-8')
BASE = sys.argv[1] if len(sys.argv)>1 else 'http://127.0.0.1:8088'
checks = 0
def request(path, method='GET', body=None, status=200):
    global checks
    data = None if body is None else json.dumps(body).encode()
    req = urllib.request.Request(BASE+path, data=data, method=method, headers={'Content-Type':'application/json'})
    try:
        response=urllib.request.urlopen(req, timeout=20)
    except urllib.error.HTTPError as error:
        response=error
    payload=json.load(response)
    assert response.status==status,(path,response.status,payload)
    assert {'success','message','data','errors'}<=payload.keys(),(path,payload)
    assert isinstance(payload['errors'],list) and payload['success']==(status<400)
    if status>=400: assert payload['data'] is None and payload['errors']
    checks+=1
    return payload,response.headers

resources={'semesters':('semesterId','semesterName',5,'semester','courses'),'subjects':('subjectId','subjectName',10,'subject',None),'courses':('courseId','courseName',20,'course','semester'),'students':('studentId','fullName',50,'nguy','enrollments'),'enrollments':('enrollmentId','status',500,'active','student,course')}
request('/health')
for resource,(key,text,minimum,search,expand) in resources.items():
    route='/api/'+resource
    normal,_=request(route)
    assert normal['pagination']['totalItems']>=minimum and normal['pagination']['pageSize']==10
    detail,_=request(route+'/1'); assert detail['data'][key]==1
    request(route+'/999999',status=404)
    selected,_=request(route+f'?fields={key},{text}')
    assert all(set(item)=={key,text} for item in selected['data'])
    sorted_rows,_=request(route+f'?sort=-{key}&size=5'); ids=[r[key] for r in sorted_rows['data']];assert ids==sorted(ids,reverse=True)
    page1,_=request(route+'?size=2&page=1');page2,_=request(route+'?size=2&page=2')
    assert not {r[key] for r in page1['data']}&{r[key] for r in page2['data']}
    lower,_=request(route+'?search='+search);upper,_=request(route+'?search='+search.upper());assert lower['data']==upper['data'] and lower['data']
    empty,_=request(route+'?search=zzzzunmatched');assert empty['data']==[] and empty['pagination']['totalItems']==0
    capped,_=request(route+'?size=101');assert capped['pagination']['pageSize']==100
    for bad in ['page=0','size=-1','page=abc','sort=invalid','fields=invalid','expand=invalid','search=zzzzunmatched&fields=invalid']:
        request(route+'?'+bad,status=400)
    if expand:
        expanded,_=request(route+'?expand='+expand)
        assert all(all(relation in row for relation in expand.split(',')) for row in expanded['data'])
        plain,_=request(route);assert all(not any(relation in row for relation in expand.split(',')) for row in plain['data'])
        combo,_=request(route+f'?fields={key}&expand={expand}&size=2&sort=-{key}')
        assert all(set(row)=={key} for row in combo['data'])
stamp=datetime.now().strftime('%H%M%S%f')
body={'fullName':'HTTP QA '+stamp,'email':f'qa{stamp}@example.test','dateOfBirth':'2003-01-15'}
created,headers=request('/api/students','POST',body,201);identifier=created['data']['studentId']
try:
    assert headers['Location'].endswith('/api/students/'+str(identifier))
    found,_=request('/api/students/'+str(identifier));assert found['data']['fullName']==body['fullName']
    body['fullName']='Updated '+stamp
    request('/api/students/'+str(identifier),'PUT',body)
    found,_=request('/api/students/'+str(identifier));assert found['data']['fullName']==body['fullName']
    for bad in [{},{**body,'fullName':''},{**body,'email':'wrong'},{**body,'dateOfBirth':'1600-01-01'}]:
        request('/api/students','POST',bad,400)
    request('/api/students/999999','PUT',body,404)
finally:
    request('/api/students/'+str(identifier),'DELETE')
request('/api/students/'+str(identifier),status=404)
request('/api/students/'+str(identifier),'DELETE',status=404)
tie_ids=[]
try:
    for birth in ['2001-01-01','2005-01-01']:
        tie,_=request('/api/students','POST',{'fullName':'Tie QA '+stamp,'email':f'tie{birth}{stamp}@example.test','dateOfBirth':birth},201)
        tie_ids.append(tie['data']['studentId'])
    tied,_=request('/api/students?search='+urllib.parse.quote('Tie QA '+stamp)+'&sort=fullName,-dateOfBirth')
    assert [row['studentId'] for row in tied['data']]==list(reversed(tie_ids))
finally:
    for tie_id in tie_ids: request('/api/students/'+str(tie_id),'DELETE')
with urllib.request.urlopen(BASE+'/swagger/v1/swagger.json') as response:
    doc=json.load(response)
assert sum(method in {'get','post','put','delete'} for path,methods in doc['paths'].items() if path.startswith('/api/') for method in methods)==13
assert {p['name'] for p in doc['paths']['/api/students']['get']['parameters']}=={'search','sort','page','size','fields','expand'}
print(f'PASS: {checks} HTTP contract checks; five-resource query matrix, persisted Student lifecycle, error envelopes and 13 OpenAPI operations.')

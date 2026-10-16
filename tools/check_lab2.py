"""Lab 2 acceptance against an explicitly selected disposable test stack."""
import concurrent.futures
import json
import os
import subprocess
import sys
import urllib.request
import urllib.error
import xml.etree.ElementTree as ET
from pathlib import Path

BASE=sys.argv[1] if len(sys.argv)>1 else 'http://127.0.0.1:8093'
checks=0
def request(path, method='GET', body=None, token=None, status=200, accept='application/json', extra=None):
    global checks
    headers={'Content-Type':'application/json','Accept':accept, **(extra or {})}
    if token: headers['Authorization']='Bearer '+token
    data=json.dumps(body).encode() if body is not None else None
    req=urllib.request.Request(BASE+path,data=data,method=method,headers=headers)
    try: response=urllib.request.urlopen(req,timeout=25)
    except urllib.error.HTTPError as e: response=e
    raw=response.read()
    assert response.status==status,(path,response.status,raw[:400])
    checks+=1
    if accept=='application/xml' and status==200: return ET.fromstring(raw),response.headers
    value=json.loads(raw)
    assert {'success','message','data','errors'}<=value.keys(),(path,value)
    assert value['success']==(status<400)
    return value,response.headers

def login(name,password): return request('/api/v1/auth/login','POST',{'username':name,'password':password})[0]['data']
admin=login('admin','Admin@123'); student=login('student','Student@123')
a=admin['accessToken']; s=student['accessToken']
request('/api/v1/auth/login','POST',{'username':'admin','password':'wrong'},status=401)
request('/api/v1/auth/login','POST',{'username':'unknown','password':'wrong'},status=401)
request('/api/v1/auth/me',status=401)
request('/api/v1/auth/me',token='invalid',status=401)
me,_=request('/api/v1/auth/me',token=s); assert me['data']['role']=='Student'
request('/api/v1/enrollments',token=a)
request('/api/v1/enrollments',token=s,status=403)
request('/api/enrollments',token=s,status=403)
request('/api/v1/enrollments',status=401)
request('/api/v1/students','POST',{},status=401)
request('/api/v1/students','POST',{},token=s,status=403)
for resource in ['students','subjects','courses','semesters','enrollments']:
    plain,_=request('/api/v1/'+resource,token=a)
    xml,headers=request('/api/v1/'+resource,token=a,accept='application/xml')
    assert xml.tag=='response' and xml.find('success').text=='true' and len(xml.findall('data/item'))==len(plain['data'])
    assert 'xml' in headers['Content-Type']
    request('/api/v1/'+resource,token=a,status=406,accept='application/pdf')
request('/api/v1/students/abc',status=404)
request('/api/v1/students?page=0',status=400)
_,headers=request('/api/v1/students',extra={'X-Request-Id':'lab2-check'})
assert headers['X-Request-Id']=='lab2-check'
nested,_=request('/api/v1/courses/1/students'); assert len(nested['data'])>0
request('/api/v1/courses/999999/students',status=404)
valid={'fullName':'Lab2 Acceptance','email':'lab2@example.test','dateOfBirth':'2003-01-15','phoneNumber':'+84901234567'}
for bad in [{},{**valid,'phoneNumber':'not-a-phone'},{**valid,'dateOfBirth':'2099-01-01'},{**valid,'dateOfBirth':'1600-01-01'},{**valid,'fullName':'   '},{**valid,'email':'é@example.test'},{**valid,'credit':-99}]:
    value,_=request('/api/v1/students','POST',bad,token=a,status=400);assert value['errors']
created,headers=request('/api/v1/students','POST',valid,token=a,status=201)
identifier=created['data']['studentId']
try:
    assert headers['Location'].endswith('/api/v1/students/'+str(identifier))
    record,_=request('/api/v1/students/'+str(identifier)); assert record['data']['phoneNumber']==valid['phoneNumber']
    selected,_=request('/api/v1/students?fields=studentId,phoneNumber&size=2');assert all(set(x)=={'studentId','phoneNumber'} for x in selected['data'])
finally: request('/api/v1/students/'+str(identifier),'DELETE',token=a)
fresh,_=request('/api/v1/auth/refresh-token','POST',{'refreshToken':admin['refreshToken']})
request('/api/v1/auth/me',token=fresh['data']['accessToken'])
assert fresh['data']['refreshToken']!=admin['refreshToken']
request('/api/v1/auth/refresh-token','POST',{'refreshToken':admin['refreshToken']},status=401)
request('/api/v1/auth/refresh-token','POST',{'refreshToken':'invalid'},status=401)
# Two simultaneous exchanges: exactly one may consume the one-time token.
def exchange(_):
    req=urllib.request.Request(BASE+'/api/v1/auth/refresh-token',data=json.dumps({'refreshToken':student['refreshToken']}).encode(),headers={'Content-Type':'application/json'})
    try:
        with urllib.request.urlopen(req,timeout=25) as r: return r.status
    except urllib.error.HTTPError as e: return e.code
with concurrent.futures.ThreadPoolExecutor(2) as pool: assert sorted(pool.map(exchange,range(2)))==[200,401]
checks+=1
env={**os.environ,'LMS_ACCESS_TOKEN':a}
subprocess.run([sys.executable,str(Path(__file__).with_name('check_api.py')),BASE],env=env,check=True)
print(f'PASS: {checks} Lab 2 HTTP checks plus Lab 1 regression suite; refresh replay/concurrency, roles, JSON/XML, validation, nested route, persistence.')

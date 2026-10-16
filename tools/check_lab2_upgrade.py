"""Prove additive schema upgrade on a new database in the disposable Lab 2 SQL container."""
import subprocess, threading, json, os
import requests

NAME='prn232-lab2-upgrade-check'
PROJECT=os.environ.get('LAB2_COMPOSE_PROJECT','prn232-lab2')
NETWORK=PROJECT+'_default'
connection='Server=db;Database=PRN232Lab2UpgradeCheck;User Id=sa;Password=Lab1Only_ChangeMe123!;TrustServerCertificate=True'
def run(*args): return subprocess.check_output(['docker',*args],text=True).strip()
assert not run('ps','-a','--filter','name=^/'+NAME+'$','--format','{{.Names}}'), 'Test container name is already in use'
def start(image, lab2=False):
    args=['run','-d','--name',NAME,'--network',NETWORK,'-p','127.0.0.1:8094:8080','-e','ConnectionStrings__Lms='+connection]
    if lab2: args+=['-e','Jwt__Key=UpgradeCheckLocalOnlySigningKey_2026!']
    run(*args,image)
    logs=subprocess.Popen(['docker','logs','--follow',NAME],stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True)
    timer=threading.Timer(55,logs.terminate);timer.start()
    ready=False
    try:
        for line in logs.stdout:
            if 'Now listening on:' in line: ready=True;break
    finally:
        timer.cancel();logs.terminate();logs.wait(timeout=10)
    assert ready,'Container did not become ready; inspect docker logs for '+NAME
    assert requests.get('http://127.0.0.1:8094/health',timeout=10).status_code==200
def snapshot():
    return {r:requests.get('http://127.0.0.1:8094/api/'+r+'?size=100',timeout=20).json()['data'] for r in ['semesters','subjects','courses','students']}
try:
    start('prn232-lab1-acceptance:compliance')
    before=snapshot()
    run('rm','-f',NAME)
    start(PROJECT+'-api:latest',True)
    after=snapshot()
    for resource,rows in before.items():
        assert len(rows)==len(after[resource])
        for old,new in zip(rows,after[resource]): assert all(new[k]==v for k,v in old.items()),(resource,old)
    login=requests.post('http://127.0.0.1:8094/api/v1/auth/login',json={'username':'admin','password':'Admin@123'},timeout=20)
    assert login.status_code==200
    result={'result':'PASS','database':'PRN232Lab2UpgradeCheck','preserved':{k:len(v) for k,v in before.items()},'authAfterUpgrade':200}
    print(json.dumps(result))
finally:
    if run('ps','-a','--filter','name=^/'+NAME+'$','--format','{{.Names}}'): run('rm','-f',NAME)

import {saveSession} from './auth.js';
const form=document.querySelector('#login'), error=document.querySelector('#error');
form.addEventListener('submit',async event=>{
  event.preventDefault(); const button=form.querySelector('button');button.disabled=true;error.textContent='';
  try {
    const response=await fetch('/api/v1/auth/login',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(Object.fromEntries(new FormData(form)))});
    const result=await response.json();
    if(!response.ok) throw new Error(response.status===401?'Tên đăng nhập hoặc mật khẩu không đúng.':(result.message||'Không thể đăng nhập.'));
    saveSession(result.data);location.href='/students';
  } catch(e) {error.textContent=e.message;button.disabled=false;}
});

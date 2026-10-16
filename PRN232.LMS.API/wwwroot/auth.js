const storageKey = 'lms.lab2.session';
export function session() {
  try { return JSON.parse(sessionStorage.getItem(storageKey) || 'null'); } catch { return null; }
}
export function saveSession(tokens) { sessionStorage.setItem(storageKey, JSON.stringify(tokens)); }
export function logout() { sessionStorage.removeItem(storageKey); location.href = '/login.html'; }
export async function authenticatedFetch(url, options = {}) {
  const tokens = session();
  const send = token => fetch(url, {...options, headers: {...options.headers, ...(token ? {Authorization:`Bearer ${token}`} : {})}});
  let response = await send(tokens?.accessToken);
  if (response.status === 401 && tokens?.refreshToken) {
    const refresh = await fetch('/api/v1/auth/refresh-token', {method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({refreshToken:tokens.refreshToken})});
    if (refresh.ok) {
      const result = await refresh.json(); saveSession(result.data); response = await send(result.data.accessToken);
    } else sessionStorage.removeItem(storageKey);
  }
  return response;
}
const account = document.querySelector('#account');
if (account) {
  const link = document.createElement('a');
  link.href='/login.html'; link.textContent=session() ? 'Đăng xuất' : 'Đăng nhập';
  if(session()) link.addEventListener('click',e=>{e.preventDefault();logout();});
  account.append(link);
}

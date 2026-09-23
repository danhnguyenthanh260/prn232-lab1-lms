export function el(tag, attrs = {}, ...children) {
  const node = document.createElement(tag);
  for (const [key, value] of Object.entries(attrs)) {
    if (value === null || value === undefined || value === false) continue;
    if (key.startsWith('on')) node.addEventListener(key.slice(2), value);
    else if (key === 'class') node.className = value;
    else if (key === 'value') node.value = value;
    else node.setAttribute(key, value === true ? '' : String(value));
  }
  node.append(...children.flat().filter(x => x !== null && x !== undefined));
  return node;
}
export const button = (text, attrs = {}) => el('button', {type:'button', ...attrs}, text);
export const link = (text, href, kind = '') => el('a', {href, class:`button ${kind}`}, text);
export const notice = (text, kind = '') => el('div', {class:`notice ${kind}`, role:kind === 'error' ? 'alert' : 'status'}, text);
export function field(label, name, attrs = {}) {
  return el('div', {class:'field'}, el('label', {for:name}, label), el('input', {id:name, name, ...attrs}));
}
export function dataTable(title, columns, rows) {
  const table = el('table', {}, el('caption', {}, title));
  table.append(el('thead', {}, el('tr', {}, columns.map(c => el('th', {scope:'col', 'aria-sort':c.sort}, c.heading ?? c.label)))));
  table.append(el('tbody', {}, rows.map(row => el('tr', {}, columns.map(c => el('td', {}, c.render ? c.render(row) : String(row[c.key] ?? '—')))))));
  return el('div', {class:'table-wrap', tabindex:'0', 'aria-label':title}, table);
}
export function empty(title, description, action) {
  return el('section', {class:'empty'}, el('div', {class:'empty-mark','aria-hidden':'true'}, '◇'), el('h2', {}, title), el('p', {class:'muted'}, description), action ?? '');
}
export function confirmDelete(name, onConfirm) {
  const dialog = el('dialog', {'aria-labelledby':'delete-title'});
  const error = el('div');
  const cancel = button('Giữ lại', {autofocus:true, onclick:() => dialog.close()});
  const submit = button('Xóa sinh viên', {class:'danger', onclick:async () => {
    submit.disabled = cancel.disabled = true;
    submit.textContent = 'Đang xóa…';
    try { await onConfirm(); dialog.close(); }
    catch (e) { error.replaceChildren(notice(e.message, 'error')); submit.textContent = 'Xóa sinh viên'; submit.disabled = cancel.disabled = false; }
  }});
  dialog.append(el('h2', {id:'delete-title'}, 'Xóa sinh viên này?'), el('p', {}, `Bạn sẽ xóa ${name} và các đăng ký học của sinh viên này. Thao tác không thể hoàn tác.`), error, el('div', {class:'actions'}, cancel, submit));
  dialog.addEventListener('cancel', event => { if (submit.disabled) event.preventDefault(); });
  dialog.addEventListener('close', () => dialog.remove());
  document.body.append(dialog); dialog.showModal();
}

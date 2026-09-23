import {el,button,link,field,notice,dataTable,empty,confirmDelete} from './ui.js';

const resources = {
  students:{title:'Sinh viên',singular:'sinh viên',icon:'♙',description:'Mỗi hành trình học tập bắt đầu từ một hồ sơ.',id:'studentId',name:'fullName',columns:['fullName','email','dateOfBirth']},
  semesters:{title:'Học kỳ',singular:'học kỳ',icon:'▦',description:'Tra cứu các học kỳ và thời gian học.',id:'semesterId',name:'semesterName',columns:['semesterName','startDate','endDate']},
  subjects:{title:'Môn học',singular:'môn học',icon:'▤',description:'Danh mục môn học và số tín chỉ.',id:'subjectId',name:'subjectName',columns:['subjectCode','subjectName','credit']},
  courses:{title:'Lớp học',singular:'lớp học',icon:'▧',description:'Tra cứu lớp học và các đăng ký liên quan.',id:'courseId',name:'courseName',columns:['courseName','semesterId']},
  enrollments:{title:'Đăng ký học',singular:'đăng ký',icon:'✓',description:'Theo dõi sinh viên, lớp học và trạng thái đăng ký.',id:'enrollmentId',name:'enrollmentId',columns:['enrollmentId','studentId','courseId','enrollDate','status']}
};
const labels = {studentId:'Mã sinh viên',fullName:'Họ và tên',email:'Email',dateOfBirth:'Ngày sinh',semesterId:'Mã học kỳ',semesterName:'Tên học kỳ',startDate:'Bắt đầu',endDate:'Kết thúc',subjectId:'Mã môn',subjectCode:'Mã môn học',subjectName:'Tên môn học',credit:'Tín chỉ',courseId:'Mã lớp',courseName:'Tên lớp học',enrollmentId:'Mã đăng ký',enrollDate:'Ngày đăng ký',status:'Trạng thái'};
const params = new URLSearchParams(location.search);
const segments = location.pathname.split('/').filter(Boolean);
const resource = segments[0] || 'students';
const config = resources[resource];
const content = document.querySelector('#content');
let dirty = false;
addEventListener('beforeunload', event => { if(dirty) {event.preventDefault();event.returnValue='';} });
function destination(extra = {}) {
  const query = new URLSearchParams(params);
  for(const [key,value] of Object.entries(extra)) value === null ? query.delete(key) : query.set(key,String(value));
  return `/${resource}${query.size ? '?'+query : ''}`;
}
function format(key,value) {
  if(value == null) return '—';
  if(/Date$/.test(key) || key==='dateOfBirth') { const match=String(value).match(/^(\d{4})-(\d{2})-(\d{2})/); if(match) return `${match[3]}/${match[2]}/${match[1]}`; }
  return String(value);
}
async function api(path, options = {}) {
  let response;
  try { response=await fetch(`/api/${path}`,{...options,headers:{'Content-Type':'application/json',...options.headers}}); }
  catch { throw new Error(options.method ? 'Chưa xác nhận được thao tác. Kiểm tra lại dữ liệu trước khi gửi lại.' : 'Không kết nối được máy chủ. Vui lòng thử lại.'); }
  const payload=await response.json();
  if(!response.ok) {
    const error = new Error(response.status===404 ? 'Không tìm thấy bản ghi này.' : (payload.errors?.join(' ') || payload.message || 'Có lỗi xảy ra.'));
    error.status=response.status; throw error;
  }
  return payload;
}
function heading(title,description,action) {return el('header',{class:'heading'},el('div',{},el('h1',{},title),el('p',{class:'muted'},description)),action ?? '');}
function columnsFor(target,sortable=false) {
  const meta=resources[target];
  return meta.columns.map((key,index) => {
    const sort=params.get('sort');
    return {key,label:labels[key],sort:sortable && (sort===key ? 'ascending' : sort==='-'+key ? 'descending' : 'none'),
      heading:sortable ? el('a',{href:destination({sort:sort===key ? '-'+key : key,page:1})},labels[key],sort===key?' ↑':sort==='-'+key?' ↓':' ↕') : undefined,
      render:row => index===0 ? el('a',{class:'row-link',href:`/${target}/${row[meta.id]}${target===resource?location.search:''}`},format(key,row[key]))
        : key==='status' ? el('span',{class:`badge ${row[key]==='active'?'':'neutral'}`},row[key]==='active'?'Đang học':row[key]==='completed'?'Hoàn thành':String(row[key]))
        : format(key,row[key])};
  });
}
async function list() {
  document.title=`${config.title} · LMS Lab`;
  const header=heading(config.title,config.description,resource==='students'?link('+ Thêm sinh viên','/students/new'+location.search,'primary'):null);
  const search=field('Tìm kiếm','search',{value:params.get('search') || '',placeholder:`Tìm ${config.singular}…`,maxlength:200});search.classList.add('search');
  const size=el('select',{id:'size',name:'size','aria-label':'Số dòng mỗi trang'},[10,20,50,100].map(n=>el('option',{value:n,selected:String(n)===(params.get('size')||'10')},`${n} dòng`)));
  const form=el('form',{class:'toolbar',action:`/${resource}`,method:'get'},search,el('div',{class:'field'},el('label',{for:'size'},'Hiển thị'),size),button('Tìm kiếm',{type:'submit'}));
  if(params.get('sort')) form.append(el('input',{type:'hidden',name:'sort',value:params.get('sort')}));
  if(params.get('search')) form.append(link('Xóa tìm kiếm',`/${resource}`));
  const area=el('section',{},notice('Đang tải danh sách…'));
  content.replaceChildren(header,form,area);
  if(params.get('saved')) content.insertBefore(notice('Đã lưu thay đổi.','success'),form);
  if(params.get('deleted')) content.insertBefore(notice('Đã xóa sinh viên.','success'),form);
  const request=new URLSearchParams();for(const key of ['search','sort','page','size']) if(params.has(key)) request.set(key,params.get(key));
  const result=await api(resource+'?'+request);
  const p=result.pagination;
  if(!result.data.length) {
    if(p.totalItems>0 && p.page>p.totalPages) {location.replace(destination({page:Math.max(1,p.totalPages)}));return;}
    area.replaceChildren(empty(params.get('search')?'Không tìm thấy kết quả':`Chưa có ${config.singular}`,params.get('search')?'Thử từ khóa khác hoặc xóa tìm kiếm.':'Dữ liệu sẽ xuất hiện tại đây khi được thêm vào hệ thống.',params.get('search')?link('Xóa tìm kiếm',`/${resource}`):resource==='students'?link('Thêm sinh viên','/students/new','primary'):null));return;
  }
  area.replaceChildren(dataTable(`${p.totalItems} ${config.singular} · Trang ${p.page}/${p.totalPages}`,columnsFor(resource,true),result.data));
  const paging=el('div',{class:'actions'});
  if(p.page>1) paging.append(link('← Trang trước',destination({page:p.page-1})));
  if(p.page<p.totalPages) paging.append(link('Trang sau →',destination({page:p.page+1})));
  area.append(el('div',{class:'pagination'},el('span',{class:'muted'},`Hiển thị ${(p.page-1)*p.pageSize+1}–${Math.min(p.page*p.pageSize,p.totalItems)} / ${p.totalItems}`),paging));
}
async function detail(id) {
  const result=await api(`${resource}/${id}`);const data=result.data;
  const title=String(data[config.name]);document.title=`${title} · LMS Lab`;
  const actions=resource==='students'?el('div',{class:'actions'},link('Sửa hồ sơ',`/students/${id}/edit${location.search}`,'primary'),button('Xóa',{class:'danger',onclick:()=>confirmDelete(data.fullName,async()=>{await api(`students/${id}`,{method:'DELETE'});location.href=destination({deleted:1,saved:null});})})):null;
  const details=el('dl',{class:'details'});
  for(const [key,value] of Object.entries(data)) if(value===null || typeof value!=='object') details.append(el('div',{},el('dt',{},labels[key]||key),el('dd',{},format(key,value))));
  content.replaceChildren(el('a',{class:'breadcrumb',href:destination({saved:null,deleted:null})},`← ${config.title}`),heading(title,`Chi tiết ${config.singular} · #${id}`,actions),details);
  if(params.get('saved')) content.insertBefore(notice('Đã lưu hồ sơ thành công.','success'),details);
  for(const [key,value] of Object.entries(data)) {
    if(value===null || typeof value!=='object') continue;
    const target={student:'students',course:'courses',semester:'semesters',courses:'courses',enrollments:'enrollments'}[key];
    if(!target) continue;
    const rows=Array.isArray(value)?value:[value];
    content.append(el('h2',{},resources[target].title),rows.length?dataTable(`${rows.length} bản ghi liên quan`,columnsFor(target),rows):notice('Chưa có bản ghi liên quan.'));
  }
}
async function edit(id) {
  const existing=id?(await api(`students/${id}`)).data:null;
  document.title=`${id?'Sửa':'Thêm'} sinh viên · LMS Lab`;
  const form=el('form',{class:'form-panel'});
  const today=new Date().toISOString().slice(0,10);
  form.append(field('Họ và tên','fullName',{required:true,maxlength:100,autocomplete:'name',value:existing?.fullName||''}),field('Email','email',{type:'email',required:true,maxlength:100,autocomplete:'email',value:existing?.email||''}),field('Ngày sinh','dateOfBirth',{type:'date',required:true,min:'1753-01-01',max:today,value:existing?.dateOfBirth?.slice(0,10)||''}));
  const errorArea=el('div',{tabindex:'-1'});
  const save=button('Lưu sinh viên',{type:'submit',class:'primary'});
  form.append(el('p',{class:'helper'},'Mã sinh viên được hệ thống tạo tự động. Các trường trên đều bắt buộc.'),errorArea,el('div',{class:'actions'},save,link('Hủy',id?`/students/${id}${location.search}`:destination())));
  form.addEventListener('input',()=>{dirty=true;});
  form.addEventListener('submit',async event=>{
    event.preventDefault();if(save.disabled)return;save.disabled=true;save.textContent='Đang lưu…';errorArea.replaceChildren();
    const input=Object.fromEntries(new FormData(form));
    try { const result=await api(`students${id?'/'+id:''}`,{method:id?'PUT':'POST',body:JSON.stringify(input)});dirty=false;const query=new URLSearchParams(params);query.set('saved','1');query.delete('deleted');location.href=`/students/${result.data.studentId}?${query}`; }
    catch(error){errorArea.replaceChildren(notice(error.message,'error'));errorArea.focus();save.disabled=false;save.textContent='Lưu sinh viên';}
  });
  content.replaceChildren(el('a',{class:'breadcrumb',href:destination()},'← Sinh viên'),heading(id?'Sửa hồ sơ':'Thêm sinh viên',id?'Cập nhật thông tin của sinh viên.':'Một hồ sơ mới, một hành trình mới.'),form);
}
for(const [key,value] of Object.entries(resources))document.querySelector('#navigation').append(el('a',{href:`/${key}`,class:'nav-link','aria-current':key===resource?'page':null},el('span',{class:'nav-icon','aria-hidden':'true'},value.icon),value.title));
try {
  if(!config) throw new Error('Trang không tồn tại.');
  if(resource==='students' && segments[1]==='new' && segments.length===2) await edit();
  else if(segments.length===3 && resource==='students' && /^\d+$/.test(segments[1]) && segments[2]==='edit') await edit(segments[1]);
  else if(segments.length===2 && /^\d+$/.test(segments[1])) await detail(segments[1]);
  else if(segments.length<=1) await list();
  else throw new Error('Trang không tồn tại.');
} catch(error) {content.replaceChildren(heading('Chưa thể hiển thị dữ liệu','Bạn có thể thử lại hoặc quay về danh sách.'),notice(error.message,'error'),el('div',{class:'actions'},button('Thử lại',{onclick:()=>location.reload()}),link('Về danh sách','/students')));}

import fs from 'node:fs/promises';
import path from 'node:path';
import { pathToFileURL } from 'node:url';
const runtime = process.env.ARTIFACT_TOOL_MODULE;
if (!runtime) throw new Error('Set ARTIFACT_TOOL_MODULE to the bundled artifact_tool.mjs');
const { Presentation, PresentationFile } = await import(pathToFileURL(runtime).href);
const out = path.resolve('artifacts/lab2-presentation');
await fs.mkdir(out,{recursive:true});
const deck=Presentation.create({slideSize:{width:1280,height:720}});
const ink='#162B43', muted='#52677D', accent='#087E8B';
function text(slide,content,x,y,w,h,size=27,color=ink,bold=false){
 const shape=slide.shapes.add({geometry:'textbox',position:{left:x,top:y,width:w,height:h},fill:'none',line:{style:'solid',fill:'none',width:0}});
 shape.text=content;shape.text.style={fontSize:size,typeface:'Arial',color,bold};return shape;
}
function slide(title,subtitle,notes=''){
 const s=deck.slides.add();s.background.fill='#F7F9FC';
 text(s,title,65,50,1150,95,44,ink,true);
 if(subtitle) text(s,subtitle,65,145,1150,65,24,muted);
 text(s,'PRN232 / LAB 2',65,674,600,25,16,muted);
 text(s,String(deck.slides.items.length).padStart(2,'0'),1150,674,65,25,16,muted);
 s.speakerNotes.text=notes;return s;
}
function lines(s,items,y=240,gap=90){items.forEach((v,i)=>{text(s,String(i+1).padStart(2,'0'),65,y+i*gap,65,50,29,accent,true);text(s,v,150,y+i*gap,1040,75,29);});}
function table(s,values,widths,y=235,height=370){
 const t=s.tables.add({rows:values.length,columns:values[0].length,left:65,top:y,width:1150,height,columnWidths:widths,values});
 for(let r=0;r<values.length;r++)for(let c=0;c<values[0].length;c++){
  const cell=t.getCell(r,c);cell.fill=r===0?ink:(r%2?'#FFFFFF':'#EDF2F7');
  cell.text.style={fontSize:23,typeface:'Arial',color:r===0?'#FFFFFF':ink,bold:r===0};
 }
 t.borders.assign({style:'solid',fill:'#D8E0E8',width:1});return t;
}
let s=slide('LMS: Advanced REST API & Security','PRN232 Lab 2', 'Bài trình bày tập trung vào phần nâng cấp từ LMS Lab 1. Trình bày kiến trúc, hợp đồng HTTP, bảo mật, triển khai và demo. Tên thành viên không được suy đoán.');
text(s,'Nâng cấp API học tập\nvới JWT và phân quyền',65,275,1120,180,56,ink,true);
text(s,'ASP.NET Core 8  /  EF Core  /  SQL Server 2022',65,530,1100,70,27,accent);
s=slide('Lab 2 kế thừa LMS Lab 1','Phạm vi triển khai theo đề và phụ lục chấm tự động.','Nguồn: LAB 2 - Advanced REST API & Security, trang 1–9; PRN232_LAB2_Submission_Guideline, trang 1–6. Đây là Lab 2, không phải Final Assignment.');
table(s,[['Giữ lại','Bổ sung'],['Ba tầng và năm resource LMS','JWT, role Admin/Student, refresh token'],['Search, sort, paging, fields, expand','JSON/XML, validation, versioning'],['Docker và Swagger','Middleware, nested route, Swagger Authorize']],[530,620]);
s=slide('Nghiệp vụ nằm trong Service','Ba project ứng dụng, API và SQL Server chạy trong hai container.','API nhận request và ánh xạ response, Service xử lý nghiệp vụ, Repository dùng EF Core. Giao diện tĩnh và Swagger cùng được phục vụ bởi API. Không tách ba layer thành ba service.');
table(s,[['Thành phần','Trách nhiệm'],['API','Routing, binding, validation, JWT middleware, Swagger'],['Services','Nghiệp vụ LMS, xác thực và phát token'],['Repositories','EF Core, schema, truy vấn, lưu user và refresh token'],['SQL Server','Dữ liệu LMS và dữ liệu xác thực']],[310,840],225,400);
s=slide('Database giữ các quan hệ LMS','Bổ sung dữ liệu xác thực bằng thay đổi schema không xóa dữ liệu cũ.','Semester có nhiều Course. Student và Course liên kết qua Enrollment. Subject giữ độc lập theo schema Lab 1. Users có nhiều RefreshTokens. PhoneNumber là cột tùy chọn được thêm vào Students.');
table(s,[['Dữ liệu','Quan hệ / nội dung'],['Semester, Course','Một học kỳ có nhiều lớp học'],['Student, Enrollment','Sinh viên đăng ký lớp qua Enrollment'],['Subject','Danh mục môn học độc lập theo Lab 1'],['Users, RefreshTokens','PasswordHash, Role; token hash, hạn dùng, thời điểm thu hồi']],[380,770],225,405);
s=slide('Một hợp đồng API, hai định dạng','API v1 dùng cùng envelope cho JSON và XML.','GET /api/v1/students?fields=studentId,fullName&size=2. Đổi Accept application/json thành application/xml. Thử application/pdf để thấy 406. JSON có success, message, data, errors và pagination cho collection.');
text(s,'GET /api/v1/students?fields=studentId,fullName&size=2',65,235,1150,70,29,accent,true);
table(s,[['Accept','Kết quả'],['application/json','200 + dữ liệu JSON'],['application/xml','200 + dữ liệu XML'],['application/pdf','406 Not Acceptable']],[500,650],330,280);
s=slide('Dữ liệu sai trả lỗi 400','Kiểm tra đầu vào trước khi thực hiện nghiệp vụ hoặc ghi database.','DataAnnotations có Required, StringLength, Range, EmailAddress, Phone, RegularExpression. FluentValidation thực thi trên StudentRequestModel; custom PastDateAttribute kiểm ngày sinh từ 1753 đến hôm nay. Trường JSON ngoài hợp đồng bị từ chối.');
table(s,[['Cơ chế','Ví dụ kiểm tra'],['DataAnnotations','Họ tên, email, phone, page, username'],['FluentValidation','Họ tên không rỗng; email ASCII hợp lệ'],['PastDateAttribute','Ngày sinh không ở tương lai hoặc trước 1753'],['Response','400 + thông báo theo trường; không phát sinh lỗi 500']],[380,770],225,405);
s=slide('JWT xác định danh tính và quyền','Access token có thời hạn một giờ và được ký bằng HMAC-SHA256.','POST /api/v1/auth/login với tài khoản thực hành admin hoặc student. Không hiển thị token thật trên slide. Key nhận từ biến môi trường Jwt__Key. API kiểm chữ ký, issuer, audience và thời hạn.');
lines(s,['Client gửi username và password tới /auth/login.','Service đối chiếu PasswordHasher với hash trong DB.','API trả accessToken, refreshToken và expiresIn.','Client gửi Authorization: Bearer <accessToken>.'],235,90);
s=slide('401 và 403 thể hiện hai tình huống khác nhau','Quyền được kiểm tra trên server cho cả URL v1 và URL tương thích.','Demo: không có token gọi /auth/me nhận 401; token Student gọi /enrollments nhận 403; Admin nhận 200. Các danh mục còn lại được đọc công khai trong dữ liệu thực hành. POST/PUT/DELETE students yêu cầu Admin.');
table(s,[['Request','Kết quả'],['Không có token / token hỏng','401 Unauthorized'],['Student đọc enrollments','403 Forbidden'],['Admin đọc enrollments','200 OK'],['Admin thêm sinh viên hợp lệ','201 Created + Location']],[700,450],225,400);
s=slide('Refresh token chỉ được đổi một lần','Token ngẫu nhiên có thời hạn bảy ngày; database chỉ giữ SHA-256 của token.','POST /api/v1/auth/refresh-token. Sau đổi thành công, dùng lại token cũ nhận 401. Hai request đồng thời đổi cùng token: một request thành công, một request 401. Cập nhật thu hồi được thực hiện nguyên tử trong database.');
lines(s,['Client gửi refresh token hiện tại.','Database kiểm tra hạn dùng và đánh dấu đã thu hồi.','Service cấp cặp access token + refresh token mới.','Dùng lại refresh token cũ nhận 401.'],235,90);
s=slide('Docker và Swagger phục vụ demo','API tự chờ database, tạo schema và seed tài khoản thực hành.','Chạy Docker Compose bằng project riêng. Không dùng chung volume Lab 1. Swagger hỗ trợ nút Authorize. Request logging ghi method, path, status và thời gian. Không ghi password, access token hay request body.');
text(s,"$env:API_PORT='8093'\ndocker compose -p prn232-lab2 up --build -d",65,225,1150,100,30,accent,true);
table(s,[['Địa chỉ demo đang dùng','Mục đích'],['http://localhost:8093/swagger','API, Authorize và thử request'],['http://localhost:8093/login.html','Đăng nhập giao diện LMS'],['http://localhost:8093/health','Kiểm tra API kết nối database']],[760,390],340,270);
s=slide('Bộ chấm đi kèm đạt 10/10','Kết quả kiểm tra Docker ngày 16/10/2026.','Nguồn: báo cáo bộ chấm gốc tại artifacts/lab2-grading-delivery. 14 static và 15 dynamic đạt 100%, không có regression hoặc packaging penalty. Kiểm tra bổ sung: 120 HTTP contracts kế thừa Lab 1 và 49 HTTP checks Lab 2. Đây là kết quả bộ chấm được cung cấp, không phải điểm giảng viên đã công bố.');
text(s,'10/10',65,240,480,150,108,accent,true);
text(s,'4 điểm code và cấu hình\n6 điểm API chạy thực tế\nKhông bị trừ điểm',620,255,570,190,32);
text(s,'Kiểm tra bổ sung: 120 ca Lab 1 + 49 ca Lab 2',65,500,1150,70,31,ink,true);
text(s,'Bao gồm refresh token dùng lại, trao đổi đồng thời và JSON/XML.',65,580,1150,55,24,muted);
s=slide('Luồng demo trong Swagger','Mở http://localhost:8093/swagger và thực hiện theo thứ tự.','Demo 1: GET students JSON/XML và application/pdf. Demo 2: POST students sau login Admin, gửi thiếu email rồi body hợp lệ. Demo 3: đăng nhập Student và gọi enrollments 403. Demo 4: token hỏng 401. Demo 5: đổi refresh token và dùng lại token cũ. Demo 6: query fields/expand, nested route và logs. Các tài khoản thực hành nằm trong README bài nộp.');
lines(s,['Đọc danh sách: JSON, XML, rồi định dạng không hỗ trợ.','Login Admin, Authorize, thêm sinh viên và kiểm tra validation.','Đổi sang Student để chứng minh 403; thử token hỏng để thấy 401.','Refresh token, truy vấn nâng cao, nested route và Docker logs.'],235,90);
await fs.writeFile(path.join(out,'slides.json'),JSON.stringify({count:deck.slides.items.length},null,2));
const pptx=await PresentationFile.exportPptx(deck);await pptx.save(path.join(out,'PRN232_Lab2_Presentation.pptx'));
for(const [i,s] of deck.slides.items.entries()){
 const png=await deck.export({slide:s,format:'png',scale:1});
 await fs.writeFile(path.join(out,`slide-${String(i+1).padStart(2,'0')}.png`),new Uint8Array(await png.arrayBuffer()));
}
console.log(`Exported ${deck.slides.items.length} slides to ${out}`);

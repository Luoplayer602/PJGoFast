# Đề xuất nhiệm vụ sửa lỗi và cải thiện chất lượng

## 1) Nhiệm vụ sửa lỗi đánh máy (typo / naming consistency)
**Tiêu đề:** Chuẩn hoá tên `DbSet` từ `nhatKys` thành `NhatKys` trong `PJGoFastDbContext`.

**Vấn đề phát hiện:** `DbSet<NhatKy>` đang dùng tên thuộc tính `nhatKys` (camelCase bất thường) trong khi các `DbSet` khác dùng PascalCase (`KhachHangs`, `TaiXes`, `Admins`, ...). Điều này dễ gây nhầm lẫn khi truy vấn và khi đọc code.

**Phạm vi đề xuất:**
- Đổi tên thuộc tính trong `Data/GoFastDbContext.cs`.
- Cập nhật toàn bộ chỗ tham chiếu.
- Tạo migration rename nếu cần để tránh mất dữ liệu.

**Tiêu chí hoàn thành:**
- Không còn tham chiếu `nhatKys`.
- Build chạy được, migration áp dụng an toàn.

## 2) Nhiệm vụ sửa bug chức năng đăng nhập
**Tiêu đề:** Bổ sung middleware `UseAuthentication()` để cookie login hoạt động đúng.

**Vấn đề phát hiện:** Trong pipeline hiện tại có `UseAuthorization()` nhưng thiếu `UseAuthentication()`. Trong khi đó login dùng `SignInAsync("CookieAuth", principal)` và cấu hình `AddAuthentication("CookieAuth")`. Thiếu middleware xác thực có thể gây hành vi đăng nhập/ủy quyền không như mong đợi.

**Phạm vi đề xuất:**
- Thêm `app.UseAuthentication();` trước `app.UseAuthorization();` trong `Program.cs`.
- Smoke test luồng: đăng ký -> đăng nhập -> truy cập trang yêu cầu role.

**Tiêu chí hoàn thành:**
- Sau đăng nhập, user được nhận diện đúng trong request kế tiếp.
- Các trang có `[Authorize]` dùng đúng principal/role.

## 3) Nhiệm vụ sửa chú thích code hoặc chênh lệch tài liệu
**Tiêu đề:** Làm sạch comment debug trong `LoginController` và cập nhật README mô tả setup thật sự.

**Vấn đề phát hiện:**
- Có comment chứa stack message/debug exception ngay trong controller, gây nhiễu code review.
- `README.md` hiện gần như trống, chưa mô tả cách chạy, biến môi trường, DB migration, account seed.

**Phạm vi đề xuất:**
- Xoá comment debug không cần thiết trong `Controllers/LoginController.cs`.
- Bổ sung README: yêu cầu môi trường, chuỗi kết nối, lệnh migration/run, mô tả module chính.

**Tiêu chí hoàn thành:**
- Controller không còn comment debug runtime.
- README đủ để dev mới clone và chạy được dự án.

## 4) Nhiệm vụ cải thiện quy trình kiểm thử
**Tiêu đề:** Thiết lập pipeline kiểm thử cơ bản (build + test) cho PR.

**Vấn đề phát hiện:** Chưa có test project và chưa có hướng dẫn/automation kiểm thử; khó phát hiện regression ở các luồng đăng nhập, đăng ký.

**Phạm vi đề xuất:**
- Tạo test project (xUnit/NUnit) cho service `KhachHangService`.
- Viết test tối thiểu cho: đăng ký trùng số điện thoại, confirm mật khẩu sai, đăng nhập sai mật khẩu.
- Tạo CI workflow chạy `dotnet restore`, `dotnet build`, `dotnet test` trên pull request.

**Tiêu chí hoàn thành:**
- PR mới tự động chạy build + test.
- Có ít nhất 3 test cho các nhánh nghiệp vụ cốt lõi.

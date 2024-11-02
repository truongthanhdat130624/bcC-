using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLSV
{
    public partial class DangKy : Form
    {
        public DangKy() // Constructor cho lớp DangKy
        {
            InitializeComponent(); // Khởi tạo các thành phần trên form
        }

        private void btnDangKy_Click(object sender, EventArgs e) // Sự kiện khi nhấn nút Đăng Ký
        {
            // Kiểm tra độ dài tên đăng nhập
            if (txtTenDN.Text.Length < 3 || txtTenDN.Text.Length > 50)
            {
                MessageBox.Show("Tên đăng nhập phải có từ 3 đến 50 ký tự!"); // Thông báo lỗi
                return; // Dừng thực thi để yêu cầu người dùng nhập lại
            }

            // Kiểm tra mật khẩu và mật khẩu nhập lại
            if (txtMatKhau.Text == txtMatKhauLai.Text) // Kiểm tra mật khẩu
            {
                string connectionString = @"Data Source=DESKTOP-4J951Q8;Initial Catalog=quanlysv;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open(); // Mở kết nối tới SQL
                        string query = "INSERT INTO TaiKhoan (TenDangNhap, MatKhau, Keypass) VALUES (@TenDN, @MatKhau, @Keypass)";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@TenDN", txtTenDN.Text); // Tham số tên đăng nhập
                        cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text); // Tham số mật khẩu
                        cmd.Parameters.AddWithValue("@Keypass", txtKey.Text); // Tham số khóa bảo mật
                        cmd.ExecuteNonQuery(); // Thực thi truy vấn

                        MessageBox.Show("Đăng ký thành công!"); // Thông báo thành công
                        this.Hide(); // Ẩn form đăng ký
                        Login loginForm = new Login(); // Mở lại form đăng nhập
                        loginForm.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message); // Hiển thị lỗi
                    }
                }
            }
            else
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!"); // Thông báo mật khẩu không khớp
            }
        }

        private void DangKy_Load(object sender, EventArgs e) // Sự kiện khi form được tải
        {
            // Không hiển thị thông báo "Form Đăng Ký đã tải thành công!"
            // Bạn có thể thêm code để khởi tạo dữ liệu ở đây, nếu cần
        }

        private void btnThoat_Click(object sender, EventArgs e) // Sự kiện khi nhấn nút Thoát
        {
            this.Hide(); // Ẩn form đăng ký và quay lại form đăng nhập
            Login loginForm = new Login(); // Tạo mới form đăng nhập
            loginForm.Show(); // Hiển thị form đăng nhập
        }
    }
}

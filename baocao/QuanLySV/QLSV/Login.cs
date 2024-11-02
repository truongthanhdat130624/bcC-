using QuanLySV; // Namespace của ứng dụng
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QLSV // Namespace cho lớp Login
{
    public partial class Login : Form
    {
        public Login() // Constructor cho lớp Login
        {
            InitializeComponent(); // Khởi tạo các thành phần trên form
        }

        private void btnDangNhap_Click(object sender, EventArgs e) // Sự kiện khi nhấn nút Đăng Nhập
        {
            string connectionString = @"Data Source=DESKTOP-4J951Q8;Initial Catalog=quanlysv;Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();  // Mở kết nối tới SQL
                    string query = "SELECT COUNT(1) FROM TaiKhoan WHERE TenDangNhap=@TenDN AND MatKhau=@MatKhau";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@TenDN", txtTenDN.Text);  // Tham số tên đăng nhập
                    cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text);  // Tham số mật khẩu

                    int count = Convert.ToInt32(cmd.ExecuteScalar());  // Thực thi truy vấn

                    if (count == 1)
                    {
                        MessageBox.Show("Đăng nhập thành công!"); // Thông báo thành công
                        this.Hide();  // Ẩn form đăng nhập
                        Form2 qlsvForm = new Form2();  // Tạo mới form quản lý sinh viên
                        qlsvForm.Show();  // Hiển thị form quản lý sinh viên
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!"); // Thông báo lỗi
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message); // Hiển thị lỗi
                }
            }
        }

        private void btnDangKy_Click(object sender, EventArgs e) // Sự kiện khi nhấn nút Đăng Ký
        {
            this.Hide();  // Ẩn form đăng nhập
            DangKy dkForm = new DangKy();  // Tạo mới form đăng ký
            dkForm.Show();  // Hiển thị form đăng ký
        }

        private void btnThoat_Click(object sender, EventArgs e) // Sự kiện khi nhấn nút Thoát
        {
            Application.Exit();  // Thoát chương trình
        }

        private void Login_Load(object sender, EventArgs e) // Sự kiện khi form được tải
        {
            // Không hiển thị thông báo "Form Login đã tải thành công!"
            // Bạn có thể thêm code để khởi tạo dữ liệu ở đây, nếu cần
        }
    }
}

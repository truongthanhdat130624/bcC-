using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq; // Để sử dụng LINQ
using System.Windows.Forms;

namespace QuanLySV
{
    public partial class Form2 : Form
    {
        int rowindex = -1;
        string connectionString = @"Data Source=DESKTOP-4J951Q8;Initial Catalog=quanlysv;Integrated Security=True";
        private string selectedImagePath;

        public Form2()
        {
            InitializeComponent();
            mtxtMaSV.ReadOnly = false; // Đặt mã sinh viên có thể nhập
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadKhoa();
            btnXoa.Enabled = false;
            btnCapNhat.Enabled = false;
            btnThem.Enabled = true;
            LoadData();

            // Thiết lập Mask cho MaskedTextBox để chỉ cho phép nhập 10 chữ số cho Mã sinh viên
            mtxtMaSV.Mask = "0000000000"; // Đặt Mask để chỉ cho phép nhập 10 chữ số
        }

        private void LoadKhoa()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT TenKhoa FROM Khoa", connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                cbKhoa.DataSource = dataTable;
                cbKhoa.DisplayMember = "TenKhoa";
                cbKhoa.ValueMember = "TenKhoa";
            }
        }

        private void LoadData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                dgvDanhSach.Columns.Clear(); // Xóa tất cả các cột cũ
                dgvDanhSach.DataSource = null;

                // Thêm các cột vào DataGridView
                dgvDanhSach.Columns.Add("MaSinhVien", "Mã sinh viên");
                dgvDanhSach.Columns.Add("HoTen", "Họ tên");
                dgvDanhSach.Columns.Add("NgaySinh", "Ngày sinh");
                dgvDanhSach.Columns.Add("GioiTinh", "Giới tính");
                dgvDanhSach.Columns.Add("DiemTB", "Điểm TB");
                dgvDanhSach.Columns.Add("Khoa", "Khoa");

                // Thêm cột "Hinh" để hiển thị tên tệp hình ảnh
                dgvDanhSach.Columns.Add("Hinh", "Hình");

                // Truy vấn lấy dữ liệu sinh viên
                SqlCommand cmd = new SqlCommand("SELECT MaSinhVien, HoTen, NgaySinh, GioiTinh, DiemTB, Khoa, HinhAnh FROM SinhVien", connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int rowIndex = dgvDanhSach.Rows.Add();
                    dgvDanhSach.Rows[rowIndex].Cells["MaSinhVien"].Value = reader["MaSinhVien"];
                    dgvDanhSach.Rows[rowIndex].Cells["HoTen"].Value = reader["HoTen"];
                    dgvDanhSach.Rows[rowIndex].Cells["NgaySinh"].Value = Convert.ToDateTime(reader["NgaySinh"]).ToString("dd/MM/yyyy");
                    dgvDanhSach.Rows[rowIndex].Cells["GioiTinh"].Value = reader["GioiTinh"];
                    dgvDanhSach.Rows[rowIndex].Cells["DiemTB"].Value = reader["DiemTB"];
                    dgvDanhSach.Rows[rowIndex].Cells["Khoa"].Value = reader["Khoa"];

                    // Hiển thị tên tệp hình ảnh
                    string hinhAnh = reader["HinhAnh"] != DBNull.Value ? Path.GetFileName(reader["HinhAnh"].ToString()) : "";
                    dgvDanhSach.Rows[rowIndex].Cells["Hinh"].Value = hinhAnh;
                }
            }
        }



        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || dgvDanhSach.Rows[e.RowIndex].IsNewRow)
            {
                ResetData();
                btnThem.Enabled = true;
                btnCapNhat.Enabled = false;
                btnXoa.Enabled = false;

                // Đặt mã sinh viên có thể nhập khi thêm sinh viên mới
                mtxtMaSV.ReadOnly = false;
            }
            else
            {
                rowindex = e.RowIndex;
                mtxtMaSV.Text = dgvDanhSach.Rows[rowindex].Cells["MaSinhVien"].Value.ToString();
                mtxtMaSV.ReadOnly = true; // Đặt mã sinh viên không thể sửa đổi khi chọn sinh viên

                txtHoTen.Text = dgvDanhSach.Rows[rowindex].Cells["HoTen"].Value.ToString();
                txtDiemTB.Text = dgvDanhSach.Rows[rowindex].Cells["DiemTB"].Value.ToString();
                cbKhoa.Text = dgvDanhSach.Rows[rowindex].Cells["Khoa"].Value.ToString();

                string ngaySinhString = dgvDanhSach.Rows[rowindex].Cells["NgaySinh"].Value.ToString();
                DateTime ngaySinh;
                if (DateTime.TryParse(ngaySinhString, out ngaySinh))
                {
                    dtpNgaySinh.Value = ngaySinh;
                }

                string gioiTinh = dgvDanhSach.Rows[rowindex].Cells["GioiTinh"].Value.ToString();
                if (gioiTinh == "Nam")
                {
                    rbNam.Checked = true;
                }
                else
                {
                    rbNu.Checked = true;
                }

                // Lấy ảnh từ cơ sở dữ liệu và hiển thị trong pictureBoxStudent
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT HinhAnh FROM SinhVien WHERE MaSinhVien = @MaSV";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaSV", mtxtMaSV.Text);
                    object result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        byte[] imageBytes = (byte[])result;
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        {
                            pictureBoxStudent.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        pictureBoxStudent.Image = null; // Nếu không có ảnh, đặt ảnh mặc định hoặc xóa ảnh hiện tại
                    }
                }

                btnThem.Enabled = false;
                btnCapNhat.Enabled = true;
                btnXoa.Enabled = true;
            }
        }



        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            double diemtb;
            try
            {
                // Kiểm tra Mã sinh viên có 10 ký tự và chỉ chứa số
                if (string.IsNullOrWhiteSpace(mtxtMaSV.Text) || mtxtMaSV.Text.Length != 10 || !IsAllDigits(mtxtMaSV.Text))
                {
                    throw new Exception("Mã sinh viên phải có 10 ký tự số và không được trống.");
                }

                // Kiểm tra họ và tên
                if (string.IsNullOrWhiteSpace(txtHoTen.Text) || txtHoTen.Text.Length < 3 || txtHoTen.Text.Length > 30)
                {
                    throw new Exception("Họ tên phải từ 3 đến 30 ký tự.");
                }

                if (!checkMaSV(mtxtMaSV.Text))
                {
                    throw new Exception("Mã sinh viên đã tồn tại.");
                }

                if (!double.TryParse(txtDiemTB.Text, out diemtb))
                {
                    throw new Exception("Điểm trung bình phải là số.");
                }

                // Kiểm tra ràng buộc điểm trung bình trong khoảng từ 0 đến 10
                if (diemtb < 0 || diemtb > 10)
                {
                    throw new Exception("Điểm trung bình phải nằm trong khoảng từ 0 đến 10.");
                }

                // Mặc định chọn Nam nếu không chọn gì
                string gioiTinh = rbNam.Checked ? "Nam" : "Nữ";
                if (!rbNam.Checked && !rbNu.Checked)
                {
                    gioiTinh = "Nam"; // Mặc định chọn Nam
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO SinhVien (MaSinhVien, HoTen, NgaySinh, GioiTinh, DiemTB, Khoa, HinhAnh) " +
                                   "VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @DiemTB, @Khoa, @HinhAnh)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaSV", mtxtMaSV.Text);
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                    cmd.Parameters.AddWithValue("@DiemTB", diemtb);
                    cmd.Parameters.AddWithValue("@Khoa", cbKhoa.Text);

                    if (pictureBoxStudent.Image != null)
                    {
                        byte[] imageBytes = ImageToByteArray(pictureBoxStudent.Image);
                        cmd.Parameters.AddWithValue("@HinhAnh", imageBytes);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@HinhAnh", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm sinh viên thành công!");
                LoadData(); // Tải lại dữ liệu để cập nhật danh sách
                ResetData(); // Đặt lại form

                // Bật các nút sau khi thêm
                btnXoa.Enabled = false;
                btnCapNhat.Enabled = false;
                btnThem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo");
            }
        }

        private bool checkMaSV(string masv)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM SinhVien WHERE MaSinhVien = @MaSV", connection);
                cmd.Parameters.AddWithValue("@MaSV", masv);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0;
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            double diemtb;
            try
            {
                if (rowindex == -1 || rowindex >= dgvDanhSach.Rows.Count)
                {
                    throw new Exception("Chưa chọn sinh viên cần sửa");
                }

                // Kiểm tra họ và tên
                if (string.IsNullOrWhiteSpace(txtHoTen.Text) || txtHoTen.Text.Length < 3 || txtHoTen.Text.Length > 30)
                {
                    throw new Exception("Họ tên phải từ 3 đến 30 ký tự.");
                }

                if (string.IsNullOrWhiteSpace(mtxtMaSV.Text) || mtxtMaSV.Text.Length != 10 || !IsAllDigits(mtxtMaSV.Text))
                {
                    throw new Exception("Mã sinh viên phải có 10 ký tự số và không được trống.");
                }

                if (!double.TryParse(txtDiemTB.Text, out diemtb))
                {
                    throw new Exception("Điểm trung bình phải là số.");
                }

                // Kiểm tra ràng buộc điểm trung bình trong khoảng từ 0 đến 10
                if (diemtb < 0 || diemtb > 10)
                {
                    throw new Exception("Điểm trung bình phải nằm trong khoảng từ 0 đến 10.");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE SinhVien SET HoTen=@HoTen, NgaySinh=@NgaySinh, GioiTinh=@GioiTinh, DiemTB=@DiemTB, Khoa=@Khoa, HinhAnh=@HinhAnh WHERE MaSinhVien=@MaSV";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaSV", mtxtMaSV.Text);
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value);
                    cmd.Parameters.AddWithValue("@GioiTinh", rbNam.Checked ? "Nam" : "Nữ");
                    cmd.Parameters.AddWithValue("@DiemTB", diemtb);
                    cmd.Parameters.AddWithValue("@Khoa", cbKhoa.Text);

                    // Tạo bản sao của ảnh và lưu vào cơ sở dữ liệu
                    if (pictureBoxStudent.Image != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pictureBoxStudent.Image.Save(ms, pictureBoxStudent.Image.RawFormat);
                            byte[] imageBytes = ms.ToArray();
                            cmd.Parameters.AddWithValue("@HinhAnh", imageBytes);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@HinhAnh", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật sinh viên thành công!");
                LoadData();
                ResetData();
                btnThem.Enabled = true;
                btnCapNhat.Enabled = false;
                btnXoa.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo");
            }
        }



        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                if (rowindex == -1 || rowindex >= dgvDanhSach.Rows.Count - 1)
                {
                    throw new Exception("Chưa chọn sinh viên cần xóa");
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM SinhVien WHERE MaSinhVien = @MaSV";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@MaSV", mtxtMaSV.Text);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa sinh viên thành công!");
                LoadData();
                ResetData();
                btnXoa.Enabled = false;
                btnCapNhat.Enabled = false;
                btnThem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Thông báo");
            }
        }

        private void ResetData()
        {
            mtxtMaSV.Clear();
            txtHoTen.Clear();
            txtDiemTB.Clear();
            rbNam.Checked = false;
            rbNu.Checked = false;
            pictureBoxStudent.Image = null;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có muốn thoát không?",
                "Thông báo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void buttonChooseImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedImagePath = openFileDialog.FileName;
                pictureBoxStudent.Image = Image.FromFile(openFileDialog.FileName);
            }
        }

        private void mtxtMaSV_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số (0-9) và phím xóa
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn chặn việc nhập ký tự không hợp lệ
            }
        }

        private void txtDiemTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số (0-9) và phím xóa
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // Ngăn chặn việc nhập ký tự không hợp lệ
            }
        }

        private void dgvDanhSach_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Xử lý sự kiện khi click vào một ô trong DataGridView
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            string khoaLoc = cbKhoaLoc.SelectedItem?.ToString();
            string gioiTinhLoc = cbGioiTinhLoc.SelectedItem?.ToString();
            string diemLoc = cbDiemLoc.SelectedItem?.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Khởi tạo câu truy vấn
                string query = "SELECT MaSinhVien, HoTen, NgaySinh, GioiTinh, DiemTB, Khoa FROM SinhVien WHERE 1=1";

                // Áp dụng lọc theo Khoa
                if (!string.IsNullOrEmpty(khoaLoc))
                {
                    query += " AND Khoa = @Khoa";
                }

                // Áp dụng lọc theo Giới tính
                if (!string.IsNullOrEmpty(gioiTinhLoc))
                {
                    query += " AND GioiTinh = @GioiTinh";
                }

                // Áp dụng lọc theo Điểm
                if (!string.IsNullOrEmpty(diemLoc))
                {
                    switch (diemLoc)
                    {
                        case "Dưới 5":
                            query += " AND DiemTB < 5";
                            break;
                        case "5 đến 7":
                            query += " AND DiemTB >= 5 AND DiemTB <= 7";
                            break;
                        case "Trên 7":
                            query += " AND DiemTB > 7";
                            break;
                    }
                }

                SqlCommand cmd = new SqlCommand(query, connection);

                // Thêm các tham số nếu có
                if (!string.IsNullOrEmpty(khoaLoc))
                {
                    cmd.Parameters.AddWithValue("@Khoa", khoaLoc);
                }
                if (!string.IsNullOrEmpty(gioiTinhLoc))
                {
                    cmd.Parameters.AddWithValue("@GioiTinh", gioiTinhLoc);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                dgvDanhSach.DataSource = dataTable;
            }
        }

        // Hàm xử lý khi nhấn nút Hủy lọc
        private void btnHuyLoc_Click(object sender, EventArgs e)
        {
            // Tải lại dữ liệu mà không áp dụng bất kỳ bộ lọc nào
            LoadData();

            // Xóa các lựa chọn trong ComboBox
            cbKhoaLoc.SelectedIndex = -1;
            cbGioiTinhLoc.SelectedIndex = -1;
            cbDiemLoc.SelectedIndex = -1;
        }

        // Hàm kiểm tra xem một chuỗi có phải chỉ là số hay không
        private bool IsAllDigits(string str)
        {
            return str.All(char.IsDigit); // Sử dụng LINQ để kiểm tra tất cả ký tự là số
        }

        // Sự kiện khi nhấn vào ô Mã sinh viên
        private void mtxtMaSV_Enter(object sender, EventArgs e)
        {
            // Đưa con trỏ chuột về đầu ô mỗi khi ô "Mã sinh viên" được focus
            mtxtMaSV.SelectionStart = 0; // Đưa con trỏ chuột về đầu ô
        }

        private void mtxtMaSV_Click(object sender, EventArgs e)
        {
            // Bạn có thể bỏ qua sự kiện này nếu chỉ cần sự kiện Enter
            mtxtMaSV.SelectionStart = 0; // Đưa con trỏ chuột về đầu ô
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void rbNu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbNam_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void dtpNgaySinh_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxStudent_Click(object sender, EventArgs e)
        {

        }

        private void mtxtMaSV_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void txtDiemTB_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtHoTen_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void cbKhoaLoc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbGioiTinhLoc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbDiemLoc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }
    }
}
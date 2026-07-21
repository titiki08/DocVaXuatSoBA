using System.Data.SqlClient;

namespace DocVaXuatSoBA.DAL
{
    public class DBConnect
    {
        // Chuỗi kết nối sử dụng tài khoản KhangTT của bạn
        private readonly string _connectionString =
            @"Server=192.168.1.68;Database=eHospital_ThongNhat;User Id=KhangTT;Password=KhangTT@123!;";

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
using System.Data;

namespace DocVaXuatSoBA.DTO
{
    public class HoSoCV130DTO
    {
        public int BenhAn_Id { get; set; }
        // Dùng DataSet để chứa toàn bộ các bảng XML1, XML2, XML3,... trả về từ Stored Procedure
        public DataSet DuLieuHoSo { get; set; }
    }
}   
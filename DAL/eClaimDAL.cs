using System;
using System.Data;
using System.Data.SqlClient;
using DocVaXuatSoBA.DTO;

namespace DocVaXuatSoBA.DAL
{
    public class eClaimDAL : DBConnect
    {
        public DataSet GetDataSetCV130(int benhAnId)
        {
            DataSet ds = new DataSet("HoSoCV130");
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand("BANGKENOITRU_XML", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BenhAn_Id", benhAnId);

                    // Set timeout cao vì SP xử lý dữ liệu lớn (300s = 5 phút)
                    cmd.CommandTimeout = 300;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }
                }
            }

            // Đổi tên các bảng cho dễ gọi dựa theo thứ tự trả về của SP
            if (ds.Tables.Count > 0) ds.Tables[0].TableName = "XML1";
            if (ds.Tables.Count > 1) ds.Tables[1].TableName = "XML2";
            if (ds.Tables.Count > 2) ds.Tables[2].TableName = "XML3";
            if (ds.Tables.Count > 3) ds.Tables[3].TableName = "XML4";
            if (ds.Tables.Count > 4) ds.Tables[4].TableName = "XML5";
            if (ds.Tables.Count > 5) ds.Tables[5].TableName = "XML7";
            if (ds.Tables.Count > 6) ds.Tables[6].TableName = "XML13";
            if (ds.Tables.Count > 7) ds.Tables[7].TableName = "XML14";

            return ds;
        }
    }
}
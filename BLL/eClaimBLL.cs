using System;
using System.Data;
using System.IO;
using System.Text;
using DocVaXuatSoBA.DAL;
using System.Linq;

namespace DocVaXuatSoBA.BLL
{
    public class eClaimBLL
    {
        private eClaimDAL _dal = new eClaimDAL();

        // Lấy dữ liệu hồ sơ
        public DataSet LayDuLieuHoSo(int benhAnId)
        {
            if (benhAnId <= 0)
                throw new ArgumentException("Mã bệnh án không hợp lệ.");

            return _dal.GetDataSetCV130(benhAnId);
        }

        public string ConvertTableToXmlString(DataTable dt)
        {
            if (dt == null) return string.Empty;

            if (dt.Rows.Count == 0)
                return $"<!-- CÓ KẾT NỐI SQL: Nhưng Bảng số {dt.TableName} của bệnh án này không có dòng dữ liệu nào -->";

            // 1. XỬ LÝ ĐẶC THÙ CHO BẢNG XML1 (CẤU TRÚC TỔNG HỢP)
            if (dt.TableName == "XML1")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<TONG_HOP>");

                DataRow row = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                {
                    string tagName = col.ColumnName.ToUpper();
                    if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                        tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                    string value = row[col]?.ToString().Trim();
                    if (string.IsNullOrEmpty(value)) sb.AppendLine($"  <{tagName} />");
                    else sb.AppendLine($"  <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                }
                sb.AppendLine("</TONG_HOP>");
                return sb.ToString();
            }

            // 2. XỬ LÝ ĐẶC THÙ CHO BẢNG XML2 (DANH SÁCH THUỐC)
            if (dt.TableName == "XML2")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<CHITIEU_CHITIET_THUOC>");
                sb.AppendLine("  <DSACH_CHI_TIET_THUOC>");

                var sortedRows = dt.AsEnumerable().OrderBy(r => {
                    int stt;
                    int.TryParse(r["STT"]?.ToString(), out stt);
                    return stt;
                });

                int sttCounter = 1;
                foreach (DataRow row in sortedRows)
                {
                    sb.AppendLine("    <CHI_TIET_THUOC>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        string tagName = col.ColumnName.ToUpper();
                        if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                            tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                        string value = (tagName == "STT") ? sttCounter.ToString() : row[col]?.ToString().Trim();

                        if (string.IsNullOrEmpty(value)) sb.AppendLine($"      <{tagName} />");
                        else sb.AppendLine($"      <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                    }
                    sb.AppendLine("    </CHI_TIET_THUOC>");
                    sttCounter++;
                }
                sb.AppendLine("  </DSACH_CHI_TIET_THUOC>");
                sb.AppendLine("</CHITIEU_CHITIET_THUOC>");
                return sb.ToString();
            }

            // 3. XỬ LÝ ĐẶC THÙ CHO BẢNG XML3 (DANH SÁCH DỊCH VỤ KỸ THUẬT & VTYT)
            if (dt.TableName == "XML3")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<CHITIEU_CHITIET_DVKT_VTYT>");
                sb.AppendLine("  <DSACH_CHI_TIET_DVKT>");

                var sortedRows = dt.AsEnumerable().OrderBy(r => {
                    int stt;
                    int.TryParse(r["STT"]?.ToString(), out stt);
                    return stt;
                });

                int sttCounter = 1;
                foreach (DataRow row in sortedRows)
                {
                    sb.AppendLine("    <CHI_TIET_DVKT>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        string tagName = col.ColumnName.ToUpper();
                        if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                            tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                        string value = (tagName == "STT") ? sttCounter.ToString() : row[col]?.ToString().Trim();

                        if (string.IsNullOrEmpty(value)) sb.AppendLine($"      <{tagName} />");
                        else sb.AppendLine($"      <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                    }
                    sb.AppendLine("    </CHI_TIET_DVKT>");
                    sttCounter++;
                }
                sb.AppendLine("  </DSACH_CHI_TIET_DVKT>");
                sb.AppendLine("</CHITIEU_CHITIET_DVKT_VTYT>");
                return sb.ToString();
            }

            // 4. XỬ LÝ ĐẶC THÙ CHO BẢNG XML4 (KẾT QUẢ CẬN LÂM SÀNG)
            if (dt.TableName == "XML4")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<CHITIEU_CHITIET_DICHVUCANLAMSANG>");
                sb.AppendLine("  <DSACH_CHI_TIET_CLS>");

                var sortedRows = dt.AsEnumerable().OrderBy(r => {
                    int stt;
                    int.TryParse(r["STT"]?.ToString(), out stt);
                    return stt;
                });

                int sttCounter = 1;
                foreach (DataRow row in sortedRows)
                {
                    sb.AppendLine("    <CHI_TIET_CLS>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        string tagName = col.ColumnName.ToUpper();
                        if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                            tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                        string value = (tagName == "STT") ? sttCounter.ToString() : row[col]?.ToString().Trim();

                        if (string.IsNullOrEmpty(value)) sb.AppendLine($"      <{tagName} />");
                        else sb.AppendLine($"      <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                    }
                    sb.AppendLine("    </CHI_TIET_CLS>");
                    sttCounter++;
                }
                sb.AppendLine("  </DSACH_CHI_TIET_CLS>");
                sb.AppendLine("</CHITIEU_CHITIET_DICHVUCANLAMSANG>");
                return sb.ToString();
            }

            // 5. XỬ LÝ ĐẶC THÙ CHO BẢNG XML5 (DIỄN BIẾN LÂM SÀNG)
            if (dt.TableName == "XML5")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<CHITIEU_CHITIET_DIENBIENLAMSANG>");
                sb.AppendLine("  <DSACH_CHI_TIET_DIEN_BIEN_BENH>");

                var sortedRows = dt.AsEnumerable().OrderBy(r => {
                    int stt;
                    int.TryParse(r["STT"]?.ToString(), out stt);
                    return stt;
                });

                int sttCounter = 1;
                foreach (DataRow row in sortedRows)
                {
                    sb.AppendLine("    <CHI_TIET_DIEN_BIEN_BENH>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        string tagName = col.ColumnName.ToUpper();
                        if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                            tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                        string value = (tagName == "STT") ? sttCounter.ToString() : row[col]?.ToString().Trim();

                        if (string.IsNullOrEmpty(value)) sb.AppendLine($"      <{tagName} />");
                        else sb.AppendLine($"      <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                    }
                    sb.AppendLine("    </CHI_TIET_DIEN_BIEN_BENH>");
                    sttCounter++;
                }
                sb.AppendLine("  </DSACH_CHI_TIET_DIEN_BIEN_BENH>");
                sb.AppendLine("</CHITIEU_CHITIET_DIENBIENLAMSANG>");
                return sb.ToString();
            }

            // 6. XỬ LÝ ĐẶC THÙ CHO BẢNG XML7 (GIẤY RA VIỆN)
            if (dt.TableName == "XML7")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<CHI_TIEU_DU_LIEU_GIAY_RA_VIEN>");

                DataRow row = dt.Rows[0];
                foreach (DataColumn col in dt.Columns)
                {
                    string tagName = col.ColumnName.ToUpper();
                    if (tagName == "BENHAN_ID" || tagName == "TIEPNHAN_ID" ||
                        tagName == "BENHNHAN_ID" || tagName == "XACNHANCHIPHI_ID") continue;

                    string value = row[col]?.ToString().Trim();
                    if (string.IsNullOrEmpty(value)) sb.AppendLine($"  <{tagName} />");
                    else sb.AppendLine($"  <{tagName}>{System.Security.SecurityElement.Escape(value)}</{tagName}>");
                }
                sb.AppendLine("</CHI_TIEU_DU_LIEU_GIAY_RA_VIEN>");
                return sb.ToString();
            }

            // 7. CÁC BẢNG KHÁC (XML13, XML14...)
            using (StringWriter sw = new StringWriter())
            {
                dt.WriteXml(sw, XmlWriteMode.IgnoreSchema);
                return sw.ToString();
            }
        }

        // Convert chuỗi XML thành Base64
        public string ConvertXmlToBase64(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent)) return string.Empty;

            byte[] bytes = Encoding.UTF8.GetBytes(xmlContent);
            return Convert.ToBase64String(bytes);
        }
    }
}
using DevExpress.XtraTab;
using DocVaXuatSoBA.BLL;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DocVaXuatSoBA
{
    public partial class Form1 : Form
    {
        private eClaimBLL _bll = new eClaimBLL();
        private DataSet _dsHienTai; // Lưu trữ tạm thời DataSet của hồ sơ hiện tại

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLayDuLieu_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtBenhAnId.Text, out int benhAnId))
            {
                MessageBox.Show("Vui lòng nhập Mã Bệnh Án (số nguyên)!", "Thông báo");
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                _dsHienTai = _bll.LayDuLieuHoSo(benhAnId);

                if (_dsHienTai != null && _dsHienTai.Tables.Count > 0)
                {
                    string[] tabNames = { "XML1", "XML2", "XML3", "XML4", "XML5", "XML7", "XML13", "XML14" };
                    int maxTabs = Math.Min(_dsHienTai.Tables.Count, tabNames.Length);

                    // 1. Đổ dữ liệu vào các Tab XML
                    for (int i = 0; i < maxTabs; i++)
                    {
                        string controlName = "nrtb" + tabNames[i];
                        var nrtb = this.Controls.Find(controlName, true);

                        if (nrtb.Length > 0)
                        {
                            string xmlData = _bll.ConvertTableToXmlString(_dsHienTai.Tables[i]);
                            SetTextToInnerRichTextBox(nrtb[0], xmlData);
                        }
                    }

                    // 2. TỰ ĐỘNG TẠO FILE TỔNG <GIAMDINHHS> DẠNG BASE64
                    string giamDinhXml = TaoXMLGiamDinh(_dsHienTai, tabNames);
                    var nrtbBase = this.Controls.Find("nrtbBase64", true);
                    if (nrtbBase.Length > 0)
                    {
                        SetTextToInnerRichTextBox(nrtbBase[0], giamDinhXml);
                    }

                    MessageBox.Show($"Truy xuất thành công! Đã tạo sẵn cấu trúc Base64.", "Thông báo");
                }
                else
                {
                    MessageBox.Show("Dữ liệu trả về rỗng.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // Hàm tự động ghép các bảng thành file <GIAMDINHHS> chuẩn của BHYT
        private string TaoXMLGiamDinh(DataSet ds, string[] tabNames)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return string.Empty;

            try
            {
                // Lấy thông tin từ XML1 (Table 0)
                DataRow rowXml1 = ds.Tables[0].Rows[0];
                string maCSKCB = rowXml1.Table.Columns.Contains("MA_CSKCB") ? rowXml1["MA_CSKCB"].ToString() : "75002";
                string ngayLap = DateTime.Now.ToString("yyyyMMdd");

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<GIAMDINHHS>");
                sb.AppendLine("  <THONGTINDONVI>");
                sb.AppendLine($"    <MACSKCB>{maCSKCB}</MACSKCB>");
                sb.AppendLine("  </THONGTINDONVI>");
                sb.AppendLine("  <THONGTINHOSO>");
                sb.AppendLine($"    <NGAYLAP>{ngayLap}</NGAYLAP>");
                sb.AppendLine("    <SOLUONGHOSO>1</SOLUONGHOSO>");
                sb.AppendLine("    <DANHSACHHOSO>");
                sb.AppendLine("      <HOSO>");

                int maxTabs = Math.Min(ds.Tables.Count, tabNames.Length);

                for (int i = 0; i < maxTabs; i++)
                {
                    // Chỉ xuất thẻ <FILEHOSO> nếu bảng đó có dòng dữ liệu
                    if (ds.Tables[i].Rows.Count > 0)
                    {
                        string xmlContent = _bll.ConvertTableToXmlString(ds.Tables[i]);
                        string base64 = _bll.ConvertXmlToBase64(xmlContent); // Mã hóa Base64

                        sb.AppendLine("        <FILEHOSO>");
                        sb.AppendLine($"          <LOAIHOSO>{tabNames[i]}</LOAIHOSO>");
                        sb.AppendLine($"          <NOIDUNGFILE>{base64}</NOIDUNGFILE>");
                        sb.AppendLine("        </FILEHOSO>");
                    }
                }

                sb.AppendLine("      </HOSO>");
                sb.AppendLine("    </DANHSACHHOSO>");
                sb.AppendLine("  </THONGTINHOSO>");
                // Để trống thẻ chữ ký, có thể dùng tool ký số sau
                sb.AppendLine("  <CHUKYDONVI></CHUKYDONVI>");
                sb.AppendLine("</GIAMDINHHS>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo định dạng GIAMDINHHS: " + ex.Message);
                return string.Empty;
            }
        }

        // Hàm đệ quy lục tìm RichTextBox tận cùng bên trong NumberLineRTB
        private void SetTextToInnerRichTextBox(Control parent, string text)
        {
            parent.Text = text;
            foreach (Control child in parent.Controls)
            {
                if (child is RichTextBox) child.Text = text;
                else if (child.Controls.Count > 0) SetTextToInnerRichTextBox(child, text);
            }
        }

        private string GetTextFromInnerRichTextBox(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is RichTextBox) return child.Text;
                if (child.Controls.Count > 0)
                {
                    string innerText = GetTextFromInnerRichTextBox(child);
                    if (!string.IsNullOrEmpty(innerText)) return innerText;
                }
            }
            return parent.Text;
        }

        private void btnXuatBase64_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dsHienTai == null || _dsHienTai.Tables.Count == 0 || _dsHienTai.Tables[0].Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng tìm kiếm dữ liệu bệnh án trước!", "Thông báo");
                    return;
                }

                // Lấy nội dung tổng đã ghép từ tab Base64
                var nrtbBase = this.Controls.Find("nrtbBase64", true);
                string base64Content = string.Empty;
                if (nrtbBase.Length > 0)
                {
                    base64Content = GetTextFromInnerRichTextBox(nrtbBase[0]);
                }

                if (string.IsNullOrEmpty(base64Content))
                {
                    MessageBox.Show("Không có dữ liệu Base64 để lưu!", "Thông báo");
                    return;
                }

                // Lấy thông tin tạo tên file từ XML1 (bảng 0)
                DataRow rowXml1 = _dsHienTai.Tables[0].Rows[0];
                string maLK = rowXml1.Table.Columns.Contains("MA_LK") ? rowXml1["MA_LK"].ToString() : "Unknown";
                string ngayVao = rowXml1.Table.Columns.Contains("NGAY_VAO") ? rowXml1["NGAY_VAO"].ToString() : "Unknown";
                string ngayRa = rowXml1.Table.Columns.Contains("NGAY_RA") ? rowXml1["NGAY_RA"].ToString() : "Unknown";

                // Tên file và Đường dẫn
                string fileName = $"{maLK}_{ngayVao}_{ngayRa}.NoiTru.xml";
                string folderPath = @"C:\OneDrive_Khang\OneDrive\Máy tính\LuuFileXML";

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullPath = Path.Combine(folderPath, fileName);

                // Ghi ra file vật lý
                File.WriteAllText(fullPath, base64Content, Encoding.UTF8);

                MessageBox.Show($"Đã lưu file thành công!\n\nTên file: {fileName}\nThư mục: {folderPath}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

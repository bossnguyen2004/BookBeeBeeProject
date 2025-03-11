namespace Fe_Admin.DTO
{
    public class PagingInfo
    {
        public int TongSoItem { get; set; }
        public int SoItemTrenMotTrang { get; set; } = 12;
        public int TrangHienTai { get; set; } = 1;
        public int SoItemTrenTrangHienTai { get; set; }
        public int SoTrang => (int)Math.Ceiling((double)TongSoItem / SoItemTrenMotTrang);
    }
}

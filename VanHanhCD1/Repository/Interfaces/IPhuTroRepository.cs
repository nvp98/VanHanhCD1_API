namespace VanHanhCD1.Repository.Interfaces
{
    public interface IPhuTroRepository
    {
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioMotThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioMotThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportQuatGioMotThieuKetMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioBaThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioBaThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportQuatGioBaThieuKetHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioHaiThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioHaiThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportQuatGioHaiThieuKetMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioBonThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioBonThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportQuatGioBonThieuKetHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineMotThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineMotThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportTurbineMotThieuKetMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineHaiThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineHaiThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportTurbineHaiThieuKetMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineBaThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineBaThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportTurbineBaThieuKetHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineBonThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineBonThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportTurbineBonThieuKetHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiMatVongMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiMatVongMots(DateTime from, DateTime to);
        public Task<byte[]> ExportNoiHoiMatVongMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiMatVongHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiMatVongHais(DateTime from, DateTime to);
        public Task<byte[]> ExportNoiHoiMatVongHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiOngKhoiQuatGioLamMatVongMots(DateTime from, DateTime to);
        public Task<byte[]> ExportNoiHoiOngKhoiQuatGioLamMatVongMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiOngKhoiQuatGioLamMatVongHais(DateTime from, DateTime to);
        public Task<byte[]> ExportNoiHoiOngKhoiQuatGioLamMatVongHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursKhuKhiKhoiThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeKhuKhiKhoiThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportKhuKhiKhoiThieuKetMots(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursKhuKhiKhoiThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeKhuKhiKhoiThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportKhuKhiKhoiThieuKetHais(DateTime from, DateTime to, string path);
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTramNuocTuanHoans();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTramNuocTuanHoans(DateTime from, DateTime to);
        public Task<byte[]> ExporTramNuocTuanHoans(DateTime from, DateTime to, string path);
    }
}


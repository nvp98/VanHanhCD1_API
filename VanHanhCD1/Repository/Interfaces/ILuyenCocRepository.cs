using System.Runtime.InteropServices;
using VanHanhCD1.Models.LuyenCoc;
using VanHanhCD1.Models.ThieuKet;

namespace VanHanhCD1.Repository.Interfaces
{
    public interface ILuyenCocRepository
    {
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ1s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ1s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocCDQ1s(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ2s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocCDQ2s(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ3s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ3s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocCDQ3s(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiMoiTruongMatDat1s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiMoiTruongMatDat1s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocLocBuiMoiTruongMatDat1s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocLocBuiMoiTruongMatDat1> GetLuyenCocLocBuiMoiTruongMatDat1MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiMoiTruongMatDat2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiMoiTruongMatDat2s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocLocBuiMoiTruongMatDat2s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocLocBuiMoiTruongMatDat2> GetLuyenCocLocBuiMoiTruongMatDat2MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiNhaSang2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiNhaSang2s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocLocBuiNhaSang2s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocLocBuiNhaSang2> GetLuyenCocLocBuiNhaSang2MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocMayNghiens();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocMayNghiens(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocMayNghiens(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocMayNghien> GetLuyenCocLuyenCocMayNghienMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan1s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan1s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocQuatTuanHoan1s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocQuatTuanHoan1> GetLuyenCocLuyenCocQuatTuanHoan1MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan2s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocQuatTuanHoan2s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocQuatTuanHoan2> GetLuyenCocLuyenCocQuatTuanHoan2MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan3s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan3s(DateTime from, DateTime to);
        public Task<byte[]> ExportLuyenCocQuatTuanHoan3s(DateTime from, DateTime to, string path);
        public IEnumerable<LuyenCocQuatTuanHoan3> GetLuyenCocLuyenCocQuatTuanHoan3MinValues();

        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum12(DateTime? times);
        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum34(DateTime? times);
        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum56(DateTime? times);
        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum78(DateTime? times);
        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum910(DateTime? times);

    }
}

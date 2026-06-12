using System.Runtime.InteropServices;
using VanHanhCD1.Models.BaiLieu;
using VanHanhCD1.Models.ThieuKet;

namespace VanHanhCD1.Repository.Interfaces
{
    public interface IBaiLieuRepository
    {
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC1s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC1s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoLocBuiC1s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoLocBuiC1> GetDongCoLocBuiC1MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC2s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoLocBuiC2s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoLocBuiC2> GetDongCoLocBuiC2MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC3s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC3s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoLocBuiC3s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoLocBuiC3> GetDongCoLocBuiC3MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC4s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC4s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoLocBuiC4s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoLocBuiC4> GetDongCoLocBuiC4MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC5s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC5s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoLocBuiC5s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoLocBuiC5> GetDongCoLocBuiC5MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoTrungThes();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoTrungThes(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoTrungThes(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoTrungThe> GetDongCoTrungTheMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursMBABaiLieus();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeMBABaiLieus(DateTime from, DateTime to);
        public IEnumerable<MBABaiLieu> GetMBABaiLieuMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTuDienBaiLieus();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTuDienBaiLieus(DateTime from, DateTime to);
        public IEnumerable<TuDienBaiLieu> GetTuDienBaiLieusMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoCheBiens();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoCheBiens(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoCheBiens(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoCheBien> GetDongCoCheBienMinValues();
    }
}

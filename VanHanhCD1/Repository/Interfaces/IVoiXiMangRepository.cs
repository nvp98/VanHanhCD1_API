namespace VanHanhCD1.Repository.Interfaces
{
    public interface IVoiXiMangRepository
    {
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungMots(DateTime from, DateTime to);
        public Task<byte[]> ExportLoVoiDungMots(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungHais(DateTime from, DateTime to);
        public Task<byte[]> ExportLoVoiDungHais(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungBas();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungBas(DateTime from, DateTime to);
        public Task<byte[]> ExportLoVoiDungBas(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoDoMitMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoDoMitMots(DateTime from, DateTime to);
        public Task<byte[]> ExportLoDoMitMots(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoDoMitHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoDoMitHais(DateTime from, DateTime to);
        public Task<byte[]> ExportLoDoMitHais(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiQuay();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiQuay(DateTime from, DateTime to);
        public Task<byte[]> ExportLoVoiQuay(DateTime from, DateTime to, string path);
    }
}

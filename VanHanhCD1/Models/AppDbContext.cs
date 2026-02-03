using Microsoft.EntityFrameworkCore;
using VanHanhCD1.Models.BaiLieu;
using VanHanhCD1.Models.LuyenCoc;
using VanHanhCD1.Models.PhuTro;
using VanHanhCD1.Models.ThieuKet;
using VanHanhCD1.Models.VeVien;
using VanHanhCD1.Models.VoiXiMang;

namespace VanHanhCD1.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<VH_LoVeVien> VH_LoVeVien { get; set; }
        public DbSet<LBDO1VeVien> LBDO1VeViens { get; set; }
        public DbSet<LBDO2VeVien> LBDO2VeViens { get; set; }

        public DbSet<QuatHutChinh> quatHutChinhs { get; set; }
        public DbSet<LocBuiTinhDien> locBuiTinhDiens { get; set; }
        public DbSet<LocBuiMoiTruongPLieuBLuoc> locBuiMoiTruongPLieuBLuocs { get; set; }
        public DbSet<LBMTMTPTrungChuyen23> locBuiMoiTruongMangThanhPhamTrungChuyen23s { get; set; }
        //Thieu Ket
        public DbSet<ThieuKet1> thieuKetMots { get; set; }
        public DbSet<ThieuKet2> thieuKetHais { get; set; }
        public DbSet<LocBuiMoiTruongDuoiMay1> locBuiMoiTruongDuoiMayMots { get; set; }
        public DbSet<LocBuiMoiTruongDuoiMay2> locBuiMoiTruongDuoiMayHais { get; set; }
        public DbSet<LocBuiMoiTruongMangQuang> locBuiMoiTruongMangQuangs { get; set; }
        public DbSet<DongCoThieuKet1> dongCoThieuKet1s { get; set; }
        public DbSet<DongCoThieuKet2> dongCoThieuKet2s { get; set; }
        //Phu Tro
        public DbSet<QuatGio1ThieuKet1> quatGioMotThieuKetMots { get; set; }
        public DbSet<QuatGio2ThieuKet1> quatGioHaiThieuKetMots { get; set; }
        public DbSet<QuatGio3ThieuKet2> quatGioBaThieuKetHais { get; set; }
        public DbSet<QuatGio4ThieuKet2> quatGioBonThieuKetHais { get; set; }
        public DbSet<TurBine1ThieuKet1> turbineMotThieuKetMots { get; set; }
        public DbSet<Turbine2ThieuKet1> turbineHaiThieuKetMots { get; set; }
        public DbSet<Turbine3ThieuKet2> turbineBaThieuKetHais { get; set; }
        public DbSet<Turbine4ThieuKet2> turbineBonThieuKetHais { get; set; }
        public DbSet<NoiHoiMatVong1> noiHoiMatVongMots { get; set; }
        public DbSet<NoiHoiMatVong2> noiHoiMatVongHais { get; set; }
        public DbSet<NoiHoiOngKhoiQuatGioLamMatVong1> noiHoiOngKhoiQuatGioLamMatVongMots { get; set; }
        public DbSet<NoiHoiOngKhoiQuatGioLamMatVong2> noiHoiOngKhoiQuatGioLamMatVongHais { get; set; }
        public DbSet<KhuKhiKhoiThieuKet1> khuKhiKhoiThieuKetMots { get; set; }
        public DbSet<KhuKhiKhoiThieuKet2> khuKhiKhoiThieuKetHais { get; set; }
        public DbSet<TramNuocTuanHoan> tramNuocTuanHoans { get; set; }
        //Voi Xi Mang
        public DbSet<LoVoiDungMot> loVoiDungMots { get; set; }
        public DbSet<LoVoiDungHai> loVoiDungHais { get; set; }
        public DbSet<LoVoiDungBa> loVoiDungBas { get; set; } 
        public DbSet<LoDoMitMot> loDoMitMots { get; set; }
        public DbSet<LoDoMitHai> loDoMitHais { get; set; }
        public DbSet<LoVoiQuay> loVoiQuays { get; set; }
        public DbSet<DongCoVoiXiMang> dongCoVoiXiMangs { get; set; }
        //End Voi Xi Mang
        //Luyen Coc
        public DbSet<LuyenCocCDQ1> luyenCocCDQ1s { get; set; }
        public DbSet<LuyenCocCDQ2> luyenCocCDQ2s { get; set; }
        public DbSet<LuyenCocCDQ3> luyenCocCDQ3s { get; set; }
        public DbSet<LuyenCocLocBuiMoiTruongMatDat1> luyenCocLocBuiMoiTruongMatDat1s { get; set; }
        public DbSet<LuyenCocLocBuiMoiTruongMatDat2> luyenCocLocBuiMoiTruongMatDat2s { get; set; }
        public DbSet<LuyenCocLocBuiNhaSang2> luyenCocLocBuiNhaSang2s { get; set; }
        public DbSet<LuyenCocMayNghien> luyenCocMayNghiens { get; set; }
        public DbSet<LuyenCocQuatTuanHoan1> luyenCocQuatTuanHoan1s { get; set; }
        public DbSet<LuyenCocQuatTuanHoan2> luyenCocQuatTuanHoan2s { get; set; }
        public DbSet<LuyenCocQuatTuanHoan3> luyenCocQuatTuanHoan3s { get; set; }
        public DbSet<LuyenCocCum12> luyenCocCum12s { get; set; }
        public DbSet<LuyenCocCum34> luyenCocCum34s { get; set; }
        public DbSet<LuyenCocCum56> luyenCocCum56s { get; set; }
        public DbSet<LuyenCocCum78> luyenCocCum78s { get; set; }
        public DbSet<LuyenCocCum910> luyenCocCum910s { get; set; }

        //End Luyen Coc

        //Bai Lieu
        public DbSet<DongCoTrungThe> dongCoTrungThes { get; set; } 
        public DbSet<DongCoLocBuiC1> dongCoLocBuiC1s { get; set; }
        public DbSet<DongCoLocBuiC2> dongCoLocBuiC2s { get; set; }
        public DbSet<DongCoLocBuiC3> dongCoLocBuiC3s { get; set; }
        public DbSet<DongCoLocBuiC4> dongCoLocBuiC4s { get; set; }
        public DbSet<DongCoLocBuiC5> dongCoLocBuiC5s { get; set; }
        //End Bai Lieu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            //modelBuilder.Entity<Quyen>().ToTable("Quyen");
            //modelBuilder.Entity<NhanVien>().ToTable("NhanVien");
            modelBuilder.Entity<VH_LoVeVien>().ToTable("VH_LoVeVien");
            modelBuilder.Entity<LBDO1VeVien>().ToTable("VH_LBDO1VeVien");
            modelBuilder.Entity<LBDO2VeVien>().ToTable("VH_LBDO2VeVien");
            modelBuilder.Entity<QuatHutChinh>().ToTable("VH_QHCVeVien");
            modelBuilder.Entity<LocBuiTinhDien>().ToTable("VH_LBTDVeVien");
            modelBuilder.Entity<LocBuiMoiTruongPLieuBLuoc>().ToTable("VH_LBMT_PhoiLieuBanLuocVeVien");
            modelBuilder.Entity<LBMTMTPTrungChuyen23>().ToTable("VH_LBMT_MTP_TramTrungChuyen23VeVien");
            //Thieu Ket
            modelBuilder.Entity<ThieuKet1>().ToTable("VH_ThieuKet1");
            modelBuilder.Entity<ThieuKet2>().ToTable("VH_ThieuKet2");
            modelBuilder.Entity<LocBuiMoiTruongDuoiMay1>().ToTable("VH_LBMT_DuoiMay1");
            modelBuilder.Entity<LocBuiMoiTruongDuoiMay2>().ToTable("VH_LBMT_DuoiMay2");
            modelBuilder.Entity<LocBuiMoiTruongMangQuang>().ToTable("VH_LBMT_MangQuang");
            modelBuilder.Entity<DongCoThieuKet1>().ToTable("DongCo_ThieuKet1");
            modelBuilder.Entity<DongCoThieuKet2>().ToTable("DongCo_ThieuKet2");
            //Phu Tro
            modelBuilder.Entity<QuatGio1ThieuKet1>().ToTable("VH_QG1ThieuKet1");
            modelBuilder.Entity<QuatGio2ThieuKet1>().ToTable("VH_QG2ThieuKet1");
            modelBuilder.Entity<QuatGio3ThieuKet2>().ToTable("VH_QG3ThieuKet2");
            modelBuilder.Entity<QuatGio4ThieuKet2>().ToTable("VH_QG4ThieuKet2");
            modelBuilder.Entity<TurBine1ThieuKet1>().ToTable("VH_TB1ThieuKet1");
            modelBuilder.Entity<Turbine2ThieuKet1>().ToTable("VH_TB2ThieuKet1");
            modelBuilder.Entity<Turbine3ThieuKet2>().ToTable("VH_TB3ThieuKet2");
            modelBuilder.Entity<Turbine4ThieuKet2>().ToTable("VH_TB4ThieuKet2");
            modelBuilder.Entity<NoiHoiMatVong1>().ToTable("VH_NoiHoiMatVong1ThieuKet1");
            modelBuilder.Entity<NoiHoiMatVong2>().ToTable("VH_NoiHoiMatVong2ThieuKet2");
            modelBuilder.Entity<NoiHoiOngKhoiQuatGioLamMatVong1>().ToTable("VH_NoiHoiOngKhoiQuatGioLamMatVong1");
            modelBuilder.Entity<NoiHoiOngKhoiQuatGioLamMatVong2>().ToTable("VH_NoiHoiOngKhoiQuatGioLamMatVong2");
            modelBuilder.Entity<KhuKhiKhoiThieuKet1>().ToTable("VH_KKKPhuTroThieuKet1");
            modelBuilder.Entity<KhuKhiKhoiThieuKet2>().ToTable("VH_KKKPhuTroThieuKet2");
            modelBuilder.Entity<TramNuocTuanHoan>().ToTable("VH_TramNuocTuanHoan");
            // Voi Xi Mang
            modelBuilder.Entity<LoVoiDungMot>().ToTable("VH_LoVoiDung1");
            modelBuilder.Entity<LoVoiDungHai>().ToTable("VH_LoVoiDung2");
            modelBuilder.Entity<LoVoiDungBa>().ToTable("VH_LoVoiDung3");
            modelBuilder.Entity<LoDoMitMot>().ToTable("VH_LoDoLoMit1");
            modelBuilder.Entity<LoDoMitHai>().ToTable("VH_LoDoLoMit2");
            modelBuilder.Entity<LoVoiQuay>().ToTable("VH_LoVoiQuay");
            modelBuilder.Entity<DongCoVoiXiMang>().ToTable("DongCo_VoiXiMang");
            //LuyenCoc
            modelBuilder.Entity<LuyenCocCDQ1>().ToTable("VHLC_CDQ1");
            modelBuilder.Entity<LuyenCocCDQ2>().ToTable("VHLC_CDQ2");
            modelBuilder.Entity<LuyenCocCDQ3>().ToTable("VHLC_CDQ3");
            modelBuilder.Entity<LuyenCocLocBuiMoiTruongMatDat1>().ToTable("VHLC_LBMT1");
            modelBuilder.Entity<LuyenCocLocBuiMoiTruongMatDat2>().ToTable("VHLC_LBMT2");
            modelBuilder.Entity<LuyenCocLocBuiNhaSang2>().ToTable("VHLC_LBNS2");
            modelBuilder.Entity<LuyenCocMayNghien>().ToTable("VHLC_COAL");
            modelBuilder.Entity<LuyenCocQuatTuanHoan1>().ToTable("VHLC_QGTH1");
            modelBuilder.Entity<LuyenCocQuatTuanHoan2>().ToTable("VHLC_QGTH2");
            modelBuilder.Entity<LuyenCocQuatTuanHoan3>().ToTable("VHLC_QGTH3");
            modelBuilder.Entity<LuyenCocCum12>().ToTable("VHLC_Cum12");
            modelBuilder.Entity<LuyenCocCum34>().ToTable("VHLC_Cum34");
            modelBuilder.Entity<LuyenCocCum56>().ToTable("VHLC_Cum5");
            modelBuilder.Entity<LuyenCocCum78>().ToTable("VHLC_Cum78");
            modelBuilder.Entity<LuyenCocCum910>().ToTable("VHLC_Cum910");
            //Bai Lieu
            modelBuilder.Entity<DongCoLocBuiC1>().ToTable("DCNL_LocBuiC1");
            modelBuilder.Entity<DongCoLocBuiC2>().ToTable("DCNL_LocBuiC2");
            modelBuilder.Entity<DongCoLocBuiC3>().ToTable("DCNL_LocBuiC3");
            modelBuilder.Entity<DongCoLocBuiC4>().ToTable("DCNL_LocBuiC4");
            modelBuilder.Entity<DongCoLocBuiC5>().ToTable("DCNL_LocBuiC5");
            modelBuilder.Entity<DongCoTrungThe>().ToTable("DCNL_TrungThe");
            base.OnModelCreating(modelBuilder);
        }

    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TradeMap.Data.Entity;

#nullable disable

namespace TradeMap.Data.Context
{
    public partial class TradeZoneMapContext : DbContext
    {
        public TradeZoneMapContext()
        {
        }

        public TradeZoneMapContext(DbContextOptions<TradeZoneMapContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<BuildingStreetSegment> BuildingStreetSegments { get; set; }
        public virtual DbSet<Campus> Campuses { get; set; }
        public virtual DbSet<CampusStreetSegment> CampusStreetSegments { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<CustomerSegment> CustomerSegments { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<FloorArea> FloorAreas { get; set; }
        public virtual DbSet<GroupZone> GroupZones { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<Segment> Segments { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<StoreStreetSegment> StoreStreetSegments { get; set; }
        public virtual DbSet<StoreTradeZone> StoreTradeZones { get; set; }
        public virtual DbSet<Street> Streets { get; set; }
        public virtual DbSet<StreetSegment> StreetSegments { get; set; }
        public virtual DbSet<SystemZone> SystemZones { get; set; }
        public virtual DbSet<TestCronJob> TestCronJobs { get; set; }
        public virtual DbSet<TradeZone> TradeZones { get; set; }
        public virtual DbSet<TradeZoneVersion> TradeZoneVersions { get; set; }
        public virtual DbSet<TypeBuilding> TypeBuildings { get; set; }
        public virtual DbSet<ViolationLog> ViolationLogs { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.Email, "IX_Account")
                    .IsUnique()
                    .HasFilter("([Email] IS NOT NULL)");

                entity.HasIndex(e => e.BrandId, "IX_Account_BrandId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FcmToken)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Account_Brand");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("Asset");

                entity.HasIndex(e => e.StoreId, "IX_Asset_BrandId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Assets)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_Asset_Store");
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.ToTable("Brand");

                entity.HasIndex(e => e.Name, "IX_Brand")
                    .IsUnique();

                entity.HasIndex(e => e.SegmentId, "IX_Brand_CategoryId");

                entity.Property(e => e.IconUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.Brands)
                    .HasForeignKey(d => d.SegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Brand_Category");
            });

            modelBuilder.Entity<Building>(entity =>
            {
                entity.ToTable("Building");

                entity.HasIndex(e => e.CampusId, "IX_Building_CampusId");

                entity.HasIndex(e => e.ReferenceId, "IX_Building_ReferenceId");

                entity.HasIndex(e => e.TypeId, "IX_Building_TypeId");

                entity.Property(e => e.Active).HasDefaultValueSql("((1))");

                entity.Property(e => e.Address).HasMaxLength(300);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Campus)
                    .WithMany(p => p.Buildings)
                    .HasForeignKey(d => d.CampusId)
                    .HasConstraintName("FK_Building_Campus");

                entity.HasOne(d => d.Reference)
                    .WithMany(p => p.InverseReference)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("FK_Building_Building");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Buildings)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK_Building_TypeBuilding");
            });

            modelBuilder.Entity<BuildingStreetSegment>(entity =>
            {
                entity.HasKey(e => new { e.BuidingId, e.StreetSegmentId })
                    .HasName("PK_BuidingStreetSegment");

                entity.ToTable("BuildingStreetSegment");

                entity.HasIndex(e => e.StreetSegmentId, "IX_BuildingStreetSegment_StreetSegmentId");

                entity.HasOne(d => d.Buiding)
                    .WithMany(p => p.BuildingStreetSegments)
                    .HasForeignKey(d => d.BuidingId)
                    .HasConstraintName("FK_BuidingStreetSegment_Building");

                entity.HasOne(d => d.StreetSegment)
                    .WithMany(p => p.BuildingStreetSegments)
                    .HasForeignKey(d => d.StreetSegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BuidingStreetSegment_StreetSegment");
            });

            modelBuilder.Entity<Campus>(entity =>
            {
                entity.ToTable("Campus");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CampusStreetSegment>(entity =>
            {
                entity.HasKey(e => new { e.CampusId, e.StreetSegmentId });

                entity.ToTable("CampusStreetSegment");

                entity.HasIndex(e => e.StreetSegmentId, "IX_CampusStreetSegment_StreetSegmentId");

                entity.HasOne(d => d.Campus)
                    .WithMany(p => p.CampusStreetSegments)
                    .HasForeignKey(d => d.CampusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CampusStreetSegment_Campus");

                entity.HasOne(d => d.StreetSegment)
                    .WithMany(p => p.CampusStreetSegments)
                    .HasForeignKey(d => d.StreetSegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CampusStreetSegment_StreetSegment1");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.ToTable("Config");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<CustomerSegment>(entity =>
            {
                entity.HasKey(e => new { e.BuildingId, e.SegmentId })
                    .HasName("PK_BuildingCategory");

                entity.ToTable("CustomerSegment");

                entity.HasIndex(e => e.SegmentId, "IX_BuildingCategory_CategoryId");

                entity.Property(e => e.TimeSlot)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.CustomerSegments)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK_BuildingCategory_Building");

                entity.HasOne(d => d.Segment)
                    .WithMany(p => p.CustomerSegments)
                    .HasForeignKey(d => d.SegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BuildingCategory_Category");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("District");

                entity.HasIndex(e => e.ProvinceId, "IX_District_ProvinceId");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Districts)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Province");
            });

            modelBuilder.Entity<Floor>(entity =>
            {
                entity.ToTable("Floor");

                entity.HasIndex(e => e.BuildingId, "IX_Floor_BuildingId");

                entity.HasIndex(e => e.ReferenceId, "IX_Floor_ReferenceId");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Floors)
                    .HasForeignKey(d => d.BuildingId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Floor_Building");

                entity.HasOne(d => d.Reference)
                    .WithMany(p => p.InverseReference)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("FK_Floor_Floor");
            });

            modelBuilder.Entity<FloorArea>(entity =>
            {
                entity.ToTable("FloorArea");

                entity.HasIndex(e => e.FloorId, "IX_FloorArea_FloorId");

                entity.HasIndex(e => e.ReferenceId, "IX_FloorArea_ReferenceId");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Floor)
                    .WithMany(p => p.FloorAreas)
                    .HasForeignKey(d => d.FloorId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Area_Floor");

                entity.HasOne(d => d.Reference)
                    .WithMany(p => p.InverseReference)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("FK_FloorArea_FloorArea");
            });

            modelBuilder.Entity<GroupZone>(entity =>
            {
                entity.ToTable("GroupZone");

                entity.HasIndex(e => e.BrandId, "IX_GroupZone_BrandId");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.GroupZones)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupZone_Brand");
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("History");

                entity.HasIndex(e => e.AccountId, "IX_History_AccountId");

                entity.HasIndex(e => e.BuildingId, "IX_History_BuildingId");

                entity.HasIndex(e => e.StoreId, "IX_History_StoreId");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_History_Account");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK_History_Building");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.StoreId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_History_Store");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Segment>(entity =>
            {
                entity.ToTable("Segment");

                entity.HasIndex(e => e.Name, "IX_Category")
                    .IsUnique()
                    .HasFilter("([Name] IS NOT NULL)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Store>(entity =>
            {
                entity.ToTable("Store");

                entity.HasIndex(e => e.BrandId, "IX_Store_BrandId");

                entity.HasIndex(e => e.FloorAreaId, "IX_Store_FloorAreaId");

                entity.HasIndex(e => e.ReferenceId, "IX_Store_ReferenceId");

                entity.Property(e => e.Address).HasMaxLength(300);

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Geom).HasColumnType("geometry");

                entity.Property(e => e.ImageUrl).HasMaxLength(1000);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.TimeSlot)
                    .HasMaxLength(4)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.BrandId)
                    .HasConstraintName("FK_Store_Brand");

                entity.HasOne(d => d.FloorArea)
                    .WithMany(p => p.Stores)
                    .HasForeignKey(d => d.FloorAreaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Store_Area");

                entity.HasOne(d => d.Reference)
                    .WithMany(p => p.InverseReference)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("FK_Store_Store");
            });

            modelBuilder.Entity<StoreStreetSegment>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.StreetSegmentId });

                entity.ToTable("StoreStreetSegment");

                entity.HasIndex(e => e.StreetSegmentId, "IX_StoreStreetSegment_StreetSegmentId");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreStreetSegments)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_StoreStreetSegment_Store");

                entity.HasOne(d => d.StreetSegment)
                    .WithMany(p => p.StoreStreetSegments)
                    .HasForeignKey(d => d.StreetSegmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StoreStreetSegment_StreetSegment");
            });

            modelBuilder.Entity<StoreTradeZone>(entity =>
            {
                entity.HasKey(e => new { e.StoreId, e.TradeZoneId });

                entity.ToTable("StoreTradeZone");

                entity.HasOne(d => d.Store)
                    .WithMany(p => p.StoreTradeZones)
                    .HasForeignKey(d => d.StoreId)
                    .HasConstraintName("FK_StoreTradeZone_Store");

                entity.HasOne(d => d.TradeZone)
                    .WithMany(p => p.StoreTradeZones)
                    .HasForeignKey(d => d.TradeZoneId)
                    .HasConstraintName("FK_StoreTradeZone_TradeZone");
            });

            modelBuilder.Entity<Street>(entity =>
            {
                entity.ToTable("Street");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Geom).HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("date");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Type).HasMaxLength(50);
            });

            modelBuilder.Entity<StreetSegment>(entity =>
            {
                entity.ToTable("StreetSegment");

                entity.HasIndex(e => e.StreetId, "IX_StreetSegment_StreetId");

                entity.HasIndex(e => e.WardId, "IX_StreetSegment_WardId");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Street)
                    .WithMany(p => p.StreetSegments)
                    .HasForeignKey(d => d.StreetId)
                    .HasConstraintName("FK_StreetSegment_Street");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.StreetSegments)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StreetSegment_Ward");
            });

            modelBuilder.Entity<SystemZone>(entity =>
            {
                entity.ToTable("SystemZone");

                entity.HasIndex(e => e.WardId, "IX_SystemZone_WardId");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.SystemZones)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_SystemZone_Account");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.SystemZones)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_SystemZone_Ward");
            });

            modelBuilder.Entity<TestCronJob>(entity =>
            {
                entity.ToTable("TestCronJob");
            });

            modelBuilder.Entity<TradeZone>(entity =>
            {
                entity.ToTable("TradeZone");

                entity.HasIndex(e => e.GroupZoneId, "IX_TradeZone_GroupZoneId");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.GroupZone)
                    .WithMany(p => p.TradeZones)
                    .HasForeignKey(d => d.GroupZoneId)
                    .HasConstraintName("FK_TradeZone_GroupZone");

                entity.HasOne(d => d.TradeZoneVersion)
                    .WithMany(p => p.TradeZones)
                    .HasForeignKey(d => d.TradeZoneVersionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TradeZone_TradeZoneVersion");
            });

            modelBuilder.Entity<TradeZoneVersion>(entity =>
            {
                entity.ToTable("TradeZoneVersion");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TimeSlot)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.TradeZoneVersions)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TradeZoneVersion_Brand");
            });

            modelBuilder.Entity<TypeBuilding>(entity =>
            {
                entity.ToTable("TypeBuilding");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<ViolationLog>(entity =>
            {
                entity.ToTable("ViolationLog");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Geom).HasColumnType("geometry");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Asset)
                    .WithMany(p => p.ViolationLogs)
                    .HasForeignKey(d => d.AssetId)
                    .HasConstraintName("FK_ViolationLog_Asset");
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.ToTable("Ward");

                entity.HasIndex(e => e.DistrictId, "IX_Ward_DistrictId");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.Property(e => e.Geom)
                    .IsRequired()
                    .HasColumnType("geometry");

                entity.Property(e => e.ModifyDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Wards)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_District");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

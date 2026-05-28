using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Museum.Models;

public partial class MuseumDbContext : DbContext
{
    public MuseumDbContext()
    {
    }

    public MuseumDbContext(DbContextOptions<MuseumDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Exhibit> Exhibits { get; set; }

    public virtual DbSet<ExhibitCategory> ExhibitCategories { get; set; }

    public virtual DbSet<ExhibitMaterial> ExhibitMaterials { get; set; }

    public virtual DbSet<ExhibitOnExhibition> ExhibitOnExhibitions { get; set; }

    public virtual DbSet<ExhibitPhoto> ExhibitPhotos { get; set; }

    public virtual DbSet<ExhibitStatus> ExhibitStatuses { get; set; }

    public virtual DbSet<ExhibitTechnique> ExhibitTechniques { get; set; }

    public virtual DbSet<Exhibition> Exhibitions { get; set; }

    public virtual DbSet<ExhibitionStatus> ExhibitionStatuses { get; set; }

    public virtual DbSet<ExpositionPlaceType> ExpositionPlaceTypes { get; set; }

    public virtual DbSet<FloorEntity> FloorEntities { get; set; }

    public virtual DbSet<Hall> Halls { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PersonRole> PersonRoles { get; set; }

    public virtual DbSet<PersonType> PersonTypes { get; set; }

    public virtual DbSet<PhotoType> PhotoTypes { get; set; }

    public virtual DbSet<ReceiptAct> ReceiptActs { get; set; }

    public virtual DbSet<ReceiptMethod> ReceiptMethods { get; set; }

    public virtual DbSet<RequiredWorkType> RequiredWorkTypes { get; set; }

    public virtual DbSet<RestorationAct> RestorationActs { get; set; }

    public virtual DbSet<RestorationOrderEntity> RestorationOrderEntities { get; set; }

    public virtual DbSet<RestorationWorkType> RestorationWorkTypes { get; set; }

    public virtual DbSet<ReturnAct> ReturnActs { get; set; }

    public virtual DbSet<RoleEntity> RoleEntities { get; set; }

    public virtual DbSet<Technique> Techniques { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketStatus> TicketStatuses { get; set; }

    public virtual DbSet<WorkLogEntry> WorkLogEntries { get; set; }

    public virtual DbSet<WriteOffAct> WriteOffActs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DBSRV\\vip2025;Database=бобор;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC0794242B14");

            entity.ToTable("Category");

            entity.HasIndex(e => e.Title, "UQ__Category__2CB664DC691B0D84").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Exhibit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exhibit__3214EC07BE6D082A");

            entity.ToTable("Exhibit");

            entity.HasIndex(e => e.InventoryNumber, "UQ__Exhibit__D6D65CC8A429621E").IsUnique();

            entity.Property(e => e.Author)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValue("Неизвестно");
            entity.Property(e => e.CreationDate)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("Неизвестно");
            entity.Property(e => e.InventoryNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OriginPlace)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasDefaultValue("Неизвестно");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitStatusFkNavigation).WithMany(p => p.Exhibits)
                .HasForeignKey(d => d.ExhibitStatusFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Exhibit_Status");

            entity.HasOne(d => d.ReceiptActFkNavigation).WithMany(p => p.Exhibits)
                .HasForeignKey(d => d.ReceiptActFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Exhibit_ReceiptAct");
        });

        modelBuilder.Entity<ExhibitCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitC__3214EC0753445EFF");

            entity.ToTable("ExhibitCategory");

            entity.HasIndex(e => new { e.ExhibitFk, e.CategoryFk }, "uq_ExhibitCategory").IsUnique();

            entity.HasOne(d => d.CategoryFkNavigation).WithMany(p => p.ExhibitCategories)
                .HasForeignKey(d => d.CategoryFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitCategory_Category");

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.ExhibitCategories)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitCategory_Exhibit");
        });

        modelBuilder.Entity<ExhibitMaterial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitM__3214EC070F8D849F");

            entity.ToTable("ExhibitMaterial");

            entity.HasIndex(e => new { e.ExhibitFk, e.MaterialFk }, "uq_ExhibitMaterial").IsUnique();

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.ExhibitMaterials)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitMaterial_Exhibit");

            entity.HasOne(d => d.MaterialFkNavigation).WithMany(p => p.ExhibitMaterials)
                .HasForeignKey(d => d.MaterialFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitMaterial_Material");
        });

        modelBuilder.Entity<ExhibitOnExhibition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitO__3214EC073208316E");

            entity.ToTable("ExhibitOnExhibition");

            entity.HasIndex(e => new { e.ExhibitionFk, e.ExhibitFk }, "uq_ExhibitOnExhibition").IsUnique();

            entity.Property(e => e.LabelData)
                .HasMaxLength(4000)
                .IsUnicode(false);
            entity.Property(e => e.PlaceIdentifier)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.ExhibitOnExhibitions)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EOE_Exhibit");

            entity.HasOne(d => d.ExhibitionFkNavigation).WithMany(p => p.ExhibitOnExhibitions)
                .HasForeignKey(d => d.ExhibitionFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EOE_Exhibition");

            entity.HasOne(d => d.ExpositionPlaceTypeFkNavigation).WithMany(p => p.ExhibitOnExhibitions)
                .HasForeignKey(d => d.ExpositionPlaceTypeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_EOE_PlaceType");
        });

        modelBuilder.Entity<ExhibitPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitP__3214EC07F3D51107");

            entity.ToTable("ExhibitPhoto");

            entity.Property(e => e.FileLink)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.ExhibitPhotos)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitPhoto_Exhibit");

            entity.HasOne(d => d.PhotoTypeFkNavigation).WithMany(p => p.ExhibitPhotos)
                .HasForeignKey(d => d.PhotoTypeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitPhoto_Type");
        });

        modelBuilder.Entity<ExhibitStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitS__3214EC07176A3B56");

            entity.ToTable("ExhibitStatus");

            entity.HasIndex(e => e.Title, "UQ__ExhibitS__2CB664DC32162CD2").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExhibitTechnique>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExhibitT__3214EC07F54670CE");

            entity.ToTable("ExhibitTechnique");

            entity.HasIndex(e => new { e.ExhibitFk, e.TechniqueFk }, "uq_ExhibitTechnique").IsUnique();

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.ExhibitTechniques)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitTechnique_Exhibit");

            entity.HasOne(d => d.TechniqueFkNavigation).WithMany(p => p.ExhibitTechniques)
                .HasForeignKey(d => d.TechniqueFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ExhibitTechnique_Technique");
        });

        modelBuilder.Entity<Exhibition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exhibiti__3214EC0751B1708C");

            entity.ToTable("Exhibition");

            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitionStatusFkNavigation).WithMany(p => p.Exhibitions)
                .HasForeignKey(d => d.ExhibitionStatusFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Exhibition_Status");

            entity.HasOne(d => d.HallFkNavigation).WithMany(p => p.Exhibitions)
                .HasForeignKey(d => d.HallFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Exhibition_Hall");

            entity.HasOne(d => d.ResponsibleCuratorFkNavigation).WithMany(p => p.Exhibitions)
                .HasForeignKey(d => d.ResponsibleCuratorFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Exhibition_Curator");
        });

        modelBuilder.Entity<ExhibitionStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Exhibiti__3214EC075AADDDFE");

            entity.ToTable("ExhibitionStatus");

            entity.HasIndex(e => e.Title, "UQ__Exhibiti__2CB664DC9AD4B973").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ExpositionPlaceType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Expositi__3214EC07B8FD93DF");

            entity.ToTable("ExpositionPlaceType");

            entity.HasIndex(e => e.Title, "UQ__Expositi__2CB664DC1B550B50").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FloorEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FloorEnt__3214EC07FC7CDF37");

            entity.ToTable("FloorEntity");

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hall__3214EC07F9266FAE");

            entity.ToTable("Hall");

            entity.Property(e => e.HallNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.FloorFkNavigation).WithMany(p => p.Halls)
                .HasForeignKey(d => d.FloorFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Hall_Floor");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC073940C778");

            entity.ToTable("Material");

            entity.HasIndex(e => e.Title, "UQ__Material__2CB664DC46980B0A").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Person__3214EC0781A35A15");

            entity.ToTable("Person");

            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.PersonTypeFkNavigation).WithMany(p => p.People)
                .HasForeignKey(d => d.PersonTypeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Person_Type");
        });

        modelBuilder.Entity<PersonRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PersonRo__3214EC07BC627D29");

            entity.ToTable("PersonRole");

            entity.HasIndex(e => new { e.PersonFk, e.RoleFk }, "uq_PersonRole").IsUnique();

            entity.HasOne(d => d.PersonFkNavigation).WithMany(p => p.PersonRoles)
                .HasForeignKey(d => d.PersonFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PersonRole_Person");

            entity.HasOne(d => d.RoleFkNavigation).WithMany(p => p.PersonRoles)
                .HasForeignKey(d => d.RoleFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_PersonRole_Role");
        });

        modelBuilder.Entity<PersonType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PersonTy__3214EC0784F633FC");

            entity.ToTable("PersonType");

            entity.HasIndex(e => e.TypeName, "UQ__PersonTy__D4E7DFA88094238C").IsUnique();

            entity.Property(e => e.TypeName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PhotoType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhotoTyp__3214EC07E58F9683");

            entity.ToTable("PhotoType");

            entity.HasIndex(e => e.Title, "UQ__PhotoTyp__2CB664DC19CCEA48").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ReceiptAct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReceiptA__3214EC07B1762B56");

            entity.ToTable("ReceiptAct");

            entity.HasOne(d => d.ReceiptMethodFkNavigation).WithMany(p => p.ReceiptActs)
                .HasForeignKey(d => d.ReceiptMethodFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceiptAct_Method");

            entity.HasOne(d => d.ResponsiblePersonFkNavigation).WithMany(p => p.ReceiptActResponsiblePersonFkNavigations)
                .HasForeignKey(d => d.ResponsiblePersonFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceiptAct_Responsible");

            entity.HasOne(d => d.SourceFkNavigation).WithMany(p => p.ReceiptActSourceFkNavigations)
                .HasForeignKey(d => d.SourceFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReceiptAct_Source");
        });

        modelBuilder.Entity<ReceiptMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReceiptM__3214EC07C791EC7C");

            entity.ToTable("ReceiptMethod");

            entity.HasIndex(e => e.Title, "UQ__ReceiptM__2CB664DC94CBCADD").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RequiredWorkType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Required__3214EC07D9F82EBE");

            entity.ToTable("RequiredWorkType");

            entity.HasIndex(e => new { e.RestorationOrderFk, e.WorkTypeFk }, "uq_RequiredWorkType").IsUnique();

            entity.HasOne(d => d.RestorationOrderFkNavigation).WithMany(p => p.RequiredWorkTypes)
                .HasForeignKey(d => d.RestorationOrderFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RequiredWorkType_Order");

            entity.HasOne(d => d.WorkTypeFkNavigation).WithMany(p => p.RequiredWorkTypes)
                .HasForeignKey(d => d.WorkTypeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RequiredWorkType_WorkType");
        });

        modelBuilder.Entity<RestorationAct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restorat__3214EC07318EFBB0");

            entity.ToTable("RestorationAct");

            entity.Property(e => e.FinalReport)
                .HasMaxLength(4000)
                .IsUnicode(false);

            entity.HasOne(d => d.RestorationOrderFkNavigation).WithMany(p => p.RestorationActs)
                .HasForeignKey(d => d.RestorationOrderFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RestorationAct_Order");
        });

        modelBuilder.Entity<RestorationOrderEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restorat__3214EC073EAA3AAC");

            entity.ToTable("RestorationOrderEntity");

            entity.HasIndex(e => e.OrderNumber, "UQ__Restorat__CAC5E74374BF9B79").IsUnique();

            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ReasonDirection)
                .HasMaxLength(4000)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.RestorationOrderEntities)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RestorationOrder_Exhibit");

            entity.HasOne(d => d.InitiatorFkNavigation).WithMany(p => p.RestorationOrderEntityInitiatorFkNavigations)
                .HasForeignKey(d => d.InitiatorFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RestorationOrder_Initiator");

            entity.HasOne(d => d.RestorerFkNavigation).WithMany(p => p.RestorationOrderEntityRestorerFkNavigations)
                .HasForeignKey(d => d.RestorerFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_RestorationOrder_Restorer");
        });

        modelBuilder.Entity<RestorationWorkType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Restorat__3214EC0784DE718C");

            entity.ToTable("RestorationWorkType");

            entity.HasIndex(e => e.Title, "UQ__Restorat__2CB664DCF28F34C0").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ReturnAct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReturnAc__3214EC07F240CF73");

            entity.ToTable("ReturnAct");

            entity.HasOne(d => d.RestorationOrderFkNavigation).WithMany(p => p.ReturnActs)
                .HasForeignKey(d => d.RestorationOrderFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ReturnAct_Order");
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoleEnti__3214EC077796C3EF");

            entity.ToTable("RoleEntity");

            entity.HasIndex(e => e.Title, "UQ__RoleEnti__2CB664DC39D2FA9B").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Technique>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Techniqu__3214EC07A2609709");

            entity.ToTable("Technique");

            entity.HasIndex(e => e.Title, "UQ__Techniqu__2CB664DC6BBDDCE5").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket__3214EC07482822BB");

            entity.ToTable("Ticket");

            entity.HasIndex(e => e.TicketNumber, "UQ__Ticket__CBED06DA290CBA26").IsUnique();

            entity.Property(e => e.TicketNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitionFkNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ExhibitionFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ticket_Exhibition");

            entity.HasOne(d => d.TicketStatusFkNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.TicketStatusFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Ticket_Status");
        });

        modelBuilder.Entity<TicketStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketSt__3214EC07CBA20CC9");

            entity.ToTable("TicketStatus");

            entity.HasIndex(e => e.Title, "UQ__TicketSt__2CB664DC3605D4DC").IsUnique();

            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WorkLogEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkLogE__3214EC07E373AB07");

            entity.ToTable("WorkLogEntry");

            entity.HasOne(d => d.RestorationOrderFkNavigation).WithMany(p => p.WorkLogEntries)
                .HasForeignKey(d => d.RestorationOrderFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_WorkLogEntry_Order");

            entity.HasOne(d => d.WorkTypeFkNavigation).WithMany(p => p.WorkLogEntries)
                .HasForeignKey(d => d.WorkTypeFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_WorkLogEntry_WorkType");
        });

        modelBuilder.Entity<WriteOffAct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WriteOff__3214EC076C7338B0");

            entity.ToTable("WriteOffAct");

            entity.Property(e => e.WriteOffReason)
                .HasMaxLength(4000)
                .IsUnicode(false);

            entity.HasOne(d => d.ExhibitFkNavigation).WithMany(p => p.WriteOffActs)
                .HasForeignKey(d => d.ExhibitFk)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_WriteOffAct_Exhibit");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

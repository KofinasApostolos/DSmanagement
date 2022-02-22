using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebApi.Models
{
    public partial class DanceSchoolContext : DbContext
    {
        public DanceSchoolContext()
        {
        }


        public DanceSchoolContext(DbContextOptions<DanceSchoolContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Lessons> Lessons { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Subscriptions> Subscriptions { get; set; }
        public virtual DbSet<TeachingProgram> TeachingProgram { get; set; }
        public virtual DbSet<TeachingProgramTemp> TeachingProgramTemp { get; set; }
        public virtual DbSet<TempRegisters> TempRegisters { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserSubscriptions> UserSubscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("DESKTOP-VRAG6PD\\SQLEXPRESS;Database=DanceSchool;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lessons>(entity =>
            {
                entity.HasKey(e => e.Lessonid);

                entity.Property(e => e.Lessonid)
                    .HasColumnName("lessonid")
                    .HasMaxLength(4)
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .IsRequired()
                    .HasColumnName("descr")
                    .HasMaxLength(3000);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.Lesson)
                    .IsRequired()
                    .HasColumnName("lesson")
                    .HasMaxLength(100);

                entity.Property(e => e.PublicId).HasMaxLength(500);

                entity.Property(e => e.Teacherid).HasColumnName("teacherid");

                entity.Property(e => e.Utubeurl)
                    .IsRequired()
                    .HasColumnName("utubeurl")
                    .HasMaxLength(500);

                entity.HasOne(d => d.Teacher)
                    .WithMany(p => p.Lessons)
                    .HasForeignKey(d => d.Teacherid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lessons_User");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.RoleCode);

                entity.Property(e => e.RoleCode)
                    .HasColumnName("role_code")
                    .ValueGeneratedNever();

                entity.Property(e => e.RoleDescr)
                    .IsRequired()
                    .HasColumnName("role_descr")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Subscriptions>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.Discprice)
                    .IsRequired()
                    .HasColumnName("discprice")
                    .HasMaxLength(5)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Duration)
                    .IsRequired()
                    .HasColumnName("duration")
                    .HasMaxLength(2);

                entity.Property(e => e.Lessonid)
                    .IsRequired()
                    .HasColumnName("lessonid")
                    .HasMaxLength(4);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnName("price")
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<TeachingProgram>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.Dayofweek).HasColumnName("dayofweek");

                entity.Property(e => e.Lessonend)
                    .IsRequired()
                    .HasColumnName("lessonend")
                    .HasMaxLength(10);

                entity.Property(e => e.Lessonid)
                    .IsRequired()
                    .HasColumnName("lessonid")
                    .HasMaxLength(4);

                entity.Property(e => e.Lessonstart)
                    .IsRequired()
                    .HasColumnName("lessonstart")
                    .HasMaxLength(10);

                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.TeachingProgram)
                    .HasForeignKey(d => d.Lessonid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeachingProgram_Lessons");
            });

            modelBuilder.Entity<TeachingProgramTemp>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.Dayofweek).HasColumnName("dayofweek");

                entity.Property(e => e.Lessonend)
                    .IsRequired()
                    .HasColumnName("lessonend")
                    .HasMaxLength(10);

                entity.Property(e => e.Lessonid)
                    .IsRequired()
                    .HasColumnName("lessonid")
                    .HasMaxLength(4);

                entity.Property(e => e.Lessonstart)
                    .IsRequired()
                    .HasColumnName("lessonstart")
                    .HasMaxLength(10);

                entity.Property(e => e.Userid).HasColumnName("userid");
            });

            modelBuilder.Entity<TempRegisters>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Lessonid)
                    .HasColumnName("lessonid")
                    .HasMaxLength(4);

                entity.Property(e => e.Teachingprogramid).HasColumnName("teachingprogramid");

                entity.Property(e => e.Userid).HasColumnName("userid");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Area).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.DateofBirth).HasColumnType("date");

                entity.Property(e => e.Descr).HasMaxLength(3000);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.Phonenumber).HasMaxLength(50);

                entity.Property(e => e.Provider).HasMaxLength(50);

                entity.Property(e => e.PublicId).HasMaxLength(500);

                entity.Property(e => e.Street).HasMaxLength(50);

                entity.Property(e => e.Username).HasMaxLength(50);
            });

            modelBuilder.Entity<UserSubscriptions>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Dayid)
                    .IsRequired()
                    .HasColumnName("dayid")
                    .HasMaxLength(5);

                entity.Property(e => e.Discount).HasColumnName("discount");

                entity.Property(e => e.Duration).HasColumnName("duration");

                entity.Property(e => e.Lessonid)
                    .IsRequired()
                    .HasColumnName("lessonid")
                    .HasMaxLength(4);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnName("price")
                    .HasMaxLength(5);

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Userid).HasColumnName("userid");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSubscriptions)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSubscriptions_User");
            });
        }
    }
}

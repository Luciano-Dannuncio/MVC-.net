using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MVCnetcore.Models.DB
{
    public partial class AlkemyChallengeCDBContext : DbContext
    {
        public AlkemyChallengeCDBContext()
        {
        }

        public AlkemyChallengeCDBContext(DbContextOptions<AlkemyChallengeCDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Inscriptions> Inscriptions { get; set; }
        public virtual DbSet<Subjects> Subjects { get; set; }
        public virtual DbSet<Teachers> Teachers { get; set; }
        public virtual DbSet<Users> Users { get; set; }   
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<RolesOperations> RolesOperations { get; set; }
        public virtual DbSet<Modules> Modules { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB; Database=AlkemyChallengeC#DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inscriptions>(entity =>
            {
                entity.HasKey(e => e.IdInscriptions)
                    .HasName("PK__tmp_ms_x__A4E60854725DD580");

                entity.Property(e => e.IdInscriptions).HasColumnName("Id_Inscriptions");

                entity.Property(e => e.ActiveInscriptions).HasColumnName("Active_Inscriptions");

                entity.Property(e => e.IdClassesInscriptions).HasColumnName("Id_Classes_Inscriptions");

                entity.Property(e => e.IdUsersInscriptions).HasColumnName("Id_Users_Inscriptions");

                entity.Property(e => e.IdSubjectsInscriptions).HasColumnName("Id_Subjects_Inscriptions");
            });

            modelBuilder.Entity<Subjects>(entity =>
            {
                entity.HasKey(e => e.IdSubjects)
                    .HasName("PK__tmp_ms_x__C6D5BFE72267ACC2");

                entity.Property(e => e.IdSubjects).HasColumnName("Id_Subjects");

                entity.Property(e => e.IdHeadTeacher).HasColumnName("Id_Head_Teacher");

                entity.Property(e => e.ActiveSubjects).HasColumnName("Active_Subjects");

                entity.Property(e => e.NameSubjects)
                    .HasColumnName("Name_Subjects")
                    .HasMaxLength(100);

                entity.Property(e => e.TimeSubjects)
                    .HasColumnName("Time_Subjects")
                    .HasColumnType("time(4)");
            });

            modelBuilder.Entity<Teachers>(entity =>
            {
                entity.HasKey(e => e.IdTeachers)
                    .HasName("PK__tmp_ms_x__54C8C115838FEF2C");

                entity.Property(e => e.IdTeachers).HasColumnName("Id_Teachers");

                entity.Property(e => e.ActiveTeachers).HasColumnName("Active_Teachers");

                entity.Property(e => e.DniTeachers).HasColumnName("DNI_Teachers");

                entity.Property(e => e.LastNameTeachers)
                    .HasColumnName("Last_Name_Teachers")
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.NameTeachers)
                    .HasColumnName("Name_Teachers")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.IdUsers)
                    .HasName("PK__tmp_ms_x__FB0668EE93692FF5");

                entity.Property(e => e.IdUsers).HasColumnName("Id_Users");

                entity.Property(e => e.ActiveUsers).HasColumnName("Active_Users");

                entity.Property(e => e.EmailUsers)
                    .HasColumnName("Email_Users")
                    .HasMaxLength(100);

                entity.Property(e => e.NameUsers)
                    .HasColumnName("Name_Users")
                    .HasMaxLength(100);

                entity.Property(e => e.PasswordUsers)
                    .IsRequired()
                    .HasColumnName("Password_Users")
                    .HasMaxLength(200);

                entity.Property(e => e.IdRoles)
                    .IsRequired()
                    .HasColumnName("Id_Roles");
                   


            });

            modelBuilder.Entity<RolesOperations>(entity =>
            {
                entity.HasKey(e => e.IdRolesOperations);
                    
                entity.Property(e => e.IdRolesOperations).HasColumnName("Id_RolesOperations");

                entity.Property(e => e.IdRoles)
                .HasColumnName("Id_Roles")
                .IsRequired();

                entity.Property(e => e.IdOperations)
                .HasColumnName("Id_Operations")
                .IsRequired();

            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.IdRoles);

                entity.Property(e => e.IdRoles).HasColumnName("Id_Roles");

                entity.Property(e => e.NameRoles)
                .HasColumnName("Name_Roles")
                .HasMaxLength(50);

            });

            modelBuilder.Entity<Operations>(entity =>
            {
                entity.HasKey(e => e.IdOperations);

                entity.Property(e => e.IdOperations).HasColumnName("Id_Operations");

                entity.Property(e => e.NameOperations)
                .HasColumnName("Name_Operations")
                .HasMaxLength(50);

                entity.Property(e => e.IdModules)
                .HasColumnName("Id_Modules")
                .IsRequired();


            });

            modelBuilder.Entity<Modules>(entity =>
            {
                entity.HasKey(e => e.IdModules);

                entity.Property(e => e.IdModules).HasColumnName("Id_Modules");

                entity.Property(e => e.NameModules)
                .HasColumnName("Name_Modules")
                .HasMaxLength(50);
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.IdClasses);

                entity.Property(e => e.IdClasses).HasColumnName("Id_Classes");

                entity.Property(e => e.IdSubjects).HasColumnName("Id_Subjects");

                entity.Property(e => e.IdTeachers).HasColumnName("Id_Teachers");

                entity.Property(e => e.TimeClasses).HasColumnName("Time_Classes");

                entity.Property(e => e.MaxStudentClasses).HasColumnName("Max_Student_Classes");

                entity.Property(e => e.ActiveClasses).HasColumnName("Active_Classes");

                entity.Property(e => e.MaxCapacityClasses).HasColumnName("Max_Capacity_Classes");

                entity.Property(e => e.ClassroomClasses).HasColumnName("Classroom_Classes");

            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace sheift.Models
{
    public partial class shieftContext : DbContext
    {
      

        public shieftContext(DbContextOptions<shieftContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<DepartmentManger> DepartmentMangers { get; set; } = null!;
        public virtual DbSet<DepartmentsDataWithmanger> DepartmentsDataWithmangers { get; set; } = null!;
        public virtual DbSet<MangerUser> MangerUsers { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Shieft> Shiefts { get; set; } = null!;
        public virtual DbSet<ShiftDetail> ShiftDetails { get; set; } = null!;
        public virtual DbSet<ShiftType> ShiftTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepId);

                entity.ToTable("departments");

                entity.Property(e => e.DepId).HasColumnName("dep_id");

                entity.Property(e => e.DepName)
                    .HasMaxLength(50)
                    .HasColumnName("dep_name");
            });

            modelBuilder.Entity<DepartmentManger>(entity =>
            {
                entity.ToTable("department_manger");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DepId).HasColumnName("dep_id");

                entity.Property(e => e.MangerId).HasColumnName("manger_id");
            });

            modelBuilder.Entity<DepartmentsDataWithmanger>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("departments_data_withmanger");

                entity.Property(e => e.DepId).HasColumnName("dep_id");

                entity.Property(e => e.DepName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dep_name");

                entity.Property(e => e.MangerId).HasColumnName("manger_id");
            });

            modelBuilder.Entity<MangerUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("manger_users");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.EmployeeNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("employee_number");

                entity.Property(e => e.EntryTelephone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("entry_telephone");

                entity.Property(e => e.MangerId).HasColumnName("manger_id");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("telephone");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Shieft>(entity =>
            {
                entity.HasKey(e => e.ShiftId);

                entity.ToTable("shiefts");

                entity.Property(e => e.ShiftId).HasColumnName("shift_id");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.Date)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("date");

                entity.Property(e => e.ShiftTypeId).HasColumnName("shift_type_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

            modelBuilder.Entity<ShiftDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("shift_details");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.Property(e => e.AdminName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("admin_name");

                entity.Property(e => e.Date)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("date");

                entity.Property(e => e.DepName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("dep_name");

                entity.Property(e => e.ShiftId).HasColumnName("shift_id");

                entity.Property(e => e.ShiftName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("shift_name");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<ShiftType>(entity =>
            {
                entity.ToTable("shift_type");

                entity.Property(e => e.ShiftTypeId).HasColumnName("shift_type_id");

                entity.Property(e => e.ShiftName)
                    .HasMaxLength(50)
                    .HasColumnName("shift_name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.DeptId).HasColumnName("dept_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.EmployeeNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("employee_number");

                entity.Property(e => e.EntryTelephone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("entry_telephone");

                entity.Property(e => e.LoginDate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("login_date");

                entity.Property(e => e.Password)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.Telephone)
                    .HasMaxLength(50)
                    .HasColumnName("telephone");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("user_name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

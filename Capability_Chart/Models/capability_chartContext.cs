using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Capability_Chart.Models
{
    public partial class capability_chartContext : DbContext
    {
        public capability_chartContext()
        {
        }

        public capability_chartContext(DbContextOptions<capability_chartContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AssignedSkill> AssignedSkill { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<Teams> Teams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignedSkill>(entity =>
            {
                entity.ToTable("assigned_skill", "capability_chart");

                entity.HasIndex(e => e.EmpId)
                    .HasName("FK_Emp_Id");

                entity.HasIndex(e => e.SkillId)
                    .HasName("FK_Skill_Id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(9)");

                entity.Property(e => e.AssignedScore)
                    .HasColumnName("assigned_score")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.EmpId)
                    .HasColumnName("emp_id")
                    .HasColumnType("int(9)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SkillId)
                    .HasColumnName("skill_id")
                    .HasColumnType("int(9)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.AssignedSkill)
                    .HasForeignKey(d => d.EmpId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Emp_Id");

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.AssignedSkill)
                    .HasForeignKey(d => d.SkillId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Skill_Id");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employee", "capability_chart");

                entity.HasIndex(e => e.AssignedTeam)
                    .HasName("FK_Assigned_Team");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(9)");

                entity.Property(e => e.AssignedTeam)
                    .HasColumnName("assigned_team")
                    .HasColumnType("int(9)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnName("firstname")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasColumnName("lastname")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.AssignedTeamNavigation)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.AssignedTeam)
                    .HasConstraintName("FK_Assigned_Team");
            });

            modelBuilder.Entity<Skills>(entity =>
            {
                entity.ToTable("skills", "capability_chart");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(9)");
                    //.ValueGeneratedOnAdd();


                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Teams>(entity =>
            {
                entity.ToTable("teams", "capability_chart");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(9)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });
        }
    }
}

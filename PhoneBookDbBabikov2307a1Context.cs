using System;
using System.Collections.Generic;
using Lab11_Navigation.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab11_Navigation;

public partial class PhoneBookDbBabikov2307a1Context : DbContext
{
    public PhoneBookDbBabikov2307a1Context()
    {
    }

    public PhoneBookDbBabikov2307a1Context(DbContextOptions<PhoneBookDbBabikov2307a1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=dbsrv\\gor2025;Initial Catalog=PhoneBookDB_Babikov_2307a1;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contacts__3214EC0711252B42");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

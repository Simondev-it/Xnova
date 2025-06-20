using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Xnova.Models;

public partial class XnovaContext : DbContext
{
    public XnovaContext()
    {
    }

    public XnovaContext(DbContextOptions<XnovaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingSlot> BookingSlots { get; set; }

    public virtual DbSet<Chatbox> Chatboxes { get; set; }

    public virtual DbSet<FavoriteField> FavoriteFields { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<Friend> Friends { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<SaveField> SaveFields { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserInvitation> UserInvitations { get; set; }

    public virtual DbSet<UserVoucher> UserVouchers { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }
    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=LAPTOP-IVTKGI7B;Database=Xnova;User Id=sa;Password=Duonggaming1@;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC078FC7E0AE");

            entity.ToTable("Booking");

            entity.Property(e => e.CurrentDate).HasColumnType("datetime");
            entity.Property(e => e.Feedback).HasMaxLength(255);

            entity.HasOne(d => d.Field).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__Booking__FieldId__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Booking__UserId__4BAC3F29");
        });

        modelBuilder.Entity<BookingSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookingS__3214EC071129D345");

            entity.ToTable("BookingSlot");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingSlots)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingSl__Booki__4F7CD00D");

            entity.HasOne(d => d.Slot).WithMany(p => p.BookingSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("FK__BookingSl__SlotI__5070F446");
        });

        modelBuilder.Entity<Chatbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chatbox__3214EC0726D3C479");

            entity.ToTable("Chatbox");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Chatboxes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Chatbox__UserId__5629CD9C");
        });

        modelBuilder.Entity<FavoriteField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favorite__3214EC07519CD2E1");

            entity.ToTable("FavoriteField");

            entity.Property(e => e.SetDate).HasColumnType("datetime");

            entity.HasOne(d => d.Field).WithMany(p => p.FavoriteFields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__FavoriteF__Field__45F365D3");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteFields)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__FavoriteF__UserI__44FF419A");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Field__3214EC077F2EA036");

            entity.ToTable("Field");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Type).WithMany(p => p.Fields)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK__Field__TypeId__412EB0B6");

            entity.HasOne(d => d.Venue).WithMany(p => p.Fields)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Field__VenueId__4222D4EF");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friend__3214EC07EFFCD493");

            entity.ToTable("Friend");

            entity.HasOne(d => d.FriendNavigation).WithMany(p => p.FriendFriendNavigations)
                .HasForeignKey(d => d.FriendId)
                .HasConstraintName("FK__Friend__FriendId__619B8048");

            entity.HasOne(d => d.Relationship).WithMany(p => p.Friends)
                .HasForeignKey(d => d.RelationshipId)
                .HasConstraintName("FK__Friend__Relation__6383C8BA");

            entity.HasOne(d => d.User).WithMany(p => p.FriendUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Friend__UserId__628FA481");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Image__3214EC07CEF68E42");

            entity.ToTable("Image");

            entity.Property(e => e.Link).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Venue).WithMany(p => p.Images)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("FK__Image__VenueId__3E52440B");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invitati__3214EC07A15CB1FF");

            entity.ToTable("Invitation");

            entity.Property(e => e.KindOfSport).HasMaxLength(255);
            entity.Property(e => e.Latitude).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Longitude).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Standard).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Invitatio__Booki__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Invitatio__UserI__6C190EBB");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Message__3214EC076F6023EA");

            entity.ToTable("Message");

            entity.Property(e => e.Content).HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Chatbox).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatboxId)
                .HasConstraintName("FK__Message__Chatbox__59063A47");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC0705DAB219");

            entity.ToTable("Payment");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Method).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.Response).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Payment__Booking__534D60F1");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Relation__3214EC07F3925E12");

            entity.ToTable("Relationship");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<SaveField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SaveFiel__3214EC07738F951B");

            entity.ToTable("SaveField");

            entity.Property(e => e.SetDate).HasColumnType("datetime");

            entity.HasOne(d => d.Field).WithMany(p => p.SaveFields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__SaveField__Field__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.SaveFields)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SaveField__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Slot__3214EC074265FF70");

            entity.ToTable("Slot");

            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Field).WithMany(p => p.Slots)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK__Slot__FieldId__48CFD27E");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Type__3214EC07EA15F604");

            entity.ToTable("Type");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC079271DCC1");

            entity.ToTable("User");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        modelBuilder.Entity<UserInvitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserInvi__3214EC071B67964F");

            entity.ToTable("UserInvitation");

            entity.Property(e => e.JoinDate).HasColumnType("datetime");

            entity.HasOne(d => d.Invitation).WithMany(p => p.UserInvitations)
                .HasForeignKey(d => d.InvitationId)
                .HasConstraintName("FK__UserInvit__Invit__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.UserInvitations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserInvit__UserI__6FE99F9F");
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserVouc__3214EC070F17191A");

            entity.ToTable("UserVoucher");

            entity.Property(e => e.ReceiveDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__UserVouch__UserI__68487DD7");

            entity.HasOne(d => d.Voucher).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK__UserVouch__Vouch__693CA210");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Venue__3214EC073E02CABA");

            entity.ToTable("Venue");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Contact).HasMaxLength(255);
            entity.Property(e => e.Latitude).HasMaxLength(255);
            entity.Property(e => e.Longitude).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Venues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Venue__UserId__3B75D760");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Voucher__3214EC07A142C755");

            entity.ToTable("Voucher");

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

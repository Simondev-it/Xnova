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
        => optionsBuilder.UseNpgsql(GetConnectionString("DefaultConnection"));


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=LAPTOP-IVTKGI7B;Database=Xnova;User Id=sa;Password=Duonggaming1@;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_booking");
            entity.ToTable("booking");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Feedback).HasColumnName("feedback").HasMaxLength(255);
            entity.Property(e => e.CurrentDate).HasColumnName("currentdate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FieldId).HasColumnName("fieldid");

            entity.HasOne(d => d.Field).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("booking_fieldid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("booking_userid_fkey");
        });

        modelBuilder.Entity<BookingSlot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_bookingslot");
            entity.ToTable("bookingslot");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");
            entity.Property(e => e.SlotId).HasColumnName("slotid");

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingSlots)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("bookingslot_bookingid_fkey");

            entity.HasOne(d => d.Slot).WithMany(p => p.BookingSlots)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("bookingslot_slotid_fkey");
        });

        modelBuilder.Entity<Chatbox>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_chatbox");
            entity.ToTable("chatbox");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Chatboxes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("chatbox_userid_fkey");
        });

        modelBuilder.Entity<FavoriteField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_favoritefield");
            entity.ToTable("favoritefield");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SetDate).HasColumnName("setdate");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FieldId).HasColumnName("fieldid");

            entity.HasOne(d => d.Field).WithMany(p => p.FavoriteFields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("favoritefield_fieldid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.FavoriteFields)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("favoritefield_userid_fkey");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_field");
            entity.ToTable("field");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TypeId).HasColumnName("typeid");
            entity.Property(e => e.VenueId).HasColumnName("venueid");

            entity.HasOne(d => d.Type).WithMany(p => p.Fields)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("field_typeid_fkey");

            entity.HasOne(d => d.Venue).WithMany(p => p.Fields)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("field_venueid_fkey");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_friend");
            entity.ToTable("friend");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FriendId).HasColumnName("friendid");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.RelationshipId).HasColumnName("relationshipid");

            entity.HasOne(d => d.FriendNavigation).WithMany(p => p.FriendFriendNavigations)
                .HasForeignKey(d => d.FriendId)
                .HasConstraintName("friend_friendid_fkey");

            entity.HasOne(d => d.Relationship).WithMany(p => p.Friends)
                .HasForeignKey(d => d.RelationshipId)
                .HasConstraintName("friend_relationshipid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.FriendUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("friend_userid_fkey");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_image");
            entity.ToTable("image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Link).HasColumnName("link").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.VenueId).HasColumnName("venueid");

            entity.HasOne(d => d.Venue).WithMany(p => p.Images)
                .HasForeignKey(d => d.VenueId)
                .HasConstraintName("image_venueid_fkey");
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_invitation");
            entity.ToTable("invitation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Booked).HasColumnName("booked");
            entity.Property(e => e.JoiningCost).HasColumnName("joiningcost");
            entity.Property(e => e.TotalPlayer).HasColumnName("totalplayer");
            entity.Property(e => e.AvailablePlayer).HasColumnName("availableplayer");
            entity.Property(e => e.Standard).HasColumnName("standard").HasMaxLength(255);
            entity.Property(e => e.KindOfSport).HasColumnName("kindofsport").HasMaxLength(255);
            entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(255);
            entity.Property(e => e.Longitude).HasColumnName("longitude").HasMaxLength(255);
            entity.Property(e => e.Latitude).HasColumnName("latitude").HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.StartTime).HasColumnName("starttime");
            entity.Property(e => e.EndTime).HasColumnName("endtime");
            entity.Property(e => e.PostingDate).HasColumnName("postingdate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");

            entity.HasOne(d => d.Booking).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("invitation_bookingid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("invitation_userid_fkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_message");
            entity.ToTable("message");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content").HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ChatboxId).HasColumnName("chatboxid");

            entity.HasOne(d => d.Chatbox).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatboxId)
                .HasConstraintName("message_chatboxid_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_payment");
            entity.ToTable("payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Method).HasColumnName("method").HasMaxLength(255);
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Note).HasColumnName("note").HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Response).HasColumnName("response").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.BookingId).HasColumnName("bookingid");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("payment_bookingid_fkey");
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_relationship");
            entity.ToTable("relationship");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
        });

        modelBuilder.Entity<SaveField>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_savefield");
            entity.ToTable("savefield");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SetDate).HasColumnName("setdate");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.FieldId).HasColumnName("fieldid");

            entity.HasOne(d => d.Field).WithMany(p => p.SaveFields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("savefield_fieldid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.SaveFields)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("savefield_userid_fkey");
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_slot");
            entity.ToTable("slot");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.StartTime).HasColumnName("starttime");
            entity.Property(e => e.EndTime).HasColumnName("endtime");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.FieldId).HasColumnName("fieldid");

            entity.HasOne(d => d.Field).WithMany(p => p.Slots)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("slot_fieldid_fkey");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_types");
            entity.ToTable("types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_users");
            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(255);
            entity.Property(e => e.Image).HasColumnName("image").HasMaxLength(255);
            entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasColumnName("phonenumber").HasMaxLength(20);
            entity.Property(e => e.Point).HasColumnName("point");
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<UserInvitation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_userinvitation");
            entity.ToTable("userinvitation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JoinDate).HasColumnName("joindate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.InvitationId).HasColumnName("invitationid");

            entity.HasOne(d => d.Invitation).WithMany(p => p.UserInvitations)
                .HasForeignKey(d => d.InvitationId)
                .HasConstraintName("userinvitation_invitationid_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserInvitations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("userinvitation_userid_fkey");
        });

        modelBuilder.Entity<UserVoucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_uservoucher");
            entity.ToTable("uservoucher");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReceiveDate).HasColumnName("receivedate");
            entity.Property(e => e.UserId).HasColumnName("userid");
            entity.Property(e => e.VoucherId).HasColumnName("voucherid");

            entity.HasOne(d => d.User).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("uservoucher_userid_fkey");

            entity.HasOne(d => d.Voucher).WithMany(p => p.UserVouchers)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("uservoucher_voucherid_fkey");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_venue");
            entity.ToTable("venue");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(255);
            entity.Property(e => e.Longitude).HasColumnName("longitude").HasMaxLength(255);
            entity.Property(e => e.Latitude).HasColumnName("latitude").HasMaxLength(255);
            entity.Property(e => e.Contact).HasColumnName("contact").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userid");

            entity.HasOne(d => d.User).WithMany(p => p.Venues)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("venue_userid_fkey");
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_voucher");
            entity.ToTable("voucher");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(255);
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.MinEffect).HasColumnName("mineffect");
            entity.Property(e => e.MaxEffect).HasColumnName("maxeffect");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

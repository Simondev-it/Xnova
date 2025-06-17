using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xnova.Models;
using Xnova.Repositories;

namespace Xnova
{
    public class UnitOfWork
    {
        private XnovaContext _context;
        private BookingRepository _bookingRepository;
        private BookingSlotRepository _bookingSlotRepository;
        private FavoriteFieldRepository _favoriteFieldRepository;
        private FieldRepository _fieldRepository;
        private ImageRepository _imageRepository;
        private PaymentRepository _paymentRepository;
        private SlotRepository _slotRepository;
        private TypeRepository _typeRepository;
        private UserRepository _userRepository;
        private VenueRepository _venueRepository;

        public UnitOfWork(XnovaContext context)
        {
            _context = context;
        }
        public BookingRepository BookingRepository
        {
            get { return _bookingRepository ??= new BookingRepository(_context); }
        }
        public BookingSlotRepository BookingSlotRepository
        {
            get { return _bookingSlotRepository ??= new BookingSlotRepository(_context); }

        }
        public FavoriteFieldRepository FavoriteFieldRepository
        {
            get { return _favoriteFieldRepository ??= new FavoriteFieldRepository(_context);}
        }
        public FieldRepository FieldRepository
        {
            get { return _fieldRepository ??= new FieldRepository(_context); }
        }
        public ImageRepository ImageRepository
        { 
            get { return _imageRepository ??= new ImageRepository(_context); }
        }
        public PaymentRepository PaymentRepository
        {
            get { return _paymentRepository ??= new PaymentRepository(_context); }
        }
        public SlotRepository SlotRepository 
        {
            get { return _slotRepository ??= new SlotRepository(_context); }

        }
        public TypeRepository TypeRepository
        {
            get { return _typeRepository ??= new TypeRepository(_context); }
        }
        public UserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(_context); }
        }
        public VenueRepository VenueRepository
        {
            get { return _venueRepository ??= new VenueRepository(_context); }
        }
    }
}

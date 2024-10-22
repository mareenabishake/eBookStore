using System;
using System.Security.Cryptography;
using System.Text;
using eBookStore.Data;
using eBookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace eBookStore.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> ValidateAdminCredentials(string email, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin == null) return false;
            return VerifyPassword(password, admin.Password);
        }

        public async Task<bool> ValidateCustomerCredentials(string email, string password)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
            if (customer == null) return false;
            return VerifyPassword(password, customer.Password);
        }

        public async Task<IdentityUser> ValidateUserCredentials(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword == storedPassword;
        }

        public string HashPassword(string password)
        {
            return _userManager.PasswordHasher.HashPassword(null, password);
        }
    }
}

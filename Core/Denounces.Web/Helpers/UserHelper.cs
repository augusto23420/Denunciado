﻿using System;

namespace Denounces.Web.Helpers
{
    using Domain.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class UserHelper : IUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public string RemoveCharacters(string param)
        {
            param = param.Replace("(", "");
            param = param.Replace(")", "");
            param = param.Replace("-", "");
            param = param.Replace(" ", "");
            return param;
        }

        public async Task<bool> DisabledUserAsync(ApplicationUser user)
        {
            try
            {
                var lockoutEndDate = new DateTime(2999, 01, 01);
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, lockoutEndDate);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public async Task<IdentityResult> AddUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateEmailAsync(ApplicationUser user, string email, string token)
        {
            return await _userManager.ChangeEmailAsync(user, email, token);
        }

        public async Task<SignInResult> ValidatePasswordAsync(ApplicationUser user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                false);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<string> GenerateEmailChangeTokenAsync(ApplicationUser user, string email)
        {
            return await _userManager.GenerateChangeEmailTokenAsync(user, email);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<ApplicationUser> AddClaim(ApplicationUser user, Claim claim)
        {
            await _userManager.AddClaimAsync(user, claim);
            return user;
        }

        public async Task<List<ApplicationUser>> GetAllUsersNoLockoutAsync(long ownerId)
        {
            //.Where(p => p.Owner.Id == ownerId
            //               && (p.LockoutEnd == null || p.LockoutEnd <= DateTime.Now))
            return await _userManager.Users
                .Where(p => (p.LockoutEnd == null || p.LockoutEnd <= DateTime.Now))
                // .Include(p => p.Owner)
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Lastname)
                .ToListAsync();
        }
        public async Task<List<ApplicationUser>> GetAllUsersAsync(long ownerId)
        {
            return await _userManager.Users//.Where(p => p.Shop.Owner.Id == ownerId)
                                           // .Include(p => p.Owner)
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Lastname)
                .ToListAsync();
        }

        public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            await _userManager.DeleteAsync(user);
        }


    }
}

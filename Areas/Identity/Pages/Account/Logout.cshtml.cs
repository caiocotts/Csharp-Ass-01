// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Assignment01.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Assignment01.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly UserManager<User> _userManager;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger,  UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager  = userManager;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            
            var user = await _userManager.GetUserAsync(User);
            
            if (user != null) {
                // FIX 2: Changed message to "logged OUT"
                _logger.LogInformation("User logged OUT: {FullName} ({Email}) ", 
                    user.FullName, user.Email);
            }
            else {
                // Fallback if user object is missing
                _logger.LogInformation("User logged OUT: {Name} at ", User.Identity?.Name);
            }
            
            await _signInManager.SignOutAsync();
            
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}

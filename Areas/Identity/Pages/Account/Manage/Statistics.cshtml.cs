// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Chat.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;

namespace Chat.Areas.Identity.Pages.Account.Manage
{
    public class Statistics : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private ApplicationDbContext _dbContext;
        public int TotalUsedTokens { get; set; }
        public string CreditGranted { get; set; }
        public string Cost { get; set; }
        public Statistics(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, 
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            ClaimsPrincipal currentUser = _signInManager.Context.User;

            var aspNetUserCredit = _dbContext.GetAspNetUserCredit(currentUser, _dbContext).Result;
            TotalUsedTokens = (int)aspNetUserCredit.TotalUsedTokens;
            CreditGranted = "infinite"; //aspNetUserCredit.CreditGranted 
            Cost = (aspNetUserCredit.TotalUsedTokens / 1000) * 0.002 + "$";
        }


    }
}

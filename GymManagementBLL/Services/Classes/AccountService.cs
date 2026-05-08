using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AccountViewModels;
using GymManagementDAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace GymManagementBLL.Services.Classes
{
	public class AccountService : IAccountService
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public AccountService(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}
		public ApplicationUser? ValidateUser(LoginViewModel LoginViewModel)
		{
			var User = _userManager.FindByEmailAsync(LoginViewModel.Email).Result;
			if (User is null) return null;
			var isValid = _userManager.CheckPasswordAsync(User, LoginViewModel.Password).Result;
			return isValid ? User : null;
		}
	}
}

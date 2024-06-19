using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MVCProject.Models;
using System.Security.Claims;

namespace MVCProject.Controllers
{
	public class ProfileController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IWebHostEnvironment _environment;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment environment, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_environment = environment;
			_signInManager = signInManager;
		}
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
            return View();
            //if (string.IsNullOrEmpty(returnUrl))
            //{
            //	returnUrl = Url.Action("Index", "Profile");
            //}

            //ViewData["ReturnUrl"] = returnUrl;
            //return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (string.IsNullOrEmpty(returnUrl))
			{
				returnUrl = Url.Action("Index","Profile");
			}
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					return RedirectToLocal(returnUrl);
				}
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}
			return View("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

		//[HttpGet]
		//public IActionResult Login()
		//{
		//	return View();
		//}

		//[HttpPost]
		//public async Task<IActionResult> Login(LoginViewModel model)
  //      {
		//	if (ModelState.IsValid)
		//	{
		//		//string Username = "Nitish Kumar";
		//		//string Password = "Test@1234";
				
		//		//var result = await _signInManager.PasswordSignInAsync(Username, Password, true, lockoutOnFailure: true);
		//		//if (result.Succeeded)
		//		//{
		//			return RedirectToAction("Index", "Home");
		//		//}
		//		//else
		//		//{
		//		//	ModelState.AddModelError(string.Empty, "Invalid login attempt");
		//		//	return View(model);
		//		//}
		//	}
		//	return View(model);
		//}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			if (!User.Identity.IsAuthenticated)
			{
				 return RedirectToAction("Login", "Profile");
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}
			var model = new ProfileViewModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Address = user.Address,
				PhoneNumber = user.PhoneNumber,
				ProfilePicture = user.ProfilePicture
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Index(ProfileViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return NotFound();
				}

				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.Address = model.Address;
				user.PhoneNumber = model.PhoneNumber;

				if (model.ProfilePictureFile != null)
				{
					var filePath = Path.Combine(_environment.WebRootPath, "uploads", user.Id + Path.GetExtension(model.ProfilePictureFile.FileName));
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await model.ProfilePictureFile.CopyToAsync(fileStream);
					}
					user.ProfilePicture = filePath;
				}

				var result = await _userManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction(nameof(Index));
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser { UserName = model.UserName, Email = model.Email };
				var result = await _userManager.CreateAsync((ApplicationUser)user, model.Password);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync((ApplicationUser)user, isPersistent: false);
					return RedirectToAction("Register", "Profile");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(model);
		}
		////Logout
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Logout()
		//{
		//	await _signInManager.SignOutAsync();
		//	return RedirectToAction("Index", "Home");
		//}
		public IActionResult ResetPassword(string code = null)
		{
			//	if (code == null)
			//	{
			//		return BadRequest("A code must be supplied for password reset.");
			//	}
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(model);
		}
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}
	}
}

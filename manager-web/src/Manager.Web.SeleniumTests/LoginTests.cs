using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

namespace Manager.Web.SeleniumTests;

public class LoginTests
{
	private IWebDriver _driver = default!;

	[SetUp]
	public void SetUp()
	{
        var options = new EdgeOptions();
        _driver = new EdgeDriver(options);
    }

	[TearDown]
	public void TearDown()
	{
		_driver.Quit();
	}

    // Disabled for the time being, will commit to developing more UI tests if time permits
	//[Test]
	public void Can_Register_As_New_User()
	{
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(2));

        _driver.Navigate().GoToUrl("https://localhost:7148");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

		var registerButton = _driver.FindElement(By.Id("nav_register_button"));
        registerButton.Click();

        var ticksNow = DateTime.UtcNow.Ticks;
        var userName = $"MyNewUsername{ticksNow}";
        var email = $"mytestemail+{ticksNow}@test.dev";

        var registerForm = new RegisterForm(_driver, wait);
        registerForm.CheckRegisterFormExists();
        registerForm.SetUsername(userName);
        registerForm.SetEmailAddress(email);
        registerForm.SetPassword("Password01!");
        registerForm.Submit();

        registerForm.CheckRegisterConfirmationShows();
    }
}

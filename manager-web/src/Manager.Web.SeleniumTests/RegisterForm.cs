using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Manager.Web.SeleniumTests;

public class RegisterForm
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _defaultWait;

    public RegisterForm(IWebDriver drier, WebDriverWait defaultWait)
    {
        _driver = drier;
        _defaultWait = defaultWait;
    }

    public void CheckRegisterFormExists()
    {
        _defaultWait.Until(e => e.FindElement(By.Id("registerForm")));
    }

    public void SetUsername(string userName)
    {
        var userNameField = _driver.FindElement(By.Id("Input_Username"));
        userNameField.SendKeys(userName);
    }

    public void SetEmailAddress(string emailAddress)
    {
        var emailField = _driver.FindElement(By.Id("Input_Email"));
        emailField.SendKeys(emailAddress);
    }

    public void SetPassword(string password)
    {
        var passwordField = _driver.FindElement(By.Id("Input_Password"));
        passwordField.SendKeys(password);

        var confirmPasswordField = _driver.FindElement(By.Id("Input_ConfirmPassword"));
        confirmPasswordField.SendKeys(password);
    }

    public void Submit()
    {
        var registerButton = _driver.FindElement(By.Id("registerSubmit"));
        registerButton.Click();
    }

    public void CheckRegisterConfirmationShows()
    {
        var pageHeader = _defaultWait.Until(e => e.FindElement(By.TagName("h1")));
        Assert.That(pageHeader.Text, Is.EqualTo("Register confirmation"));
    }

    public void ClickConfirmationLink()
    {
        var registerConfirmation = _driver.FindElement(By.Id("confirm-link"));
        registerConfirmation.Click();
    }

    public void CheckRegisterCompleteShows()
    {
        var pageHeader = _defaultWait.Until(e => e.FindElement(By.TagName("h1")));
        Assert.That(pageHeader.Text, Is.EqualTo("Confirm email"));
    }
}
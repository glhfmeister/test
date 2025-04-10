using System.Data;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace _1_test;

public class Tests
{
    
    private IWebDriver _driver;

    private WebDriverWait _wait;
    
    [SetUp]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _driver.Manage().Window.Size = new Size(1920,1080);
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        _wait = new WebDriverWait (_driver, TimeSpan.FromSeconds(5));

        Login ("***", "***");

    }

    private void Login(string username, string password)
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        _driver.FindElement(By.Id("Username")).SendKeys(username);
        _driver.FindElement(By.Id("Password")).SendKeys(password);
        _driver.FindElement(By.Name("button")).Click();

        _wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }

    private void OpenPopUp ()
    {
        // Открываем всплывающее окно справа сверху
        _driver.FindElement(By.CssSelector("[data-tid='PopupMenu__caption']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='PopupContent']")));
    }

    [Test]
    public void AuthorizationTest()
    {
        Assert.That(_driver.FindElement(By.CssSelector("[data-tid='Title']")).Text, Is.EqualTo("Новости"), "Страница не успела загрузиться");
    }

    [Test]
    public void NavigationToProfileTest()
    {
        OpenPopUp ();
        // Находим и переходим в профиль
        _driver.FindElement(By.CssSelector("[data-tid='Profile']")).Click();
        Assert.That(_driver.FindElement(By.CssSelector("[aria-current='page']")).Text, Is.EqualTo("Профиль"), "Страница не успела загрузиться");
    }

    [Test]
    public void NavigationToEditorTest()
    {
        OpenPopUp ();
        // Находим и переходим в редактирование профиля
        _driver.FindElement(By.CssSelector("[data-tid='ProfileEdit']")).Click();
        var getUrl = _driver.Url;
        Assert.That(getUrl, Is.EqualTo("https://staff-testing.testkontur.ru/profile/settings/edit"), "Страница не успела загрузиться");
    }

    [Test]
    public void EditAdditionalEmailTest()
    {
        _driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/profile/settings/edit");
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        // Находим и заменяем элемент
        var additionalEmail = _driver.FindElement(By.CssSelector("[data-tid='AdditionalEmail']"));
        var inputAdditionalEmail = additionalEmail.FindElement(By.CssSelector("input"));
        
        inputAdditionalEmail.SendKeys(Keys.Control + "a" + Keys.Backspace);
        inputAdditionalEmail.SendKeys ("123@test.ru");
        // Находим и нажимаем кнопку сохранения
        var pageHeader = _driver.FindElement (By.CssSelector("[data-tid='PageHeader']"));
        var buttonToSave = pageHeader.FindElements (By.CssSelector("button"));
        buttonToSave[0].Click();

        Assert.That(_driver.FindElement(By.CssSelector("[aria-current='page']")).Text, Is.EqualTo("Профиль"), "Страница не успела загрузиться");
    }

    [Test]
    public void NavigationToStartDialogTest()
    {
        // Находим раздел "Диалоги"
        _driver.FindElement(By.CssSelector("[data-tid='Messages']")).Click();
        _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='Title']")));
        // Находим и записываем значение в строку поиска
        var divSearch = _driver.FindElement (By.CssSelector("[data-tid='PageBody']"));
        var a = divSearch.FindElement (By.CssSelector("[data-tid='SearchBar']"));
        a.Click();
        var search = a.FindElement (By.CssSelector("input"));
        search.SendKeys("Коробейников");
        //Выбераем первое совпадение
        _driver.FindElement(By.CssSelector("[data-tid='ComboBoxMenu__item']")).Click();
        
        var name = _driver.FindElement(By.CssSelector("[data-tid='Name']"));
        Assert.That(name.Text, Is.EqualTo("Коробейников Данила"), "Страница не успела загрузиться");
    }

    [TearDown]
    public void TearDown()
    {
        _driver?.Dispose();
    }
}

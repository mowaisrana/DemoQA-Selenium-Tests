using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;
using System.Drawing;
using OpenQA.Selenium.Interactions;
using System.Threading;
using OpenQA.Selenium.BiDi.BrowsingContext;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;


namespace Selenium_Pract2
{
    [TestClass]
    
    public class UnitTest1
    {
        private IWebDriver driver;
        private object ExpectedConditions;
        private object SeleniumExtras;

        [TestInitialize]
        public void TestMethod1()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://demoqa.com/elements");
        }

        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }

        [TestMethod]
        public void TestTextBox()
        {
            // 1. select Text Box from Menu
            string userName = "rmowais";
            string emailAdd = "rmowais@gmail.com";
            string currentAdd = "Hyderabad, Pakistan";
            string permanentAdd = "Karachi, Pakistan";

            var textBox = driver.FindElement(By.XPath("//*[@id=\"item-0\"]"));
            textBox.Click();

            //Verify
            string tbHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Text Box", tbHeading, "Navigation failed!");

            driver.FindElement(By.Id("userName")).SendKeys(userName);
            driver.FindElement(By.Id("userEmail")).SendKeys(emailAdd);
            driver.FindElement(By.Id("currentAddress")).SendKeys(currentAdd);
            driver.FindElement(By.Id("permanentAddress")).SendKeys(permanentAdd);

            // becase there is div for ad and its overlapping the button
            var submitButton = driver.FindElement(By.Id("submit"));

            // Scroll until button is visible
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
            submitButton.Click();

            string user = driver.FindElement(By.Id("name")).Text;
            Assert.IsTrue(user.Contains(userName), "User Name is not matching!");

            string email = driver.FindElement(By.Id("email")).Text;
            Assert.IsTrue(email.Contains(emailAdd), "Email is not matching!");

            string currAdd = driver.FindElement(By.XPath("//p[@id='currentAddress']")).Text;
            currAdd = currAdd.Replace("Current Address :", "").Trim();
            Assert.AreEqual(currentAdd, currAdd, "Current Address is not matching!");

            string perAdd = driver.FindElement(By.XPath("//p[@id='permanentAddress']")).Text;
            perAdd = perAdd.Replace("Permananet Address :", "").Trim();
            Assert.AreEqual(permanentAdd, perAdd, "Permanent Address is not matching!");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestCheckBox()
        {
            // 2. select Check Box from Menu
            var checkBox = driver.FindElement(By.XPath("//*[@id=\"item-1\"]"));
            checkBox.Click();

            //Verify 
            string cbHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Check Box", cbHeading, "Navigation failed!");

            // Expand All
            driver.FindElement(By.XPath("//*[@id='tree-node']/div/button[1]")).Click();

            // Click on "Documents" checkbox
            var documentsCheckbox = driver.FindElement(By.XPath("//span[@class='rct-title' and text()='Documents']"));
            documentsCheckbox.Click();

            // Verify result contains "documents"
            string result = driver.FindElement(By.Id("result")).Text.ToLower();
            Assert.IsTrue(result.Contains("documents"), "Documents checkbox not selected!");

            // Optionally verify children
            Assert.IsTrue(result.Contains("workspace"), "Workspace not selected!");
            Assert.IsTrue(result.Contains("react"), "React not selected!");
            Assert.IsTrue(result.Contains("angular"), "Angular not selected!");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestRadioButton()
        {
            // 3. select Radio Button from Menu
            var radioButton = driver.FindElement(By.XPath("//*[@id=\"item-2\"]"));
            radioButton.Click();

            //Verify 
            string rbHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Radio Button", rbHeading, "Navigation failed!");

            // Select "Yes" radio button (click the label)
            driver.FindElement(By.XPath("//label[@for='yesRadio']")).Click();

            // Get result text
            string rbResult = driver.FindElement(By.ClassName("text-success")).Text;
            Assert.AreEqual("Yes", rbResult, "Radio Button 'Yes' is not selected!");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestWebTables()
        {
            //4. select Web Tables from Menu
            var webTables = driver.FindElement(By.XPath("//*[@id=\"item-3\"]"));
            webTables.Click();

            //Verify 
            string wtHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Web Tables", wtHeading, "Navigation failed!");

            // Add a new record
            driver.FindElement(By.Id("addNewRecordButton")).Click();

            // Fill form fields
            driver.FindElement(By.Id("firstName")).SendKeys("Owais");
            driver.FindElement(By.Id("lastName")).SendKeys("Rana");
            driver.FindElement(By.Id("userEmail")).SendKeys("owaisrana@gmail.com");
            driver.FindElement(By.Id("age")).SendKeys("23");
            driver.FindElement(By.Id("salary")).SendKeys("55000");
            driver.FindElement(By.Id("department")).SendKeys("QA");

            // Submit
            driver.FindElement(By.Id("submit")).Click();

            // Verify
            var tableText = driver.FindElement(By.ClassName("rt-tbody")).Text;
            Assert.IsTrue(tableText.Contains("Owais"), "Record was not added!");

            // Edit the record
            driver.FindElement(By.CssSelector("span[title='Edit']")).Click();
            var ageInput = driver.FindElement(By.Id("age"));
            ageInput.Clear();
            ageInput.SendKeys("24");
            driver.FindElement(By.Id("submit")).Click();

            // Verify
            tableText = driver.FindElement(By.ClassName("rt-tbody")).Text;
            Assert.IsTrue(tableText.Contains("24"), "Record was not updated!");

            // Search for record "Owais"
            driver.FindElement(By.Id("searchBox")).SendKeys("Owais");

            // Wait until the search result updates
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElement(By.ClassName("rt-tbody")).Text.Contains("Owais"));

            // Verify search worked
            string searchResult = driver.FindElement(By.ClassName("rt-tbody")).Text;
            Assert.IsTrue(searchResult.Contains("Owais"), "Search did not return the expected record!");

            // Now delete the filtered record
            driver.FindElement(By.CssSelector("span[title='Delete']")).Click();

            // Wait until record disappears from search result
            bool isDeleted = wait.Until(d =>
            {
                var tableText1 = d.FindElement(By.ClassName("rt-tbody")).Text;
                return !tableText1.Contains("Owais");
            });

            //Assert deletion worked
            Assert.IsTrue(isDeleted, "Record 'Owais' was not deleted!");

            Thread.Sleep(2000);
        }

        [TestMethod]
        public void TestButtons()
        {
            //5. select Buttons from Menu
            var buttons = driver.FindElement(By.XPath("//*[@id=\"item-4\"]"));
            buttons.Click();

            //Verify 
            string bHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Buttons", bHeading, "Navigation failed!");

            // Actions instance for complex mouse events
            Actions actions = new Actions(driver);

            // Double Click
            var doubleClickBtn = driver.FindElement(By.Id("doubleClickBtn"));
            actions.DoubleClick(doubleClickBtn).Perform();
            string doubleClickMsg = driver.FindElement(By.Id("doubleClickMessage")).Text;
            Assert.AreEqual("You have done a double click", doubleClickMsg, "Double Click action failed!");

            // Right Click
            var rightClickBtn = driver.FindElement(By.Id("rightClickBtn"));
            actions.ContextClick(rightClickBtn).Perform();
            string rightClickMsg = driver.FindElement(By.Id("rightClickMessage")).Text;
            Assert.AreEqual("You have done a right click", rightClickMsg, "Right Click action failed!");

            // Dynamic Click (normal single click)
            var dynamicClickBtn = driver.FindElement(By.XPath("//button[text()='Click Me']"));
            dynamicClickBtn.Click();
            string dynamicClickMsg = driver.FindElement(By.Id("dynamicClickMessage")).Text;
            Assert.AreEqual("You have done a dynamic click", dynamicClickMsg, "Dynamic Click action failed!");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestLinks()
        {
            driver.FindElement(By.Id("item-5")).Click();

            string lHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Links", lHeading, "Navigation failed!");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            //Helper method for API response validation
            void ClickAndValidate(string linkId, string expectedCode)
            {
                driver.FindElement(By.Id(linkId)).Click();

                // Wait until response text contains expected code
                bool responseOk = wait.Until(d =>
                {
                    var resp = d.FindElement(By.Id("linkResponse")).Text;
                    return resp.Contains(expectedCode);
                });

                Assert.IsTrue(responseOk, $"Link '{linkId}' did not return status {expectedCode}!");
            }

            //Normal Link
            var homeLink = driver.FindElement(By.Id("simpleLink"));
            homeLink.Click();
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            Assert.IsTrue(driver.Url.Contains("demoqa.com"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles.First());

            //Dynamic Link
            var dynamicHomeLink = driver.FindElement(By.Id("dynamicLink"));
            dynamicHomeLink.Click();
            driver.SwitchTo().Window(driver.WindowHandles.Last());
            Assert.IsTrue(driver.Url.Contains("demoqa.com"));
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles.First());

            // API Response Links
            ClickAndValidate("created", "201");
            ClickAndValidate("no-content", "204");
            ClickAndValidate("bad-request", "400");
            ClickAndValidate("forbidden", "403");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestBrokenLinks()
        {
            // Navigate to Broken Links - Images
            var brokenLinksMenu = driver.FindElement(By.Id("item-6"));
            brokenLinksMenu.Click();

            // Verify heading
            string heading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Broken Links - Images", heading, "Navigation failed!");

            // Save main window handle
            string mainWindow = driver.CurrentWindowHandle;

            // --- IMAGE CHECKS ---
            var validImage = driver.FindElement(By.XPath("//div[@class='col-12 mt-4 col-md-6']//img[1]"));
            bool validImgDisplayed = (bool)((IJavaScriptExecutor)driver)
                .ExecuteScript("return arguments[0].complete && arguments[0].naturalWidth > 0", validImage);
            Assert.IsTrue(validImgDisplayed, "Valid image is broken!");

            var brokenImage = driver.FindElement(By.XPath("//div[@class='col-12 mt-4 col-md-6']//img[2]"));
            bool brokenImgDisplayed = (bool)((IJavaScriptExecutor)driver)
                .ExecuteScript("return arguments[0].complete && arguments[0].naturalWidth > 0", brokenImage);
            Assert.IsFalse(brokenImgDisplayed, "Broken image is not detected!");

            // --- LINK CHECKS ---
            var validLink = driver.FindElement(By.LinkText("Click Here for Valid Link"));
            validLink.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            // Case 1: Link opened in new tab
            if (driver.WindowHandles.Count > 1)
            {
                var newWindow = driver.WindowHandles.Last();
                driver.SwitchTo().Window(newWindow);
                Assert.IsTrue(driver.Url.Contains("demoqa.com"), "Valid link did not redirect correctly in new tab!");
                driver.SwitchTo().Window(mainWindow);
            }
            else
            {
                // Case 2: Same tab navigation
                wait.Until(d => d.Url.Contains("demoqa.com"));
                Assert.IsTrue(driver.Url.Contains("demoqa.com"), "Valid link did not redirect correctly in same tab!");
                driver.Navigate().Back(); // go back to Broken Links page
            }

            // Broken Link (should redirect to status codes page)
            var brokenLink = driver.FindElement(By.LinkText("Click Here for Broken Link"));
            brokenLink.Click();

            wait.Until(d => d.Url.Contains("status_codes"));
            Assert.IsTrue(driver.Url.Contains("status_codes"), "Broken link did not navigate to error page!");

            // Ensure back to main
            driver.SwitchTo().Window(mainWindow);
        }

        [TestMethod]
        public void TestUploadDownload()
        {
            //8. select Upload and Download from Menu
            var uploadDownload = driver.FindElement(By.XPath("//*[@id=\"item-7\"]"));
            uploadDownload.Click();

            //Verify 
            string udHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Upload and Download", udHeading, "Navigation failed!");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

            //DOWNLOAD
            var downloadButton = driver.FindElement(By.Id("downloadButton"));
            downloadButton.Click();

            Assert.IsTrue(downloadButton.Enabled, "Download button was not clickable!");

            //UPLOAD
            var uploadInput = driver.FindElement(By.Id("uploadFile"));

            // valid path to a local file
            string filePath = @"C:\Users\owais\Desktop\testfile.txt";

            uploadInput.SendKeys(filePath);

            // Verify
            var uploadedPath = driver.FindElement(By.Id("uploadedFilePath")).Text;
            Assert.IsTrue(uploadedPath.Contains("testfile.txt"), "File upload failed!");

            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestDynamicProperties()
        {
            //9. select Dynamic Properties from Menu
            var dynamicProperties = driver.FindElement(By.XPath("//*[@id=\"item-8\"]"));
            dynamicProperties.Click();

            //Verify 
            string dpHeading = driver.FindElement(By.ClassName("text-center")).Text;
            Assert.AreEqual("Dynamic Properties", dpHeading, "Navigation failed!");

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Test 1Will enable 5 seconds
            var enableButton = driver.FindElement(By.Id("enableAfter"));
            wait.Until(d => enableButton.Enabled); // wait until clickable
            Assert.IsTrue(enableButton.Enabled, "Enable button was not enabled after 5 seconds!");

            //Test 2 Color Change
            var colorButton = driver.FindElement(By.Id("colorChange"));
            string initialClass = colorButton.GetAttribute("class");
            wait.Until(d => colorButton.GetAttribute("class") != initialClass);
            string newClass = colorButton.GetAttribute("class");
            Assert.AreNotEqual(initialClass, newClass, "Color did not change!");

            // Test 3 Visible After 5 seconds
            var visibleButton = wait.Until(d => d.FindElement(By.Id("visibleAfter")));
            Assert.IsTrue(visibleButton.Displayed, "Button did not appear after 5 seconds!");

            Thread.Sleep(2000);
        }

        [TestMethod]
        public void TestPracticeForm()
        {
            // Navigate to Forms             
            var formsMenu = driver.FindElement(By.XPath("//div[@class='header-text' and text()='Forms']"));             
            formsMenu.Click();             
            
            Thread.Sleep(1000);              
            
            driver.FindElement(By.XPath("//span[text()='Practice Form']")).Click();             
            
            // Verify             
            string pfHeading = driver.FindElement(By.ClassName("text-center")).Text;             
            Assert.AreEqual("Practice Form", pfHeading, "Navigation to Practice Form failed!");             
            
            // Fill the form              
            // First Name, Last Name, Email            
            driver.FindElement(By.Id("firstName")).SendKeys("Owais");             
            driver.FindElement(By.Id("lastName")).SendKeys("Rana");             
            driver.FindElement(By.Id("userEmail")).SendKeys("owaisrana@mail.com");             
            
            // Gender            
            driver.FindElement(By.XPath("//label[text()='Male']")).Click();             
            
            // Mobile Number            
            driver.FindElement(By.Id("userNumber")).SendKeys("1234567890");             
            
            // Date of Birth             
            driver.FindElement(By.Id("dateOfBirthInput")).Click();             
            new SelectElement(driver.FindElement(By.ClassName("react-datepicker__month-select"))).SelectByText("May");             
            new SelectElement(driver.FindElement(By.ClassName("react-datepicker__year-select"))).SelectByText("1995");             
            driver.FindElement(By.XPath("//div[contains(@class,'react-datepicker__day') and text()='15']")).Click();             
            
            // Subjects            
            var subjectInput = driver.FindElement(By.Id("subjectsInput"));             
            subjectInput.SendKeys("Maths");             
            subjectInput.SendKeys(Keys.Enter);             
            
            // Hobbies            
            driver.FindElement(By.XPath("//label[text()='Sports']")).Click();             
            
            //Upload Picture            
            var upload = driver.FindElement(By.Id("uploadPicture"));             
            upload.SendKeys(@"C:\Users\owais\Desktop\testimage.png");  
                       
            // Current Address            
            driver.FindElement(By.Id("currentAddress")).SendKeys("123 Test Street, Demo City");             
            
            //State and City            
            driver.FindElement(By.Id("state")).Click();             
            driver.FindElement(By.XPath("//div[contains(text(),'NCR')]")).Click();             
            driver.FindElement(By.Id("city")).Click();             
            driver.FindElement(By.XPath("//div[contains(text(),'Delhi')]")).Click();             
            
            // Submit            
            driver.FindElement(By.Id("submit")).Click();             
            
            //Verify            
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));            
            wait.Until(d => d.FindElement(By.Id("example-modal-sizes-title-lg")).Displayed);            
            string modalTitle = driver.FindElement(By.Id("example-modal-sizes-title-lg")).Text;            
            Assert.AreEqual("Thanks for submitting the form", modalTitle, "Form was not submitted successfully!");          
            
            // Close modal            
            driver.FindElement(By.Id("closeLargeModal")).Click();
        }

    }

}



using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reg_Gmail_Android_Mi_A1 {
    class AppiumHi {
        public static void ClickXpath(AndroidDriver<AndroidElement> driver, string xpath, int timeout = 10) {
            try {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                var element = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpath)));
                element.Click();
            }
            catch {
            }
        }

        public static void SendTextXpath(AndroidDriver<AndroidElement> driver, string xpath, string text) {
            try {
                AndroidElement element = driver.FindElementByXPath(xpath);
                element.SendKeys(text);
            }
            catch {
            }
        }

        public static void ClearText(AndroidDriver<AndroidElement> driver, string xpath) {
            try {
                AndroidElement element = driver.FindElementByXPath(xpath);
                element.Clear();
            }
            catch {
            }
        }

        public static IWebElement WaitXpath(AndroidDriver<AndroidElement> driver, string xpath, int Timeout = 15) {
            try {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Timeout));
                var elementVery = wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
                return elementVery;
            }
            catch {
                return null;
            }
        }

        public static bool CheckTextXpath(AndroidDriver<AndroidElement> driver, string xpath, string text,
            int Timeout = 10) {
            try {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Timeout));
                var element = wait.Until(ExpectedConditions.ElementExists(By.XPath(xpath)));
                if (element != null) {
                    string textCheck1 = element.GetAttribute("text");
                    if (element.Text == text) {
                        return true;
                    }
                }

                return false;
            }
            catch {
                return false;
            }
        }
    }
}
using System;
using OpenQA.Selenium.Chrome;

namespace Selenium_Pract2
{
    internal class iwebdriver
    {
        public static implicit operator iwebdriver(ChromeDriver v)
        {
            throw new NotImplementedException();
        }
    }
}
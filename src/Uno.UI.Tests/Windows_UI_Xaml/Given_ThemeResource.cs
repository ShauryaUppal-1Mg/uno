﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uno.UI.Tests.App.Xaml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Uno.UI.Tests.Windows_UI_Xaml
{
	[TestClass]
	public class Given_ThemeResource
	{
		[TestMethod]
		public void When_System_ThemeResource()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();
			app.HostView.Children.Add(control);

			Assert.AreEqual(Color.FromArgb(0xDE, 0x00, 0x00, 0x00), (control.TestTextBlock.Foreground as SolidColorBrush).Color);
		}

		[TestMethod]
		public void When_App_ThemeResource_Default()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();
			app.HostView.Children.Add(control);

			Assert.AreEqual(Colors.LemonChiffon, (control.TestBorder.Background as SolidColorBrush).Color);
		}

		[TestMethod]
		public void When_App_ThemeResource_Light()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();
			app.HostView.Children.Add(control);

			Assert.AreEqual(ApplicationTheme.Light, app.RequestedTheme);
			Assert.AreEqual(Colors.RosyBrown, (control.TestBorder.BorderBrush as SolidColorBrush).Color);
		}

		[TestMethod]
		public void When_Inherited_In_Template_Applied()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();

			app.HostView.Children.Add(control); //Note: this is not necessary for this test on UWP, only for Uno

			control.InlineTemplateControl.ApplyTemplate();
			control.TemplateFromResourceControl.ApplyTemplate();

			var text2InlineBefore = control.InlineTemplateControl.TextBlock2.Text;
			var text2ResourceTemplateBefore = control.TemplateFromResourceControl.TextBlock2.Text;

			var text4InlineBefore = control.InlineTemplateControl.TextBlock4.Text;
			var text4ResourceTemplateBefore = control.TemplateFromResourceControl.TextBlock4.Text;

			app.HostView.Children.Add(control);

			var text2InlineAfter = control.InlineTemplateControl.TextBlock2.Text;
			var text2ResourceTemplateAfter = control.TemplateFromResourceControl.TextBlock2.Text;

			Assert.AreEqual("LocalVisualTree", text2InlineBefore);

			Assert.AreEqual("ApplicationLevel", text4InlineBefore);

			Assert.AreEqual("LocalVisualTree", text2InlineAfter);
		}

		[TestMethod]
		[Ignore("Uno's StaticResource resolution doesn't exactly match UWP. Here, we don't use the parse-time scope.")]
		public void When_Inherited_In_Template_Applied_XAML_Scope()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();

			app.HostView.Children.Add(control); //Note: this is not necessary for this test on UWP, only for Uno

			control.InlineTemplateControl.ApplyTemplate();
			control.TemplateFromResourceControl.ApplyTemplate();

			var text2InlineBefore = control.InlineTemplateControl.TextBlock2.Text;
			var text2ResourceTemplateBefore = control.TemplateFromResourceControl.TextBlock2.Text;

			var text4InlineBefore = control.InlineTemplateControl.TextBlock4.Text;
			var text4ResourceTemplateBefore = control.TemplateFromResourceControl.TextBlock4.Text;

			app.HostView.Children.Add(control);

			var text2InlineAfter = control.InlineTemplateControl.TextBlock2.Text;
			var text2ResourceTemplateAfter = control.TemplateFromResourceControl.TextBlock2.Text;
			
			Assert.AreEqual("OuterVisualTree", text2ResourceTemplateBefore);
			
			Assert.AreEqual("ApplicationLevel", text4ResourceTemplateBefore);
			
			Assert.AreEqual("LocalVisualTree", text2ResourceTemplateAfter);
		}

		[TestMethod]
		public async Task When_Theme_Changed()
		{
			var app = UnitTestsApp.App.EnsureApplication();

			var control = new Test_Control();
			app.HostView.Children.Add(control);

			control.Measure(new Size(1000, 1000));

			var textLightStaticMarkup = control.TemplateFromResourceControl.TextBlock5.Text;
			var textLightThemeMarkup = control.TemplateFromResourceControl.TextBlock6.Text;

			Assert.AreEqual("ApplicationLevelLight", textLightStaticMarkup);
			Assert.AreEqual("LocalVisualTreeLight", textLightThemeMarkup);

			if (await SwapSystemTheme())
			{
				var textDarkStaticMarkup = control.TemplateFromResourceControl.TextBlock5.Text;
				var textDarkThemeMarkup = control.TemplateFromResourceControl.TextBlock6.Text;
				Assert.AreEqual("ApplicationLevelLight", textDarkStaticMarkup); //StaticResource markup doesn't change
				Assert.AreEqual("LocalVisualTreeDark", textDarkThemeMarkup); //ThemeResource markup change lookup uses the visual tree (rather than original XAML namescope)
				;
			}
		}

		private static async Task<bool> SwapSystemTheme()
		{
			var currentTheme = Application.Current.RequestedTheme;
#if NETFX_CORE
			if (!UnitTestsApp.App.EnableInteractiveTests)
			{
				return false;
			}

			var dialog = new ContentDialog { Content = "Set default app mode as 'dark' in settings", CloseButtonText = "Done" };
			await dialog.ShowAsync();
#else
#endif
			Assert.AreEqual(ApplicationTheme.Dark, Application.Current.RequestedTheme);
			return true;
		}
	}
}

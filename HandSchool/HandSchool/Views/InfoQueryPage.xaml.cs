﻿using HandSchool.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HandSchool.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InfoQueryPage : ContentPage
	{
		public InfoQueryPage()
		{
			InitializeComponent();
            BindingContext = InfoQueryViewModel.Instance;
        }
	}
}
﻿using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace wguterms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InputError : ContentPage
    {
        public InputError()
        {
            InitializeComponent();
        }

        private void btnOK_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}

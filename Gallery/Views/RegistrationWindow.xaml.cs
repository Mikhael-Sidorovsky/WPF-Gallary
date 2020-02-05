using Gallery.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Gallery
{
    /// <summary>
    /// Interaction logic for Autorization.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            using (RegistrationViewModel context = new RegistrationViewModel())
            { 
                DataContext = context;
            }
        }
    }
}

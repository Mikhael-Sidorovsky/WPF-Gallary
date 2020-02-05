using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Gallery.Models;

namespace Gallery.ViewModels
{
    class LoginViewModel : DependencyObject, IDataErrorInfo, IDisposable
    {
        #region Properties

        private GallaryContext context;
        private List<User> Users { get; set; }
        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(LoginViewModel), new PropertyMetadata(""));

        private string Password;

        #endregion

        #region Commands
        private Command loginCommand;
        public Command LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand = new Command(Login, CanLogin));
            }
        }

        private void Login(object obj)
        {
            try
            {
                if (Users.Where(x => x.Email == Email && x.Password == Password).Any())
                {
                    User user = Users.Where(x => x.Email == Email && x.Password == Password).FirstOrDefault();
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    foreach (Window el in Application.Current.Windows)
                    {
                        if (el.ToString().Contains("Login"))
                        {
                            el.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid email or password!");
                }
            }
            catch (Exception) { }

        }

        private bool CanLogin(object obj)
        {
            if (!IsValidEmail || string.IsNullOrEmpty(Email) || Password.Length < 6)
                return false;
            else
                return true;
        }

        private Command registrationCommand;
        public Command RegistrationCommand
        {
            get
            {
                return registrationCommand ?? (registrationCommand = new Command(Registration, CanRegistration));
            }
        }

        private void Registration(object obj)
        {
            RegistrationWindow autorizationWindow = new RegistrationWindow();
            autorizationWindow.Show();
            foreach (Window el in Application.Current.Windows)
            {
                if (el.ToString().Contains("Login"))
                {
                    el.Close();
                    break;
                }
            }
        }

        private bool CanRegistration(object obj)
        {
            return true;
        }

        private Command passwordChangedCommand;
        public Command PasswordChangedCommand
        {
            get
            {
                return passwordChangedCommand ?? (passwordChangedCommand = new Command(PasswordChanged, CanPasswordChanged));
            }
        }

        private void PasswordChanged(object obj)
        {
            if (obj != null)
                Password = (obj as PasswordBox).Password;
        }

        private bool CanPasswordChanged(object obj)
        {
            return true;
        }

        public void Dispose()
        {
            if (this == null)
                context?.Dispose();
        }
        #endregion

        #region Implementation IDataErrorInfo
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                if (string.IsNullOrEmpty(Email))
                {
                    error = "Email является обязательным полем!";
                }
                else if (!IsValidEmail)
                {
                    error = "Некорректный ввод Email!";
                }
                return error;
            }
        }

        private Regex emailReg;

        public bool IsValidEmail
        {
            get { return emailReg.IsMatch(Email); }
        }
        #endregion
        public LoginViewModel()
        {
            GetDataFromDBAsync();
            emailReg = new Regex(@"^([\w\.\-]+)@([a-z\-]+)((\.([a-z]){2,3})+)$");
            Password = "";
        }

        private async void GetDataFromDBAsync()
        {
            await Task.Run(() =>
            {
                context = new GallaryContext();
                Users = context.Users.ToList<User>();
            });
        }
    }
}

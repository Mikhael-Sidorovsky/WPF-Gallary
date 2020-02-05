using Gallery.Models;
using Gallery.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Gallery.ViewModels
{
    class RegistrationViewModel : DependencyObject, IDataErrorInfo, IDisposable
    {
        #region Properties

        GallaryContext context;
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(RegistrationViewModel), new PropertyMetadata(""));

        public string Surname
        {
            get { return (string)GetValue(SurnameProperty); }
            set { SetValue(SurnameProperty, value); }
        }

        public static readonly DependencyProperty SurnameProperty =
            DependencyProperty.Register("Surname", typeof(string), typeof(RegistrationViewModel), new PropertyMetadata(""));

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public static readonly DependencyProperty EmailProperty =
            DependencyProperty.Register("Email", typeof(string), typeof(RegistrationViewModel), new PropertyMetadata(""));

        private string Password;

        private string ConfirmPassword;

        #endregion

        #region Commands

        private Command cancelCommand;
        public Command CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new Command(Cancel, CanCancel));
            }
        }

        private void Cancel(object obj)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            foreach (Window el in Application.Current.Windows)
            {
                if (el.ToString().Contains("Registration"))
                {
                    el.Close();
                    break;
                }
            }
        }

        private bool CanCancel(object obj)
        {
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
            if (Password == ConfirmPassword)
            {
                if (!context.Users.Where(x => x.Email == Email).Any())
                {
                    User user = new User();
                    user.Name = Name;
                    user.Surname = Surname;
                    user.Email = Email;
                    user.Password = Password;
                    context.Users.Add(user);
                    context.SaveChanges();
                    MainWindow mainWindow = new MainWindow(user);
                    mainWindow.Show();
                    foreach (Window el in Application.Current.Windows)
                    {
                        if (el.ToString().Contains("Registration"))
                        {
                            el.Close();
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("User with this email already exists!!!");
                }
            }
            else
                MessageBox.Show("Password confirmation does not match password");
        }

        private bool CanRegistration(object obj)
        {
            if (!IsValidEmail || string.IsNullOrEmpty(Email)
                || string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname)
                || Password.Length < 6 || ConfirmPassword.Length < 6)
                return false;
            else
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

        private Command confirmPasswordChangedCommand;
        public Command ConfirmPasswordChangedCommand
        {
            get
            {
                return confirmPasswordChangedCommand ?? (confirmPasswordChangedCommand = new Command(ConfirmPasswordChanged, CanConfirmPasswordChanged));
            }
        }

        private void ConfirmPasswordChanged(object obj)
        {
            if (obj != null)
                ConfirmPassword = (obj as PasswordBox).Password;
        }

        private bool CanConfirmPasswordChanged(object obj)
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
        public RegistrationViewModel()
        {
            context = new GallaryContext();
            emailReg = new Regex(@"^([\w\.\-]+)@([a-z\-]+)((\.([a-z]){2,3})+)$");
            Password = "";
            ConfirmPassword = "";
        }
    }
}

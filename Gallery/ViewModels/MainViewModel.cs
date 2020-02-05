using Gallery.Models;
using Gallery.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Gallery.ViewModels
{
    class MainViewModel : DependencyObject, INotifyPropertyChanged, IDisposable
    {
        #region Properties
        private List<MyImage> Images { get; set; }
        private MyImage currentImage;
        private User currentUser;
        private int curImgInd;
        private List<Mark> Marks { get; set; }
        private GallaryContext context;
        public int currentImagesIndex
        {
            get => curImgInd;
            set
            {
                curImgInd = value;
                ChangeImage();
                OnPropertyChanged("currentImagesIndex");
            }
        }

        public BitmapSource Image
        {
            get { return (BitmapSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(BitmapSource), typeof(MainViewModel), new PropertyMetadata(null));

        public string ImageDescription
        {
            get { return (string)GetValue(ImageDescriptionProperty); }
            set { SetValue(ImageDescriptionProperty, value); }
        }

        public static readonly DependencyProperty ImageDescriptionProperty =
            DependencyProperty.Register("ImageDescription", typeof(string), typeof(MainViewModel), new PropertyMetadata(""));

        public int SliderMaxValue
        {
            get { return (int)GetValue(SliderMaxValueProperty); }
            set { SetValue(SliderMaxValueProperty, value); }
        }

        public static readonly DependencyProperty SliderMaxValueProperty =
            DependencyProperty.Register("SliderMaxValue", typeof(int), typeof(MainViewModel), new PropertyMetadata(0));

        private string mark;

        public string CurrentMark
        {
            get => mark;
            set
            {
                mark = value;
                ChangeMark();
                OnPropertyChanged("CurrentMark");
            }
        }

        #endregion

        #region Constructor
        public MainViewModel(User user)
        {
            context = new GallaryContext();
            currentUser = context.Users.Where(u => u.Id == user.Id).FirstOrDefault();
            currentImagesIndex = 0;
            Marks = context.Marks.ToList<Mark>();
            Images = context.Images.ToList<MyImage>();
            SliderMaxValue = Images.Count() - 1;
            ChangeImage();
        }
        #endregion

        #region Commands

        private Command firstCommand;
        public Command FirstCommand
        {
            get
            {
                return firstCommand ?? (firstCommand = new Command(First, CanFirst));
            }
        }

        private void First(object obj)
        {
            currentImagesIndex = 0;
            ChangeImage();
        }

        private bool CanFirst(object obj)
        {
            return true;
        }

        private Command lastCommand;
        public Command LastCommand
        {
            get
            {
                return lastCommand ?? (lastCommand = new Command(Last, CanLast));
            }
        }

        private void Last(object obj)
        {
            currentImagesIndex = 14;
            ChangeImage();
        }
        private bool CanLast(object obj)
        {
            return true;
        }

        private Command previousCommand;
        public Command PreviousCommand
        {
            get
            {
                return previousCommand ?? (previousCommand = new Command(Previous, CanPrevious));
            }
        }

        private void Previous(object obj)
        {
            if (currentImagesIndex == 0)
                currentImagesIndex = 14;
            else
                currentImagesIndex--;
            ChangeImage();
        }

        private bool CanPrevious(object obj)
        {
            return true;
        }

        private Command nextCommand;
        public Command NextCommand
        {
            get
            {
                return nextCommand ?? (nextCommand = new Command(Next, CanNext));
            }
        }

        private void Next(object obj)
        {
            if (currentImagesIndex == 14)
                currentImagesIndex = 0;
            else
                currentImagesIndex++;
            ChangeImage();
        }

        private bool CanNext(object obj)
        {
            return true;
        }

        private Command exitCommand;

        public Command ExitCommand
        {
            get
            {
                return exitCommand ?? (exitCommand = new Command(Exit, CanExit));
            }
        }

        private void Exit(object obj)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            foreach (Window el in Application.Current.Windows)
            {
                if (el.ToString().Contains("Main"))
                    el.Close();
                break;
            }
        }

        private bool CanExit(object obj)
        {
            return true;
        }

        #endregion

        #region Service methods

        private void SetMark()
        {
            if (Marks.Where(m => m.UserId == currentUser.Id && m.ImageId == currentImage.Id).FirstOrDefault() != null)
                CurrentMark = Marks.Where(m => m.UserId == currentUser.Id && m.ImageId == currentImage.Id).FirstOrDefault().Name;
            else
                CurrentMark = "";
        }

        private void ChangeMark()
        {
            if (CurrentMark != "")
            {
                Mark new_mark = null;
                int markIncrement;

                if (Marks.Where(m => m.UserId == currentUser.Id && m.ImageId == currentImage.Id).Any())
                {
                    new_mark = Marks.Where(m => m.UserId == currentUser.Id
                                                   && m.ImageId == currentImage.Id).FirstOrDefault();
                    int.TryParse(new_mark.Name, out markIncrement);
                    currentImage.SumOfMarks -= markIncrement;
                    int.TryParse(CurrentMark, out markIncrement);
                    currentImage.SumOfMarks += markIncrement;
                    new_mark.Name = CurrentMark;
                    context.SaveChanges();
                }
                else
                {
                    new_mark = new Mark();
                    new_mark.Name = CurrentMark;
                    new_mark.ImageId = currentImage.Id;
                    new_mark.UserId = currentUser.Id;
                    Marks.Add(new_mark);
                    context.Marks.Add(new_mark);
                    int.TryParse(CurrentMark, out markIncrement);
                    currentImage.SumOfMarks += markIncrement;
                    currentImage.CountOfMarks++;
                    context.SaveChanges();
                }
                ImageDescription = currentImage.ToString();
            }
        }

        private void ChangeImage()
        {
            if (Images != null)
            {
                currentImage = Images[currentImagesIndex];
                Image = GetBitmapImage(currentImage.ImgData);
                ImageDescription = currentImage.ToString();
                SetMark();
            }
        }

        private BitmapSource GetBitmapImage(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                Bitmap image = new Bitmap(ms);
                IntPtr bmpPt = image.GetHbitmap();
                BitmapSource bitmapSource =
                 System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bmpPt,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                DeleteObject(bmpPt);

                return bitmapSource;
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void Dispose()
        {
            if (this == null)
                context?.Dispose();
        }
        #endregion
    }
}

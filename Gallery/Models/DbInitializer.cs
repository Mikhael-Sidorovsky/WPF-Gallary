using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Models
{
    class DbInitializer : DropCreateDatabaseIfModelChanges<GallaryContext>
    {
		List<string> images = new List<string> { "../../Models/Images/1.jpg", "../../Models/Images/2.jpg", "../../Models/Images/3.jpg",
												 "../../Models/Images/4.jpg", "../../Models/Images/5.jpg", "../../Models/Images/6.jpg",
												 "../../Models/Images/7.jpg", "../../Models/Images/8.jpg", "../../Models/Images/9.jpg",
												 "../../Models/Images/10.jpg", "../../Models/Images/11.jpg", "../../Models/Images/12.jpg",
												 "../../Models/Images/13.jpg", "../../Models/Images/14.jpg", "../../Models/Images/15.jpg"};

		protected override void Seed(GallaryContext context)
		{
			List<MyImage> Img = new List<MyImage>();
			foreach(var item in images)
			{
				Img.Add(ImageConvertor(item));
			}
			context.Images.AddRange(Img);
			context.SaveChanges();
			base.Seed(context);
		}


		private static MyImage ImageConvertor(string file)
		{
			// конвертация изображения в байты
			MyImage image = new MyImage();
			FileInfo fInfo = new FileInfo(file);
			long numBytes = fInfo.Length;
			using (FileStream fStream = new FileStream(file, FileMode.Open, FileAccess.Read))
			{
				BinaryReader br = new BinaryReader(fStream);
				image.ImgData = br.ReadBytes((int)numBytes);
				image.Name = file;
			}
			return image;
		}
	}
}

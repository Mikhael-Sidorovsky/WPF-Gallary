using System;

namespace Gallery.Models
{
    class MyImage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImgData { get; set; }
        public string Author { get; set; }
        public double CountOfMarks { get; set; }
        public double SumOfMarks { get; set; }
        public DateTime Date { get; set; }

        public MyImage()
        {
            Author = "Admin";
            CountOfMarks = 0;
            SumOfMarks = 0;
            ImgData = null;
            Date = DateTime.Now;
            Name = "";
        }

        public override string ToString()
        {
            double avgMark = CountOfMarks == 0 ? 0 : SumOfMarks / CountOfMarks;

            return $"Image information:\nName: {Name.Substring(20)}\n" +
                $"Date: {Date.ToShortDateString()}\n" +
                $"Author: {Author}\n" +
                $"Average rating: {Math.Round(avgMark, 2)}";
        }
    }

}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickType;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Configuration;

namespace WpfApp_Chrome_Bookmarks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(openFileDialog.FileName);
                var books = Welcome.FromJson(jsonString);

                var otherChildren = books.Roots.Other.Children.ToList();
                    //.DeepClone()
                    ;
                //.Where(x => x.Type == TypeEnum.Url)
                //.OrderBy(x => ConvertWebKitTime(Convert.ToInt64(x.DateAdded)));

                books.Roots.Other.Children.Clear();
                    //= new OtherChild[] { };

                foreach (var c in otherChildren.OrderBy(x => ConvertWebKitTime(Convert.ToInt64(x.DateAdded))))
                {                    
                    books.Roots.Other.Children.Add(c);
                }

                var jsonString2 = books.ToJson();
                //Debug.Assert(jsonString == jsonString2);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, jsonString2);
                }
            }
        }

        public static DateTime ConvertWebKitTime(long webkitEpoch)
        {
            const long epochDifferenceMicroseconds = 11644473600000000; // difference in microseconds between 1601 and 1970
            var epoch = (webkitEpoch - epochDifferenceMicroseconds) / 1000000; // adjust to seconds since 1st Jan 1970
            return DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime; // convert to datetime
        }

    }
    static class Extensions
    {
        // Return a deep clone of an object of type T.
        public static T DeepClone<T>(this T obj)
        {
            using (MemoryStream memory_stream = new MemoryStream())
            {
                // Serialize the object into the memory stream.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memory_stream, obj);

                // Rewind the stream and use it to create a new object.
                memory_stream.Position = 0;
                return (T)formatter.Deserialize(memory_stream);
            }
        }

        // Return a deep clone of a list.
        public static List<T> DeepClone<T>(this List<T> items)
        {
            var query = from T item in items select item.DeepClone();
            return new List<T>(query);
        }

        // Return a deep clone of an array.
        public static T[] DeepClone<T>(this T[] items)
        {
            var query = from T item in items select item.DeepClone();
            return query.ToArray();
        }

        // Return a shallow clone of a list.
        public static List<T> ShallowClone<T>(this List<T> items)
        {
            return new List<T>(items);
        }

        // Return a shallow clone of an array.
        public static T[] ShallowClone<T>(this T[] items)
        {
            return (T[])items.Clone();
        }
    }
}

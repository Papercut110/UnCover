using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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


namespace UnCover
{
    public partial class MainWindow : Window
    {
        enum MethodType
        {
            Public,
            Protected
        }

        
        private Dictionary<ClassEntity, List<Tuple<MethodInfo, MethodType>>> metodsCollection;
        private string[] scriptPath;
        private MethodType methodType = 0;

        public MainWindow()
        {
            metodsCollection = new Dictionary<ClassEntity, List<Tuple<MethodInfo, MethodType>>>();

            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var folder = new OpenFileDialog();
            folder.Multiselect = true;

            folder.ShowDialog();
            scriptPath = folder.FileNames;

            foreach (var item in scriptPath)
                Unwrap(item);

            List<ClassEntity> classEntities = new List<ClassEntity>();

            foreach (var item in metodsCollection)
                classEntities.Add(item.Key);

            var grouped = classEntities.OrderBy(x => x.ClassName).ToList();

            classesCollection.ItemsSource = grouped;
        }

        private void Unwrap(string assemblyPath)
        {
            if (!string.IsNullOrEmpty(assemblyPath))
            {
                Assembly assembly = Assembly.LoadFile(assemblyPath);
                Type[] types = assembly.GetTypes();

                if (types != null && types.Length > 0)
                {
                    foreach (var item in types)
                    {
                        List<Tuple<MethodInfo, MethodType>> list = new List<Tuple<MethodInfo, MethodType>>();
                        List<MethodInfo> methodInfos = item.GetMethods(BindingFlags.Public | BindingFlags.Instance 
                            | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Static).ToList();

                        foreach (var _item in methodInfos)
                        {
                            if (_item.IsPrivate)
                                continue;

                            methodType = _item.IsFamily == true ? MethodType.Protected : MethodType.Public; 
                            var tuple = Tuple.Create(_item, methodType);
                            list.Add(tuple);
                        }


                        metodsCollection.Add(new ClassEntity() { ClassName = item.Name }, list);
                    }
                }
                else
                {
                    MessageBox.Show("There is no types found");
                }
            }
        }

        private void classesCollection_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var value = classesCollection.SelectedItem as ClassEntity;

            if (value == null)
                return;

            string message = string.Empty;

            List<Tuple<MethodInfo, MethodType>> methods;

            metodsCollection.TryGetValue(value, out methods);

            
            foreach (var item in methods)
            {
                try
                {
                    message += $"{item.Item1 }   {item.Item2}\n";
                }
                catch { }
                finally
                {
                    message += "";
                }
            }
                

            MessageBox.Show(message);
        }
    }
}

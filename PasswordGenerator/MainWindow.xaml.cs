using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneratorDLL;

namespace PasswordGenerator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ExecuteGenerator();
            }
        }

        /// <summary>
        ///     Set up generator
        /// </summary>
        private void ExecuteGenerator()
        {
            try
            {
                if (Convert.ToInt32(txtMin.Text) > Convert.ToInt32(txtMax.Text))
                {
                    MessageBox.Show("Minimal length cannot be greater than maximal", "Error");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
                return;
            }

            ThreadPool.SetMaxThreads(4, 100);
            ThreadPool.QueueUserWorkItem(GeneratePasswordsAsync, null);
        }

        /// <summary>
        ///     Generate Passwords Async
        /// </summary>
        /// <param name="o">Callback object</param>
        private void GeneratePasswordsAsync(object o)
        {
            GC.Collect();
            var seed = new Random();
            var passwords = new List<string>();
            int minLength = 0, maxLength = 0, count = 0;
            Dispatcher.Invoke(() =>
            {
                GC.Collect();
                PasswordList.ItemsSource = null;
                PasswordList.Items.Refresh();
                minLength = Convert.ToInt32(txtMin.Text) + 1;
                maxLength = Convert.ToInt32(txtMax.Text) + 1;
                count = Convert.ToInt32(txtCount.Text);
                Progress.Minimum = 0;
                Progress.Maximum = count;
            });
            var st = new Stopwatch();
            st.Start();
            for (var i = 0; i < count; i++)
            {
                var currentLength = seed.Next(minLength, maxLength);
                passwords.Add(Generator.Generate(currentLength, seed));
                Dispatcher.Invoke(() => { Progress.Value = i; });
            }
            st.Stop();
            Dispatcher.Invoke(() =>
            {
                lblTimeElapsed.Content = st.ElapsedMilliseconds + " ms";
                PasswordList.ItemsSource = passwords;
                PasswordList.Items.Refresh();
                GC.Collect();
            });
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                lblMinLength.Content = Convert.ToDouble(txtMin.Text)*8;
                CheckBox_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void txtMax_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                lblMaxLength.Content = Convert.ToDouble(txtMax.Text)*8;
                CheckBox_Checked(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Generator.AcceptableCharacters = "";
            double possiblePasswords = 0;
            var countBase = 0;
            if (cbAlphabetic.IsChecked == true)
            {
                countBase += 26;
                Generator.AcceptableCharacters += "abcdefghijklmnopqrstuwvxyz";
            }
            if (cbCapital.IsChecked == true)
            {
                countBase += 26;
                Generator.AcceptableCharacters += "ABCDEFGHIJKLMNOPQRSTUWVXYZ";
            }
            if (cbNumeric.IsChecked == true)
            {
                countBase += 10;
                Generator.AcceptableCharacters += "0123456789";
            }
            if (cbSpecial.IsChecked == true)
            {
                countBase += 32;
                Generator.AcceptableCharacters += "!@#$%^&*()_+{}|:';<>?,./[]`~\"";
            }
            try
            {
                for (var i = Convert.ToInt32(txtMin.Text); i <= Convert.ToInt32(txtMax.Text); i++)
                {
                    possiblePasswords = possiblePasswords + Convert.ToDouble(Math.Pow(countBase, i));
                }
                lblAmount.Content = possiblePasswords;
            }
            catch (OverflowException)
            {
                lblAmount.Content = "Unable to process";
            }
            catch (Exception ex)
            {
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            using (var sw = new StreamWriter("pass.txt"))
            {
                try
                {
                    foreach (var c in PasswordList.Items)
                    {
                        sw.WriteLine(c.ToString());
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            ExecuteGenerator();
        }
    }
}
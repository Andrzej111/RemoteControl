using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoteControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CommandsHandler _command_handler;

        public MainPage()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(1000, 1800));
            this.InitializeComponent();

            _command_handler = new CommandsHandler();
        }

        public async void update_playlist()
        {
            try
            {
                JsonArray arr = await _command_handler.get_playlist();
                ItemCollection coll = new ListBox().Items;
                playlist.Items.Clear();

                foreach (var it in arr)
                {
                    if (it.GetObject().ContainsKey("title"))
                    {
                        //MessageDialog m = new MessageDialog(it.GetObject()["title"].ToString());
                        //await m.ShowAsync();
                        ListBoxItem item = new ListBoxItem();
                        item.Content = it.GetObject()["title"].ToString();
                        item.DataContext = it.GetObject()["id"].ToString();
                        item.Tapped += ListBoxItem_Tapped;
                        playlist.Items.Add(item);
                    }
                    else {
                        playlist.Items.Add("Uknown");
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private void click_play(object sender, RoutedEventArgs e)
        {
            _command_handler.play();
        }

        private void click_pause(object sender, RoutedEventArgs e)
        {
            _command_handler.pause();
        }

        private void click_next(object sender, RoutedEventArgs e)
        {
            _command_handler.next();
        }

        private void click_prev(object sender, RoutedEventArgs e)
        {
            _command_handler.previous();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.search_and_play(textBlock.Text);
        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            // bug with double firing event
            if (e.KeyStatus.RepeatCount == 1)
            {
                if (e.Key == Windows.System.VirtualKey.Enter)
                {
                    _command_handler.search_and_play(textBlock.Text);
                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //    ComboBox combo = (ComboBox)sender;
            //    combo.SelectedIndex = 1;
        }

        private void radiostacja_SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            comboBox.SelectedItem = sender;
            comboBox.IsDropDownOpen = false;
        }

        private void comboBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            comboBox.IsDropDownOpen = true;
        }

        private void radio_button_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.play_radio((comboBox.SelectedIndex + 1).ToString());
        }

        private void volume_down(object sender, RoutedEventArgs e)
        {
            _command_handler.volume_down();
        }

        private void volume_mute(object sender, RoutedEventArgs e)
        {
            _command_handler.volume_mute();
        }

        private void volume_up(object sender, RoutedEventArgs e)
        {
            _command_handler.volume_up();
        }

        private void key_up_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.send_key("Up");
        }

        private void key_left_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.send_key("Left");
        }

        private void key_down_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.send_key("Down");
        }

        private void key_right_Click(object sender, RoutedEventArgs e)
        {
            _command_handler.send_key("Right");
        }

        // choose from playlist
        private void ListBoxItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListBoxItem ittt = (ListBoxItem)sender;
            string id = ittt.DataContext.ToString().Trim('"');
            _command_handler.play_id(id);

        }

        private void PivotItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void PivotItem_Loading(FrameworkElement sender, object args)
        {
            update_playlist();
        }

        private void Clear_Tapped(object sender, TappedRoutedEventArgs e)
        {
            _command_handler.clear();
            update_playlist();
        }

        private void PivotItem_GotFocus(object sender, RoutedEventArgs e)
        {
            update_playlist();

        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            update_playlist();

        }

        private void ip_address_TextChanged(object sender, TextChangedEventArgs e)
        {
            _command_handler.set_ip(ip_address.Text.ToString());
        }

    }
}

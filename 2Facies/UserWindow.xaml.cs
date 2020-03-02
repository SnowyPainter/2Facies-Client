﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace _2Facies
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        //readonly constants variables
        private readonly string Token = null;
        //data variables
        private Packet.DataPublic userData;
        public WsClient client;

        private bool testing;
        public UserWindow(string token)
        {
            InitializeComponent();
            userData = new Packet.DataPublic();
            client = new WsClient(ErrorHandler);
            Token = token;
        }
        public UserWindow() { testing = true; }
        public void ErrorHandler(Packet.ErrorCode code)
        {
            switch (code)
            {
                case Packet.ErrorCode.WrongCode:
                    MessageBox.Show("Wrong Code");
                    break;
                case Packet.ErrorCode.RoomJoin:
                    MessageBox.Show("There was an error with join room");
                    break;
                case Packet.ErrorCode.RoomLeave:
                    MessageBox.Show("There was an error with leave room");
                    break;
            }
        }

        //-------------------------------------------------------------------------------------
        //---------------------Window Initialize, navigion bar btn events------------------
        //-------------------------------------------------------------------------------------
        private void AsyncControlsInitilize(Packet.DataPublic user)
        {
            NameBlock.Text = user.Name;
            AgeBlock.Text = user.Age.ToString();
            EmailBlock.Text = user.Email;
        }
        private void ContorlsInitilize()
        {
            Title_Text.Text = $"2FACIES 안녕하세요";
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (testing) return;

            var loading = new LoadingWindow("서버와 연결중입니다 ...");
            loading.Show();

            var data = GetPrivateData(Token);
            var reqCheck = ServerClient.ServerConnectionCheck();

            //processing sync
            ContorlsInitilize();

            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, string>>(await data);

            if (jsonData.ContainsKey("result") && jsonData["result"] == "false")
            {
                MessageBox.Show("토큰 정보가 올바르지 않습니다 로그인 창으로 돌아갑니다.");
                loading.LoadingDone();
                this.Close();
            }
            else
            {
                userData.Bind(jsonData);
                AsyncControlsInitilize(userData);
            }

            if (!(await reqCheck))
            {
                MessageBox.Show("서버와의 연결에 실패했습니다.");
                loading.LoadingDone();
                this.Close();
            }

            loading.LoadingDone();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (WsClient.Room != null)
                client.Leave(WsClient.Room.Id);
        }

        private void WindowClose_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void WindowMinimize_Btn_Clicked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void NavigationBar_DragMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------

        private bool isLookingForPlayer = false;
        private async Task<string> GetPrivateData(string token)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                var getDataByToken = $"{Request.Domain}/{Request.UserTokenInfoURL}";
                string tokenData = await (await ServerClient.RequestGet(getDataByToken, client)).ReadAsStringAsync();
                return tokenData;
            }
        }
        private void FindPlayerButton_Clicked(object sender, RoutedEventArgs e)
        {
            isLookingForPlayer = !isLookingForPlayer;
            MessageBox.Show(isLookingForPlayer ? "상대 매칭을 시작합니다." : "상대 매칭을 취소했습니다.");
            MaterialDesignThemes.Wpf.ButtonProgressAssist.SetIsIndicatorVisible(FindFaciesButton, isLookingForPlayer);
            if (!isLookingForPlayer)
            {
                client.Leave("1");
                return;
            }
            //create test room
            client.Join("1");
            
            /*client.Emit("broadcast", "1", "Hello!");
            client.On("message", (ev) =>
            {
                MessageBox.Show(ev.Data.Split('@')[1]);
            });*/
            //---------------
        }
        private async void OpenRoomBrowser_Clicked(object sender, RoutedEventArgs e)
        {
            var raw = await (await ServerClient.RequestGet($"{Request.Domain}/{Request.RoomListURL}")).ReadAsStringAsync();
            var roomList = JsonConvert.DeserializeObject<List<Packet.Room>>(raw);

            if (roomList.Count > 0)
            {
                var browser = new RoomBrowserWindow(roomList);
                browser.ShowDialog();
            }
            else
                MessageBox.Show("There's no room");
        }

        //deaccomplished
        private void TakePictureButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png" };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                ProfileCardImage.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }
    }
}

using AutoMapper;
using ChessEngineLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KingdomChessGame_Desktop
{
    /// <summary>
    /// Interaction logic for PlayerSetter.xaml
    /// </summary>
    public partial class Players : Window
    {
        private readonly GameUIManager _manager;
        private readonly Mapper _mapper;
        public PlayerViewModel White { get; set; }
        public PlayerViewModel Black { get; set; }

        public Players()
        {
            InitializeComponent();
            _mapper = AutoMapperProfile.InitializeMapper();
            _manager = new GameUIManager();
            var players = _mapper.Map<List<PlayerViewModel>>(_manager.GetPlayers());
          

            Whites.ItemsSource = players.Where(p => p.PlayerId % 2 !=0);
            Blacks.ItemsSource = players.Where(p => p.PlayerId % 2 == 0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Whites.SelectedItem != null && Blacks.SelectedItem != null)
            {
                White = Whites.SelectedItem as PlayerViewModel;
                Black = Blacks.SelectedItem as PlayerViewModel;

                this.DialogResult = true;
                Close();
            }
        }
    }
}

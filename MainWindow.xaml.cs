using System;
using System.Collections.Generic;
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

namespace tictactoe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly Dictionary<player, ImageSource> imagesource = new()
        {
            { player.x , new BitmapImage(new Uri("pack://application:,,,/assets/X15.png"))  },
            { player.o , new BitmapImage(new Uri("pack://application:,,,/assets/O15.png"))  }
        };

        private readonly Image[,] imagecontrols = new Image[3, 3];
        private readonly gamestate gamestate = new gamestate();
        public MainWindow()
        {
            InitializeComponent();
            setupgamegrid();

            gamestate.movemade += onmovemade;
            gamestate.gameended += ongameended;
            gamestate.gamerestarted += ongamerestarted;
        }

        private void setupgamegrid()
        {
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    Image imagecontrol = new Image();
                    gamegrid.Children.Add(imagecontrol);
                    imagecontrols[i,j] = imagecontrol;
                }
            }
        }

        private void transition(string text,ImageSource winnerimages )
        {
            turnpanel.Visibility = Visibility.Hidden;
            gamecanvas.Visibility = Visibility.Hidden;
            resulttext.Text = text;
            winnerimage.Source = winnerimages;
            endscreen.Visibility = Visibility.Visible;
        }
        private void onmovemade(int r,int c)
        {
            player playr = gamestate.gamegrid[r, c];
            imagecontrols[r, c].Source = imagesource[playr];
            playerimage.Source = imagesource[gamestate.currentplayer];
        }

        private async void ongameended(gameresult game)
        {
            await Task.Delay(500);
            if(game.winner == player.none)
            {
                transition("It's a tie ", null);
            }
            else
            {

                showline(game.wininfo);
                await Task.Delay(1500);
                transition("winner : ", imagesource[game.winner]);
            }
        }

        private (Point,Point) findlinepoints(wininfo winfo)
        {
            double squaresize = gamegrid.Width / 3;
            double margin = squaresize / 2;

            if(winfo.type == wintype.row)
            {
                double y = winfo.number * squaresize + margin;
                return (new Point(0,y), new Point(gamegrid.Width,y));
            }
            if(winfo.type == wintype.column)
            {
                double x = winfo.number * squaresize + margin;
                return (new Point(x,0), new Point(x,gamegrid.Height));
            }
            if(winfo.type == wintype.maindiagonale)
            {
                return (new Point(0,0),new Point(gamegrid.Width,gamegrid.Height));
            }
            return (new Point(gamegrid.Width, 0), new Point(0, gamegrid.Height));
        }

        private void showline(wininfo winfo)
        {
            (Point start, Point end)= findlinepoints(winfo);
            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;
            line.Visibility = Visibility.Visible;
        }
        private void ongamerestarted()
        {
            for(int i = 0;i<3;i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    imagecontrols[i, j].Source = null;
                }
            }
            playerimage.Source = imagesource[gamestate.currentplayer];
            transitiontogame();
        }

        private void transitiontogame()
        {
            endscreen.Visibility = Visibility.Hidden;
            line.Visibility = Visibility.Hidden;
            turnpanel.Visibility = Visibility.Visible;
            gamecanvas.Visibility = Visibility.Visible;
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gamestate.reset();
        }

        private void GameGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squaresize = gamegrid.Width / 3;
            Point clickposition = e.GetPosition(gamegrid);
            int row = (int)(clickposition.Y / squaresize);
            int col = (int)(clickposition.X / squaresize);
            gamestate.makemove(row, col);
        }
    }
}

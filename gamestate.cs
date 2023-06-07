using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace tictactoe
{
    public class gamestate
    {
        public player[,] gamegrid { get; private set; }
        public player currentplayer { get; private set; }
        public int turnspassed { get; private set; }
        public bool gameover { get; private set; }

        public event Action<int, int> movemade;
        public event Action<gameresult> gameended;
        public event Action gamerestarted;

        public gamestate()
        {
            gamegrid = new player[3, 3];
            currentplayer = player.x;
            turnspassed = 0;
            gameover = false;

        }
        private bool canmakemove(int r ,int c)
        {
            return !gameover && gamegrid[r, c] == player.none;
        }

        private bool isgridfull()
        {
            return turnspassed == 9;
        }

        private void switchplayer()
        {
            if(currentplayer == player.x)
            {
                currentplayer = player.o;
            }
            else
            {
                currentplayer = player.x;
            }
        }

        private bool aresquaresmarked((int , int)[] squares ,player players )
        {
            foreach((int r,int c) in squares)
            {
                if (gamegrid[r, c] != players)
                {
                    return false;
                }
            }
            return true;
        }

        private bool didmovewin(int r, int c, out wininfo wininfos)
        {
            (int, int)[] row = new[] { (r, 0), (r, 1), (r, 2) };
            (int, int)[] col = new[] { ( 0,c), (1,c), (2,c) };
            (int, int)[] maindiag = new[] { (0, 0), (1, 1), (2, 2) };
            (int, int)[] antidiag = new[] { (0, 2), (1, 1), (2, 0) };

            if (aresquaresmarked(row, currentplayer))
            {
                wininfos = new wininfo { type = wintype.row, number = r };
                return true;
            }

            if (aresquaresmarked(col, currentplayer))
            {
                wininfos = new wininfo { type = wintype.column, number = c };
                return true;
            }

            if (aresquaresmarked(maindiag, currentplayer))
            {
                wininfos = new wininfo { type = wintype.maindiagonale };
                return true;
            }

            if (aresquaresmarked(antidiag, currentplayer))
            {
                wininfos = new wininfo { type = wintype.antidiagonal };
                return true;
            }

            wininfos = null;
            return false;
        }

        private bool  didmoveendgame(int r,int c,out gameresult gameresults)
        {
            if (didmovewin(r,c,out wininfo wininfos))
            {
                gameresults = new gameresult { winner = currentplayer, wininfo = wininfos };
                return true;    
            }

            if (isgridfull())
            {
                gameresults = new gameresult { winner = player.none };
                return true;
            }

            gameresults = null;
            return false;
        }

        public void makemove(int r,int c)
        {
            if (!canmakemove(r, c))
            {
                return;
            }

            gamegrid[r, c] = currentplayer;
            turnspassed++;
            if(didmoveendgame(r,c,out gameresult gameresults))
            {
                gameover = true;
                movemade?.Invoke(r, c);
                gameended?.Invoke(gameresults);
            }
            else
            {
                switchplayer();
                movemade?.Invoke(r, c);
            }
        }

        public void reset()
        {
            gamegrid = new player[3, 3];
            currentplayer = player.x;
            turnspassed = 0;
            gameover = false;
            gamerestarted?.Invoke();
        }
    }

}

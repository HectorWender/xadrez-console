using System;
using tabuleiro;
using xadrez;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                Tabuleiro tabuleiro = new Tabuleiro(8, 8);

                tabuleiro.colocarPeca(new Torre(tabuleiro, Cor.Preto), new Posicao(0, 0));
                tabuleiro.colocarPeca(new Rei(tabuleiro, Cor.Preto), new Posicao(1, 3));
                tabuleiro.colocarPeca(new Rei(tabuleiro, Cor.Preto), new Posicao(1, 9));

                Tela.imprimirTabuleiro(tabuleiro);

              
            } catch(TabuleiroException e) {
                Console.WriteLine(e.Message);
            }

  Console.ReadLine();
        }
    }
}

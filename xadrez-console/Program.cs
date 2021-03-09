using System;
using tabuleiro;
using xadrez;

namespace xadrez_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Tabuleiro tabuleiro = new Tabuleiro(8, 8);

            tabuleiro.colocarPeca(new Torre(tabuleiro, Cor.Preto), new Posicao(0,0));
            tabuleiro.colocarPeca(new Rei(tabuleiro, Cor.Preto), new Posicao(1, 3));

            Tela.imprimirTabuleiro(tabuleiro);

            Console.ReadLine();

        }
    }
}

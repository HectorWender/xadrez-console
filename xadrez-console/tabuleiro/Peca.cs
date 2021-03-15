﻿namespace tabuleiro
{
    abstract class Peca
    {
        private Tabuleiro tabuleiro;

        public Posicao posicao { get; set; }
        public Cor cor { get; protected set; }
        public int qtdMovimentos { get; protected set; }
        public Tabuleiro tab { get; protected set; }

        public Peca(Tabuleiro tab, Cor cor)
        {
            this.posicao = null;
            this.cor = cor;
            this.tab = tab;
            this.qtdMovimentos = 0;
        }

        public void incrementarQtdMovimentos() { qtdMovimentos++; }

        public abstract bool[,] movimentosPossiveis();
    }
}

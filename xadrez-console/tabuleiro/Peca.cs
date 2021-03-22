namespace tabuleiro
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

        public bool existeMovimentosPossiveis()
        {
            bool[,] matriz = movimentosPossiveis();
            for(int i = 0; i < tab.linhas; i++) {
                for(int j = 0; j < tab.colunas; j++) {
                    if(matriz[i,j]) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool podeMoverPara(Posicao posicao)
        {
            return movimentosPossiveis()[posicao.linha, posicao.coluna];
        }

        public abstract bool[,] movimentosPossiveis();
    }
}

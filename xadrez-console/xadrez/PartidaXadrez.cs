using System;
using tabuleiro;

namespace xadrez
{
    class PartidaXadrez
    {
        public Tabuleiro tabuleiro { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }

        public PartidaXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branco;
            terminada = false;
            colocarPecas();
        }

        public void executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tabuleiro.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = tabuleiro.retirarPeca(destino);
            tabuleiro.colocarPeca(p, destino);

        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            executaMovimento(origem, destino);
            turno++;
            mudaJogador();
        }

        public void validarPosicaoOrigem(Posicao posicao)
        {
            if(tabuleiro.peca(posicao) == null) 
                throw new TabuleiroException("Não existe peça nessa posição de origem escolhida");

            if(jogadorAtual != tabuleiro.peca(posicao).cor)
                throw new TabuleiroException("A peça de origem escolhida não é sua!");

            if(!tabuleiro.peca(posicao).existeMovimentosPossiveis())
                throw new TabuleiroException("Não há movimentos possíveis para a peça de origem escolhida!");

        }

        public void validarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if(!tabuleiro.peca(origem).podeMoverPara(destino)) 
                throw new TabuleiroException("Posição de destino inválida!");
            
        }

        public void mudaJogador()
        {
            if(jogadorAtual == Cor.Branco) {
                jogadorAtual = Cor.Preto;
            } else {
                jogadorAtual = Cor.Branco;
            }
        }

        private void colocarPecas()
        {
            tabuleiro.colocarPeca(new Torre(tabuleiro, Cor.Branco), new PosicaoXadrez('a', 1).toPosicao());
            tabuleiro.colocarPeca(new Torre(tabuleiro, Cor.Preto), new PosicaoXadrez('b', 8).toPosicao());
            tabuleiro.colocarPeca(new Rei(tabuleiro, Cor.Preto), new PosicaoXadrez('e', 4).toPosicao());
        }

    }
}

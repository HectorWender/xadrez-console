using System;
using System.Collections.Generic;
using tabuleiro;

namespace xadrez
{
    class PartidaXadrez
    {
        public Tabuleiro tabuleiro { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }

        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;

        public PartidaXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branco;
            terminada = false;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public void executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tabuleiro.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = tabuleiro.retirarPeca(destino);
            tabuleiro.colocarPeca(p, destino);

            if(pecaCapturada != null ) {
                capturadas.Add(pecaCapturada);
            }
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

        public HashSet<Peca> pecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach(Peca item in capturadas) {
                if(item.cor == cor)
                    aux.Add(item);
            }

            return aux;
        }

        public HashSet<Peca> pecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();

            foreach(Peca item in pecas) {
                if(item.cor == cor)
                    aux.Add(item);
            }

            aux.ExceptWith(pecasCapturadas(cor));

            return aux;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tabuleiro.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            colocarNovaPeca('c', 1, new Torre(tabuleiro, Cor.Branco));
            colocarNovaPeca('a', 1, new Torre(tabuleiro, Cor.Branco));
            colocarNovaPeca('b', 8, new Torre(tabuleiro, Cor.Preto));
            colocarNovaPeca('e', 4, new Rei(tabuleiro, Cor.Preto));
        }

    }
}

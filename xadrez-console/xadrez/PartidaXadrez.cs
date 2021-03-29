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
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }

        private HashSet<Peca> pecas;
        private HashSet<Peca> capturadas;


        public PartidaXadrez()
        {
            tabuleiro = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branco;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            capturadas = new HashSet<Peca>();
            colocarPecas();
        }

        public Peca executaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tabuleiro.retirarPeca(origem);
            p.incrementarQtdMovimentos();
            Peca pecaCapturada = tabuleiro.retirarPeca(destino);
            tabuleiro.colocarPeca(p, destino);

            if(pecaCapturada != null) {
                capturadas.Add(pecaCapturada);
            }

            //#jogadaEspecial - roque pequeno
            if(p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);

                Peca T = tabuleiro.retirarPeca(origemT);
                T.incrementarQtdMovimentos();
                tabuleiro.colocarPeca(T, destinoT);
            }

            //#jogadaEspecial - roque grande
            if(p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);

                Peca T = tabuleiro.retirarPeca(origemT);
                T.incrementarQtdMovimentos();
                tabuleiro.colocarPeca(T, destinoT);
            }

            //#jogadaEspecial - en passant
            if(p is Peao) {
                if(origem.coluna != destino.coluna && pecaCapturada == null) {
                    Posicao posP;
                    if(p.cor == Cor.Branco) {
                        posP = new Posicao(destino.linha + 1, destino.coluna);
                    } else {
                        posP = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tabuleiro.retirarPeca(posP);
                    capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tabuleiro.retirarPeca(destino);
            p.decrementarQtdMovimentos();

            if(pecaCapturada != null) {
                tabuleiro.colocarPeca(pecaCapturada, destino);
                capturadas.Remove(pecaCapturada);
            }

            tabuleiro.colocarPeca(p, origem);

            //#jogadaEspecial - roque pequeno
            if(p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna + 1);

                Peca T = tabuleiro.retirarPeca(origemT);
                T.decrementarQtdMovimentos();
                tabuleiro.colocarPeca(T, origemT);
            }

            //#jogadaEspecial - roque grande
            if(p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemT = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoT = new Posicao(origem.linha, origem.coluna - 1);

                Peca T = tabuleiro.retirarPeca(origemT);
                T.incrementarQtdMovimentos();
                tabuleiro.colocarPeca(T, destinoT);
            }

            //#jogadaEspecial - en passant
            if(p is Peao) {
                if(origem.coluna != destino.coluna && pecaCapturada == vulneravelEnPassant) {
                    Peca peao = tabuleiro.retirarPeca(destino);
                    Posicao posP;
                    if(p.cor == Cor.Branco) {
                        posP = new Posicao(3, destino.coluna);
                    } else {
                        posP = new Posicao(4, destino.coluna);
                    }
                    tabuleiro.colocarPeca(peao, posP);
                }
            }
        }

        public void realizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if(estaEmXeque(jogadorAtual)) {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            Peca p = tabuleiro.peca(destino);
            // #jogadaEspecial - Promoção
            if(p is Peao) {
                if((p.cor == Cor.Branco && destino.linha == 0) || (p.cor == Cor.Preto && destino.linha == 7)) {
                    p = tabuleiro.retirarPeca(destino);
                    pecas.Remove(p);
                    Peca rainha = new Rainha(tabuleiro, p.cor);
                    tabuleiro.colocarPeca(rainha, destino);
                    pecas.Add(rainha);
                }
            }

            if(estaEmXeque(adversaria(jogadorAtual))) {
                xeque = true;
            } else {
                xeque = false;
            }

            if(testeXequemate(adversaria(jogadorAtual))) {
                terminada = true;
            } else {
                turno++;
                mudaJogador();
            }

            //#jogadaEspecial - en passant
            if(p is Peao && (destino.linha == origem.linha - 2 || destino.linha == origem.linha + 2)) {
                vulneravelEnPassant = p;
            } else {
                vulneravelEnPassant = null;
            }
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
            if(!tabuleiro.peca(origem).movimentoPossivel(destino))
                throw new TabuleiroException("Posição de destino inválida!");
        }

        public void mudaJogador()
        {
            if(jogadorAtual == Cor.Branco)
                jogadorAtual = Cor.Preto;
            else
                jogadorAtual = Cor.Branco;

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

        private Cor adversaria(Cor cor)
        {
            if(cor == Cor.Branco)
                return Cor.Preto;

            return Cor.Branco;
        }

        private Peca rei(Cor cor)
        {
            foreach(Peca item in pecasEmJogo(cor)) {
                if(item is Rei)
                    return item;

            }
            return null;
        }

        public bool estaEmXeque(Cor cor)
        {
            Peca R = rei(cor);

            if(R == null)
                throw new TabuleiroException("Não existe rei da cor " + cor + " no tabuleiro!");

            foreach(Peca item in pecasEmJogo(adversaria(cor))) {
                bool[,] mat = item.movimentosPossiveis();
                if(mat[R.posicao.linha, R.posicao.coluna])
                    return true;

            }

            return false;
        }

        public bool testeXequemate(Cor cor)
        {
            if(!estaEmXeque(cor))
                return false;

            foreach(Peca item in pecasEmJogo(cor)) {
                bool[,] matriz = item.movimentosPossiveis();
                for(int i = 0; i < tabuleiro.linhas; i++) {
                    for(int j = 0; j < tabuleiro.colunas; j++) {
                        if(matriz[i, j]) {
                            Posicao origem = item.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecacapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecacapturada);

                            if(!testeXeque)
                                return false;

                        }
                    }
                }
            }
            return true;
        }

        public void colocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tabuleiro.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        private void colocarPecas()
        {
            char[] aux = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            //Brancas
            colocarNovaPeca('a', 1, new Torre(tabuleiro, Cor.Branco));
            colocarNovaPeca('b', 1, new Cavalo(tabuleiro, Cor.Branco));
            colocarNovaPeca('c', 1, new Bispo(tabuleiro, Cor.Branco));
            colocarNovaPeca('d', 1, new Rainha(tabuleiro, Cor.Branco));
            colocarNovaPeca('e', 1, new Rei(tabuleiro, Cor.Branco, this));
            colocarNovaPeca('f', 1, new Bispo(tabuleiro, Cor.Branco));
            colocarNovaPeca('g', 1, new Cavalo(tabuleiro, Cor.Branco));
            colocarNovaPeca('h', 1, new Torre(tabuleiro, Cor.Branco));

            foreach(char item in aux) {
                colocarNovaPeca(item, 2, new Peao(tabuleiro, Cor.Branco, this));
            }

            //Pretas
            colocarNovaPeca('a', 8, new Torre(tabuleiro, Cor.Preto));
            colocarNovaPeca('b', 8, new Cavalo(tabuleiro, Cor.Preto));
            colocarNovaPeca('c', 8, new Bispo(tabuleiro, Cor.Preto));
            colocarNovaPeca('d', 8, new Rainha(tabuleiro, Cor.Preto));
            colocarNovaPeca('e', 8, new Rei(tabuleiro, Cor.Preto, this));
            colocarNovaPeca('f', 8, new Bispo(tabuleiro, Cor.Preto));
            colocarNovaPeca('g', 8, new Cavalo(tabuleiro, Cor.Preto));
            colocarNovaPeca('h', 8, new Torre(tabuleiro, Cor.Preto));

            foreach(char item in aux) {
                colocarNovaPeca(item, 7, new Peao(tabuleiro, Cor.Preto, this));
            }
        }

    }
}

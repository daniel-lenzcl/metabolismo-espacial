// EncontroInfo.cs
using UnityEngine;

public static class EncontroInfo
{
    public struct Encontro
    {
        public cPessoa outro;   // quem encontrei
        public Vector3 posicao;    // onde encontrei
        public int dia;           // em que hora/dia
        public int hora;           // em que hora/dia
        public int min;           // em que hora/dia

        public Encontro(cPessoa outro, Vector3 posicao, int dia, int hora, int min)
        {
            this.outro = outro;
            this.posicao = posicao;
            this.dia = dia;
            this.hora = hora;
            this.min = min;
        }
    }
}

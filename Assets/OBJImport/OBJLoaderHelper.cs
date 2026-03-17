using System.Globalization;
using UnityEngine;

namespace Dummiesman
{
    public static class OBJLoaderHelper
    {
        /// <summary>
        /// Enables transparency mode on standard materials
        /// </summary>
        public static void EnableMaterialTransparency(Material mtl)
        {
            mtl.SetFloat("_Mode", 3f);
            mtl.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mtl.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mtl.SetInt("_ZWrite", 0);
            mtl.DisableKeyword("_ALPHATEST_ON");
            mtl.EnableKeyword("_ALPHABLEND_ON");
            mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mtl.renderQueue = 3000;
        }

        /// <summary>
        /// Modified from https://codereview.stackexchange.com/a/76891. Faster than float.Parse
        /// </summary>
        public static float FastFloatParse(string input)
        {
            if (input.Contains("e") || input.Contains("E"))
                return float.Parse(input, CultureInfo.InvariantCulture);

            float result = 0;
            int pos = 0;
            int len = input.Length;

            if (len == 0) return float.NaN;
            char c = input[0];
            float sign = 1;
            if (c == '-')
            {
                sign = -1;
                ++pos;
                if (pos >= len) return float.NaN;
            }

            while (true) // breaks inside on pos >= len or non-digit character
            {
                if (pos >= len) return sign * result;
                c = input[pos++];
                if (c < '0' || c > '9') break;
                result = (result * 10.0f) + (c - '0');
            }

            if (c != '.' && c != ',') return float.NaN;
            float exp = 0.1f;
            while (pos < len)
            {
                c = input[pos++];
                if (c < '0' || c > '9') return float.NaN;
                result += (c - '0') * exp;
                exp *= 0.1f;
            }
            return sign * result;
        }

        /// <summary>
        /// Modified from http://cc.davelozinski.com/c-sharp/fastest-way-to-convert-a-string-to-an-int. Faster than int.Parse
        /// </summary>
        public static int FastIntParse(string input)
        {
            int result = 0;
            bool isNegative = (input[0] == '-');

            for (int i = (isNegative) ? 1 : 0; i < input.Length; i++)
                result = result * 10 + (input[i] - '0');
            return (isNegative) ? -result : result;
        }

        public static Material CreateNullMaterial()
        {
            // Lista de candidatos (ordem de preferência)
            string[] candidates = new string[]
            {
            "Standard (Specular setup)",
            "Standard",
            "Universal Render Pipeline/Lit",
            "HDRP/Lit",
            "Unlit/Texture",
            "Sprites/Default",
            "Hidden/InternalErrorShader"
            };

            Shader sh = null;
            foreach (var name in candidates)
            {
                if (string.IsNullOrEmpty(name)) continue;
                sh = Shader.Find(name);
                if (sh != null)
                {
                    DebugController.Log(DebugCategoria.WebGLFileUploader, $"OBJLoaderHelper: usando shader '{name}' para material fallback.");
                    break;
                }
            }

            if (sh == null)
            {
                DebugController.LogWarning(DebugCategoria.WebGLFileUploader, "OBJLoaderHelper: nenhum shader candidato encontrado. Verifique __Edit > Project Settings > Graphics__ > __Always Included Shaders__.");
                // tenta opções mais simples
                sh = Shader.Find("Standard") ?? Shader.Find("Unlit/Texture") ?? Shader.Find("Sprites/Default");
            }

            // Garante que nunca criamos new Material(null)
            if (sh != null)
            {
                var mat = new Material(sh);
                mat.name = "obj_fallback_material";
                return mat;
            }

            // Último recurso: tenta Hidden/InternalErrorShader ou cria com Standard (pode ser null também, tentamos evitar)
            var finalShader = Shader.Find("Hidden/InternalErrorShader") ?? Shader.Find("Standard");
            if (finalShader != null)
            {
                var mat = new Material(finalShader) { name = "obj_fallback_material_final" };
                DebugController.LogWarning(DebugCategoria.WebGLFileUploader, "OBJLoaderHelper: fallback final aplicado.");
                return mat;
            }

            // Se chegar aqui, algo muito errado no build — cria um material com cor base (não ideal, mas evita exception)
            var fallback = new Material(Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Texture"));
            fallback.name = "obj_fallback_material_defensive";
            DebugController.LogWarning(DebugCategoria.WebGLFileUploader, "OBJLoaderHelper: nenhum shader disponível, criado material defensivo.");
            return fallback;
        }

        public static Vector3 VectorFromStrArray(string[] cmps)
        {
            float x = FastFloatParse(cmps[1]);
            float y = FastFloatParse(cmps[2]);
            if (cmps.Length == 4)
            {
                float z = FastFloatParse(cmps[3]);
                return new Vector3(x, y, z);
            }
            return new Vector2(x, y);
        }

        public static Color ColorFromStrArray(string[] cmps, float scalar = 1.0f)
        {
            float Kr = FastFloatParse(cmps[1]) * scalar;
            float Kg = FastFloatParse(cmps[2]) * scalar;
            float Kb = FastFloatParse(cmps[3]) * scalar;
            return new Color(Kr, Kg, Kb);
        }
    }
}
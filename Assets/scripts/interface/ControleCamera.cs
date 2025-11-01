//----------------------------------------------
// ControleCamera.cs (fix: centralizar mantém controles ativos) funfando
//----------------------------------------------
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class ControleCamera : MonoBehaviour
{
    [Header("Referência opcional à luz direcional")]
    public Light dirLight;

    [Header("Alvo / Mapa")]
    public Transform alvoRaiz;

    [Header("Zoom")]
    public float minZoom = 20f;
    public float maxZoom = 2000f;
    public float zoomVel = 10000f;

    [Header("Pan")]
    public float panVelMouse = 5f;
    public float panVelKeys = 20f;
    [Tooltip("Pan segue a orientação da câmera (right/forward projetados no chão)")] public bool panRelativoAoVisual = true;
    [Tooltip("Se true, interpola a posição até o alvo (efeito de 'slide')")]
    public bool suavizarMovimento = true;
    [Tooltip("Intensidade da suavização (maior = converge mais rápido, menor = desliza mais)")]
    public float panSmooth = 25f;

    [Header("Orbit (novo)")]
    public bool orbitEnabled = true;
    [Tooltip("Ativa/desativa o modo orbit (padrão: O)")] public bool orbitActive = false;
    public KeyCode orbitToggleKey = KeyCode.O;
    public KeyCode orbitFocusKey = KeyCode.F;
    public float orbitYawSpeed = 200f;
    public float orbitPitchSpeed = 150f;
    [Range(0f, 89.9f)] public float orbitMinPitch = 10f;
    [Range(0f, 89.9f)] public float orbitMaxPitch = 85f;
    public float orbitMinDistance = 5f;
    public float orbitMaxDistance = 2000f;
    public float orbitZoomVel = 1000f;
    public LayerMask orbitRaycastMask = ~0;
    Vector3 orbitPivot;
    float orbitDistance = 50f;
    float orbitYaw = 0f;
    float orbitPitch = 60f;

    [Header("Rotacao (opcional)")]
    public float rotVel = 1000f;

    private Camera cam;
    private Vector3 upVector = Vector3.up;
    private Vector3 xVector = Vector3.right;

    private float alturaAtual;
    private Vector3 targetPosition;

    public Gerente_de_ambiente ambiente;
    public TratamentoMapaCarregado mapaTratado;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        alturaAtual = cam.transform.position.y;
        targetPosition = cam.transform.position;

        if (dirLight == null)
            dirLight = FindObjectsOfType<Light>()
                        .FirstOrDefault(l => l.type == LightType.Directional);
    }

    public void Start()
    {
        DebugController.Log(DebugCategoria.ControleCamera, "incializando camera");
        mapaTratado = FindObjectOfType<TratamentoMapaCarregado>();

        mapaTratado.OnMapaCarregado.AddListener(() =>
        {
            DebugController.Log(DebugCategoria.ControleCamera, "olhando para o mapa");
            Bounds b = mapaTratado.limites_mapa_importado;
            CenterOnBounds(b, true);
            alvoRaiz = mapaTratado.mapaImportadoRaiz.transform;
        });

        if (mapaTratado.mapaImportadoRaiz == null)
        {
            DebugController.Log(DebugCategoria.ControleCamera, "olhando para o terreno");
            Terrain terreno = FindObjectOfType<Terrain>();
            Bounds b = terreno.terrainData.bounds;
            b.center += terreno.transform.position;
            CenterOnBounds(b, true);
            alvoRaiz = terreno.transform;
        }
    }

    void Update()
    {
        //--------------- Teclas de orbit ---------------
        if (orbitEnabled)
        {
            if (Input.GetKeyDown(orbitToggleKey)) orbitActive = !orbitActive;
            if (Input.GetKeyDown(orbitFocusKey))
            {
                if (TryGetOrbitPivotFromMouse(out var p)) SetOrbitPivot(p);
                else if (mapaTratado != null) SetOrbitPivot(mapaTratado.limites_mapa_importado.center);
            }
        }

        //--------------- Zoom ---------------
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.PageUp)) scroll += 0.1f;
        if (Input.GetKey(KeyCode.PageDown)) scroll -= 0.1f;

        if (Mathf.Abs(scroll) > 0.001f)
        {
            if (orbitEnabled && orbitActive)
            {
                orbitDistance -= scroll * orbitZoomVel * Time.deltaTime;
                orbitDistance = Mathf.Clamp(orbitDistance, orbitMinDistance, orbitMaxDistance);
            }
            else
            {
                alturaAtual -= scroll * zoomVel * Time.deltaTime;
                alturaAtual = Mathf.Clamp(alturaAtual, minZoom, maxZoom);
            }
        }

        //--------------- Orbit vs Pan ---------------
        bool draggingOrbit = orbitEnabled && orbitActive && (Input.GetMouseButton(2) || (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(0));

        if (draggingOrbit)
        {
            orbitYaw += Input.GetAxis("Mouse X") * orbitYawSpeed * Time.deltaTime;
            orbitPitch -= Input.GetAxis("Mouse Y") * orbitPitchSpeed * Time.deltaTime;
            orbitPitch = Mathf.Clamp(orbitPitch, orbitMinPitch, orbitMaxPitch);

            // aplica a órbita (posição calculada ao redor do pivot)
            Quaternion rot = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
            Vector3 offset = rot * new Vector3(0f, 0f, -orbitDistance);
            Vector3 desired = orbitPivot + offset;

            targetPosition = desired;
            if (suavizarMovimento && panSmooth > 0f)
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panSmooth);
            else
                transform.position = targetPosition;

            transform.rotation = Quaternion.LookRotation(orbitPivot - transform.position, Vector3.up);
            alturaAtual = transform.position.y; // mantém altura coerente caso saia do modo orbit
        }
        else
        {
            // Pan relativo à orientação da câmera
            Vector3 pan = Vector3.zero;
            float escalaMouse = panVelMouse * Time.deltaTime * Mathf.Max(1f, alturaAtual * 0.05f);

            if (Input.GetMouseButton(1))
            {
                float mx = Input.GetAxis("Mouse X");
                float my = Input.GetAxis("Mouse Y");

                if (panRelativoAoVisual || (orbitEnabled && orbitActive))
                {
                    Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
                    if (right.sqrMagnitude < 1e-6f) right = Vector3.right;
                    Vector3 fwd = Vector3.Cross(Vector3.up, right).normalized;
                    pan += (-mx * right + my * fwd) * escalaMouse; // movimento acompanha o mouse
                }
                else
                {
                    pan.x -= mx * escalaMouse;
                    pan.z += my * escalaMouse;
                }
            }

            float hor = Input.GetAxisRaw("Horizontal");
            float ver = Input.GetAxisRaw("Vertical");
            if (Mathf.Abs(hor) > 0f || Mathf.Abs(ver) > 0f)
            {
                Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
                if (right.sqrMagnitude < 1e-6f) right = Vector3.right;
                Vector3 fwd = Vector3.Cross(Vector3.up, right).normalized;
                pan += (right * hor + fwd * ver) * panVelKeys * Time.deltaTime;
            }

            targetPosition += pan;
            targetPosition.y = alturaAtual;

            if (suavizarMovimento && panSmooth > 0f)
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * panSmooth);
            else
                transform.position = targetPosition;
        }

        //--------------- Atalho de centralizar ---------------
        if (Input.GetKeyDown(KeyCode.C))
        {
            CentralizarCamera();
        }

        SyncDirectionalLight();
    }

    [ContextMenu("Centralizar na Cena Atual")]
    public void CentralizarCamera()
    {
        if (mapaTratado != null && mapaTratado.mapaImportadoRaiz != null)
        {
            CenterOnBounds(mapaTratado.limites_mapa_importado, true);
        }
        else
        {
            Terrain terreno = FindObjectOfType<Terrain>();
            if (terreno != null)
            {
                Bounds b = terreno.terrainData.bounds;
                b.center += terreno.transform.position;
                CenterOnBounds(b, true);
            }
        }
    }

    public void CenterOnBounds(Bounds b, bool snap = true)
    {
        Vector3 centro = b.center;
        // define pivot para orbit e distância básica
        orbitPivot = new Vector3(centro.x, 0f, centro.z);

        float maiorLado = Mathf.Max(b.size.x, b.size.z);
        alturaAtual = Mathf.Clamp(maiorLado, minZoom, maxZoom);

        // coloca visão top‑down ao centralizar
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        orbitYaw = 0f;            // opcional: zera yaw
        orbitPitch = 60f;         // ângulo padrão ao entrar em orbit
        orbitDistance = Mathf.Clamp(alturaAtual, orbitMinDistance, orbitMaxDistance);

        Vector3 novaPos = new Vector3(centro.x, alturaAtual, centro.z);
        targetPosition = novaPos;
        if (snap) transform.position = novaPos;
    }

    private void SyncDirectionalLight()
    {
        if (dirLight == null) return;
        dirLight.transform.position = transform.position;
        dirLight.transform.rotation = transform.rotation;
    }

    bool TryGetOrbitPivotFromMouse(out Vector3 p)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100000f, orbitRaycastMask))
        {
            p = hit.point;
            return true;
        }
        p = Vector3.zero;
        return false;
    }

    void SetOrbitPivot(Vector3 p)
    {
        orbitPivot = p;
        Vector3 delta = transform.position - p;
        orbitDistance = Mathf.Clamp(delta.magnitude, orbitMinDistance, orbitMaxDistance);
        float horiz = new Vector2(delta.x, delta.z).magnitude;
        orbitYaw = Mathf.Atan2(delta.x, delta.z) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Atan2(delta.y, horiz) * Mathf.Rad2Deg;
        orbitPitch = Mathf.Clamp(orbitPitch, orbitMinPitch, orbitMaxPitch);
        orbitActive = true;
    }
}

using UnityEngine;
using System.Collections;

public class Fractal : MonoBehaviour {

    public Mesh[] meshes;
    public Material material;

    public int maxDepth;
    public int depth;
    public float childScale;

    private static Vector3[] childDirection = { Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };
    private static Quaternion[] childOrientation = { Quaternion.identity, Quaternion.Euler(0, 0, -90), Quaternion.Euler(0, 0, 90), Quaternion.Euler(0, 90, 0), Quaternion.Euler(0, -90, 0) };

    private Material[,] materials;

    [Range(0, 1)]
    public float spawnProbability = 0.7f;

    [Range(0, 100)]
    public float maxRotationSpeed = 60f;

    private float rotationSpeed;

    [Range(0, 180)]
    public float maxTwist = 180f;

    private void Start()
    {
        if(materials == null)
        {
            InitializeMaterials();
        }


        gameObject.AddComponent<MeshFilter>().mesh = meshes[Random.Range(0, meshes.Length)];
        gameObject.AddComponent<MeshRenderer>().material = materials[depth, Random.Range(0, 2)];


        if (depth < maxDepth)
        {
            // create fractal children
            StartCoroutine(CreateChildren());
        }

        rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);

        // twist
        transform.Rotate(Random.Range(-maxTwist, maxTwist), 0, 0);
    }

    private void Initialize(Fractal parent, int index)
    {
        // children inherit from parent
        meshes = parent.meshes;
        material = parent.material;
        materials = parent.materials;
        spawnProbability = parent.spawnProbability;
        maxRotationSpeed = parent.maxRotationSpeed;
        maxTwist = parent.maxTwist;

        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;

        transform.parent = parent.transform;

        childScale = parent.childScale;
        // children inherit from parent

        transform.localScale = Vector3.one * childScale;

        transform.localPosition = childDirection[index] * (0.5f + 0.5f * childScale);

        transform.localRotation = childOrientation[index];
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childDirection.Length; i++)
        {
            if (Random.value < spawnProbability)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
                new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, i);
            }
        }
    }

    private void InitializeMaterials()
    {
        materials = new Material[maxDepth + 1, 2];
        
        for(int i = 0; i <= maxDepth; i++)
        {
            materials[i, 0] = new Material(material);
            materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, (float)i / maxDepth);

            materials[i, 1] = new Material(material);
            materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, (float)i / maxDepth);
        }

        // end of fractal
        materials[maxDepth, 0].color = Color.magenta;
        materials[maxDepth, 1].color = Color.red;

        Debug.Log("Initializing materials...");
    }

    void Update()
    {
        // rotate
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private List<GameObject> Roads = new List<GameObject>();
    [SerializeField] private Transform playerPrefab; // Karakteri buraya sürükle
    [SerializeField] Transform carSpawn;
    
    private float previousPlayerZ;
    [SerializeField] private int currentScore;
    [SerializeField] private int addScoreAmount = 100;
    [SerializeField] private TextMeshProUGUI txtScore,txtInGameScore;

    [Tooltip("Yolun uzunluðu. Boþluk kalýrsa burayý deðiþtir (Örn: 5, 10, 20)")]
    [SerializeField] private float roadPartDistance = 5.0f;

    private float roadLength = 0f;
    int count = 5;

    void Start()
    {
        // Baþlangýçta 10 tane yol oluþturarak sahneyi doldur
        for (int i = 0; i < count; i++)
        {
            CreateRoad();
        }
        currentScore = 0;
        txtInGameScore.text = "Score: " + currentScore.ToString();
    }

    void Update()
    {
        // Karakterin null olup olmadýðýný kontrol edelim (hata almamak için)
        if (playerPrefab != null)
        {
            // Karakter mevcut yolun sonuna yaklaþtýysa yeni yol ekle
            if (playerPrefab.position.z > roadLength - (roadPartDistance * count))
            {
                CreateRoad();
            }
        }

    }
    private void FixedUpdate()
    {
        float deltaZ = playerPrefab.position.z - previousPlayerZ;

        carSpawn.position += new Vector3(0, 0, deltaZ);

        previousPlayerZ = playerPrefab.position.z;
    }

    public void SetFinalScore()
    {
        txtScore.text = "Score: " + currentScore.ToString();
        txtInGameScore.gameObject.SetActive(false);
    }


    void CreateRoad()
    {
        // X ve Y'yi 0 yaparak saða-sola veya yukarý-aþaðý kaymayý engelliyoruz
        Vector3 spawnPos = new Vector3(0, 0, roadLength);

        // Quaternion.Euler(0, 0, 0) ile tüm modellerin ayný yöne bakmasýný saðlýyoruz
        // Eðer modellerin hepsi yan duruyorsa (0, 90, 0) veya (0, -90, 0) deneyebilirsin
        Quaternion fixedRotation = Quaternion.Euler(0, 0, 0);

        if (Roads.Count > 0)
        {
            GameObject newRoad = Instantiate(Roads[Random.Range(0, Roads.Count)], spawnPos, fixedRotation);

            // Kod her ihtimale karþý objenin içindeki lokal kaymalarý da sýfýrlasýn
            newRoad.transform.position = spawnPos;
        }

        // Bir sonraki yolun ekleneceði mesafeyi artýrýyoruz
        roadLength += roadPartDistance;

        //Skoru Arttýr
        currentScore += addScoreAmount;
        txtInGameScore.text = "Score: " + currentScore.ToString();
    }
}
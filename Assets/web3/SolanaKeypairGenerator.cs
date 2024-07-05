using UnityEngine;
using UnityEngine.UI;
using Solana.Unity.Wallet;
using System.IO;

public class SolanaKeypairGenerator : MonoBehaviour
{
    public Button generateKeypairButton;

    void Start()
    {
        // Check if the button is assigned
        if (generateKeypairButton != null)
        {
            // Add a listener to the button to call the GenerateKeypair method when clicked
            generateKeypairButton.onClick.AddListener(GenerateKeypair);
        }
        else
        {
            Debug.LogError("Button is not assigned in the Inspector");
        }
    }

    public void GenerateKeypair()
    {
        // Generate a new keypair
        var keypair = new Account(); // Adjust the class name according to your SDK

        // Display the public key and secret key
        Debug.Log("Public Key: " + keypair.PublicKey);
        Debug.Log("Secret Key: " + keypair.PrivateKey);

        // Create a KeypairData object
        KeypairData keypairData = new KeypairData
        {
            PublicKey = keypair.PublicKey,
            SecretKey = keypair.PrivateKey
        };

        // Convert the KeypairData object to JSON
        string json = JsonUtility.ToJson(keypairData, true);

        // Save the JSON to a file
        string path = Path.Combine(Application.persistentDataPath, "keypair.json");
        File.WriteAllText(path, json);

        Debug.Log("Keypair saved to: " + path);
    }

    [System.Serializable]
    public class KeypairData
    {
        public string PublicKey;
        public string SecretKey;
    }
}
